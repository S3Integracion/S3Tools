using System;
using System.IO;

namespace S3Tools
{
    /// <summary>
    /// Persistencia ligera de estado local de la UI.
    /// Hoy solo guarda la última carpeta de salida del módulo ASIN Batcher
    /// para que <see cref="SitemapEngineClient"/> pueda precargar archivos recientes.
    /// Las rutas viven en <c>%LocalAppData%\S3Tools</c>.
    /// </summary>
    internal static class AppState
    {
        private const string LastAsinOutputFileName = "last_asin_output_dir.txt";

        private static readonly string StateDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "S3Tools");

        private static readonly string LastAsinOutputFile = Path.Combine(StateDir, LastAsinOutputFileName);

        /// <summary>
        /// Persiste la carpeta de salida más reciente del ASIN Batcher.
        /// Los fallos de I/O se registran pero no se propagan al caller para no bloquear la UI.
        /// </summary>
        public static void SetLastAsinOutputDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                Directory.CreateDirectory(StateDir);
                File.WriteAllText(LastAsinOutputFile, path.Trim());
            }
            catch (IOException ex)
            {
                FileLogger.Warn(nameof(AppState), "No se pudo persistir la última carpeta ASIN Batcher.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Warn(nameof(AppState), "Sin permisos para persistir la última carpeta ASIN Batcher.", ex);
            }
        }

        /// <summary>
        /// Persiste el id de la última plantilla usada en <paramref name="context"/> (<c>"sitemap"</c> o <c>"formato"</c>).
        /// Los fallos de I/O se registran pero no se propagan al caller para no bloquear la UI.
        /// </summary>
        /// <exception cref="ArgumentException">Si <paramref name="context"/> no es <c>"sitemap"</c> ni <c>"formato"</c>.</exception>
        public static void SetLastTemplateId(string context, string id)
        {
            var file = GetLastTemplateFile(context);
            if (string.IsNullOrWhiteSpace(id)) return;

            try
            {
                Directory.CreateDirectory(StateDir);
                File.WriteAllText(file, id.Trim());
            }
            catch (IOException ex)
            {
                FileLogger.Warn(nameof(AppState), "No se pudo persistir la última plantilla de " + context + ".", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Warn(nameof(AppState), "Sin permisos para persistir la última plantilla de " + context + ".", ex);
            }
        }

        /// <summary>
        /// Recupera el id de la última plantilla usada en <paramref name="context"/>. No comprueba que
        /// la plantilla siga existiendo: eso lo valida la UI contra el repositorio.
        /// </summary>
        /// <exception cref="ArgumentException">Si <paramref name="context"/> no es <c>"sitemap"</c> ni <c>"formato"</c>.</exception>
        public static bool TryGetLastTemplateId(string context, out string id)
        {
            id = null;
            var file = GetLastTemplateFile(context);

            try
            {
                if (!File.Exists(file)) return false;

                var stored = (File.ReadAllText(file) ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(stored)) return false;

                id = stored;
                return true;
            }
            catch (IOException ex)
            {
                FileLogger.Warn(nameof(AppState), "No se pudo leer la última plantilla de " + context + ".", ex);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Warn(nameof(AppState), "Sin permisos para leer la última plantilla de " + context + ".", ex);
                return false;
            }
        }

        private static string GetLastTemplateFile(string context)
        {
            if (context != "sitemap" && context != "formato")
            {
                throw new ArgumentException("El contexto debe ser \"sitemap\" o \"formato\".", nameof(context));
            }

            return Path.Combine(StateDir, "last_template_" + context + ".txt");
        }

        /// <summary>
        /// Recupera la última carpeta de salida del ASIN Batcher si existe y sigue siendo válida.
        /// </summary>
        public static bool TryGetLastAsinOutputDir(out string path)
        {
            path = null;
            try
            {
                if (!File.Exists(LastAsinOutputFile)) return false;

                var stored = (File.ReadAllText(LastAsinOutputFile) ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(stored) || !Directory.Exists(stored)) return false;

                path = stored;
                return true;
            }
            catch (IOException ex)
            {
                FileLogger.Warn(nameof(AppState), "No se pudo leer la última carpeta ASIN Batcher.", ex);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                FileLogger.Warn(nameof(AppState), "Sin permisos para leer la última carpeta ASIN Batcher.", ex);
                return false;
            }
        }
    }
}
