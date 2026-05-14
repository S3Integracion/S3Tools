using System;
using System.IO;
using System.Threading.Tasks;

namespace S3Tools
{
    /// <summary>
    /// Cliente de alto nivel para el motor C# de ASIN Batcher.
    /// Expone una API asíncrona; internamente delega en <see cref="AsinBatcherEngine"/>.
    /// </summary>
    internal sealed class AsinBatcherEngineClient
    {
        /// <summary>Recuento previo de únicos/duplicados sin generar archivos.</summary>
        public Task<EngineResponse> PreviewAsync(string inputPath)
        {
            return Task.Run(() => AsinBatcherEngine.Handle(new EngineRequest
            {
                Action = "preview",
                InputPath = inputPath,
            }));
        }

        /// <summary>Exporta a .csv solo los ASIN duplicados.</summary>
        public Task<EngineResponse> ExportDuplicatesAsync(string inputPath, string outputDir)
        {
            return Task.Run(() => AsinBatcherEngine.Handle(new EngineRequest
            {
                Action = "export_duplicates",
                InputPath = inputPath,
                OutputDir = outputDir,
            }));
        }

        /// <summary>Genera los lotes y archivos finales según los parámetros del request.</summary>
        public Task<EngineResponse> ProcessAsync(EngineRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Action = "process";
            return Task.Run(() => AsinBatcherEngine.Handle(request));
        }
    }

    /// <summary>
    /// Orquestador del motor ASIN Batcher. Dispatch de acciones (preview / process / export_duplicates)
    /// hacia los componentes especializados: <see cref="AsinExtractor"/>, <see cref="AsinBatchSplitter"/>,
    /// <see cref="AsinUrlBuilder"/> y <see cref="AsinOutputWriter"/>.
    /// </summary>
    internal static class AsinBatcherEngine
    {
        private const int DefaultBatches = 30;
        private const string DefaultMarket = "US";
        private const string DefaultOrder = "Ordenado";

        private static readonly System.Collections.Generic.HashSet<string> ValidMarkets =
            new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase) { "MX", "US" };

        private static readonly System.Collections.Generic.HashSet<string> ValidOrders =
            new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Ordenado", "Inverso", "Aleatorio" };

        /// <summary>Punto único de entrada al motor: enruta a la acción solicitada.</summary>
        public static EngineResponse Handle(EngineRequest request)
        {
            var action = (request?.Action ?? string.Empty).Trim().ToLowerInvariant();

            switch (action)
            {
                case "preview":            return HandlePreview(request);
                case "process":            return HandleProcess(request);
                case "export_duplicates":  return HandleExportDuplicates(request);
                default:                   return Error("Unknown action");
            }
        }

        private static EngineResponse HandlePreview(EngineRequest request)
        {
            var inputPath = (request?.InputPath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath)) return Error("Missing input_path");
            if (!File.Exists(inputPath))              return Error("Input file not found");

