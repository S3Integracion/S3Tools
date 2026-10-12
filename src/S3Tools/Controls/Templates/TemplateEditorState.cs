using System;
using System.Collections.Generic;
using System.Linq;

namespace S3Tools
{
    /// <summary>
    /// Estado del editor de plantillas, sin dependencias de UI: valores editables, valores originales
    /// (los últimos cargados o guardados) y detección de cambios sin guardar.
    /// <para>
    /// Reglas de comparación: nombre y descripción recortados (<c>null</c> equivale a vacío); tiendas como
    /// conjunto sin distinguir mayúsculas; JSON exacto como texto (reformatearlo cuenta como cambio).
    /// </para>
    /// </summary>
    internal sealed class TemplateEditorState
    {
        private const string StarterSitemap =
            "{ \"_id\": \"nueva_plantilla\", \"startUrl\": [], \"selectors\": [ { \"id\": \"titulo\", \"type\": \"SelectorText\", " +
            "\"parentSelectors\": [\"_root\"], \"selector\": \"h1\", \"multiple\": false, \"regex\": \"\" } ] }";

        private static readonly string StarterJson =
            ToCrLf(SitemapTemplateValidator.Normalize(SitemapTemplateValidator.Validate(StarterSitemap).Sitemap));

        private string _originalName = string.Empty;
        private string _originalDescription = string.Empty;
        private string[] _originalStores = Array.Empty<string>();
        private string _originalJson = StarterJson;
        private string[] _stores = Array.Empty<string>();

        public TemplateEditorState()
        {
            LoadNew();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>Tiendas marcadas; el valor asignado se copia (nunca es <c>null</c>).</summary>
        public IReadOnlyList<string> Stores
        {
            get { return _stores; }
            set { _stores = value == null ? Array.Empty<string>() : value.ToArray(); }
        }

        /// <summary>Texto JSON del sitemap tal como está en el editor (con saltos de línea CRLF).</summary>
        public string SitemapJson { get; set; }

        /// <summary>Verdadero cuando se edita una plantilla aún no guardada.</summary>
        public bool IsNew { get; private set; }

        /// <summary>Identificador de la plantilla en edición; <c>null</c> si es nueva.</summary>
        public string EditingId { get; private set; }

        /// <summary>Verdadero cuando algún valor difiere del original según las reglas de comparación.</summary>
        public bool IsDirty
        {
            get
            {
                return !string.Equals(Clean(Name), _originalName, StringComparison.Ordinal)
                    || !string.Equals(Clean(Description), _originalDescription, StringComparison.Ordinal)
                    || !new HashSet<string>(_stores, StringComparer.OrdinalIgnoreCase).SetEquals(_originalStores)
                    || !string.Equals(SitemapJson ?? string.Empty, _originalJson, StringComparison.Ordinal);
            }
        }

        /// <summary>Carga una plantilla existente; sus valores pasan a ser los originales (sin cambios).</summary>
        public void Load(SitemapTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            IsNew = false;
            EditingId = template.Id;
            SetOriginal(template.Name, template.Description, template.Stores, ToCrLf(template.SitemapJson ?? string.Empty));
        }

        /// <summary>Prepara una plantilla nueva: nombre vacío, sin tiendas y un sitemap inicial de ejemplo.</summary>
        public void LoadNew()
        {
            IsNew = true;
            EditingId = null;
            SetOriginal(string.Empty, string.Empty, null, StarterJson);
        }

        /// <summary>Borrador para el repositorio, con nombre y descripción recortados.</summary>
        public SitemapTemplateDraft ToDraft()
        {
            return new SitemapTemplateDraft
            {
                Name = Clean(Name),
                Description = Clean(Description),
                Stores = _stores.ToArray(),
                SitemapJson = SitemapJson,
            };
        }

        private void SetOriginal(string name, string description, IReadOnlyList<string> stores, string json)
        {
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            Stores = stores;
            SitemapJson = json;

            _originalName = Clean(Name);
            _originalDescription = Clean(Description);
            _originalStores = _stores.ToArray();
            _originalJson = json;
        }

        private static string Clean(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        /// <summary>El <c>TextBox</c> multilínea solo muestra saltos CRLF; se unifican para que la comparación sea estable.</summary>
        private static string ToCrLf(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }
    }
}
