using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace S3Tools
{
    /// <summary>
    /// Extrae URLs absolutas (http/https) desde archivos de entrada de cualquier tipo soportado.
    /// </summary>
    internal static class UrlExtractor
    {
        private static readonly Regex UrlRegex = new Regex(
            "https?://[^\\s\"']+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>Lee URLs desde .txt, .csv, .json (texto) o .xlsx.</summary>
        public static List<string> ReadUrlsFromFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".xlsx") return ReadUrlsFromExcel(path);
            if (ext == ".xls")  throw new NotSupportedException("Legacy .xls input is not supported in .NET mode. Convert to .xlsx or .txt.");

            var text = ReadTextFallback(path);
            return string.IsNullOrWhiteSpace(text) ? new List<string>() : MatchUrls(text);
        }

        private static List<string> ReadUrlsFromExcel(string path)
        {
            var urls = new List<string>();
            using (var wb = new XLWorkbook(path))
            {
                foreach (var ws in wb.Worksheets)
                {
                    var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                    var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
                    for (var r = 1; r <= lastRow; r++)
                    {
                        for (var c = 1; c <= lastCol; c++)
                        {
                            var value = ws.Cell(r, c).GetString();
                            if (string.IsNullOrWhiteSpace(value)) continue;
                            urls.AddRange(MatchUrls(value));
                        }
                    }
                }
            }
            return urls;
        }

        private static List<string> MatchUrls(string text)
        {
            var urls = new List<string>();
            foreach (Match match in UrlRegex.Matches(text))
            {
                if (match.Success && !string.IsNullOrWhiteSpace(match.Value))
                {
                    urls.Add(match.Value);
                }
            }
            return urls;
        }

        /// <summary>
        /// Intenta leer el archivo con UTF-8 (con/sin BOM) y luego latin1.
        /// Si todos fallan, devuelve <see cref="File.ReadAllText(string)"/> (el sistema decide).
        /// </summary>
        private static string ReadTextFallback(string path)
        {
            string text;
            if (TryRead(path, new UTF8Encoding(true),  out text)) return text;
            if (TryRead(path, new UTF8Encoding(false), out text)) return text;
            if (TryRead(path, Encoding.GetEncoding("latin1"), out text)) return text;
            return File.ReadAllText(path);
        }

        private static bool TryRead(string path, Encoding encoding, out string text)
        {
            try
            {
                using (var reader = new StreamReader(path, encoding, true))
                {
                    text = reader.ReadToEnd();
                    return true;
                }
            }
            catch (DecoderFallbackException) { text = null; return false; }
            catch (IOException)               { text = null; return false; }
        }
    }
}