            var extraction = AsinExtractor.ExtractAsinsAny(inputPath);
            return PreviewResponse(extraction.Uniques, extraction.Duplicates);
        }

        private static EngineResponse HandleExportDuplicates(EngineRequest request)
        {
            var inputPath = (request?.InputPath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath)) return Error("Missing input_path");
            if (!File.Exists(inputPath))              return Error("Input file not found");

            var outputDir = (request?.OutputDir ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(outputDir))
            {
                outputDir = AsinOutputWriter.GetDownloadsPath();
            }

            var extraction = AsinExtractor.ExtractAsinsAny(inputPath);
            var csvPath = AsinOutputWriter.ExportDuplicatesCsv(extraction.Duplicates, outputDir);

            return new EngineResponse
            {
                Ok = true,
                Duplicates = extraction.Duplicates.Count,
                CsvPath = csvPath,
            };
        }

        private static EngineResponse HandleProcess(EngineRequest request)
        {
            var inputPath = (request?.InputPath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath)) return Error("Missing input_path");
            if (!File.Exists(inputPath))              return Error("Input file not found");

            var outputDir = (request?.OutputDir ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(outputDir))
            {
                outputDir = AsinOutputWriter.GetDownloadsPath();
            }
            Directory.CreateDirectory(outputDir);

            var market = ValidMarkets.Contains(request?.Market ?? string.Empty) ? request.Market : DefaultMarket;
            var order  = ValidOrders.Contains(request?.Order ?? string.Empty)   ? request.Order  : DefaultOrder;

            // Decide el "baseLabel" del nombre de salida. Si el usuario pasó prefijos o un nombre
            // de tienda explícito, los concatenamos; si no, se usa el store seleccionado + fileLabel.
            var prefix1    = (request?.NamePrefix1 ?? string.Empty).Trim();
            var prefix2    = (request?.NamePrefix2 ?? string.Empty).Trim();
            var storeName  = (request?.StoreName ?? string.Empty).Trim();
            var useNewName = !string.IsNullOrWhiteSpace(storeName) ||
                             !string.IsNullOrWhiteSpace(prefix1)   ||
                             !string.IsNullOrWhiteSpace(prefix2);

            string baseLabel;
            if (useNewName)
            {
                if (string.IsNullOrWhiteSpace(storeName))
                {
                    storeName = (request?.Store ?? string.Empty).Trim();
                }
                if (string.IsNullOrWhiteSpace(storeName))
                {
                    return Error("Missing store_name");
                }
                baseLabel = prefix1 + prefix2 + storeName;
            }
            else
            {
                var store = AsinBatchSplitter.ComputeStoreFromSelection(request?.Store);
                var fileLabel = (request?.FileLabel ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(fileLabel)) return Error("Missing file_label");
                baseLabel = store + "_" + fileLabel;
            }

            var batches = request?.Batches ?? DefaultBatches;
            if (batches < 1) batches = DefaultBatches;

            var zipOut = request?.ZipOutput ?? false;
            var showSellerOnOpen = request?.ShowSellerOnOpen ?? false;

            var extraction = AsinExtractor.ExtractAsinsAny(inputPath);
            if (extraction.Uniques.Count == 0)
            {
                return Error("No valid ASINs found");
            }

            if (batches > extraction.Uniques.Count)
            {
                return Error("La cantidad de lotes no puede ser mayor que la cantidad de URLs. URLs: " +
                             extraction.Uniques.Count + " | Lotes: " + batches);
            }

            var ordered = AsinBatchSplitter.ReorderAsins(extraction.Uniques, order);

            var folderName = AsinOutputWriter.SanitizeFilename(baseLabel) + "_" +
                             DateTime.Now.ToString("ddMMyy") + "_" +
                             DateTime.Now.ToString("HHmm");
            var workDir = Path.Combine(outputDir, folderName);
            Directory.CreateDirectory(workDir);

            var batchesList = AsinBatchSplitter.SplitInBatches(ordered, batches);
            var outFiles = AsinOutputWriter.WriteBatchesAsTxt(batchesList, workDir, market, baseLabel, showSellerOnOpen);

            var zipPath = string.Empty;
            if (zipOut)
            {
                zipPath = Path.Combine(outputDir, AsinOutputWriter.SanitizeFilename(baseLabel) + ".zip");
                AsinOutputWriter.ZipOutputs(outFiles, zipPath);
                try
                {
                    Directory.Delete(workDir, true);
                }
                catch (IOException)
                {
                    // Si el directorio sigue bloqueado lo dejamos; el .zip ya está generado.
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            var response = PreviewResponse(ordered, extraction.Duplicates);
            response.OutputFolder = zipOut ? string.Empty : workDir;
            response.ZipPath = zipPath;
            return response;
        }

        private static EngineResponse PreviewResponse(
            System.Collections.Generic.ICollection<string> uniques,
            System.Collections.Generic.ICollection<string> dups)
        {
            return new EngineResponse
            {
                Ok = true,
                Total = uniques.Count + dups.Count,
                Unique = uniques.Count,
                Duplicates = dups.Count,
            };
        }

        private static EngineResponse Error(string message)
        {
            return new EngineResponse
            {
                Ok = false,
                Error = message,
                Traceback = string.Empty,
            };
        }
    }
}
