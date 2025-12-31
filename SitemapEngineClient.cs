// Client wrapper for the Sitemap Python engine.
// Resolves the engine executable/script and exchanges JSON via stdin/stdout.
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
    internal sealed class SitemapEngineClient
    {
        private const string EngineScriptName = "form_site.py";
        private static readonly string EngineExeName = Path.ChangeExtension(EngineScriptName, ".exe");
        private const string EngineEnvVar = "SITEMAP_ENGINE_PATH";
        private static readonly string EngineRelativeFolder = Path.Combine("Engines", "Sitemap");

        public Task<SitemapEngineResponse> ProcessAsync(SitemapEngineRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.Action = "process";
            return Task.Run(() => Send(request));
        }

        private SitemapEngineResponse Send(SitemapEngineRequest request)
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
                        return new SitemapEngineResponse
                        {
                            Ok = false,
                            Error = "Engine returned no output.",
                            Traceback = stderr ?? string.Empty,
                        };
                    }

                    var response = Deserialize(stdout);
                    if (response == null)
                    {
                        return new SitemapEngineResponse
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
                return new SitemapEngineResponse
                {
                    Ok = false,
                    Error = ex.Message,
                    Traceback = ex.ToString(),
                };
            }
        }

        // Resolution order: env var -> embedded exe -> local exe/script (same base name).
        private static EngineCommand ResolveEngine()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var env = Environment.GetEnvironmentVariable(EngineEnvVar);
            if (TryResolveEngineFromPath(env, baseDir, out var envCommand))
            {
                return envCommand;
            }

            var extracted = TryExtractEmbeddedEngine();
            if (!string.IsNullOrWhiteSpace(extracted) && File.Exists(extracted))
            {
                return new EngineCommand(extracted, string.Empty);
            }

            var scriptRelative = Path.Combine(EngineRelativeFolder, EngineScriptName);
            if (TryResolveEngineFromPath(scriptRelative, baseDir, out var localCommand))
            {
                return localCommand;
            }

            throw new FileNotFoundException("Sitemap engine not found. Configure SITEMAP_ENGINE_PATH or embed the engine.");
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

            var tempDir = Path.Combine(Path.GetTempPath(), "S3Integracion", "SitemapEngine");
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

        private static string Serialize(SitemapEngineRequest request)
        {
            var serializer = new DataContractJsonSerializer(typeof(SitemapEngineRequest));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, request);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static SitemapEngineResponse Deserialize(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(SitemapEngineResponse));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return serializer.ReadObject(ms) as SitemapEngineResponse;
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

        private static bool TryResolveEngineFromPath(string input, string baseDir, out EngineCommand command)
        {
            command = null;
            foreach (var candidate in GetPathCandidates(input))
            {
                if (TryResolveRelativePath(candidate, baseDir, out var resolved))
                {
                    command = CreateCommand(resolved);
                    return true;
                }
            }
            return false;
        }

        private static string[] GetPathCandidates(string input)
        {
            var trimmed = (input ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return Array.Empty<string>();
            }

            var ext = Path.GetExtension(trimmed);
            if (string.IsNullOrWhiteSpace(ext))
            {
                return new[]
                {
                    trimmed + ".exe",
                    trimmed + ".py",
                    trimmed,
                };
            }

            if (ext.Equals(".py", StringComparison.OrdinalIgnoreCase))
            {
                return new[]
                {
                    Path.ChangeExtension(trimmed, ".exe"),
                    trimmed,
                };
            }

            if (ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return new[]
                {
                    trimmed,
                    Path.ChangeExtension(trimmed, ".py"),
                };
            }

            return new[]
            {
                trimmed,
            };
        }

        private static EngineCommand CreateCommand(string resolvedPath)
        {
            if (resolvedPath.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
            {
                return new EngineCommand("python", Quote(resolvedPath));
            }
            return new EngineCommand(resolvedPath, string.Empty);
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
    internal sealed class SitemapEngineRequest
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "input_files")]
        public string[] InputFiles { get; set; }

        [DataMember(Name = "output_dir")]
        public string OutputDir { get; set; }

        [DataMember(Name = "base_name")]
        public string BaseName { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "zip_output")]
        public bool? ZipOutput { get; set; }
    }

    [DataContract]
    internal sealed class SitemapEngineResponse
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "traceback")]
        public string Traceback { get; set; }

        [DataMember(Name = "output_folder")]
        public string OutputFolder { get; set; }

        [DataMember(Name = "zip_path")]
        public string ZipPath { get; set; }

        [DataMember(Name = "output_files")]
        public string[] OutputFiles { get; set; }
    }
}
