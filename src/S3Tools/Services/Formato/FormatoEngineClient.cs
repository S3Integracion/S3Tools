using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S3Tools
{
    /// <summary>
    /// Cliente de alto nivel para el motor de Formato. Solo expone <see cref="ProcessAsync"/>;
    /// delega el trabajo en <see cref="FormatoEngine"/>.
    /// </summary>
    internal sealed class FormatoEngineClient
    {
        public Task<FormatoEngineResponse> ProcessAsync(FormatoEngineRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Action = "process";
            return Task.Run(() => FormatoEngine.Handle(request));
        }
    }

    /// <summary>
    /// Orquestador del motor de Formato. Recibe la solicitud, valida parámetros,
    /// itera los archivos y delega en <see cref="XlsxHeaderNormalizer"/> o
    /// <see cref="CsvHeaderNormalizer"/> según la extensión.
    /// </summary>
    internal static class FormatoEngine
    {
        private const string DefaultHeaderFormat = "underscore";

        public static FormatoEngineResponse Handle(FormatoEngineRequest request)
        {
            var action = (request?.Action ?? string.Empty).Trim().ToLowerInvariant();
            return action == "process" ? HandleProcess(request) : Error("Unknown action");
        }

        private static FormatoEngineResponse HandleProcess(FormatoEngineRequest data)
        {
            var inputFiles = (data?.InputFiles ?? Array.Empty<string>())
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();

            if (inputFiles.Length == 0) return Error("Missing input_files");

            var templateChoice = (data?.Template ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(templateChoice)) templateChoice = "auto";

            var headerFormat = (data?.HeaderFormat ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(headerFormat)) headerFormat = DefaultHeaderFormat;
            if (!TemplateResolver.HeaderFormats.ContainsKey(headerFormat))
            {
                return Error("Invalid header_format. Use 'hyphen' or 'underscore'.");
            }

            var updatedFiles = new List<string>();
            var templateCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var fp in inputFiles)
            {
                if (!File.Exists(fp)) return Error("Input file not found: " + fp);

                try
                {
                    var ext = Path.GetExtension(fp).ToLowerInvariant();
                    string updatedTemplate;
                    if (ext == ".xlsx")
                    {
                        updatedTemplate = XlsxHeaderNormalizer.UpdateHeaders(fp, templateChoice, headerFormat);
                    }
                    else if (ext == ".csv")
                    {
                        updatedTemplate = CsvHeaderNormalizer.UpdateHeaders(fp, templateChoice, headerFormat);
                    }
                    else
                    {
                        return Error("Failed to update " + fp + ": Unsupported file extension. Use .csv or .xlsx.");
                    }

                    updatedFiles.Add(fp);
                    if (!templateCounts.ContainsKey(updatedTemplate)) templateCounts[updatedTemplate] = 0;
                    templateCounts[updatedTemplate]++;
                }
                catch (Exception ex)
                {
                    return Error("Failed to update " + fp + ": " + ex.Message, ex.ToString());
                }
            }

            return new FormatoEngineResponse
            {
                Ok = true,
                UpdatedFiles = updatedFiles.ToArray(),
                TemplateCounts = templateCounts,
            };
        }

        private static FormatoEngineResponse Error(string message)
        {
            return Error(message, string.Empty);
        }

        private static FormatoEngineResponse Error(string message, string traceback)
        {
            return new FormatoEngineResponse
            {
                Ok = false,
                Error = message,
                Traceback = traceback ?? string.Empty,
            };
        }
    }
}
