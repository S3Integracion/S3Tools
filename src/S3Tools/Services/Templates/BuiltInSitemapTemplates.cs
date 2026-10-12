using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace S3Tools
{
    /// <summary>Plantillas de fábrica, embebidas en el ensamblado, con las que se siembra el almacén.</summary>
    internal static class BuiltInSitemapTemplates
    {
        public const string TiendasKey = "tiendas";
        public const string BBvsKey = "bbvs";

        private const string ResourcePrefix = "S3Tools.Templates.";

        /// <summary>Crea las plantillas de fábrica con sus tiendas por defecto.</summary>
        public static IReadOnlyList<SitemapTemplate> CreateDefaults(DateTime utcNow)
        {
            return new[]
            {
                CreateDefault(TiendasKey, "Tiendas (Normal)", StoreCatalog.StoresLeft, utcNow),
                CreateDefault(BBvsKey, "BBvs (Nube)", StoreCatalog.StoresRight, utcNow),
            };
        }

        /// <summary>Lee el JSON embebido de la plantilla de fábrica <paramref name="builtInKey"/>.</summary>
        public static string GetSitemapJson(string builtInKey)
        {
            if (builtInKey != TiendasKey && builtInKey != BBvsKey)
            {
                throw new ArgumentException("Clave de plantilla de fábrica desconocida: " + builtInKey, nameof(builtInKey));
            }

            var resourceName = ResourcePrefix + builtInKey + ".json";
            using (var stream = typeof(BuiltInSitemapTemplates).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Falta el recurso embebido " + resourceName + ".");
                }

                using (var reader = new StreamReader(stream, new UTF8Encoding(false), true))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static SitemapTemplate CreateDefault(string key, string name, string[] stores, DateTime utcNow)
        {
            // Se normaliza para que el texto coincida con el que devolverá una carga posterior del archivo.
            var sitemap = SitemapTemplateValidator.Validate(GetSitemapJson(key)).Sitemap;
            return new SitemapTemplate(
                key,
                name,
                string.Empty,
                key,
                (string[])stores.Clone(),
                utcNow,
                utcNow,
                SitemapTemplateValidator.Normalize(sitemap));
        }
    }
}
