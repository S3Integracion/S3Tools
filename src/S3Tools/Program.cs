using System;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>
    /// Punto de entrada de la aplicación WinForms.
    /// Mantiene el arranque mínimo y delega en <see cref="MainForm"/>.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            FileLogger.Info("Program", "S3Tools starting (v" +
                typeof(Program).Assembly.GetName().Version + ")");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                FileLogger.Error("Program", "Fatal error in main loop", ex);
                throw;
            }
            finally
            {
                FileLogger.Info("Program", "S3Tools shutting down");
            }
        }
    }
}
