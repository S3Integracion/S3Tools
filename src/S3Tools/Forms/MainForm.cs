using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>
    /// Ventana principal de la aplicación. Aloja las pestañas de las cinco herramientas
    /// (Asin Batcher, Sitemap, Formato, Asin no Report, Categorías) más la de Plantillas,
    /// y resuelve el ícono.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string AppTitle = "S3Tools";
        private const string IconFileName = "S3Tools.ico";

        // La pestaña Plantillas se crea en código (no en el diseñador) para no instanciar el repositorio en tiempo de diseño.
        private TabPage tabPlantillas;
        private TemplateManagerControl templateManagerControl;

        public MainForm()
        {
            InitializeComponent();
            Text = AppTitle;
            TrySetAppIcon();
            AddTemplatesTab();
        }

        /// <summary>
        /// Agrega la pestaña «Plantillas» al final, con el administrador de plantillas de sitemap. Al estar en
        /// los controles de la pestaña, el panel se desecha junto con el formulario.
        /// </summary>
        private void AddTemplatesTab()
        {
            templateManagerControl = new TemplateManagerControl(SitemapTemplateRepository.Default)
            {
                Dock = DockStyle.Fill,
                Name = "templateManagerControl",
            };
            tabPlantillas = new TabPage
            {
                Name = "tabPlantillas",
                Text = "Plantillas",
                Padding = new Padding(3),
                UseVisualStyleBackColor = true,
            };
            tabPlantillas.Controls.Add(templateManagerControl);
            tabControlPrograms.TabPages.Add(tabPlantillas);
            tabControlPrograms.Deselecting += TabControlPrograms_Deselecting;
        }

        /// <summary>Al salir de Plantillas con cambios sin guardar, pregunta; «Cancelar» se queda en la pestaña.</summary>
        private void TabControlPrograms_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPlantillas && !templateManagerControl.ConfirmLeave())
            {
                e.Cancel = true;
            }
        }

        /// <summary>Al cerrar la aplicación con cambios sin guardar en Plantillas, pregunta; «Cancelar» no cierra.</summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!e.Cancel && templateManagerControl != null && !templateManagerControl.ConfirmLeave())
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
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
