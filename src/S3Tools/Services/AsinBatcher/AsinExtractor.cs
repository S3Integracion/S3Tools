using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace S3Tools
{
    /// <summary>
    /// Extrae ASINs desde archivos de entrada (.txt, .xlsx).
    /// Detecta automáticamente reportes de inventario y aplica la lectura adecuada.
    /// </summary>
    internal static class AsinExtractor
    {
        private static readonly Regex InventoryReportRegex = new Regex(
            "^Reporte\\+de\\+inventario\\+\\d{2}-\\d{2}-\\d{4}\\.(txt|xlsx|xls)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>Punto de entrada: detecta extensión y delega en el lector apropiado.</summary>
        public static ExtractionResult ExtractAsinsAny(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            List<string> asins;

            if (ext == ".xlsx")
            {
                asins = IsInventoryReport(path)
                    ? ReadAsinsFromInventoryExcel(path)
                    : ReadAsinsFromFirstExcelColumn(path);
            }
            else if (ext == ".xls")
            {
                throw new NotSupportedException("Legacy .xls input is not supported in .NET mode. Convert to .xlsx or .txt.");
            }
            else if (ext == ".txt")
            {
                asins = IsInventoryReport(path)
                    ? ReadAsinsFromInventoryTxt(path)
                    : ReadAsinsFromPlainTxt(path);
            }
            else
            {
                asins = ReadAsinsFromPlainTxt(path);
            }

            return Deduplicate(asins);
        }

        /// <summary>Limpia un valor para dejar solo letras/dígitos en mayúsculas.</summary>
        public static string CleanAsin(string value)
        {
            var upper = (value ?? string.Empty).Trim().ToUpperInvariant();
            var chars = upper.Where(char.IsLetterOrDigit).ToArray();
            return new string(chars);
        }

        private static ExtractionResult Deduplicate(List<string> asins)
        {
            var uniques = new List<string>();
            var dups = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var asin in asins)
            {
                if (seen.Contains(asin))
                {
                    dups.Add(asin);
                }
                else
                {
                    seen.Add(asin);
                    uniques.Add(asin);
                }
            }

            return new ExtractionResult(uniques, dups);
        }

        private static bool IsInventoryReport(string fileName)
        {
            var baseName = Path.GetFileName(fileName) ?? string.Empty;
            if (InventoryReportRegex.IsMatch(baseName))
            {
                return true;
            }

            try
            {
                using (var reader = new StreamReader(fileName, Encoding.UTF8, true))
                {
                    var first = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(first) || first.IndexOf('\t') < 0)
                    {
                        return false;
                    }
                    var headers = first.Split('\t').Select(h => (h ?? string.Empty).Trim().ToLowerInvariant());
                    return headers.Contains("asin");
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static List<string> ReadAsinsFromInventoryTxt(string path)
        {
            var rows = new List<string[]>();
            foreach (var encName in new[] { "utf-8", "latin1" })
            {
                try
                {
                    using (var reader = new StreamReader(path, Encoding.GetEncoding(encName), true))
                    {
                        rows.Clear();
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            rows.Add((line ?? string.Empty).Split('\t'));
                        }
                    }
                    break;
                }
                catch (DecoderFallbackException)
                {
                    rows.Clear();
                }
                catch (IOException)
                {
                    rows.Clear();
                }
            }

            if (rows.Count == 0)
            {
                return new List<string>();
            }

            var header = rows[0].Select(c => (c ?? string.Empty).Trim().ToLowerInvariant()).ToList();
            var asins = new List<string>();

            if (header.Contains("asin"))
            {
                var idx = header.IndexOf("asin");
                for (var i = 1; i < rows.Count; i++)
                {
                    if (idx >= rows[i].Length) continue;
                    var cell = (rows[i][idx] ?? string.Empty).Trim().ToUpperInvariant();
                    var m = RegexPatterns.Asin.Match(cell);
                    if (!m.Success) continue;
                    var clean = CleanAsin(m.Value);
                    if (!string.IsNullOrWhiteSpace(clean)) asins.Add(clean);
                }
                return asins;
            }

            foreach (var row in rows)
            {
                foreach (var cell in row)
                {
                    var m = RegexPatterns.Asin.Match((cell ?? string.Empty).Trim().ToUpperInvariant());
                    if (!m.Success) continue;
                    var clean = CleanAsin(m.Value);
                    if (!string.IsNullOrWhiteSpace(clean)) asins.Add(clean);
                }
            }

            return asins;
        }

        private static List<string> ReadAsinsFromInventoryExcel(string path)
        {
            var asins = new List<string>();
            using (var wb = new XLWorkbook(path))
            {
                var ws = wb.Worksheets.FirstOrDefault();
                if (ws == null) return asins;

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
                if (lastRow <= 0 || lastCol <= 0) return asins;

                var asinCol = -1;
                for (var c = 1; c <= lastCol; c++)
                {
                    var header = (ws.Cell(1, c).GetString() ?? string.Empty).Trim().ToLowerInvariant();
                    if (header == "asin")
                    {
                        asinCol = c;
                        break;
                    }
                }

                if (asinCol > 0)
                {
                    for (var r = 2; r <= lastRow; r++)
                    {
                        TryAppendAsinFromCell(ws.Cell(r, asinCol).GetString(), asins);
                    }
                    return asins;
                }

                for (var r = 1; r <= lastRow; r++)
                {
                    for (var c = 1; c <= lastCol; c++)
                    {
                        TryAppendAsinFromCell(ws.Cell(r, c).GetString(), asins);
                    }
                }
            }
            return asins;
        }

        private static List<string> ReadAsinsFromFirstExcelColumn(string path)
        {
            var asins = new List<string>();
            using (var wb = new XLWorkbook(path))
            {
                var ws = wb.Worksheets.FirstOrDefault();
                if (ws == null) return asins;

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
                if (lastRow <= 0) return asins;

                for (var r = 1; r <= lastRow; r++)
                {
                    TryAppendAsinFromCell(ws.Cell(r, 1).GetString(), asins);
                }
            }
            return asins;
        }

        private static void TryAppendAsinFromCell(string rawCell, List<string> asins)
        {
            var cell = (rawCell ?? string.Empty).Trim().ToUpperInvariant();
            var m = RegexPatterns.Asin.Match(cell);
            if (!m.Success) return;
            var clean = CleanAsin(m.Value);
            if (!string.IsNullOrWhiteSpace(clean)) asins.Add(clean);
        }

        private static List<string> ReadAsinsFromPlainTxt(string path)
        {
            string text;
            if (!TryReadText(path, new UTF8Encoding(true), out text) &&
                !TryReadText(path, Encoding.GetEncoding("latin1"), out text))
            {
                text = File.ReadAllText(path);
            }

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var output = new List<string>();
            foreach (var line in lines)
            {
                var clean = CleanAsin(line);
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    output.Add(clean);
                }
            }
            return output;
        }

        private static bool TryReadText(string path, Encoding encoding, out string text)
        {
            try
            {
                using (var reader = new StreamReader(path, encoding, true))
                {
                    text = reader.ReadToEnd();
                    return true;
                }
            }
            catch (DecoderFallbackException)
            {
                text = null;
                return false;
            }
            catch (IOException)
            {
                text = null;
                return false;
            }
        }
    }
}
