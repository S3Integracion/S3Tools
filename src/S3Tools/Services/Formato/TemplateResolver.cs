using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace S3Tools
{
    /// <summary>
    /// Determina qué plantilla de WebScraper (tiendas / bbvs) corresponde a un archivo
    /// basándose en la similitud de sus encabezados.
    /// </summary>
    internal static class TemplateResolver
    {
        private const string DefaultTemplate = "tiendas";

        /// <summary>Archivos JSON de plantilla por clave (búsqueda case-insensitive).</summary>
        public static readonly IReadOnlyDictionary<string, string> TemplateFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "tiendas", "PlantillaSitemapsTiendas.json" },
            { "bbvs",    "PlantillaSitemapsBBvs.json" },
        };

        /// <summary>Encabezados estándar por formato seleccionado.</summary>
        public static readonly IReadOnlyDictionary<string, string[]> HeaderFormats = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "hyphen",     new[] { "web-scraper-order", "web-scraper-start-url" } },
            { "underscore", new[] { "web_scraper_order", "web_scraper_start_url" } },
        };

        private static readonly Regex NormalizeHeaderRegex      = new Regex("[^a-zA-Z0-9_']", RegexOptions.Compiled);
        private static readonly Regex MultipleUnderscoreRegex   = new Regex("_+", RegexOptions.Compiled);

        // Estado mutable estático protegido por lock — preserva el comportamiento original.
        private static readonly object CacheSync = new object();
        private static readonly Dictionary<string, TemplateDefinition> TemplateCache       = new Dictionary<string, TemplateDefinition>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, List<string>>       ExpectedHeadersCache = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Si <paramref name="choice"/> es explícito y válido lo respeta; si no, detecta por encabezados.</summary>
        public static string ResolveTemplate(string choice, IList<string> headers)
        {
            return TemplateFiles.ContainsKey(choice) ? choice : DetectTemplate(headers);
        }

        /// <summary>Devuelve la plantilla cuyos selectores coinciden mejor con los encabezados dados.</summary>
        public static string DetectTemplate(IList<string> headers)
        {
            var normalizedHeaders = new HashSet<string>(
                (headers ?? Array.Empty<string>())
                    .Select(NormalizeHeader)
                    .Where(h => !string.IsNullOrWhiteSpace(h)),
                StringComparer.OrdinalIgnoreCase);

            var best = DefaultTemplate;
            var bestCount = -1;

            foreach (var key in TemplateFiles.Keys)
            {
                var expected = new HashSet<string>(
                    GetExpectedHeaders(key)
                        .Select(NormalizeHeader)
                        .Where(h => !h.StartsWith("web_scraper", StringComparison.OrdinalIgnoreCase)),
                    StringComparer.OrdinalIgnoreCase);

                var count = normalizedHeaders.Intersect(expected, StringComparer.OrdinalIgnoreCase).Count();
                if (count > bestCount)
                {
                    bestCount = count;
                    best = key;
                }
            }

            return bestCount <= 0 ? DefaultTemplate : best;
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

        private static IEnumerable<string> GetExpectedHeaders(string templateKey)
        {
            lock (CacheSync)
            {
                if (ExpectedHeadersCache.TryGetValue(templateKey, out var cached))
                {
                    return cached;
                }
            }

            var output = new List<string> { "web_scraper_order", "web_scraper_start_url" };
            var template = LoadTemplateDefinition(templateKey);
            if (template?.Selectors != null)
            {
                foreach (var selector in template.Selectors)
                {
                    var type = (selector?.Type ?? string.Empty).Trim().ToLowerInvariant();
                    if (type.Contains("elementclick")) continue;

                    var id = (selector?.Id ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(id)) output.Add(id);
                }
            }

            lock (CacheSync)
            {
                ExpectedHeadersCache[templateKey] = output;
            }
            return output;
        }

        private static TemplateDefinition LoadTemplateDefinition(string templateKey)
        {
            lock (CacheSync)
            {
                if (TemplateCache.TryGetValue(templateKey, out var cached)) return cached;
            }

            if (!TemplateFiles.TryGetValue(templateKey, out var templateName))
            {
                throw new InvalidOperationException("Unknown template key: " + templateKey);
            }

            var path = ResolveTemplatePath(templateName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Sitemap template not found: " + templateName);
            }

            TemplateDefinition model;
            var serializer = new DataContractJsonSerializer(typeof(TemplateDefinition));
            using (var stream = File.OpenRead(path))
            {
                model = serializer.ReadObject(stream) as TemplateDefinition;
            }

            lock (CacheSync)
            {
                TemplateCache[templateKey] = model;
            }
            return model;
        }

        private static string ResolveTemplatePath(string templateName)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                templateName,
                Path.Combine(baseDir, templateName),
                Path.Combine(Environment.CurrentDirectory, templateName),
            };

            return candidates.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c) && File.Exists(c)) ?? templateName;
        }
    }
}
