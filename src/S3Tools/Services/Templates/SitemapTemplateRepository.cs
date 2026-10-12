using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>
    /// Repositorio en memoria de las plantillas de sitemap, respaldado por <see cref="SitemapTemplateStore"/>.
    /// Aplica los invariantes del modelo (nombre único de 1–60 caracteres, tiendas del catálogo vinculadas a
    /// lo sumo a una plantilla, sitemap válido, al menos una plantilla) en cada mutación.
    /// <para>
    /// Patrón de mutación: se copia la lista, se aplica el cambio, se persiste la copia y solo entonces se
    /// reemplaza el estado en memoria y se dispara <see cref="Changed"/> (fuera del bloqueo). Si guardar falla,
    /// el estado no cambia. Un único bloqueo protege la lista y todas las llamadas a <c>Save</c>.
    /// </para>
    /// <para>La carga es perezosa: el constructor no hace E/S; el primer acceso a cualquier miembro carga el archivo.</para>
    /// </summary>
    internal sealed class SitemapTemplateRepository
    {
        public const int MaxNameLength = 60;
        public const int MaxDescriptionLength = 200;

        private const string Source = nameof(SitemapTemplateRepository);
        private const string FutureSchemaMessage = "El archivo de plantillas fue creado por una versión más reciente de S3Tools.";
        private const string IoErrorMessage = "No se pudo leer el archivo de plantillas. Los cambios están deshabilitados hasta reiniciar S3Tools.";
        private const string MissingMessage = "La plantilla no existe.";
        private const string DefaultImportName = "Plantilla importada";

        private static readonly Lazy<SitemapTemplateRepository> DefaultInstance =
            new Lazy<SitemapTemplateRepository>(() => new SitemapTemplateRepository(DefaultFilePath()));

        private readonly object _sync = new object();
        private readonly SitemapTemplateStore _store;
        private List<SitemapTemplate> _templates;
        private string _loadWarning;
        private StoreReadOnlyReason _readOnlyReason;

        public SitemapTemplateRepository(string filePath)
        {
            _store = new SitemapTemplateStore(filePath);
        }

        /// <summary>Instancia compartida de la aplicación, sobre <c>%LocalAppData%\S3Tools\sitemap-templates.json</c>.</summary>
        public static SitemapTemplateRepository Default
        {
            get { return DefaultInstance.Value; }
        }

        /// <summary>Se dispara (fuera del bloqueo) tras cada mutación exitosa. Los manejadores pueden correr en cualquier hilo.</summary>
        public event EventHandler Changed;

        /// <summary>Resumen de las correcciones aplicadas al cargar; <c>null</c> si la carga fue limpia.</summary>
        public string LoadWarning
        {
            get
            {
                lock (_sync)
                {
                    EnsureLoaded();
                    return _loadWarning;
                }
            }
        }

        /// <summary>Verdadero cuando el archivo no se puede modificar; toda mutación devuelve error.</summary>
        public bool IsReadOnly
        {
            get
            {
                lock (_sync)
                {
                    EnsureLoaded();
                    return _readOnlyReason != StoreReadOnlyReason.None;
                }
            }
        }

        /// <summary>Motivo del solo lectura (para mostrar el aviso adecuado); <see cref="StoreReadOnlyReason.None"/> si es modificable.</summary>
        public StoreReadOnlyReason ReadOnlyReason
        {
            get
            {
                lock (_sync)
                {
                    EnsureLoaded();
                    return _readOnlyReason;
                }
            }
        }

        /// <summary>Copia de las plantillas en el orden persistido (segura para leer desde otros hilos).</summary>
        public IReadOnlyList<SitemapTemplate> GetAll()
        {
            lock (_sync)
            {
                EnsureLoaded();
                return _templates.ToArray();
            }
        }

        /// <summary>Busca por identificador exacto; <c>null</c> si no existe.</summary>
        public SitemapTemplate Find(string id)
        {
            lock (_sync)
            {
                EnsureLoaded();
                return id == null ? null : _templates.FirstOrDefault(t => t.Id == id);
            }
        }

        /// <summary>Plantilla vinculada a la tienda (sin distinguir mayúsculas); <c>null</c> si ninguna.</summary>
        public SitemapTemplate FindByStore(string store)
        {
            lock (_sync)
            {
                EnsureLoaded();
                return store == null ? null : _templates.FirstOrDefault(t => LinksStore(t, store));
            }
        }

        public TemplateSaveResult Create(SitemapTemplateDraft draft)
        {
            if (draft == null)
            {
                throw new ArgumentNullException(nameof(draft));
            }

            return Mutate((working, issues) =>
            {
                var prepared = Prepare(draft, working, null, issues);
                if (prepared == null)
                {
                    return null;
                }

                var now = DateTime.UtcNow;
                var template = new SitemapTemplate(
                    Guid.NewGuid().ToString("N"),
                    prepared.Name,
                    prepared.Description,
                    null,
                    prepared.Stores,
                    now,
                    now,
                    prepared.SitemapJson);
                MoveStores(working, template.Id, prepared.Stores, now, issues);
                working.Add(template);
                return template;
            });
        }

        public TemplateSaveResult Update(string id, SitemapTemplateDraft draft)
        {
            if (draft == null)
            {
                throw new ArgumentNullException(nameof(draft));
            }

            return Mutate((working, issues) =>
            {
                var index = IndexOf(working, id);
                if (index < 0)
                {
                    issues.Add(Error(MissingMessage));
                    return null;
                }

                var prepared = Prepare(draft, working, id, issues);
                if (prepared == null)
                {
                    return null;
                }

                var existing = working[index];
                var now = DateTime.UtcNow;
                var updated = new SitemapTemplate(
                    existing.Id,
                    prepared.Name,
                    prepared.Description,
                    existing.BuiltInKey,
                    prepared.Stores,
                    existing.CreatedUtc,
                    now,
                    prepared.SitemapJson);
                MoveStores(working, existing.Id, prepared.Stores, now, issues);
                working[index] = updated;
                return updated;
            });
        }

        public TemplateSaveResult Delete(string id)
        {
            return Mutate((working, issues) =>
            {
                var index = IndexOf(working, id);
                if (index < 0)
                {
                    issues.Add(Error(MissingMessage));
                    return null;
                }

                if (working.Count <= 1)
                {
                    issues.Add(Error("No se puede eliminar la última plantilla."));
                    return null;
                }

                var removed = working[index];
                working.RemoveAt(index);
                return removed;
            });
        }

        /// <summary>Copia sin tiendas ni clave de fábrica, llamada «{Nombre} (copia)», «(copia 2)», …</summary>
        public TemplateSaveResult Duplicate(string id)
        {
            return Mutate((working, issues) =>
            {
                var index = IndexOf(working, id);
                if (index < 0)
                {
                    issues.Add(Error(MissingMessage));
                    return null;
                }

                var source = working[index];
                var now = DateTime.UtcNow;
                var copy = new SitemapTemplate(
                    Guid.NewGuid().ToString("N"),
                    MakeUniqueName(source.Name, n => n == 1 ? " (copia)" : " (copia " + n.ToString(CultureInfo.InvariantCulture) + ")", working, null),
                    source.Description,
                    null,
                    null,
                    now,
                    now,
                    source.SitemapJson);
                working.Add(copy);
                return copy;
            });
        }

        /// <summary>
        /// Importa un sitemap como plantilla nueva (sin tiendas ni clave de fábrica). El nombre es
        /// <paramref name="suggestedName"/> (p. ej. el nombre del archivo); si está vacío, el <c>_id</c> del JSON;
        /// si tampoco hay, «Plantilla importada». Se recorta a 60 caracteres y se desambigua con « (2)», « (3)»…
        /// </summary>
        public TemplateSaveResult Import(string json, string suggestedName)
        {
            return Mutate((working, issues) =>
            {
                var validation = SitemapTemplateValidator.Validate(json);
                issues.AddRange(validation.Issues);
                if (!validation.IsValid)
                {
                    return null;
                }

                var baseName = (suggestedName ?? string.Empty).Trim();
                if (baseName.Length == 0)
                {
                    baseName = ReadSitemapId(validation.Sitemap) ?? DefaultImportName;
                }

                var now = DateTime.UtcNow;
                var template = new SitemapTemplate(
                    Guid.NewGuid().ToString("N"),
                    MakeUniqueName(baseName, NumberSuffix, working, null),
                    string.Empty,
                    null,
                    null,
                    now,
                    now,
                    SitemapTemplateValidator.Normalize(validation.Sitemap));
                working.Add(template);
                return template;
            });
        }

        /// <summary>
        /// Restaura las plantillas de fábrica sin tocar las personalizadas. Una de fábrica existente (por
        /// <c>BuiltInKey</c>) recupera su sitemap y nombre originales (con sufijo « (2)» si el nombre lo ocupa otra
        /// plantilla) y suma las tiendas de fábrica que ninguna otra plantilla vincule; una faltante se recrea
        /// (id = clave si está libre, si no un GUID). Devuelve <see cref="TemplateSaveResult"/> con
        /// <c>Template == null</c>; si guardar falla, <c>Ok == false</c> y el estado no cambia.
        /// </summary>
        public TemplateSaveResult RestoreDefaults()
        {
            return Mutate((working, issues) =>
            {
                var now = DateTime.UtcNow;
                foreach (var factory in BuiltInSitemapTemplates.CreateDefaults(now))
                {
                    var index = working.FindIndex(t => t.BuiltInKey == factory.BuiltInKey);
                    if (index >= 0)
                    {
                        var existing = working[index];
                        working[index] = new SitemapTemplate(
                            existing.Id,
                            MakeUniqueName(factory.Name, NumberSuffix, working, existing.Id),
                            existing.Description,
                            existing.BuiltInKey,
                            existing.Stores.Concat(FreeFactoryStores(factory, working, existing.Id).Where(store => !LinksStore(existing, store))).ToArray(),
                            existing.CreatedUtc,
                            now,
                            factory.SitemapJson);
                    }
                    else
                    {
                        var id = working.Any(t => t.Id == factory.BuiltInKey) ? Guid.NewGuid().ToString("N") : factory.BuiltInKey;
                        working.Add(new SitemapTemplate(
                            id,
                            MakeUniqueName(factory.Name, NumberSuffix, working, null),
                            factory.Description,
                            factory.BuiltInKey,
                            FreeFactoryStores(factory, working, id).ToArray(),
                            now,
                            now,
                            factory.SitemapJson));
                    }
                }

                return null;
            });
        }

        private static string DefaultFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "S3Tools",
                "sitemap-templates.json");
        }

        /// <summary>Carga el archivo la primera vez. Se invoca siempre con <c>_sync</c> tomado.</summary>
        private void EnsureLoaded()
        {
            if (_templates != null)
            {
                return;
            }

            var loaded = _store.Load();
            _templates = new List<SitemapTemplate>(loaded.Templates);
            _loadWarning = loaded.Warning;
            _readOnlyReason = loaded.ReadOnlyReason;
        }

        /// <summary>
        /// Ejecuta <paramref name="body"/> sobre una copia de la lista. El cuerpo agrega errores a
        /// <c>issues</c> para abortar y devuelve la plantilla resultante (o <c>null</c>).
        /// </summary>
        private TemplateSaveResult Mutate(Func<List<SitemapTemplate>, List<TemplateIssue>, SitemapTemplate> body)
        {
            TemplateSaveResult result;
            lock (_sync)
            {
                result = MutateLocked(body);
            }

            if (result.Ok)
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }

        private TemplateSaveResult MutateLocked(Func<List<SitemapTemplate>, List<TemplateIssue>, SitemapTemplate> body)
        {
            EnsureLoaded();
            var issues = new List<TemplateIssue>();
            if (_readOnlyReason != StoreReadOnlyReason.None)
            {
                issues.Add(Error(_readOnlyReason == StoreReadOnlyReason.FutureSchema ? FutureSchemaMessage : IoErrorMessage));
                return new TemplateSaveResult(false, null, issues);
            }

            var working = new List<SitemapTemplate>(_templates);
            var template = body(working, issues);
            if (issues.Any(i => i.Severity == TemplateIssueSeverity.Error))
            {
                return new TemplateSaveResult(false, null, issues);
            }

            try
            {
                _store.Save(working);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                FileLogger.Error(Source, "No se pudo guardar el archivo de plantillas.", ex);
                issues.Add(Error("No se pudo guardar: " + ex.Message));
                return new TemplateSaveResult(false, null, issues);
            }

            _templates = working;
            return new TemplateSaveResult(true, template, issues);
        }

        /// <summary>
        /// Valida y normaliza el borrador. Agrega todos los errores encontrados (y las advertencias del
        /// validador) a <paramref name="issues"/>; devuelve <c>null</c> si hay algún error.
        /// </summary>
        private static PreparedDraft Prepare(SitemapTemplateDraft draft, List<SitemapTemplate> working, string selfId, List<TemplateIssue> issues)
        {
            var errorsBefore = issues.Count(i => i.Severity == TemplateIssueSeverity.Error);

            var name = (draft.Name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                issues.Add(Error("El nombre es obligatorio."));
            }
            else if (name.Length > MaxNameLength)
            {
                issues.Add(Error("El nombre no puede superar " + MaxNameLength + " caracteres."));
            }
            else if (working.Any(t => t.Id != selfId && string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                issues.Add(Error("Ya existe una plantilla llamada «" + name + "»."));
            }

            var description = draft.Description ?? string.Empty;
            if (description.Length > MaxDescriptionLength)
            {
                issues.Add(Error("La descripción no puede superar " + MaxDescriptionLength + " caracteres."));
            }

            var stores = CanonicalStores(draft.Stores, issues);

            var validation = SitemapTemplateValidator.Validate(draft.SitemapJson);
            issues.AddRange(validation.Issues);

            if (issues.Count(i => i.Severity == TemplateIssueSeverity.Error) > errorsBefore)
            {
                return null;
            }

            return new PreparedDraft(name, description, stores, SitemapTemplateValidator.Normalize(validation.Sitemap));
        }

        /// <summary>Convierte las tiendas del borrador al nombre canónico del catálogo, sin repetidas.</summary>
        private static string[] CanonicalStores(IReadOnlyList<string> requested, List<TemplateIssue> issues)
        {
            var result = new List<string>();
            if (requested == null)
            {
                return result.ToArray();
            }

            foreach (var store in requested)
            {
                var canonical = StoreCatalog.StoresLeft.Concat(StoreCatalog.StoresRight)
                    .FirstOrDefault(s => string.Equals(s, store == null ? null : store.Trim(), StringComparison.OrdinalIgnoreCase));
                if (canonical == null)
                {
                    issues.Add(Error("La tienda «" + store + "» no existe en el catálogo."));
                }
                else if (!result.Contains(canonical))
                {
                    result.Add(canonical);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Desvincula <paramref name="stores"/> de las demás plantillas (sustituyéndolas en
        /// <paramref name="working"/>) y agrega una advertencia por cada tienda movida.
        /// </summary>
        private static void MoveStores(List<SitemapTemplate> working, string ownerId, IEnumerable<string> stores, DateTime now, List<TemplateIssue> issues)
        {
            foreach (var store in stores)
            {
                for (var i = 0; i < working.Count; i++)
                {
                    var other = working[i];
                    if (other.Id == ownerId || !LinksStore(other, store))
                    {
                        continue;
                    }

                    working[i] = new SitemapTemplate(
                        other.Id,
                        other.Name,
                        other.Description,
                        other.BuiltInKey,
                        other.Stores.Where(s => !string.Equals(s, store, StringComparison.OrdinalIgnoreCase)).ToArray(),
                        other.CreatedUtc,
                        now,
                        other.SitemapJson);
                    issues.Add(new TemplateIssue(TemplateIssueSeverity.Warning, "La tienda " + store + " se desvinculó de «" + other.Name + "»."));
                }
            }
        }

        /// <summary>Tiendas de la plantilla de fábrica que ninguna otra plantilla (distinta de <paramref name="ownerId"/>) vincula.</summary>
        private static IEnumerable<string> FreeFactoryStores(SitemapTemplate factory, List<SitemapTemplate> working, string ownerId)
        {
            return factory.Stores.Where(store => !working.Any(t => t.Id != ownerId && LinksStore(t, store)));
        }

        /// <summary>
        /// Nombre libre derivado de <paramref name="baseName"/>: el propio nombre si está libre; si no, con el
        /// sufijo de <paramref name="suffixFor"/>(2, 3, …) o, para la primera copia, <c>suffixFor(1)</c>.
        /// El resultado siempre cabe en <see cref="MaxNameLength"/> (se recorta la base).
        /// </summary>
        private static string MakeUniqueName(string baseName, Func<int, string> suffixFor, List<SitemapTemplate> working, string ignoreId)
        {
            for (var n = 1; ; n++)
            {
                var suffix = suffixFor(n);
                var candidate = Fit(baseName, suffix);
                if (!working.Any(t => t.Id != ignoreId && string.Equals(t.Name, candidate, StringComparison.OrdinalIgnoreCase)))
                {
                    return candidate;
                }
            }
        }

        /// <summary>Sin sufijo para el primer intento; « (2)», « (3)»… para los siguientes.</summary>
        private static string NumberSuffix(int n)
        {
            return n == 1 ? string.Empty : " (" + n.ToString(CultureInfo.InvariantCulture) + ")";
        }

        private static string Fit(string baseName, string suffix)
        {
            var room = MaxNameLength - suffix.Length;
            var root = baseName.Length > room ? baseName.Substring(0, room).TrimEnd() : baseName;
            return root + suffix;
        }

        private static string ReadSitemapId(JsonObject sitemap)
        {
            if (sitemap.TryGetPropertyValue("_id", out var node) && node is JsonValue value && value.TryGetValue<string>(out var id))
            {
                id = id.Trim();
                return id.Length == 0 ? null : id;
            }

            return null;
        }

        private static int IndexOf(List<SitemapTemplate> templates, string id)
        {
            return id == null ? -1 : templates.FindIndex(t => t.Id == id);
        }

        private static bool LinksStore(SitemapTemplate template, string store)
        {
            return template.Stores.Any(s => string.Equals(s, store, StringComparison.OrdinalIgnoreCase));
        }

        private static TemplateIssue Error(string message)
        {
            return new TemplateIssue(TemplateIssueSeverity.Error, message);
        }

        private sealed class PreparedDraft
        {
            public PreparedDraft(string name, string description, string[] stores, string sitemapJson)
            {
                Name = name;
                Description = description;
                Stores = stores;
                SitemapJson = sitemapJson;
            }

            public string Name { get; }

            public string Description { get; }

            public string[] Stores { get; }

            public string SitemapJson { get; }
        }
    }
}
