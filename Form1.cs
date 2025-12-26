// Main shell form that hosts the tool tabs and injects their controls.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Integración_programs
{
    public partial class Form1 : Form
    {
        private AsinBatcherControl asinBatcherControl;
        private SitemapControl sitemapControl;
        private FormatoControl formatoControl;
        private S3ScraperControl s3ScraperControl;
        private ControlRemotoControl controlRemotoControl;

        public Form1()
        {
            InitializeComponent();
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

            if (s3ScraperControl == null)
            {
                s3ScraperControl = new S3ScraperControl
                {
                    Dock = DockStyle.Fill,
                };
                s3ScraperPanel.Controls.Add(s3ScraperControl);
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

        private void s3ScraperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTab(tabS3Scraper);
        }

        private void controlRemotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTab(tabControlRemoto);
        }
    }
}
