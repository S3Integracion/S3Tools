using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace S3Integración_programs
{
    internal sealed class AsinNoReportEngineClient
    {
        public Task<AsinNoReportSheetListResponse> ListSheetsAsync(string baseFilePath)
        {
            return Task.Run(() => AsinNoReportDotNetEngine.ListSheets(baseFilePath));
        }

        public Task<AsinNoReportCompareResponse> CompareAsync(AsinNoReportCompareRequest request)
        {
            return Task.Run(() => AsinNoReportDotNetEngine.Compare(request));
        }
    }

    internal sealed class AsinNoReportCompareRequest
    {
        public string BaseFilePath { get; set; }
        public string BaseSheetName { get; set; }
        public string[] ReportPaths { get; set; }
    }

    internal sealed class AsinNoReportSheetListResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public string[] Sheets { get; set; }
    }

    internal sealed class AsinNoReportCompareResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public string Traceback { get; set; }
        public int BaseRowsWithAsin { get; set; }
        public int BaseUniqueAsins { get; set; }
        public int ReportsUniqueAsins { get; set; }
        public int FoundInReports { get; set; }
        public int MissingAsinsCount { get; set; }
        public string[] MissingAsins { get; set; }
        public string MissingAsinsText { get; set; }
    }

    internal static class AsinNoReportDotNetEngine
    {
        private static readonly Regex AsinRegex = new Regex("\\b[A-Z0-9]{10}\\b", RegexOptions.Compiled);
        private static readonly Regex HeaderNormalizeRegex = new Regex("[^a-z0-9]", RegexOptions.Compiled);
        private static readonly string[] BaseAsinHeaders = { "asin", "asins", "asin1" };
        private static readonly string[] ReportAsinHeaders = { "asin", "asins", "asin1" };

        public static AsinNoReportSheetListResponse ListSheets(string baseFilePath)
        {
            try
            {
                var path = (baseFilePath ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(path))
                {
                    return ErrorSheets("Selecciona un archivo base.");
                }
                if (!File.Exists(path))
                {
                    return ErrorSheets("No existe el archivo base.");
                }

                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext != ".xlsx")
                {
                    return new AsinNoReportSheetListResponse
                    {
                        Ok = true,
                        Sheets = Array.Empty<string>(),
                    };
                }

                using (var workbook = new XLWorkbook(path))
                {
                    var sheets = workbook.Worksheets.Select(ws => ws.Name).Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
                    return new AsinNoReportSheetListResponse
                    {
                        Ok = true,
                        Sheets = sheets,
                    };
                }
            }
            catch (Exception ex)
            {
                return new AsinNoReportSheetListResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                    Sheets = Array.Empty<string>(),
                };
            }
        }

        public static AsinNoReportCompareResponse Compare(AsinNoReportCompareRequest request)
        {
            try
            {
                var basePath = (request?.BaseFilePath ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(basePath))
                {
                    return ErrorCompare("Selecciona un archivo base (.csv/.xlsx).");
                }
                if (!File.Exists(basePath))
                {
                    return ErrorCompare("No existe el archivo base.");
                }

                var reportPaths = (request?.ReportPaths ?? Array.Empty<string>())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
                    .Where(File.Exists)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (reportPaths.Length == 0)
                {
                    return ErrorCompare("Debes importar al menos un reporte .txt.");
                }

                var baseRead = ReadBaseAsins(basePath, request?.BaseSheetName);
                var reportSet = ReadReportsAsins(reportPaths);
                var missing = baseRead.OrderedUniqueAsins.Where(a => !reportSet.Contains(a)).ToList();
                var found = baseRead.OrderedUniqueAsins.Count - missing.Count;

                return new AsinNoReportCompareResponse
                {
                    Ok = true,
                    BaseRowsWithAsin = baseRead.RowsWithDetectedAsin,
                    BaseUniqueAsins = baseRead.OrderedUniqueAsins.Count,
                    ReportsUniqueAsins = reportSet.Count,
                    FoundInReports = found,
                    MissingAsinsCount = missing.Count,
                    MissingAsins = missing.ToArray(),
                    MissingAsinsText = string.Join(Environment.NewLine, missing),
                };
            }
            catch (Exception ex)
            {
                return new AsinNoReportCompareResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                    MissingAsins = Array.Empty<string>(),
                    MissingAsinsText = string.Empty,
                };
            }
        }

        private static BaseReadResult ReadBaseAsins(string basePath, string sheetName)
        {
            var ext = Path.GetExtension(basePath).ToLowerInvariant();
            if (ext == ".xlsx")
            {
                return ReadBaseAsinsFromExcel(basePath, sheetName);
            }
            if (ext == ".csv")
            {
                return ReadBaseAsinsFromCsv(basePath);
            }

            throw new InvalidOperationException("El archivo base debe ser .csv o .xlsx.");
        }

        private static BaseReadResult ReadBaseAsinsFromCsv(string path)
        {
            var lines = ReadAllLinesSafe(path);
            if (lines.Count == 0)
            {
                throw new InvalidOperationException("El archivo base CSV está vacío.");
            }

            var delimiter = DetectCsvDelimiter(lines[0]);
            var headerFields = ParseCsvLine(lines[0], delimiter);
            var asinColumn = FindAsinColumn(headerFields, BaseAsinHeaders);
            if (asinColumn < 0)
            {
                throw new InvalidOperationException("No se encontró columna ASIN/ASINS en el archivo base.");
            }

            var ordered = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var rowsWithAsin = 0;

            for (var i = 1; i < lines.Count; i++)
            {
                var row = ParseCsvLine(lines[i], delimiter);
                if (asinColumn >= row.Count)
                {
                    continue;
                }

                var asin = ExtractValidAsin(row[asinColumn]);
                if (string.IsNullOrWhiteSpace(asin))
                {
                    continue;
                }

                rowsWithAsin++;
                if (seen.Add(asin))
                {
                    ordered.Add(asin);
                }
            }

            return new BaseReadResult(ordered, rowsWithAsin);
        }

        private static BaseReadResult ReadBaseAsinsFromExcel(string path, string sheetName)
        {
            using (var workbook = new XLWorkbook(path))
            {
                var worksheet = SelectWorksheet(workbook, sheetName);
                if (worksheet == null)
                {
                    throw new InvalidOperationException("No se encontró la hoja seleccionada en el archivo base.");
                }

                var firstRow = worksheet.FirstRowUsed()?.RowNumber() ?? 0;
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                var lastCol = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;
                if (firstRow <= 0 || lastRow <= 0 || lastCol <= 0)
                {
                    return new BaseReadResult(new List<string>(), 0);
                }

                var headers = new List<string>();
                for (var c = 1; c <= lastCol; c++)
                {
                    headers.Add(worksheet.Cell(firstRow, c).GetString());
                }

                var asinColumn = FindAsinColumn(headers, BaseAsinHeaders);
                if (asinColumn < 0)
                {
                    throw new InvalidOperationException("No se encontró columna ASIN/ASINS en la hoja base seleccionada.");
                }

                var ordered = new List<string>();
                var seen = new HashSet<string>(StringComparer.Ordinal);
                var rowsWithAsin = 0;

                for (var r = firstRow + 1; r <= lastRow; r++)
                {
                    var asin = ExtractValidAsin(worksheet.Cell(r, asinColumn + 1).GetString());
                    if (string.IsNullOrWhiteSpace(asin))
                    {
                        continue;
                    }

                    rowsWithAsin++;
                    if (seen.Add(asin))
                    {
                        ordered.Add(asin);
                    }
                }

                return new BaseReadResult(ordered, rowsWithAsin);
            }
        }

        private static IXLWorksheet SelectWorksheet(XLWorkbook workbook, string sheetName)
        {
            if (workbook == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                var byName = workbook.Worksheets.FirstOrDefault(ws => string.Equals(ws.Name, sheetName, StringComparison.Ordinal));
                if (byName != null)
                {
                    return byName;
                }
            }

            return workbook.Worksheets.FirstOrDefault();
        }

        private static HashSet<string> ReadReportsAsins(IEnumerable<string> reportPaths)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);

            foreach (var path in reportPaths)
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext != ".txt")
                {
                    continue;
                }

                var lines = ReadAllLinesSafe(path);
                if (lines.Count == 0)
                {
                    continue;
                }

                var headers = lines[0].Split('\t').Select(h => h ?? string.Empty).ToList();
                var asinColumn = FindAsinColumn(headers, ReportAsinHeaders);

                for (var i = 1; i < lines.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                    {
                        continue;
                    }

                    var row = lines[i].Split('\t');
                    if (asinColumn >= 0)
                    {
                        if (asinColumn >= row.Length)
                        {
                            continue;
                        }

                        var asin = ExtractValidAsin(row[asinColumn]);
                        if (!string.IsNullOrWhiteSpace(asin))
                        {
                            set.Add(asin);
                        }
                        continue;
                    }

                    foreach (var cell in row)
                    {
                        var asin = ExtractValidAsin(cell);
                        if (!string.IsNullOrWhiteSpace(asin))
                        {
                            set.Add(asin);
                        }
                    }
                }
            }

            return set;
        }

        private static int FindAsinColumn(IReadOnlyList<string> headers, IEnumerable<string> accepted)
        {
            if (headers == null || headers.Count == 0)
            {
                return -1;
            }

            var allowed = new HashSet<string>(accepted.Select(NormalizeHeader), StringComparer.Ordinal);
            for (var i = 0; i < headers.Count; i++)
            {
                var normalized = NormalizeHeader(headers[i]);
                if (allowed.Contains(normalized))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string NormalizeHeader(string value)
        {
            var text = (value ?? string.Empty).Trim().ToLowerInvariant();
            return HeaderNormalizeRegex.Replace(text, string.Empty);
        }

        private static string ExtractValidAsin(string value)
        {
            var text = (value ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var match = AsinRegex.Match(text);
            if (!match.Success)
            {
                return string.Empty;
            }

            var asin = match.Value;
            var onlyDigits = true;
            foreach (var ch in asin)
            {
                if (!char.IsDigit(ch))
                {
                    onlyDigits = false;
                    break;
                }
            }

            return onlyDigits ? string.Empty : asin;
        }

        private static List<string> ReadAllLinesSafe(string path)
        {
            foreach (var encoding in new[] { new UTF8Encoding(true), Encoding.GetEncoding("latin1") })
            {
                try
                {
                    var lines = new List<string>();
                    using (var reader = new StreamReader(path, encoding, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                    return lines;
                }
                catch
                {
                }
            }

            return File.ReadAllLines(path).ToList();
        }

        private static char DetectCsvDelimiter(string headerLine)
        {
            var line = headerLine ?? string.Empty;
            var comma = line.Count(ch => ch == ',');
            var semicolon = line.Count(ch => ch == ';');
            return semicolon > comma ? ';' : ',';
        }

        private static List<string> ParseCsvLine(string line, char delimiter)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            foreach (var ch in line ?? string.Empty)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            result.Add(current.ToString());
            return result;
        }

        private static AsinNoReportSheetListResponse ErrorSheets(string message)
        {
            return new AsinNoReportSheetListResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
                Sheets = Array.Empty<string>(),
            };
        }

        private static AsinNoReportCompareResponse ErrorCompare(string message)
        {
            return new AsinNoReportCompareResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
                MissingAsins = Array.Empty<string>(),
                MissingAsinsText = string.Empty,
            };
        }

        private sealed class BaseReadResult
        {
            public BaseReadResult(List<string> orderedUniqueAsins, int rowsWithDetectedAsin)
            {
                OrderedUniqueAsins = orderedUniqueAsins ?? new List<string>();
                RowsWithDetectedAsin = rowsWithDetectedAsin;
            }

            public List<string> OrderedUniqueAsins { get; }
            public int RowsWithDetectedAsin { get; }
        }
    }
}
