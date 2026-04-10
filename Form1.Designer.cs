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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabControlPrograms = new System.Windows.Forms.TabControl();
            tabAsinBatcher = new System.Windows.Forms.TabPage();
            asinBatcherPanel = new System.Windows.Forms.Panel();
            asinBatcherControl = new AsinBatcherControl();
            tabSitemap = new System.Windows.Forms.TabPage();
            sitemapPanel = new System.Windows.Forms.Panel();
            sitemapControl = new SitemapControl();
            tabFormato = new System.Windows.Forms.TabPage();
            formatoPanel = new System.Windows.Forms.Panel();
            formatoControl = new FormatoControl();
            tabPage5 = new System.Windows.Forms.TabPage();
            asinNoReportPanel = new System.Windows.Forms.Panel();
            asinNoReportControl = new AsinNoReportControl();
            tabControlPrograms.SuspendLayout();
            tabAsinBatcher.SuspendLayout();
            asinBatcherPanel.SuspendLayout();
            tabSitemap.SuspendLayout();
            sitemapPanel.SuspendLayout();
            tabFormato.SuspendLayout();
            formatoPanel.SuspendLayout();
            tabPage5.SuspendLayout();
            asinNoReportPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlPrograms
            // 
            tabControlPrograms.Controls.Add(tabAsinBatcher);
            tabControlPrograms.Controls.Add(tabSitemap);
            tabControlPrograms.Controls.Add(tabFormato);
            tabControlPrograms.Controls.Add(tabPage5);
            tabControlPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlPrograms.Location = new System.Drawing.Point(0, 0);
            tabControlPrograms.Name = "tabControlPrograms";
            tabControlPrograms.SelectedIndex = 0;
            tabControlPrograms.Size = new System.Drawing.Size(837, 785);
            tabControlPrograms.TabIndex = 1;
            // 
            // tabAsinBatcher
            // 
            tabAsinBatcher.Controls.Add(asinBatcherPanel);
            tabAsinBatcher.Location = new System.Drawing.Point(4, 24);
            tabAsinBatcher.Name = "tabAsinBatcher";
            tabAsinBatcher.Padding = new System.Windows.Forms.Padding(3);
            tabAsinBatcher.Size = new System.Drawing.Size(829, 757);
            tabAsinBatcher.TabIndex = 0;
            tabAsinBatcher.Text = "Asin Batcher";
            tabAsinBatcher.UseVisualStyleBackColor = true;
            // 
            // asinBatcherPanel
            // 
            asinBatcherPanel.Controls.Add(asinBatcherControl);
            asinBatcherPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            asinBatcherPanel.Location = new System.Drawing.Point(3, 3);
            asinBatcherPanel.Name = "asinBatcherPanel";
            asinBatcherPanel.Size = new System.Drawing.Size(823, 751);
            asinBatcherPanel.TabIndex = 0;
            // 
            // asinBatcherControl
            // 
            asinBatcherControl.AutoScroll = true;
            asinBatcherControl.Dock = System.Windows.Forms.DockStyle.Fill;
            asinBatcherControl.Location = new System.Drawing.Point(0, 0);
            asinBatcherControl.Name = "asinBatcherControl";
            asinBatcherControl.Size = new System.Drawing.Size(823, 751);
            asinBatcherControl.TabIndex = 0;
            asinBatcherControl.Load += asinBatcherControl_Load;
            // 
            // tabSitemap
            // 
            tabSitemap.Controls.Add(sitemapPanel);
            tabSitemap.Location = new System.Drawing.Point(4, 24);
            tabSitemap.Name = "tabSitemap";
            tabSitemap.Padding = new System.Windows.Forms.Padding(3);
            tabSitemap.Size = new System.Drawing.Size(192, 72);
            tabSitemap.TabIndex = 1;
            tabSitemap.Text = "Sitemap";
            tabSitemap.UseVisualStyleBackColor = true;
            // 
            // sitemapPanel
            // 
            sitemapPanel.Controls.Add(sitemapControl);
            sitemapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            sitemapPanel.Location = new System.Drawing.Point(3, 3);
            sitemapPanel.Name = "sitemapPanel";
            sitemapPanel.Size = new System.Drawing.Size(186, 66);
            sitemapPanel.TabIndex = 0;
            // 
            // sitemapControl
            // 
            sitemapControl.AutoScroll = true;
            sitemapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            sitemapControl.Location = new System.Drawing.Point(0, 0);
            sitemapControl.Name = "sitemapControl";
            sitemapControl.Size = new System.Drawing.Size(186, 66);
            sitemapControl.TabIndex = 0;
            // 
            // tabFormato
            // 
            tabFormato.Controls.Add(formatoPanel);
            tabFormato.Location = new System.Drawing.Point(4, 24);
            tabFormato.Name = "tabFormato";
            tabFormato.Padding = new System.Windows.Forms.Padding(3);
            tabFormato.Size = new System.Drawing.Size(192, 72);
            tabFormato.TabIndex = 2;
            tabFormato.Text = "Formato";
            tabFormato.UseVisualStyleBackColor = true;
            // 
            // formatoPanel
            // 
            formatoPanel.Controls.Add(formatoControl);
            formatoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            formatoPanel.Location = new System.Drawing.Point(3, 3);
            formatoPanel.Name = "formatoPanel";
            formatoPanel.Size = new System.Drawing.Size(186, 66);
            formatoPanel.TabIndex = 0;
            // 
            // formatoControl
            // 
            formatoControl.AutoScroll = true;
            formatoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            formatoControl.Location = new System.Drawing.Point(0, 0);
            formatoControl.Name = "formatoControl";
            formatoControl.Size = new System.Drawing.Size(186, 66);
            formatoControl.TabIndex = 0;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(asinNoReportPanel);
            tabPage5.Location = new System.Drawing.Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new System.Drawing.Size(829, 757);
            tabPage5.TabIndex = 5;
            tabPage5.Text = "Asin no Report";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // asinNoReportPanel
            // 
            asinNoReportPanel.Controls.Add(asinNoReportControl);
            asinNoReportPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            asinNoReportPanel.Location = new System.Drawing.Point(0, 0);
            asinNoReportPanel.Name = "asinNoReportPanel";
            asinNoReportPanel.Size = new System.Drawing.Size(829, 757);
            asinNoReportPanel.TabIndex = 0;
            // 
            // asinNoReportControl
            // 
            asinNoReportControl.AutoScroll = true;
            asinNoReportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            asinNoReportControl.Location = new System.Drawing.Point(0, 0);
            asinNoReportControl.Name = "asinNoReportControl";
            asinNoReportControl.Size = new System.Drawing.Size(829, 757);
            asinNoReportControl.TabIndex = 0;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(837, 785);
            Controls.Add(tabControlPrograms);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "S3Tools";
            Load += Form1_Load;
            tabControlPrograms.ResumeLayout(false);
            tabAsinBatcher.ResumeLayout(false);
            asinBatcherPanel.ResumeLayout(false);
            tabSitemap.ResumeLayout(false);
            sitemapPanel.ResumeLayout(false);
            tabFormato.ResumeLayout(false);
            formatoPanel.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            asinNoReportPanel.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControlPrograms;
        private System.Windows.Forms.TabPage tabAsinBatcher;
        private System.Windows.Forms.TabPage tabSitemap;
        private System.Windows.Forms.TabPage tabFormato;
        private System.Windows.Forms.Panel asinBatcherPanel;
        private System.Windows.Forms.Panel sitemapPanel;
        private System.Windows.Forms.Panel formatoPanel;
        private AsinBatcherControl asinBatcherControl;
        private SitemapControl sitemapControl;
        private FormatoControl formatoControl;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Panel asinNoReportPanel;
        private AsinNoReportControl asinNoReportControl;
    }
}

