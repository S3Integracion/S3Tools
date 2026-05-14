using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace S3Tools
{
    /// <summary>
    /// Reemplaza los dos primeros encabezados de cada hoja de un .xlsx usando manipulación
    /// directa del ZIP y del XML interno. Devuelve la plantilla detectada o seleccionada.
    /// </summary>
    internal static class XlsxHeaderNormalizer
    {
        private static readonly XNamespace SpreadsheetNs    = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private static readonly XNamespace RelationshipsNs  = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

        /// <summary>
        /// Aplica el cambio de encabezados a todas las hojas del workbook.
        /// </summary>
        /// <returns>Clave de plantilla efectiva ("tiendas"/"bbvs").</returns>
        public static string UpdateHeaders(string path, string templateChoice, string headerFormat)
        {
            var selected = TemplateResolver.HeaderFormats[headerFormat];

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Update))
            {
                var workbookEntry     = archive.GetEntry("xl/workbook.xml");
                var workbookRelsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels");
                if (workbookEntry == null || workbookRelsEntry == null)
                {
                    throw new InvalidOperationException("Invalid XLSX file");
                }

                var workbookDoc   = LoadXml(workbookEntry);
                var relsDoc       = LoadXml(workbookRelsEntry);
                var relMap        = BuildRelationshipsMap(relsDoc);
                var sharedStrings = LoadSharedStrings(archive);

                var worksheets         = new List<WorksheetUpdateData>();
                string templateKey     = null;
                var useExplicitTemplate = TemplateResolver.TemplateFiles.ContainsKey(templateChoice);
                if (useExplicitTemplate) templateKey = templateChoice;

                foreach (var sheet in workbookDoc.Descendants(SpreadsheetNs + "sheet"))
                {
                    var relId = (string)sheet.Attribute(RelationshipsNs + "id");
                    if (string.IsNullOrWhiteSpace(relId) || !relMap.TryGetValue(relId, out var relTarget)) continue;

                    var sheetPath = ResolveZipPath("xl", relTarget);
                    var worksheetEntry = archive.GetEntry(sheetPath);
                    if (worksheetEntry == null) continue;

                    var worksheetDoc = LoadXml(worksheetEntry);
                    var maxColumn = GetWorksheetMaxColumn(worksheetDoc);
                    if (maxColumn < 1) continue;

                    var headers = GetWorksheetHeaders(worksheetDoc, maxColumn, sharedStrings);
                    if (!useExplicitTemplate && string.IsNullOrWhiteSpace(templateKey) && headers.Count > 0)
                    {
                        templateKey = TemplateResolver.DetectTemplate(headers);
                    }

                    worksheets.Add(new WorksheetUpdateData(sheetPath, worksheetDoc, maxColumn));
                }

                if (string.IsNullOrWhiteSpace(templateKey)) templateKey = "tiendas";

                foreach (var ws in worksheets)
                {
                    if (ws.MaxColumn >= 1) SetWorksheetCellValue(ws.Document, 1, selected[0]);
                    if (ws.MaxColumn >= 2) SetWorksheetCellValue(ws.Document, 2, selected[1]);
                    WriteXml(archive, ws.Path, ws.Document);
                }

                return templateKey;
            }
        }

        private static XDocument LoadXml(ZipArchiveEntry entry)
        {
            using (var stream = entry.Open())
            {
                return XDocument.Load(stream);
            }
        }

        private static void WriteXml(ZipArchive archive, string entryPath, XDocument document)
        {
            archive.GetEntry(entryPath)?.Delete();
            var updated = archive.CreateEntry(entryPath, CompressionLevel.Optimal);
            using (var s = updated.Open())
            {
                document.Save(s);
            }
        }

        private static Dictionary<string, string> BuildRelationshipsMap(XDocument relsDoc)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var rel in relsDoc.Descendants().Where(e => e.Name.LocalName == "Relationship"))
            {
                var id = (string)rel.Attribute("Id");
                var target = (string)rel.Attribute("Target");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(target)) continue;
                map[id] = target;
            }
            return map;
        }

        private static List<string> LoadSharedStrings(ZipArchive archive)
        {
            var output = new List<string>();
            var entry = archive.GetEntry("xl/sharedStrings.xml");
            if (entry == null) return output;

            var doc = LoadXml(entry);
            foreach (var si in doc.Descendants(SpreadsheetNs + "si"))
            {
                var text = string.Concat(si.Descendants(SpreadsheetNs + "t").Select(t => t.Value));
                output.Add(text);
            }
            return output;
        }

        private static int GetWorksheetMaxColumn(XDocument worksheetDoc)
        {
            var maxColumn = 0;
            foreach (var cell in worksheetDoc.Descendants(SpreadsheetNs + "c"))
            {
                var reference = (string)cell.Attribute("r");
                if (string.IsNullOrWhiteSpace(reference)) continue;

                var col = SpreadsheetReference.GetColumnIndex(reference);
                if (col > maxColumn) maxColumn = col;
            }
            return maxColumn;
        }

        private static List<string> GetWorksheetHeaders(XDocument worksheetDoc, int maxColumn, IList<string> sharedStrings)
        {
            var headersByColumn = new Dictionary<int, string>();
            var row1 = worksheetDoc
                .Descendants(SpreadsheetNs + "row")
                .FirstOrDefault(r => string.Equals((string)r.Attribute("r"), "1", StringComparison.OrdinalIgnoreCase));

            if (row1 != null)
            {
                foreach (var cell in row1.Elements(SpreadsheetNs + "c"))
                {
                    var reference = (string)cell.Attribute("r");
                    if (string.IsNullOrWhiteSpace(reference)) continue;

                    var column = SpreadsheetReference.GetColumnIndex(reference);
                    headersByColumn[column] = GetCellText(cell, sharedStrings);
                }
            }

            var headers = new List<string>();
            for (var i = 1; i <= maxColumn; i++)
            {
                headers.Add(headersByColumn.TryGetValue(i, out var value) ? value : string.Empty);
            }
            return headers;
        }

        private static string GetCellText(XElement cell, IList<string> sharedStrings)
        {
            var type = ((string)cell.Attribute("t") ?? string.Empty).Trim().ToLowerInvariant();

            if (type == "s")
            {
                var idxText = (string)cell.Element(SpreadsheetNs + "v");
                if (int.TryParse(idxText, out var idx) && idx >= 0 && idx < sharedStrings.Count)
                {
                    return sharedStrings[idx] ?? string.Empty;
                }
                return string.Empty;
            }

            if (type == "inlinestr")
            {
                var inline = cell.Element(SpreadsheetNs + "is");
                return inline == null
                    ? string.Empty
                    : string.Concat(inline.Descendants(SpreadsheetNs + "t").Select(t => t.Value));
            }

            return (string)cell.Element(SpreadsheetNs + "v") ?? string.Empty;
        }

        private static void SetWorksheetCellValue(XDocument worksheetDoc, int columnIndex, string value)
        {
            var root = worksheetDoc.Root;
            if (root == null) return;

            var sheetData = root.Element(SpreadsheetNs + "sheetData");
            if (sheetData == null)
            {
                sheetData = new XElement(SpreadsheetNs + "sheetData");
                root.Add(sheetData);
            }

            var row = sheetData.Elements(SpreadsheetNs + "row")
                .FirstOrDefault(r => string.Equals((string)r.Attribute("r"), "1", StringComparison.OrdinalIgnoreCase));

            if (row == null)
            {
                row = new XElement(SpreadsheetNs + "row", new XAttribute("r", "1"));
                var firstRow = sheetData.Elements(SpreadsheetNs + "row")
                    .OrderBy(r => SpreadsheetReference.GetRowIndex((string)r.Attribute("r")))
                    .FirstOrDefault();
                if (firstRow == null) sheetData.Add(row);
                else firstRow.AddBeforeSelf(row);
            }

            var cellRef = SpreadsheetReference.GetColumnLetters(columnIndex) + "1";
            var cell = row.Elements(SpreadsheetNs + "c")
                .FirstOrDefault(c => string.Equals((string)c.Attribute("r"), cellRef, StringComparison.OrdinalIgnoreCase));

            if (cell == null)
            {
                cell = new XElement(SpreadsheetNs + "c", new XAttribute("r", cellRef));
                var nextCell = row.Elements(SpreadsheetNs + "c")
                    .OrderBy(c => SpreadsheetReference.GetColumnIndex((string)c.Attribute("r")))
                    .FirstOrDefault(c => SpreadsheetReference.GetColumnIndex((string)c.Attribute("r")) > columnIndex);

                if (nextCell == null) row.Add(cell);
                else nextCell.AddBeforeSelf(cell);
            }

            cell.SetAttributeValue("t", "inlineStr");
            cell.Elements().Remove();
            cell.Add(new XElement(SpreadsheetNs + "is", new XElement(SpreadsheetNs + "t", value ?? string.Empty)));
        }

        private static string ResolveZipPath(string baseFolder, string target)
        {
            var clean = (target ?? string.Empty).Replace("\\", "/").Trim();
            if (clean.StartsWith("/")) clean = clean.TrimStart('/');
            if (clean.StartsWith("xl/", StringComparison.OrdinalIgnoreCase)) return clean;
            return (baseFolder.TrimEnd('/') + "/" + clean).Replace("//", "/");
        }

        private sealed class WorksheetUpdateData
        {
            public WorksheetUpdateData(string path, XDocument document, int maxColumn)
            {
                Path = path;
                Document = document;
                MaxColumn = maxColumn;
            }

            public string Path { get; }
            public XDocument Document { get; }
            public int MaxColumn { get; }
        }
    }

    /// <summary>
    /// Conversiones de referencias OpenXML estilo "A1": columna ↔ índice numérico,
    /// fila ↔ índice. Reutilizable por cualquier consumidor de XLSX bajo.
    /// </summary>
    internal static class SpreadsheetReference
    {
        /// <summary>"A1" → 1, "B" → 2, "AA" → 27.</summary>
        public static int GetColumnIndex(string cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference)) return 0;

            var letters = new string(cellReference.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            if (letters.Length == 0) return 0;

            var col = 0;
            foreach (var ch in letters)
            {
                col = (col * 26) + (ch - 'A' + 1);
            }
            return col;
        }

        /// <summary>Devuelve el índice de fila numérico, o <see cref="int.MaxValue"/> si no parsea.</summary>
        public static int GetRowIndex(string rowValue)
        {
            if (string.IsNullOrWhiteSpace(rowValue)) return int.MaxValue;
            return int.TryParse(rowValue, out var row) ? row : int.MaxValue;
        }

        /// <summary>1 → "A", 27 → "AA".</summary>
        public static string GetColumnLetters(int columnIndex)
        {
            if (columnIndex <= 0) return "A";

            var sb = new StringBuilder();
            var value = columnIndex;
            while (value > 0)
            {
                value--;
                sb.Insert(0, (char)('A' + (value % 26)));
                value /= 26;
            }
            return sb.ToString();
        }
    }
}
