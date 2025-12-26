namespace S3Integración_programs
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tabControlPrograms = new System.Windows.Forms.TabControl();
            this.tabAsinBatcher = new System.Windows.Forms.TabPage();
            this.asinBatcherPanel = new System.Windows.Forms.Panel();
            this.tabSitemap = new System.Windows.Forms.TabPage();
            this.sitemapPanel = new System.Windows.Forms.Panel();
            this.tabFormato = new System.Windows.Forms.TabPage();
            this.formatoPanel = new System.Windows.Forms.Panel();
            this.tabS3Scraper = new System.Windows.Forms.TabPage();
            this.s3ScraperPanel = new System.Windows.Forms.Panel();
            this.tabControlRemoto = new System.Windows.Forms.TabPage();
            this.controlRemotoPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabControlPrograms.SuspendLayout();
            this.tabAsinBatcher.SuspendLayout();
            this.tabSitemap.SuspendLayout();
            this.tabFormato.SuspendLayout();
            this.tabS3Scraper.SuspendLayout();
            this.tabControlRemoto.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlPrograms
            // 
            this.tabControlPrograms.Controls.Add(this.tabAsinBatcher);
            this.tabControlPrograms.Controls.Add(this.tabSitemap);
            this.tabControlPrograms.Controls.Add(this.tabFormato);
            this.tabControlPrograms.Controls.Add(this.tabS3Scraper);
            this.tabControlPrograms.Controls.Add(this.tabControlRemoto);
            this.tabControlPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPrograms.Location = new System.Drawing.Point(0, 0);
            this.tabControlPrograms.Name = "tabControlPrograms";
            this.tabControlPrograms.SelectedIndex = 0;
            this.tabControlPrograms.Size = new System.Drawing.Size(1200, 800);
            this.tabControlPrograms.TabIndex = 1;
            // 
            // tabAsinBatcher
            // 
            this.tabAsinBatcher.Controls.Add(this.asinBatcherPanel);
            this.tabAsinBatcher.Location = new System.Drawing.Point(4, 25);
            this.tabAsinBatcher.Name = "tabAsinBatcher";
            this.tabAsinBatcher.Padding = new System.Windows.Forms.Padding(3);
            this.tabAsinBatcher.Size = new System.Drawing.Size(1192, 771);
            this.tabAsinBatcher.TabIndex = 0;
            this.tabAsinBatcher.Text = "Asin Batcher";
            this.tabAsinBatcher.UseVisualStyleBackColor = true;
            // 
            // asinBatcherPanel
            // 
            this.asinBatcherPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.asinBatcherPanel.Location = new System.Drawing.Point(3, 3);
            this.asinBatcherPanel.Name = "asinBatcherPanel";
            this.asinBatcherPanel.Size = new System.Drawing.Size(1186, 765);
            this.asinBatcherPanel.TabIndex = 0;
            // 
            // tabSitemap
            // 
            this.tabSitemap.Controls.Add(this.sitemapPanel);
            this.tabSitemap.Location = new System.Drawing.Point(4, 25);
            this.tabSitemap.Name = "tabSitemap";
            this.tabSitemap.Padding = new System.Windows.Forms.Padding(3);
            this.tabSitemap.Size = new System.Drawing.Size(1192, 771);
            this.tabSitemap.TabIndex = 1;
            this.tabSitemap.Text = "Sitemap";
            this.tabSitemap.UseVisualStyleBackColor = true;
            // 
            // sitemapPanel
            // 
            this.sitemapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sitemapPanel.Location = new System.Drawing.Point(3, 3);
            this.sitemapPanel.Name = "sitemapPanel";
            this.sitemapPanel.Size = new System.Drawing.Size(1186, 765);
            this.sitemapPanel.TabIndex = 0;
            // 
            // tabFormato
            // 
            this.tabFormato.Controls.Add(this.formatoPanel);
            this.tabFormato.Location = new System.Drawing.Point(4, 25);
            this.tabFormato.Name = "tabFormato";
            this.tabFormato.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormato.Size = new System.Drawing.Size(1192, 771);
            this.tabFormato.TabIndex = 2;
            this.tabFormato.Text = "Formato";
            this.tabFormato.UseVisualStyleBackColor = true;
            // 
            // formatoPanel
            // 
            this.formatoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatoPanel.Location = new System.Drawing.Point(3, 3);
            this.formatoPanel.Name = "formatoPanel";
            this.formatoPanel.Size = new System.Drawing.Size(1186, 765);
            this.formatoPanel.TabIndex = 0;
            // 
            // tabS3Scraper
            // 
            this.tabS3Scraper.Controls.Add(this.s3ScraperPanel);
            this.tabS3Scraper.Location = new System.Drawing.Point(4, 25);
            this.tabS3Scraper.Name = "tabS3Scraper";
            this.tabS3Scraper.Padding = new System.Windows.Forms.Padding(3);
            this.tabS3Scraper.Size = new System.Drawing.Size(1174, 644);
            this.tabS3Scraper.TabIndex = 3;
            this.tabS3Scraper.Text = "S3Scraper";
            this.tabS3Scraper.UseVisualStyleBackColor = true;
            // 
            // s3ScraperPanel
            // 
            this.s3ScraperPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.s3ScraperPanel.Location = new System.Drawing.Point(3, 3);
            this.s3ScraperPanel.Name = "s3ScraperPanel";
            this.s3ScraperPanel.Size = new System.Drawing.Size(1168, 638);
            this.s3ScraperPanel.TabIndex = 0;
            // 
            // tabControlRemoto
            // 
            this.tabControlRemoto.Controls.Add(this.controlRemotoPanel);
            this.tabControlRemoto.Location = new System.Drawing.Point(4, 25);
            this.tabControlRemoto.Name = "tabControlRemoto";
            this.tabControlRemoto.Padding = new System.Windows.Forms.Padding(3);
            this.tabControlRemoto.Size = new System.Drawing.Size(1192, 771);
            this.tabControlRemoto.TabIndex = 4;
            this.tabControlRemoto.Text = "Control_Remoto";
            this.tabControlRemoto.UseVisualStyleBackColor = true;
            // 
            // controlRemotoPanel
            // 
            this.controlRemotoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlRemotoPanel.Location = new System.Drawing.Point(3, 3);
            this.controlRemotoPanel.Name = "controlRemotoPanel";
            this.controlRemotoPanel.Size = new System.Drawing.Size(1186, 765);
            this.controlRemotoPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.tabControlPrograms);
            this.Name = "Form1";
            this.Text = "S3integración_programs";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabControlPrograms.ResumeLayout(false);
            this.tabAsinBatcher.ResumeLayout(false);
            this.tabSitemap.ResumeLayout(false);
            this.tabFormato.ResumeLayout(false);
            this.tabS3Scraper.ResumeLayout(false);
            this.tabControlRemoto.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TabControl tabControlPrograms;
        private System.Windows.Forms.TabPage tabAsinBatcher;
        private System.Windows.Forms.TabPage tabSitemap;
        private System.Windows.Forms.TabPage tabFormato;
        private System.Windows.Forms.TabPage tabS3Scraper;
        private System.Windows.Forms.TabPage tabControlRemoto;
        private System.Windows.Forms.Panel asinBatcherPanel;
        private System.Windows.Forms.Panel sitemapPanel;
        private System.Windows.Forms.Panel formatoPanel;
        private System.Windows.Forms.Panel s3ScraperPanel;
        private System.Windows.Forms.Panel controlRemotoPanel;
    }
}

