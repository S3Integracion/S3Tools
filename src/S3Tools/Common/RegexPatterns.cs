using System.Text.RegularExpressions;

namespace S3Tools
{
    /// <summary>
    /// Patrones de expresiones regulares compartidos por todo el proyecto.
    /// Centralizar aquí evita duplicación y garantiza una única definición canónica.
    /// </summary>
    internal static class RegexPatterns
    {
        /// <summary>
        /// ASIN de Amazon: 10 caracteres alfanuméricos en mayúsculas (B0XXXXXXXX y similares).
        /// </summary>
        public static readonly Regex Asin = new Regex(
            "\\b[A-Z0-9]{10}\\b",
            RegexOptions.Compiled);

        /// <summary>
        /// URL absoluta http/https hasta el primer espacio o comilla.
        /// </summary>
        public static readonly Regex Url = new Regex(
            "https?://[^\\s\"']+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
