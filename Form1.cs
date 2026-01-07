// Main shell form that hosts the tool tabs and injects their controls.
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace S3Integración_programs
{
    public partial class Form1 : Form
    {
        private AsinBatcherControl asinBatcherControl;
        private SitemapControl sitemapControl;
        private FormatoControl formatoControl;
        private ControlRemotoControl controlRemotoControl;

        public Form1()
        {
            InitializeComponent();
            Text = "S3Tools";
            TrySetAppIcon();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Lazy-create controls to keep designer code minimal.
            if (asinBatcherControl == null)
            {
                asinBatcherControl = new AsinBatcherControl
                {
                    Dock = DockStyle.Fill,
                };
                asinBatcherPanel.Controls.Add(asinBatcherControl);
            }

            if (sitemapControl == null)
            {
                sitemapControl = new SitemapControl
                {
                    Dock = DockStyle.Fill,
                };
                sitemapPanel.Controls.Add(sitemapControl);
            }

            if (formatoControl == null)
            {
                formatoControl = new FormatoControl
                {
                    Dock = DockStyle.Fill,
                };
                formatoPanel.Controls.Add(formatoControl);
            }

            if (controlRemotoControl == null)
            {
                controlRemotoControl = new ControlRemotoControl
                {
                    Dock = DockStyle.Fill,
                };
                controlRemotoPanel.Controls.Add(controlRemotoControl);
            }
        }

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

        private void controlRemotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTab(tabControlRemoto);
        }

        private void TrySetAppIcon()
        {
            try
            {
                var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "S3Tools.ico");
                if (File.Exists(iconPath))
                {
                    Icon = new Icon(iconPath);
                }
            }
            catch
            {
                // Keep default icon if the custom one cannot be loaded.
            }
        }
    }
}
