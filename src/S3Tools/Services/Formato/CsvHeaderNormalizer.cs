using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace S3Tools
{
    /// <summary>
    /// Reemplaza los dos primeros encabezados de un CSV preservando encoding,
    /// delimitador y comillas originales en las filas restantes.
    /// </summary>
    internal static class CsvHeaderNormalizer
    {
        /// <summary>Aplica el cambio de encabezados al CSV in-place.</summary>
        /// <returns>Clave de plantilla efectiva ("tiendas"/"bbvs").</returns>
        public static string UpdateHeaders(string path, string templateChoice, string headerFormat)
        {
            var selected = TemplateResolver.HeaderFormats[headerFormat];
            var text = ReadTextWithEncoding(path, out var encoding);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("Empty CSV file");
            }

            var delimiter = DetectCsvDelimiter(text.Length > 4096 ? text.Substring(0, 4096) : text);
            var rows = ParseCsvRows(text, delimiter);
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("Empty CSV file");
            }

            var headers = rows[0];
            var templateKey = TemplateResolver.ResolveTemplate(templateChoice, headers);

            if (headers.Count >= 1) headers[0] = selected[0];
            if (headers.Count >= 2) headers[1] = selected[1];

            File.WriteAllText(path, BuildCsvText(rows, delimiter), encoding);
            return templateKey;
        }

        private static string ReadTextWithEncoding(string path, out Encoding encoding)
        {
            string text;
            if (TryReadText(path, new UTF8Encoding(true),  out text)) { encoding = new UTF8Encoding(true);  return text; }
            if (TryReadText(path, new UTF8Encoding(false), out text)) { encoding = new UTF8Encoding(false); return text; }
            if (TryReadText(path, Encoding.GetEncoding("latin1"), out text)) { encoding = Encoding.GetEncoding("latin1"); return text; }

            encoding = new UTF8Encoding(false);
            return File.ReadAllText(path, encoding);
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
            catch (DecoderFallbackException) { text = null; return false; }
            catch (IOException)               { text = null; return false; }
        }

        /// <summary>Detecta el delimitador más frecuente entre ';', '\t', ',', '|'.</summary>
        private static char DetectCsvDelimiter(string sample)
        {
            var counts = new Dictionary<char, int>
            {
                { ';',  sample.Count(c => c == ';') },
                { '\t', sample.Count(c => c == '\t') },
                { ',',  sample.Count(c => c == ',') },
                { '|',  sample.Count(c => c == '|') },
            };

            var best = counts.OrderByDescending(kv => kv.Value).First();
            if (best.Value > 0) return best.Key;

            if (sample.Contains(";") && !sample.Contains(",")) return ';';
            if (sample.Contains("\t")) return '\t';
            if (sample.Contains("|"))  return '|';
            return ',';
        }

        /// <summary>
        /// Parser manual de CSV con soporte de comillas dobles y escapes "" dentro de campos.
        /// </summary>
        private static List<List<string>> ParseCsvRows(string text, char delimiter)
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < (text ?? string.Empty).Length; i++)
            {
                var ch = text[i];

                if (ch == '"')
                {
                    if (inQuotes && i + 1 < text.Length && text[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                        continue;
                    }
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == delimiter && !inQuotes)
                {
                    row.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                if ((ch == '\n' || ch == '\r') && !inQuotes)
                {
                    if (ch == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                    row.Add(sb.ToString());
                    sb.Clear();
                    rows.Add(row);
                    row = new List<string>();
                    continue;
                }

                sb.Append(ch);
            }

            if (sb.Length > 0 || row.Count > 0)
            {
                row.Add(sb.ToString());
                rows.Add(row);
            }

            return rows;
        }

        private static string BuildCsvText(IEnumerable<List<string>> rows, char delimiter)
        {
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                sb.AppendLine(BuildCsvLine(row, delimiter));
            }
            return sb.ToString();
        }

        private static string BuildCsvLine(IEnumerable<string> values, char delimiter)
        {
            return string.Join(delimiter.ToString(), (values ?? Enumerable.Empty<string>()).Select(EscapeCsvCell));
        }

        private static string EscapeCsvCell(string value)
        {
            var text = value ?? string.Empty;
            if (text.Contains("\""))
            {
                text = text.Replace("\"", "\"\"");
            }

            // Si contiene cualquier delimitador, salto de línea o comilla — citamos toda la celda.
            if (text.Contains(",")  || text.Contains(";") ||
                text.Contains("\t") || text.Contains("|") ||
                text.Contains("\n") || text.Contains("\r") ||
                text.Contains("\""))
            {
                return "\"" + text + "\"";
            }
            return text;
        }
    }
}
