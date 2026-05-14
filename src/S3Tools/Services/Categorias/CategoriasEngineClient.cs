using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace S3Tools
{
    internal sealed class CategoriasEngineClient
    {
        public Task<CategoriasLoadResponse> LoadCategoriesAsync()
        {
            return Task.Run(() => CategoriasDotNetEngine.LoadCategories());
        }

        public Task<CategoriasAnalyzeResponse> AnalyzeUrlAsync(string url)
        {
            return Task.Run(() => CategoriasDotNetEngine.AnalyzeUrl(url));
        }

        public Task<CategoriasGenerateResponse> GenerateAsync(CategoriasGenerateRequest request)
        {
            return Task.Run(() => CategoriasDotNetEngine.Generate(request));
        }

        public Task<CategoriasExportResponse> ExportAsync(CategoriasExportRequest request)
        {
            return Task.Run(() => CategoriasDotNetEngine.Export(request));
        }
    }

    internal sealed class CategoriaAmazon
    {
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public string Nodo { get; set; }
        public string Plantilla { get; set; }
        public bool Activo { get; set; }
        public int Orden { get; set; }
    }

    internal sealed class UrlGenerada
    {
        public string Categoria { get; set; }
        public string Pagina { get; set; }
        public string Tienda { get; set; }
        public string Url { get; set; }
        public string Plantilla { get; set; }
    }

    internal sealed class CategoriasLoadResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public CategoriaAmazon[] Categorias { get; set; }
        public string[] PlantillasInvalidas { get; set; }
        public string SourcePath { get; set; }
    }

    internal sealed class CategoriasAnalyzeResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public string Tienda { get; set; }
    }

    internal sealed class CategoriasGenerateRequest
    {
        public string Tienda { get; set; }
        public CategoriaAmazon[] CategoriasSeleccionadas { get; set; }
    }

    internal sealed class CategoriasGenerateResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public UrlGenerada[] Urls { get; set; }
    }

    internal sealed class CategoriasExportRequest
    {
        public UrlGenerada[] Urls { get; set; }
        public string FilePath { get; set; }
        public string Format { get; set; }
    }

    internal sealed class CategoriasExportResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public string FilePath { get; set; }
    }

    internal static class CategoriasDotNetEngine
    {
        private const string TemplateFileName = "PlantillaCategoriasAmazon.json";
        private const string StoreMarker = "{store}";
        private const string PageMarker = "{page}";
        public const int MaxAllowedPages = 1000;

        private static readonly Regex StoreIdPatternEncoded = new Regex(
            "p_6%3A([A-Z0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex StoreIdPatternDecoded = new Regex(
            "p_6:([A-Z0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex StoreIdPatternSeller = new Regex(
            "[?&]seller=([A-Z0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex StoreIdPatternMe = new Regex(
            "[?&]me=([A-Z0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex StoreIdShape = new Regex(
            "^[A-Z0-9]{8,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] AmazonHosts =
        {
            "amazon.com",
            "www.amazon.com",
            "amazon.com.mx",
            "www.amazon.com.mx",
        };

        public static CategoriasLoadResponse LoadCategories()
        {
            try
            {
                var path = ResolveTemplatePath();
                if (!File.Exists(path))
                {
                    return new CategoriasLoadResponse
                    {
                        Ok = false,
                        Error = "No se encontró el archivo " + TemplateFileName + ".",
                        Traceback = string.Empty,
                        Categorias = Array.Empty<CategoriaAmazon>(),
                        PlantillasInvalidas = Array.Empty<string>(),
                        SourcePath = path,
                    };
                }

                var json = File.ReadAllText(path, Encoding.UTF8);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var raw = JsonSerializer.Deserialize<List<CategoriaAmazon>>(json, options) ?? new List<CategoriaAmazon>();

                var validas = new List<CategoriaAmazon>();
                var invalidas = new List<string>();

                foreach (var cat in raw)
                {
                    if (cat == null || string.IsNullOrWhiteSpace(cat.Nombre))
                    {
                        continue;
                    }

                    if (!cat.Activo)
                    {
                        continue;
                    }

                    if (!IsTemplateValid(cat.Plantilla))
                    {
                        invalidas.Add(cat.Nombre);
                        continue;
                    }

                    validas.Add(cat);
                }

                var ordered = validas
                    .OrderBy(c => c.Orden)
                    .ThenBy(c => c.Nombre, StringComparer.CurrentCultureIgnoreCase)
                    .ToArray();

                return new CategoriasLoadResponse
                {
                    Ok = true,
                    Categorias = ordered,
                    PlantillasInvalidas = invalidas.ToArray(),
                    SourcePath = path,
                };
            }
            catch (Exception ex)
            {
                return new CategoriasLoadResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                    Categorias = Array.Empty<CategoriaAmazon>(),
                    PlantillasInvalidas = Array.Empty<string>(),
                };
            }
        }

        public static CategoriasAnalyzeResponse AnalyzeUrl(string url)
        {
            try
            {
                var trimmed = (url ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    return AnalyzeError("La URL no puede estar vacia.");
                }

                if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var parsed))
                {
                    return AnalyzeError("La entrada no tiene un formato de URL valido.");
                }

                if (!IsAmazonHost(parsed.Host))
                {
                    return AnalyzeError("La URL debe pertenecer a amazon.com o amazon.com.mx.");
                }

                var tienda = ExtractStoreId(trimmed);
                if (string.IsNullOrWhiteSpace(tienda))
                {
                    return AnalyzeError("No se encontro un identificador de tienda compatible en la URL ingresada.");
                }

                if (!StoreIdShape.IsMatch(tienda))
                {
                    return AnalyzeError("El identificador detectado no cumple con el formato esperado.");
                }

                return new CategoriasAnalyzeResponse
                {
                    Ok = true,
                    Tienda = tienda.ToUpperInvariant(),
                };
            }
            catch (Exception ex)
            {
                return new CategoriasAnalyzeResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                };
            }
        }

        public static CategoriasGenerateResponse Generate(CategoriasGenerateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return GenerateError("Solicitud invalida.");
                }

                var tienda = (request.Tienda ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(tienda) || !StoreIdShape.IsMatch(tienda))
                {
                    return GenerateError("Identificador de tienda invalido.");
                }

                var categorias = (request.CategoriasSeleccionadas ?? Array.Empty<CategoriaAmazon>())
                    .Where(c => c != null && IsTemplateValid(c.Plantilla))
                    .ToArray();

                if (categorias.Length == 0)
                {
                    return GenerateError("Selecciona al menos una categoria para generar URLs.");
                }

                const string defaultRangeToken = "[1-1]";
                var urls = new List<UrlGenerada>(categorias.Length);

                foreach (var cat in categorias)
                {
                    urls.Add(new UrlGenerada
                    {
                        Categoria = cat.Nombre,
                        Pagina = defaultRangeToken,
                        Tienda = tienda,
                        Url = BuildUrl(cat.Plantilla, tienda, defaultRangeToken),
                        Plantilla = cat.Plantilla,
                    });
                }

                return new CategoriasGenerateResponse
                {
                    Ok = true,
                    Urls = urls.ToArray(),
                };
            }
            catch (Exception ex)
            {
                return new CategoriasGenerateResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                    Urls = Array.Empty<UrlGenerada>(),
                };
            }
        }

        public static CategoriasExportResponse Export(CategoriasExportRequest request)
        {
            try
            {
                if (request == null || request.Urls == null || request.Urls.Length == 0)
                {
                    return ExportError("No hay URLs para exportar.");
                }

                var path = (request.FilePath ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(path))
                {
                    return ExportError("Selecciona una ruta de salida.");
                }

                var format = (request.Format ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(format))
                {
                    format = (Path.GetExtension(path) ?? string.Empty).TrimStart('.').ToLowerInvariant();
                }

                switch (format)
                {
                    case "txt":
                        WriteTxt(path, request.Urls);
                        break;
                    case "csv":
                        WriteCsv(path, request.Urls);
                        break;
                    case "json":
                        WriteJson(path, request.Urls);
                        break;
                    default:
                        return ExportError("Formato de exportacion no soportado: " + format);
                }

                return new CategoriasExportResponse
                {
                    Ok = true,
                    FilePath = path,
                };
            }
            catch (Exception ex)
            {
                return new CategoriasExportResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                };
            }
        }

        public static string BuildUrl(string plantilla, string tienda, int page)
        {
            return BuildUrl(plantilla, tienda, page.ToString());
        }

        public static string BuildUrl(string plantilla, string tienda, string pageToken)
        {
            var template = plantilla ?? string.Empty;
            return template
                .Replace(StoreMarker, tienda ?? string.Empty)
                .Replace(PageMarker, pageToken ?? string.Empty);
        }

        public static bool IsTemplateValid(string plantilla)
        {
            if (string.IsNullOrWhiteSpace(plantilla))
            {
                return false;
            }

            return plantilla.Contains(StoreMarker, StringComparison.Ordinal)
                && plantilla.Contains(PageMarker, StringComparison.Ordinal);
        }

        private static string ExtractStoreId(string url)
        {
            var candidate = TryMatch(StoreIdPatternEncoded, url);
            if (!string.IsNullOrEmpty(candidate))
            {
                return candidate;
            }

            candidate = TryMatch(StoreIdPatternSeller, url);
            if (!string.IsNullOrEmpty(candidate))
            {
                return candidate;
            }

            candidate = TryMatch(StoreIdPatternMe, url);
            if (!string.IsNullOrEmpty(candidate))
            {
                return candidate;
            }

            try
            {
                var decoded = Uri.UnescapeDataString(url);
                candidate = TryMatch(StoreIdPatternDecoded, decoded);
                if (!string.IsNullOrEmpty(candidate))
                {
                    return candidate;
                }

                candidate = TryMatch(StoreIdPatternSeller, decoded);
                if (!string.IsNullOrEmpty(candidate))
                {
                    return candidate;
                }

                candidate = TryMatch(StoreIdPatternMe, decoded);
                if (!string.IsNullOrEmpty(candidate))
                {
                    return candidate;
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        private static string TryMatch(Regex pattern, string input)
        {
            var match = pattern.Match(input);
            if (match.Success && match.Groups.Count > 1)
            {
                var value = match.Groups[1].Value;
                if (StoreIdShape.IsMatch(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }

        private static bool IsAmazonHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            var normalized = host.Trim().ToLowerInvariant();
            return AmazonHosts.Any(h => string.Equals(h, normalized, StringComparison.Ordinal));
        }

        private static string ResolveTemplatePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            var direct = Path.Combine(baseDir, TemplateFileName);
            if (File.Exists(direct))
            {
                return direct;
            }

            var current = new DirectoryInfo(baseDir);
            for (var i = 0; i < 6 && current != null; i++)
            {
                var candidate = Path.Combine(current.FullName, TemplateFileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                current = current.Parent;
            }

            return direct;
        }

        private static void WriteTxt(string path, IEnumerable<UrlGenerada> urls)
        {
            var lines = urls.Select(u => u.Url ?? string.Empty);
            File.WriteAllLines(path, lines, new UTF8Encoding(false));
        }

        private static void WriteCsv(string path, IEnumerable<UrlGenerada> urls)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Categoria,Pagina,Tienda,Url");
            foreach (var u in urls)
            {
                sb.Append(EscapeCsv(u.Categoria));
                sb.Append(',');
                sb.Append(u.Pagina);
                sb.Append(',');
                sb.Append(EscapeCsv(u.Tienda));
                sb.Append(',');
                sb.Append(EscapeCsv(u.Url));
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        }

        private static void WriteJson(string path, IEnumerable<UrlGenerada> urls)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            var json = JsonSerializer.Serialize(urls.ToArray(), options);
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        private static string EscapeCsv(string value)
        {
            var text = value ?? string.Empty;
            if (text.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0)
            {
                return text;
            }
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }

        private static CategoriasAnalyzeResponse AnalyzeError(string message)
        {
            return new CategoriasAnalyzeResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
            };
        }

        private static CategoriasGenerateResponse GenerateError(string message)
        {
            return new CategoriasGenerateResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
                Urls = Array.Empty<UrlGenerada>(),
            };
        }

        private static CategoriasExportResponse ExportError(string message)
        {
            return new CategoriasExportResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
            };
        }
    }
}
