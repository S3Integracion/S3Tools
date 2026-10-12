using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>
    /// Carga las plantillas JSON de WebScraper y construye payloads asignando
    /// los campos <c>_id</c> y <c>startUrl</c> con los valores del usuario.
    /// </summary>
    internal static class SitemapJsonBuilder
    {
        private const string TemplateTiendas = "PlantillaSitemapsTiendas.json";
        private const string TemplateBbvs    = "PlantillaSitemapsBBvs.json";

        private static readonly JsonSerializerOptions CompactOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

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
        /// Construye el payload a partir de la plantilla: interpreta el JSON (de forma tolerante),
        /// asigna <c>_id</c> y <c>startUrl</c> con los valores dados y serializa en una sola línea.
        /// Si la plantilla no los traía, se insertan al inicio para conservar la forma de WebScraper.
        /// Lanza <see cref="JsonException"/> si el texto no es JSON válido e
        /// <see cref="InvalidOperationException"/> si la raíz no es un objeto.
        /// </summary>
        public static string BuildSitemapPayload(string templateText, string title, IList<string> urls)
        {
            var sitemap = SitemapTemplateValidator.ParseTolerant(templateText) as JsonObject;
            if (sitemap == null)
            {
                throw new InvalidOperationException("La plantilla de sitemap debe ser un objeto JSON.");
            }

            var startUrl = new JsonArray();
            foreach (var url in urls)
            {
                startUrl.Add(JsonValue.Create(url ?? string.Empty));
            }

            SetOrInsertFirst(sitemap, "_id", JsonValue.Create(title ?? string.Empty), 0);
            SetOrInsertFirst(sitemap, "startUrl", startUrl, 1);

            return sitemap.ToJsonString(CompactOptions);
        }

        private static void SetOrInsertFirst(JsonObject sitemap, string name, JsonNode value, int index)
        {
            if (sitemap.ContainsKey(name))
            {
                sitemap[name] = value;
            }
            else
            {
                sitemap.Insert(Math.Min(index, sitemap.Count), name, value);
            }
        }
    }
}
