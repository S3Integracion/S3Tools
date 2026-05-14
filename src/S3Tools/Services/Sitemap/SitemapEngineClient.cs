using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;

namespace S3Tools
{
    /// <summary>
    /// Cliente de alto nivel para el motor de Sitemap. Expone un único método público
    /// <see cref="ProcessAsync"/> y delega en <see cref="SitemapEngine"/>.
    /// </summary>
    internal sealed class SitemapEngineClient
    {
        public Task<SitemapEngineResponse> ProcessAsync(SitemapEngineRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Action = "process";
            return Task.Run(() => SitemapEngine.Handle(request));
        }
    }

    /// <summary>
    /// Orquestador del motor Sitemap. Convierte listas de URLs en archivos sitemap JSON
    /// compatibles con WebScraper aplicando una plantilla seleccionada.
    /// </summary>
    internal static class SitemapEngine
    {
        private const string DefaultSitemapName = "sitemap";
        private static readonly Regex NameAllowedRegex      = new Regex("[^a-zA-Z0-9_()+-]", RegexOptions.Compiled);
        private static readonly Regex MultipleUnderscoreRegex = new Regex("_+", RegexOptions.Compiled);
        private static readonly Regex TrailingDigitsRegex   = new Regex("(\\d+)$", RegexOptions.Compiled);

        public static SitemapEngineResponse Handle(SitemapEngineRequest request)
        {
            var action = (request?.Action ?? string.Empty).Trim().ToLowerInvariant();
            return action == "process" ? HandleProcess(request) : Error("Unknown action");
        }

        private static SitemapEngineResponse HandleProcess(SitemapEngineRequest data)
        {
            var inputFiles = (data?.InputFiles ?? Array.Empty<string>())
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();

            if (inputFiles.Length == 0) return Error("Missing input_files");

            foreach (var fp in inputFiles)
            {
                if (!File.Exists(fp)) return Error("Input file not found: " + fp);
            }

            var outputDir = (data?.OutputDir ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(outputDir))
            {
                outputDir = GetDownloadsPath();
            }

            var zipOutput = data?.ZipOutput == true;

            // Resolución del baseLabel: si hay prefijos/storeName explícitos los concatena;
            // si no, usa Store + BaseName.
            var prefix1      = (data?.NamePrefix1 ?? string.Empty).Trim();
            var prefix2      = (data?.NamePrefix2 ?? string.Empty).Trim();
            var templateMode = (data?.TemplateMode ?? string.Empty).Trim().ToLowerInvariant();
            var storeName    = (data?.StoreName ?? string.Empty).Trim();
            var useNewName   = !string.IsNullOrWhiteSpace(storeName) ||
                               !string.IsNullOrWhiteSpace(prefix1)   ||
                               !string.IsNullOrWhiteSpace(prefix2);

            string baseLabel;
            if (useNewName)
            {
                if (string.IsNullOrWhiteSpace(storeName))
                {
                    storeName = (data?.Store ?? string.Empty).Trim();
                }
                if (string.IsNullOrWhiteSpace(storeName)) return Error("Missing store_name");

                baseLabel = prefix1 + prefix2 + storeName;
            }
            else
            {
                var baseName = (data?.BaseName ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(baseName)) return Error("Missing base_name");

                var store = (data?.Store ?? string.Empty).Trim();
                baseLabel = string.IsNullOrWhiteSpace(store) ? baseName : (store + "_" + baseName);
            }

            var templateName = SitemapJsonBuilder.SelectTemplate(templateMode);
            var templateText = SitemapJsonBuilder.LoadTemplateText(templateName);

            var folderName = SanitizeName(baseLabel) + "_" +
                             DateTime.Now.ToString("ddMMyy") + "_" +
                             DateTime.Now.ToString("HHmm");
            var workDir = Path.Combine(outputDir, folderName);
            Directory.CreateDirectory(workDir);

            var baseId = SanitizeName(baseLabel);
            var outputFiles = new List<string>();
            var usedTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < inputFiles.Length; i++)
            {
                var fp = inputFiles[i];
                var urls = UrlExtractor.ReadUrlsFromFile(fp);
                if (urls.Count == 0) return Error("No URLs found in: " + fp);

                var title = ResolveSitemapTitle(baseId, inputFiles.Length, i, fp);
                if (!usedTitles.Add(title))
                {
                    title = title + "_" + (i + 1);
                    usedTitles.Add(title);
                }

                var jsonPayload = SitemapJsonBuilder.BuildSitemapPayload(templateText, title, urls);
                var outPath = Path.Combine(workDir, title + ".json");
                File.WriteAllText(outPath, jsonPayload, new UTF8Encoding(false));
                outputFiles.Add(outPath);
            }

            var zipPath = string.Empty;
            if (zipOutput)
            {
                zipPath = Path.Combine(outputDir, folderName + ".zip");
                ZipOutputs(outputFiles, zipPath);
                try
                {
                    Directory.Delete(workDir, true);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            return new SitemapEngineResponse
            {
                Ok = true,
                OutputFolder = zipOutput ? string.Empty : workDir,
                ZipPath = zipPath,
                OutputFiles = outputFiles.ToArray(),
            };
        }

        private static string ResolveSitemapTitle(string baseId, int total, int index, string sourcePath)
        {
            var suffix = ExtractTrailingNumber(sourcePath);
            if (!string.IsNullOrWhiteSpace(suffix)) return baseId + "_" + suffix;
            if (total > 1) return baseId + "_" + (index + 1);
            return baseId;
        }

        private static string ExtractTrailingNumber(string path)
        {
            var stem = Path.GetFileNameWithoutExtension(path) ?? string.Empty;
            var match = TrailingDigitsRegex.Match(stem);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static void ZipOutputs(IEnumerable<string> files, string targetZip)
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

        private static string SanitizeName(string text)
        {
            var repl = (text ?? string.Empty).Trim();
            repl = repl.Replace(" ", "_").Replace("-", "_");
            repl = NameAllowedRegex.Replace(repl, "_");
            repl = MultipleUnderscoreRegex.Replace(repl, "_");
            repl = repl.Trim('_').Trim('.');
            return string.IsNullOrWhiteSpace(repl) ? DefaultSitemapName : repl;
        }

        private static string GetDownloadsPath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, "Downloads");
        }

        private static SitemapEngineResponse Error(string message)
        {
            return new SitemapEngineResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
                OutputFiles = Array.Empty<string>(),
            };
        }
    }
}
