using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace S3Tools
{
    /// <summary>
    /// Niveles soportados por <see cref="FileLogger"/>, en orden de severidad ascendente.
    /// </summary>
    internal enum LogLevel
    {
        Info,
        Warning,
        Error,
    }

    /// <summary>
    /// Logger de archivo sencillo, thread-safe, sin dependencias externas.
    /// Escribe a <c>%LocalAppData%\S3Tools\logs\app-YYYYMMDD.log</c>.
    /// Diseñado para no levantar excepciones bajo ninguna circunstancia:
    /// si la escritura falla la app sigue funcionando.
    /// </summary>
    internal static class FileLogger
    {
        private static readonly object WriteLock = new object();

        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "S3Tools",
            "logs");

        public static void Info(string message)                                => Write(LogLevel.Info,    null, message, null);
        public static void Info(string source, string message)                 => Write(LogLevel.Info,    source, message, null);
        public static void Warn(string source, string message)                 => Write(LogLevel.Warning, source, message, null);
        public static void Warn(string source, string message, Exception ex)   => Write(LogLevel.Warning, source, message, ex);
        public static void Error(string source, string message, Exception ex)  => Write(LogLevel.Error,   source, message, ex);

        private static void Write(LogLevel level, string source, string message, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                var logPath = Path.Combine(
                    LogDirectory,
                    "app-" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");

                var line = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:yyyy-MM-dd HH:mm:ss.fff} [{1,-5}] [{2,4}] {3}{4}",
                    DateTime.Now,
                    LevelLabel(level),
                    Thread.CurrentThread.ManagedThreadId,
                    string.IsNullOrWhiteSpace(source) ? string.Empty : ("[" + source + "] "),
                    message ?? string.Empty);

                if (ex != null)
                {
                    line += Environment.NewLine + ex;
                }

                lock (WriteLock)
                {
                    File.AppendAllText(logPath, line + Environment.NewLine);
                }
            }
            catch (IOException)
            {
                // El log es best-effort: si falla la escritura, no afectamos el flujo de la app.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static string LevelLabel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:     return "INFO";
                case LogLevel.Warning:  return "WARN";
                case LogLevel.Error:    return "ERROR";
                default:                return "INFO";
            }
        }
    }
}
