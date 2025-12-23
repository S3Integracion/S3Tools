using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace S3Integración_programs
{
    internal sealed class AsinBatcherEngineClient
    {
        private const string EngineExeName = "AsinBatcherEngine.exe";
        private const string EngineScriptName = "engine.py";
        private const string EngineEnvVar = "ASIN_BATCHER_ENGINE_PATH";
        private static readonly string EngineRelativeFolder = Path.Combine("Engines", "AsinBatcherEngine");

        public Task<EngineResponse> PreviewAsync(string inputPath)
        {
            return SendAsync(new EngineRequest
            {
                Action = "preview",
                InputPath = inputPath,
            });
        }

        public Task<EngineResponse> ExportDuplicatesAsync(string inputPath, string outputDir)
        {
            return SendAsync(new EngineRequest
            {
                Action = "export_duplicates",
                InputPath = inputPath,
                OutputDir = outputDir,
            });
        }

        public Task<EngineResponse> ProcessAsync(EngineRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.Action = "process";
            return SendAsync(request);
        }

        private Task<EngineResponse> SendAsync(EngineRequest request)
        {
            return Task.Run(() => Send(request));
        }

        private EngineResponse Send(EngineRequest request)
        {
            try
            {
                var command = ResolveEngine();
                var json = Serialize(request);
                var psi = new ProcessStartInfo
                {
                    FileName = command.FileName,
                    Arguments = command.Arguments,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                };

                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();
                    using (var writer = process.StandardInput)
                    {
                        writer.Write(json);
                    }
                    var stdout = process.StandardOutput.ReadToEnd();
                    var stderr = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    stdout = (stdout ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(stdout))
                    {
                        return new EngineResponse
                        {
                            Ok = false,
                            Error = "Engine returned no output.",
                            Traceback = stderr ?? string.Empty,
                        };
                    }

                    var response = Deserialize(stdout);
                    if (response == null)
                    {
                        return new EngineResponse
                        {
                            Ok = false,
                            Error = "Invalid engine response.",
                            Traceback = stdout + Environment.NewLine + (stderr ?? string.Empty),
                        };
                    }

                    if (!response.Ok && string.IsNullOrWhiteSpace(response.Traceback) && !string.IsNullOrWhiteSpace(stderr))
                    {
                        response.Traceback = stderr;
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new EngineResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                };
            }
        }

        private static EngineCommand ResolveEngine()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var env = Environment.GetEnvironmentVariable(EngineEnvVar);
            if (!string.IsNullOrWhiteSpace(env))
            {
                if (TryResolveRelativePath(env, baseDir, out var resolvedEnv))
                {
                    if (resolvedEnv.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
                    {
                        return new EngineCommand("python", Quote(resolvedEnv));
                    }
                    return new EngineCommand(resolvedEnv, string.Empty);
                }
            }

            var extracted = TryExtractEmbeddedEngine();
            if (!string.IsNullOrWhiteSpace(extracted) && File.Exists(extracted))
            {
                return new EngineCommand(extracted, string.Empty);
            }

            var exeRelative = Path.Combine(EngineRelativeFolder, EngineExeName);
            var exePath = Path.Combine(baseDir, exeRelative);
            if (File.Exists(exePath))
            {
                return new EngineCommand(exeRelative, string.Empty);
            }

            var scriptRelative = Path.Combine(EngineRelativeFolder, EngineScriptName);
            var scriptPath = Path.Combine(baseDir, scriptRelative);
            if (File.Exists(scriptPath))
            {
                return new EngineCommand("python", Quote(scriptRelative));
            }

            throw new FileNotFoundException("AsinBatcher engine not found. Configure ASIN_BATCHER_ENGINE_PATH or embed the engine.");
        }

        private static string TryExtractEmbeddedEngine()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith(EngineExeName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                return null;
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "S3Integracion", "AsinBatcherEngine");
            Directory.CreateDirectory(tempDir);
            var targetPath = Path.Combine(tempDir, EngineExeName);
            if (File.Exists(targetPath))
            {
                return targetPath;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }
                using (var file = File.Create(targetPath))
                {
                    stream.CopyTo(file);
                }
            }
            return targetPath;
        }

        private static string Serialize(EngineRequest request)
        {
            var serializer = new DataContractJsonSerializer(typeof(EngineRequest));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, request);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static EngineResponse Deserialize(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(EngineResponse));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return serializer.ReadObject(ms) as EngineResponse;
            }
        }

        private static string Quote(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "\"\"";
            }
            return "\"" + value.Replace("\"", "\\\"") + "\"";
        }

        private static bool TryResolveRelativePath(string input, string baseDir, out string relativePath)
        {
            relativePath = null;
            var candidate = (input ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return false;
            }

            string fullPath;
            if (Path.IsPathRooted(candidate))
            {
                fullPath = candidate;
                if (!File.Exists(fullPath))
                {
                    return false;
                }
            }
            else
            {
                fullPath = candidate;
                if (!File.Exists(fullPath))
                {
                    var combined = Path.Combine(baseDir, candidate);
                    if (!File.Exists(combined))
                    {
                        return false;
                    }
                    fullPath = combined;
                }
            }

            fullPath = Path.GetFullPath(fullPath);
            var rel = GetRelativePath(baseDir, fullPath);
            if (Path.IsPathRooted(rel))
            {
                if (!Path.IsPathRooted(candidate))
                {
                    relativePath = candidate;
                    return true;
                }
                return false;
            }

            relativePath = rel;
            return true;
        }

        private static string GetRelativePath(string baseDir, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(baseDir) || string.IsNullOrWhiteSpace(fullPath))
            {
                return fullPath;
            }

            var baseUri = new Uri(AppendDirectorySeparator(baseDir));
            var fullUri = new Uri(fullPath);
            if (!string.Equals(baseUri.Scheme, fullUri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return fullPath;
            }

            var relativeUri = baseUri.MakeRelativeUri(fullUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        private static string AppendDirectorySeparator(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                return path;
            }
            return path + Path.DirectorySeparatorChar;
        }

        private sealed class EngineCommand
        {
            public EngineCommand(string fileName, string arguments)
            {
                FileName = fileName;
                Arguments = arguments;
            }

            public string FileName { get; }
            public string Arguments { get; }
        }
    }

    [DataContract]
    internal sealed class EngineRequest
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "input_path")]
        public string InputPath { get; set; }

        [DataMember(Name = "output_dir")]
        public string OutputDir { get; set; }

        [DataMember(Name = "market")]
        public string Market { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "order")]
        public string Order { get; set; }

        [DataMember(Name = "batches")]
        public int? Batches { get; set; }

        [DataMember(Name = "zip_output")]
        public bool? ZipOutput { get; set; }

        [DataMember(Name = "file_label")]
        public string FileLabel { get; set; }
    }

    [DataContract]
    internal sealed class EngineResponse
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "traceback")]
        public string Traceback { get; set; }

        [DataMember(Name = "total")]
        public int? Total { get; set; }

        [DataMember(Name = "unique")]
        public int? Unique { get; set; }

        [DataMember(Name = "duplicates")]
        public int? Duplicates { get; set; }

        [DataMember(Name = "output_folder")]
        public string OutputFolder { get; set; }

        [DataMember(Name = "zip_path")]
        public string ZipPath { get; set; }

        [DataMember(Name = "csv_path")]
        public string CsvPath { get; set; }
    }
}
