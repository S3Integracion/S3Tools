// Client wrapper for the Sitemap Python engine.
// Resolves the engine executable/script and exchanges JSON via stdin/stdout.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            EngineCommand command = null;
            try
            {
                command = ResolveEngine();
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
            catch (Win32Exception ex)
            {
                return new SitemapEngineResponse
                {
                    Ok = false,
                    Error = BuildStartError(ex, command),
                    Traceback = ex.ToString(),
                };
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
            var searchRoots = GetSearchRoots(baseDir);
            var env = Environment.GetEnvironmentVariable(EngineEnvVar);
            if (TryResolveEngineFromPath(env, searchRoots, out var envCommand))
            {
                return envCommand;
            }

            var extracted = TryExtractEmbeddedEngine();
            if (!string.IsNullOrWhiteSpace(extracted) && File.Exists(extracted))
            {
                return new EngineCommand(extracted, string.Empty);
            }

            var scriptRelative = Path.Combine(EngineRelativeFolder, EngineScriptName);
            if (TryResolveEngineFromPath(scriptRelative, searchRoots, out var localCommand))
            {
                return localCommand;
            }

            throw new FileNotFoundException(BuildEngineNotFoundMessage(searchRoots));
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

        private static bool TryResolveEngineFromPath(string input, IEnumerable<string> searchRoots, out EngineCommand command)
        {
            command = null;
            foreach (var candidate in GetPathCandidates(input))
            {
                if (TryResolveExistingPath(candidate, searchRoots, out var resolved))
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

        private static bool TryResolveExistingPath(string input, IEnumerable<string> searchRoots, out string resolvedPath)
        {
            resolvedPath = null;
            var candidate = (input ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return false;
            }

            if (Path.IsPathRooted(candidate))
            {
                if (!File.Exists(candidate))
                {
                    return false;
                }
                resolvedPath = Path.GetFullPath(candidate);
                return true;
            }

            if (File.Exists(candidate))
            {
                resolvedPath = Path.GetFullPath(candidate);
                return true;
            }

            foreach (var root in searchRoots ?? Array.Empty<string>())
            {
                var combined = Path.Combine(root, candidate);
                if (!File.Exists(combined))
                {
                    continue;
                }
                resolvedPath = Path.GetFullPath(combined);
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetSearchRoots(string baseDir)
        {
            var roots = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddRoot(string path)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }
                string fullPath;
                try
                {
                    fullPath = Path.GetFullPath(path);
                }
                catch
                {
                    return;
                }
                if (!Directory.Exists(fullPath))
                {
                    return;
                }
                if (seen.Add(fullPath))
                {
                    roots.Add(fullPath);
                }
            }

            AddRoot(baseDir);
            AddRoot(Path.Combine(baseDir, "bin", "Debug"));
            AddRoot(Path.Combine(baseDir, "bin", "Release"));

            var current = new DirectoryInfo(baseDir);
            for (var i = 0; i < 3 && current?.Parent != null; i++)
            {
                current = current.Parent;
                if (current == null)
                {
                    break;
                }
                AddRoot(current.FullName);
                AddRoot(Path.Combine(current.FullName, "bin", "Debug"));
                AddRoot(Path.Combine(current.FullName, "bin", "Release"));
            }

            return roots;
        }

        private static string BuildEngineNotFoundMessage(IEnumerable<string> searchRoots)
        {
            var roots = string.Join(Environment.NewLine, searchRoots.Select(root => " - " + root));
            var expected = Path.Combine(EngineRelativeFolder, EngineExeName);
            return "No se encontro el motor de Sitemap. Esperado: " + expected +
                   Environment.NewLine + "Rutas buscadas:" + Environment.NewLine + roots +
                   Environment.NewLine + "Ejecuta build_engines.ps1 o configura " + EngineEnvVar + ".";
        }

        private static string BuildStartError(Exception ex, EngineCommand command)
        {
            if (command == null)
            {
                return ex.Message;
            }
            return ex.Message + Environment.NewLine + "Comando: " + command.FileName + " " + command.Arguments;
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

        [DataMember(Name = "name_prefix_1")]
        public string NamePrefix1 { get; set; }

        [DataMember(Name = "name_prefix_2")]
        public string NamePrefix2 { get; set; }

        [DataMember(Name = "store_name")]
        public string StoreName { get; set; }
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
