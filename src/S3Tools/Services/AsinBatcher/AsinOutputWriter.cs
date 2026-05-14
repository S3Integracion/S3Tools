using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace S3Tools
{
    /// <summary>
    /// Escribe los lotes de ASINs como archivos .txt con encabezado <c>start_url</c>,
    /// los empaca opcionalmente en .zip y exporta duplicados a .csv.
    /// </summary>
    internal static class AsinOutputWriter
    {
        private const string StartUrlHeader = "start_url";
        private const string DuplicatesFilePrefix = "duplicados_";
        private const string DefaultFileName = "archivo";

        private static readonly Regex NameAllowedRegex = new Regex("[^a-zA-Z0-9_()+-]", RegexOptions.Compiled);
        private static readonly Regex MultipleUnderscoreRegex = new Regex("_+", RegexOptions.Compiled);

        /// <summary>
        /// Escribe cada lote como un .txt en <paramref name="folder"/>.
        /// Si hay un solo lote se usa el nombre base; si son varios se sufija <c>_N</c>.
        /// </summary>
        public static List<string> WriteBatchesAsTxt(
            IList<List<string>> batchesList,
            string folder,
            string market,
            string baseLabel,
            bool showSellerOnOpen)
        {
            var output = new List<string>();
            var safeBase = SanitizeFilename(baseLabel);
            var total = batchesList.Count;

            for (var i = 0; i < total; i++)
            {
                var fileName = total > 1 ? safeBase + "_" + (i + 1) + ".txt" : safeBase + ".txt";
                var path = Path.Combine(folder, fileName);
                using (var writer = new StreamWriter(path, false, new UTF8Encoding(false)))
                {
                    writer.WriteLine(StartUrlHeader);
                    foreach (var asin in batchesList[i])
                    {
                        writer.WriteLine(AsinUrlBuilder.ToUrl(asin, market, showSellerOnOpen));
                    }
                }
                output.Add(path);
            }
            return output;
        }

        /// <summary>Empaca los archivos generados en un solo .zip.</summary>
        public static void ZipOutputs(IEnumerable<string> files, string targetZip)
        {
            var list = (files ?? Enumerable.Empty<string>())
                .Where(File.Exists)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (list.Count == 0) return;

            var parent = Path.GetDirectoryName(targetZip);
            if (!string.IsNullOrWhiteSpace(parent))
            {
                Directory.CreateDirectory(parent);
            }

            if (File.Exists(targetZip))
            {
                File.Delete(targetZip);
            }

            using (var archive = ZipFile.Open(targetZip, ZipArchiveMode.Create))
            {
                foreach (var file in list)
                {
                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }

        /// <summary>
        /// Exporta los ASINs duplicados a un .csv timestampeado en <paramref name="outdir"/>.
        /// Devuelve la ruta del .csv generado o cadena vacía si no había duplicados.
        /// </summary>
        public static string ExportDuplicatesCsv(IEnumerable<string> dups, string outdir)
        {
            var duplicateList = (dups ?? Enumerable.Empty<string>()).ToList();
            if (duplicateList.Count == 0) return string.Empty;

            Directory.CreateDirectory(outdir);
            var file = Path.Combine(outdir, DuplicatesFilePrefix + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");

            using (var writer = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                writer.WriteLine("asin");
                var seen = new HashSet<string>(StringComparer.Ordinal);
                foreach (var asin in duplicateList)
                {
                    if (!seen.Add(asin)) continue;
                    writer.WriteLine(asin);
                }
            }
            return file;
        }

        /// <summary>
        /// Sanea un texto para usarse como nombre de archivo seguro:
        /// reemplaza separadores y caracteres inválidos por '_', colapsa duplicados.
        /// </summary>
        public static string SanitizeFilename(string text)
        {
            var repl = (text ?? string.Empty).Trim();
            repl = repl.Replace(" ", "_").Replace("-", "_");
            repl = NameAllowedRegex.Replace(repl, "_");
            repl = MultipleUnderscoreRegex.Replace(repl, "_");
            repl = repl.Trim('_').Trim('.');
            return string.IsNullOrWhiteSpace(repl) ? DefaultFileName : repl;
        }

        /// <summary>Devuelve la carpeta Descargas del usuario actual.</summary>
        public static string GetDownloadsPath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, "Downloads");
        }
    }
}
