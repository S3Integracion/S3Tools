using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>
    /// Construye payloads de sitemap a partir del JSON de una plantilla de WebScraper, asignando
    /// los campos <c>_id</c> y <c>startUrl</c> con los valores del usuario.
    /// </summary>
    internal static class SitemapJsonBuilder
    {
        private static readonly JsonSerializerOptions CompactOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Construye el payload a partir de la plantilla: interpreta el JSON (de forma tolerante),
        /// asigna <c>_id</c> y <c>startUrl</c> con los valores dados y serializa en una sola línea.
        /// Si la plantilla no los traía, se insertan al inicio para conservar la forma de WebScraper.
        /// Lanza <see cref="JsonException"/> si el texto no es JSON válido e
        /// <see cref="InvalidOperationException"/> si la raíz no es un objeto.
        /// </summary>
        public static string BuildSitemapPayload(string templateJson, string title, IList<string> urls)
        {
            var sitemap = SitemapTemplateValidator.ParseTolerant(templateJson) as JsonObject;
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
