using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>
    /// Ventana principal de la aplicación. Aloja las pestañas de las cinco herramientas
    /// (Asin Batcher, Sitemap, Formato, Asin no Report, Categorías) y resuelve el ícono.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string AppTitle = "S3Tools";
        private const string IconFileName = "S3Tools.ico";

        public MainForm()
        {
            InitializeComponent();
            Text = AppTitle;
            TrySetAppIcon();
        }

        /// <summary>Activa una pestaña por instancia (helper utilizado por los menús).</summary>
        private void ShowTab(TabPage tabPage)
        {
            if (tabPage != null)
            {
                tabControlPrograms.SelectedTab = tabPage;
            }
        }

        private void asinBatcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTab(tabAsinBatcher);
        }

        /// <summary>
        /// Carga el ícono de la aplicación desde el directorio del ejecutable.
        /// Si no se puede leer, conserva el ícono por defecto.
        /// </summary>
        private void TrySetAppIcon()
        {
            try
            {
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, IconFileName);
                if (File.Exists(iconPath))
                {
                    Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                // El ícono es decorativo: si falla, seguimos con el predeterminado.
                FileLogger.Warn(nameof(MainForm), "No se pudo cargar el ícono.", ex);
            }
        }
    }
}
