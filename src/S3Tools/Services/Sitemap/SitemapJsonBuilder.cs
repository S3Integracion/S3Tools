using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace S3Tools
{
    /// <summary>
    /// Carga las plantillas JSON de WebScraper y construye payloads sustituyendo
    /// los campos <c>_id</c> y <c>startUrl</c> con los valores del usuario.
    /// </summary>
    internal static class SitemapJsonBuilder
    {
        private const string TemplateTiendas = "PlantillaSitemapsTiendas.json";
        private const string TemplateBbvs    = "PlantillaSitemapsBBvs.json";

        private static readonly Regex IdFieldRegex = new Regex(
            "\"_id\"\\s*:\\s*\".*?\"",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StartUrlFieldRegex = new Regex(
            "\"startUrl\"\\s*:\\s*\\[[\\s\\S]*?\\]",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>Selecciona la plantilla según el modo declarado por el usuario.</summary>
        public static string SelectTemplate(string templateMode)
        {
            return string.Equals(templateMode, "nube", StringComparison.OrdinalIgnoreCase)
                ? TemplateBbvs
                : TemplateTiendas;
        }

        /// <summary>
        /// Carga el contenido textual de la plantilla buscándola en varias rutas conocidas.
        /// Lanza <see cref="FileNotFoundException"/> si no se encuentra.
        /// </summary>
        public static string LoadTemplateText(string templateName)
        {
            var candidates = new[]
            {
                templateName,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateName),
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return File.ReadAllText(candidate, new UTF8Encoding(false));
                }
            }

            throw new FileNotFoundException("Sitemap template not found: " + templateName);
        }

        /// <summary>
        /// Sustituye <c>_id</c> y <c>startUrl</c> de la plantilla con los valores dados,
        /// preservando el resto del JSON original. La sustitución es textual y no
        /// re-formatea el JSON (intencional, para mantener orden de campos).
        /// </summary>
        public static string BuildSitemapPayload(string templateText, string title, IList<string> urls)
        {
            var idJson = "\"_id\":\"" + EscapeJson(title) + "\"";
            var startUrlsJson = "\"startUrl\":[" + string.Join(",", urls.Select(u => "\"" + EscapeJson(u) + "\"")) + "]";

            var withId        = IdFieldRegex.Replace(templateText, idJson, 1);
            var withStartUrls = StartUrlFieldRegex.Replace(withId, startUrlsJson, 1);
            return withStartUrls;
        }

        private static string EscapeJson(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}
