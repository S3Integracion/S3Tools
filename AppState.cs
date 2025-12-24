using System;
using System.IO;

namespace S3Integración_programs
{
    internal static class AppState
    {
        private static readonly string StateDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "S3Integracion");

        private static readonly string LastAsinOutputFile = Path.Combine(StateDir, "last_asin_output_dir.txt");

        public static void SetLastAsinOutputDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(StateDir);
                File.WriteAllText(LastAsinOutputFile, path.Trim());
            }
            catch
            {
                // Ignore persistence failures to avoid blocking the UI.
            }
        }

        public static bool TryGetLastAsinOutputDir(out string path)
        {
            path = null;
            try
            {
                if (!File.Exists(LastAsinOutputFile))
                {
                    return false;
                }

                var stored = (File.ReadAllText(LastAsinOutputFile) ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(stored) || !Directory.Exists(stored))
                {
                    return false;
                }

                path = stored;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
