using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace S3Tools
{
    /// <summary>
    /// Determina qué plantilla de WebScraper corresponde a un archivo basándose en la similitud
    /// de sus encabezados con los selectores de cada plantilla del repositorio.
    /// </summary>
    internal static class TemplateResolver
    {
        /// <summary>Valor de elección que activa la detección automática por encabezados.</summary>
        public const string AutoChoice = "auto";

        /// <summary>Mensaje cuando la plantilla elegida ya no existe (por ejemplo, la eliminaron del panel).</summary>
        public const string MissingTemplateMessage = "La plantilla seleccionada ya no existe.";

        /// <summary>Encabezados estándar por formato seleccionado.</summary>
        public static readonly IReadOnlyDictionary<string, string[]> HeaderFormats = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "hyphen",     new[] { "web-scraper-order", "web-scraper-start-url" } },
            { "underscore", new[] { "web_scraper_order", "web_scraper_start_url" } },
        };

        private static readonly Regex NormalizeHeaderRegex      = new Regex("[^a-zA-Z0-9_']", RegexOptions.Compiled);
        private static readonly Regex MultipleUnderscoreRegex   = new Regex("_+", RegexOptions.Compiled);

        // Estado mutable estático protegido por lock. La clave incluye UpdatedUtc, así que editar
        // una plantilla invalida solo su entrada.
        private static readonly object CacheSync = new object();
        private static readonly Dictionary<string, List<string>> ExpectedHeadersCache = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        /// <summary>
        /// Devuelve el id de la plantilla efectiva: con <c>"auto"</c> detecta por encabezados; si no,
        /// valida que el id exista. Lanza <see cref="InvalidOperationException"/> si ya no existe.
        /// </summary>
        public static string ResolveTemplate(string choice, IList<string> headers, IReadOnlyList<SitemapTemplate> templates)
        {
            if (templates == null) throw new ArgumentNullException(nameof(templates));

            if (string.Equals(choice, AutoChoice, StringComparison.OrdinalIgnoreCase))
            {
                return DetectTemplate(headers, templates);
            }

            var match = templates.FirstOrDefault(t => string.Equals(t.Id, choice, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                throw new InvalidOperationException(MissingTemplateMessage);
            }
            return match.Id;
        }

        /// <summary>
        /// Devuelve el id de la plantilla cuyos selectores coinciden mejor con los encabezados dados.
        /// Sin coincidencias, se usa la de fábrica "tiendas" o, si no existe, la primera; en empate gana la primera de la lista.
        /// </summary>
        public static string DetectTemplate(IList<string> headers, IReadOnlyList<SitemapTemplate> templates)
        {
            if (templates == null) throw new ArgumentNullException(nameof(templates));
            if (templates.Count == 0)
            {
                throw new InvalidOperationException("No hay plantillas de sitemap disponibles.");
            }

            var normalizedHeaders = new HashSet<string>(
                (headers ?? Array.Empty<string>())
                    .Select(NormalizeHeader)
                    .Where(h => !string.IsNullOrWhiteSpace(h)),
                StringComparer.OrdinalIgnoreCase);

            SitemapTemplate best = null;
            var bestCount = 0;

            foreach (var template in templates)
            {
                var expected = new HashSet<string>(
                    GetExpectedHeaders(template)
                        .Select(NormalizeHeader)
                        .Where(h => !h.StartsWith("web_scraper", StringComparison.OrdinalIgnoreCase)),
                    StringComparer.OrdinalIgnoreCase);

                var count = normalizedHeaders.Intersect(expected, StringComparer.OrdinalIgnoreCase).Count();
                if (count > bestCount)
                {
                    bestCount = count;
                    best = template;
                }
            }

            if (best != null) return best.Id;

            var fallback = templates.FirstOrDefault(t => t.BuiltInKey == BuiltInSitemapTemplates.TiendasKey) ?? templates[0];
            return fallback.Id;
        }

        /// <summary>Normaliza un encabezado a su forma canónica (lowercase, underscore-separated).</summary>
        public static string NormalizeHeader(string value)
        {
            var text = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.Replace("-", "_").Replace(" ", "_");
            text = NormalizeHeaderRegex.Replace(text, "_");
            text = MultipleUnderscoreRegex.Replace(text, "_");
            return text.ToLowerInvariant();
        }

        private static IEnumerable<string> GetExpectedHeaders(SitemapTemplate template)
        {
            var cacheKey = template.Id + "|" + template.UpdatedUtc.Ticks;

            lock (CacheSync)
            {
                if (ExpectedHeadersCache.TryGetValue(cacheKey, out var cached))
                {
                    return cached;
                }
            }

            var output = new List<string> { "web_scraper_order", "web_scraper_start_url" };
            output.AddRange(ReadSelectorIds(template));

            lock (CacheSync)
            {
                // Descarta versiones anteriores de la misma plantilla para que la caché no crezca con cada edición.
                var prefix = template.Id + "|";
                foreach (var stale in ExpectedHeadersCache.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList())
                {
                    ExpectedHeadersCache.Remove(stale);
                }
                ExpectedHeadersCache[cacheKey] = output;
            }
            return output;
        }

        /// <summary>Ids de los selectores que producen columnas (se omiten los de clic de elemento).</summary>
        private static List<string> ReadSelectorIds(SitemapTemplate template)
        {
            var ids = new List<string>();

            JsonNode root;
            try
            {
                root = SitemapTemplateValidator.ParseTolerant(template.SitemapJson ?? string.Empty);
            }
            catch (JsonException ex)
            {
                FileLogger.Warn(nameof(TemplateResolver), "La plantilla «" + template.Name + "» no se pudo interpretar; se ignora al detectar.", ex);
                return ids;
            }

            var selectors = (root as JsonObject)?["selectors"] as JsonArray;
            if (selectors == null) return ids;

            foreach (var node in selectors)
            {
                var selector = node as JsonObject;
                if (selector == null) continue;

                var type = (ReadString(selector, "type") ?? string.Empty).Trim().ToLowerInvariant();
                if (type.Contains("elementclick")) continue;

                var id = (ReadString(selector, "id") ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(id)) ids.Add(id);
            }
            return ids;
        }

        private static string ReadString(JsonObject obj, string name)
        {
            return obj[name] is JsonValue value && value.TryGetValue(out string text) ? text : null;
        }
    }
}
