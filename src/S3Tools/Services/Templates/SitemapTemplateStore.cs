using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>
    /// Almacén en disco de las plantillas de sitemap (un único archivo JSON). Escribe de forma
    /// atómica (<c>.tmp</c> + <c>File.Replace</c> con respaldo <c>.bak</c>) y al cargar recupera el
    /// respaldo, aísla archivos dañados y depura entradas inválidas. <see cref="Load"/> no lanza
    /// por contenido dañado: el arranque de la aplicación nunca debe fallar por este archivo.
    /// </summary>
    internal sealed class SitemapTemplateStore
    {
        /// <summary>Versión de esquema que esta aplicación sabe leer y escribir.</summary>
        public const int CurrentSchemaVersion = 1;

        private const int MaxIdLength = 64;
        private const string Source = nameof(SitemapTemplateStore);

        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private readonly string _filePath;

        public SitemapTemplateStore(string filePath)
        {
            _filePath = filePath;
        }

        private string BackupPath
        {
            get { return _filePath + ".bak"; }
        }

        private string TempPath
        {
            get { return _filePath + ".tmp"; }
        }

        /// <summary>
        /// Carga las plantillas aplicando la recuperación y depuración descritas en el diseño.
        /// Ante un fallo de E/S inesperado devuelve las plantillas de fábrica en memoria y solo lectura.
        /// </summary>
        public StoreLoadResult Load()
        {
            try
            {
                return LoadCore();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                FileLogger.Error(Source, "No se pudo acceder al archivo de plantillas " + _filePath, ex);
                return new StoreLoadResult(
                    BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow),
                    "No se pudo acceder al archivo de plantillas (" + ex.Message + "). Se usan las plantillas de fábrica y los cambios no se guardarán.",
                    StoreReadOnlyReason.IoError);
            }
        }

        /// <summary>
        /// Guarda <paramref name="templates"/> de forma atómica. Crea el directorio si falta y no deja
        /// el archivo temporal. Lanza <see cref="IOException"/> si no puede escribir.
        /// </summary>
        public void Save(IReadOnlyList<SitemapTemplate> templates)
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(_filePath));
            Directory.CreateDirectory(directory);

            var text = Serialize(templates);
            try
            {
                File.WriteAllText(TempPath, text, Utf8NoBom);
                if (File.Exists(_filePath))
                {
                    File.Replace(TempPath, _filePath, BackupPath);
                }
                else
                {
                    File.Move(TempPath, _filePath);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                DeleteQuietly(TempPath);
                throw;
            }
        }

        private StoreLoadResult LoadCore()
        {
            var main = TryRead(_filePath, out var mainFile, out var mainReason);
            if (main == ReadOutcome.Ok)
            {
                return Build(mainFile, false, null);
            }

            if (main == ReadOutcome.Missing && !File.Exists(BackupPath))
            {
                var seeded = BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow);
                Save(seeded);
                return new StoreLoadResult(seeded, null, StoreReadOnlyReason.None);
            }

            var backup = TryRead(BackupPath, out var backupFile, out var backupReason);
            if (backup == ReadOutcome.Ok)
            {
                FileLogger.Warn(Source, "Archivo principal no utilizable (" + (mainReason ?? "no existe") + "); se recupera el respaldo.");

                // Se conserva el archivo dañado aparte: al volver a guardar, File.Replace no debe
                // mandarlo a .bak (se perdería en el siguiente guardado).
                var preserved = main == ReadOutcome.Corrupt ? MoveCorruptAside() : null;
                return Build(backupFile, true, preserved);
            }

            FileLogger.Warn(Source, "Archivo principal (" + (mainReason ?? "no existe") + ") y respaldo (" + (backupReason ?? "no existe") + ") no utilizables.");
            var movedTo = main == ReadOutcome.Corrupt ? MoveCorruptAside() : null;
            var defaults = BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow);
            Save(defaults);
            var warning = movedTo != null
                ? "El archivo de plantillas estaba dañado y se guardó como " + Path.GetFileName(movedTo) + ". Se restauraron las plantillas de fábrica."
                : "No se pudo leer el respaldo de plantillas. Se restauraron las plantillas de fábrica.";
            return new StoreLoadResult(defaults, warning, StoreReadOnlyReason.None);
        }

        /// <summary>Depura el contenido leído y decide si hay que reescribirlo o dejarlo intacto.</summary>
        private StoreLoadResult Build(RawFile file, bool recoveredFromBackup, string preservedCorruptPath)
        {
            var warnings = new List<string>();
            if (recoveredFromBackup)
            {
                warnings.Add("El archivo de plantillas estaba dañado; se recuperó la copia de respaldo." +
                             (preservedCorruptPath != null ? " El archivo dañado se guardó como " + Path.GetFileName(preservedCorruptPath) + "." : string.Empty));
            }

            var future = file.SchemaVersion > CurrentSchemaVersion;
            if (future)
            {
                var message = "El archivo de plantillas usa una versión de esquema más nueva (" + file.SchemaVersion +
                              "); se abre en solo lectura y no se modificará.";
                FileLogger.Warn(Source, message);
                warnings.Add(message);
            }

            var templates = Sanitize(file.Entries, warnings);
            if (templates.Count == 0)
            {
                var message = "El archivo no contiene plantillas válidas; se usan las plantillas de fábrica.";
                FileLogger.Warn(Source, message);
                warnings.Add(message);
                templates = new List<SitemapTemplate>(BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow));
                if (!future)
                {
                    // El archivo principal legible pero sin entradas útiles se conserva aparte antes de resembrar.
                    if (File.Exists(_filePath))
                    {
                        var preserved = MoveCorruptAside();
                        warnings.Add("El archivo original se guardó como " + Path.GetFileName(preserved) + ".");
                    }

                    Save(templates);
                }
            }
            else if (recoveredFromBackup && !future)
            {
                Save(templates);
            }

            return new StoreLoadResult(templates, warnings.Count == 0 ? null : string.Join(Environment.NewLine, warnings), future ? StoreReadOnlyReason.FutureSchema : StoreReadOnlyReason.None);
        }

        private static List<SitemapTemplate> Sanitize(JsonArray entries, List<string> warnings)
        {
            var result = new List<SitemapTemplate>();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var linkedStores = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var now = DateTime.UtcNow;

            for (var i = 0; i < entries.Count; i++)
            {
                var position = i + 1;
                var entry = entries[i] as JsonObject;
                if (entry == null)
                {
                    Fix(warnings, "Se descartó la entrada #" + position + ": no es un objeto.");
                    continue;
                }

                var id = ReadString(entry, "id");
                if (!IsValidId(id))
                {
                    Fix(warnings, "Se descartó la entrada #" + position + ": identificador inválido.");
                    continue;
                }

                if (!ids.Add(id))
                {
                    Fix(warnings, "Se descartó la entrada #" + position + " (" + id + "): identificador duplicado.");
                    continue;
                }

                var name = (ReadString(entry, "name") ?? string.Empty).Trim();
                if (name.Length == 0)
                {
                    Fix(warnings, "Se descartó la entrada \"" + id + "\": no tiene nombre.");
                    continue;
                }

                var sitemapJson = ReadValidSitemap(entry, id, warnings);
                if (sitemapJson == null)
                {
                    continue;
                }

                var uniqueName = MakeUniqueName(name, names);
                if (uniqueName != name)
                {
                    Fix(warnings, "Se renombró \"" + name + "\" a \"" + uniqueName + "\" (nombre duplicado).");
                }

                var builtInKey = ReadString(entry, "builtInKey");
                if (builtInKey != null && builtInKey != BuiltInSitemapTemplates.TiendasKey && builtInKey != BuiltInSitemapTemplates.BBvsKey)
                {
                    Fix(warnings, "La plantilla \"" + uniqueName + "\" tenía una clave de fábrica desconocida (\"" + builtInKey + "\"); se ignoró.");
                    builtInKey = null;
                }

                result.Add(new SitemapTemplate(
                    id,
                    uniqueName,
                    ReadString(entry, "description"),
                    builtInKey,
                    ReadStores(entry, uniqueName, linkedStores, warnings),
                    ReadDate(entry, "createdUtc", now),
                    ReadDate(entry, "updatedUtc", now),
                    sitemapJson));
            }

            return result;
        }

        private static string ReadValidSitemap(JsonObject entry, string id, List<string> warnings)
        {
            var sitemap = entry["sitemap"] as JsonObject;
            if (sitemap == null)
            {
                Fix(warnings, "Se descartó la entrada \"" + id + "\": falta el sitemap.");
                return null;
            }

            var validation = SitemapTemplateValidator.Validate(sitemap.ToJsonString());
            if (!validation.IsValid)
            {
                Fix(warnings, "Se descartó la entrada \"" + id + "\": el sitemap no es válido.");
                return null;
            }

            return SitemapTemplateValidator.Normalize(validation.Sitemap);
        }

        private static string MakeUniqueName(string name, HashSet<string> usedNames)
        {
            var candidate = name;
            for (var n = 2; !usedNames.Add(candidate); n++)
            {
                candidate = name + " (" + n.ToString(CultureInfo.InvariantCulture) + ")";
            }

            return candidate;
        }

        private static List<string> ReadStores(JsonObject entry, string templateName, HashSet<string> linkedStores, List<string> warnings)
        {
            var stores = new List<string>();
            var array = entry["stores"] as JsonArray;
            if (array == null)
            {
                return stores;
            }

            foreach (var node in array)
            {
                var store = AsString(node);
                if (string.IsNullOrWhiteSpace(store))
                {
                    continue;
                }

                if (linkedStores.Add(store))
                {
                    stores.Add(store);
                }
                else
                {
                    Fix(warnings, "La tienda \"" + store + "\" estaba vinculada a más de una plantilla; se conserva solo la primera (se quitó de \"" + templateName + "\").");
                }
            }

            return stores;
        }

        private static DateTime ReadDate(JsonObject entry, string property, DateTime fallback)
        {
            var text = ReadString(entry, property);
            if (text != null && DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var value))
            {
                return value;
            }

            return fallback;
        }

        private static string ReadString(JsonObject obj, string property)
        {
            return obj.TryGetPropertyValue(property, out var node) ? AsString(node) : null;
        }

        private static string AsString(JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
        }

        private static bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length > MaxIdLength)
            {
                return false;
            }

            foreach (var c in id)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
                {
                    return false;
                }
            }

            return true;
        }

        private static void Fix(List<string> warnings, string message)
        {
            FileLogger.Warn(Source, message);
            warnings.Add(message);
        }

        private static string Serialize(IReadOnlyList<SitemapTemplate> templates)
        {
            var array = new JsonArray();
            foreach (var template in templates)
            {
                var sitemap = SitemapTemplateValidator.ParseTolerant(template.SitemapJson) as JsonObject;
                if (sitemap == null)
                {
                    throw new ArgumentException("El sitemap de la plantilla \"" + template.Id + "\" no es un objeto JSON.", nameof(templates));
                }

                var stores = new JsonArray();
                foreach (var store in template.Stores)
                {
                    stores.Add(store);
                }

                array.Add(new JsonObject
                {
                    ["id"] = template.Id,
                    ["name"] = template.Name,
                    ["description"] = template.Description,
                    ["builtInKey"] = template.BuiltInKey,
                    ["stores"] = stores,
                    ["createdUtc"] = FormatUtc(template.CreatedUtc),
                    ["updatedUtc"] = FormatUtc(template.UpdatedUtc),
                    ["sitemap"] = sitemap,
                });
            }

            var root = new JsonObject
            {
                ["schemaVersion"] = CurrentSchemaVersion,
                ["templates"] = array,
            };
            return root.ToJsonString(WriteOptions);
        }

        private static string FormatUtc(DateTime utc)
        {
            return utc.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Lee y valida la estructura mínima del archivo. Un texto que no sea JSON, sin raíz objeto,
        /// sin <c>templates</c> o con <c>schemaVersion</c> inválido se considera dañado; los errores de
        /// E/S se propagan. Con un esquema futuro, un <c>templates</c> irreconocible se trata como vacío.
        /// </summary>
        private static ReadOutcome TryRead(string path, out RawFile file, out string reason)
        {
            file = null;
            reason = null;
            if (!File.Exists(path))
            {
                return ReadOutcome.Missing;
            }

            JsonObject root;
            try
            {
                root = SitemapTemplateValidator.ParseTolerant(File.ReadAllText(path, Utf8NoBom)) as JsonObject;
            }
            catch (JsonException ex)
            {
                reason = "JSON inválido: " + ex.Message;
                return ReadOutcome.Corrupt;
            }

            if (root == null)
            {
                reason = "la raíz no es un objeto JSON";
                return ReadOutcome.Corrupt;
            }

            var version = CurrentSchemaVersion;
            if (root.TryGetPropertyValue("schemaVersion", out var versionNode) && versionNode != null)
            {
                if (!(versionNode is JsonValue versionValue) || !versionValue.TryGetValue<int>(out version) || version < 1)
                {
                    reason = "schemaVersion inválido";
                    return ReadOutcome.Corrupt;
                }
            }

            var entries = root["templates"] as JsonArray;
            if (entries == null)
            {
                if (version <= CurrentSchemaVersion)
                {
                    reason = "falta el arreglo \"templates\"";
                    return ReadOutcome.Corrupt;
                }

                entries = new JsonArray();
            }

            file = new RawFile(version, entries);
            return ReadOutcome.Ok;
        }

        /// <summary>Renombra el archivo principal dañado a <c>sitemap-templates.corrupt-yyyyMMddHHmmss.json</c>.</summary>
        private string MoveCorruptAside()
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(_filePath));
            var stamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var target = Path.Combine(directory, "sitemap-templates.corrupt-" + stamp + ".json");
            for (var n = 1; File.Exists(target); n++)
            {
                target = Path.Combine(directory, "sitemap-templates.corrupt-" + stamp + "-" + n.ToString(CultureInfo.InvariantCulture) + ".json");
            }

            File.Move(_filePath, target);
            FileLogger.Warn(Source, "Archivo dañado conservado como " + target);
            return target;
        }

        private static void DeleteQuietly(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                FileLogger.Warn(Source, "No se pudo eliminar el archivo temporal " + path, ex);
            }
        }

        private enum ReadOutcome
        {
            Missing,
            Corrupt,
            Ok,
        }

        private sealed class RawFile
        {
            public RawFile(int schemaVersion, JsonArray entries)
            {
                SchemaVersion = schemaVersion;
                Entries = entries;
            }

            public int SchemaVersion { get; }

            public JsonArray Entries { get; }
        }
    }
}
