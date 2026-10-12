using System;
using System.IO;

namespace S3Tools.Tests
{
    /// <summary>Localiza archivos del repositorio sin depender de lo que se copie al directorio de salida.</summary>
    internal static class TestPaths
    {
        private static readonly string RepoRoot = FindRepoRoot();

        /// <summary>Carpeta <c>src/S3Tools/Resources</c> con las plantillas de fábrica.</summary>
        public static string ResourcesDir
        {
            get { return Path.Combine(RepoRoot, "src", "S3Tools", "Resources"); }
        }

        /// <summary>Ruta absoluta de un archivo dentro de <c>src/S3Tools/Resources</c>.</summary>
        public static string Resource(string fileName)
        {
            return Path.Combine(ResourcesDir, fileName);
        }

        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "S3Tools.slnx")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }

            throw new InvalidOperationException(
                "No se encontró S3Tools.slnx subiendo desde " + AppContext.BaseDirectory);
        }
    }
}
