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
            this.tabControlPrograms = new System.Windows.Forms.TabControl();
            this.tabAsinBatcher = new System.Windows.Forms.TabPage();
            this.asinBatcherPanel = new System.Windows.Forms.Panel();
            this.asinBatcherControl = new S3Integración_programs.AsinBatcherControl();
            this.tabSitemap = new System.Windows.Forms.TabPage();
            this.sitemapPanel = new System.Windows.Forms.Panel();
            this.sitemapControl = new S3Integración_programs.SitemapControl();
            this.tabFormato = new System.Windows.Forms.TabPage();
            this.formatoPanel = new System.Windows.Forms.Panel();
            this.formatoControl = new S3Integración_programs.FormatoControl();
            this.tabControlRemoto = new System.Windows.Forms.TabPage();
            this.controlRemotoPanel = new System.Windows.Forms.Panel();
            this.controlRemotoControl = new S3Integración_programs.ControlRemotoControl();
            this.tabControlPrograms.SuspendLayout();
            this.tabAsinBatcher.SuspendLayout();
            this.asinBatcherPanel.SuspendLayout();
            this.tabSitemap.SuspendLayout();
            this.sitemapPanel.SuspendLayout();
            this.tabFormato.SuspendLayout();
            this.formatoPanel.SuspendLayout();
            this.tabControlRemoto.SuspendLayout();
            this.controlRemotoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlPrograms
            // 
            this.tabControlPrograms.Controls.Add(this.tabAsinBatcher);
            this.tabControlPrograms.Controls.Add(this.tabSitemap);
            this.tabControlPrograms.Controls.Add(this.tabFormato);
            this.tabControlPrograms.Controls.Add(this.tabControlRemoto);
            this.tabControlPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPrograms.Location = new System.Drawing.Point(0, 0);
            this.tabControlPrograms.Name = "tabControlPrograms";
            this.tabControlPrograms.SelectedIndex = 0;
            this.tabControlPrograms.Size = new System.Drawing.Size(957, 837);
            this.tabControlPrograms.TabIndex = 1;
            // 
            // tabAsinBatcher
            // 
            this.tabAsinBatcher.Controls.Add(this.asinBatcherPanel);
            this.tabAsinBatcher.Location = new System.Drawing.Point(4, 25);
            this.tabAsinBatcher.Name = "tabAsinBatcher";
            this.tabAsinBatcher.Padding = new System.Windows.Forms.Padding(3);
            this.tabAsinBatcher.Size = new System.Drawing.Size(949, 808);
            this.tabAsinBatcher.TabIndex = 0;
            this.tabAsinBatcher.Text = "Asin Batcher";
            this.tabAsinBatcher.UseVisualStyleBackColor = true;
            // 
            // asinBatcherPanel
            // 
            this.asinBatcherPanel.Controls.Add(this.asinBatcherControl);
            this.asinBatcherPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.asinBatcherPanel.Location = new System.Drawing.Point(3, 3);
            this.asinBatcherPanel.Name = "asinBatcherPanel";
            this.asinBatcherPanel.Size = new System.Drawing.Size(943, 802);
            this.asinBatcherPanel.TabIndex = 0;
            // 
            // asinBatcherControl
            // 
            this.asinBatcherControl.AutoScroll = true;
            this.asinBatcherControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.asinBatcherControl.Location = new System.Drawing.Point(0, 0);
            this.asinBatcherControl.Name = "asinBatcherControl";
            this.asinBatcherControl.Size = new System.Drawing.Size(943, 802);
            this.asinBatcherControl.TabIndex = 0;
            this.asinBatcherControl.Load += new System.EventHandler(this.asinBatcherControl_Load);
            // 
            // tabSitemap
            // 
            this.tabSitemap.Controls.Add(this.sitemapPanel);
            this.tabSitemap.Location = new System.Drawing.Point(4, 25);
            this.tabSitemap.Name = "tabSitemap";
            this.tabSitemap.Padding = new System.Windows.Forms.Padding(3);
            this.tabSitemap.Size = new System.Drawing.Size(949, 808);
            this.tabSitemap.TabIndex = 1;
            this.tabSitemap.Text = "Sitemap";
            this.tabSitemap.UseVisualStyleBackColor = true;
            // 
            // sitemapPanel
            // 
            this.sitemapPanel.Controls.Add(this.sitemapControl);
            this.sitemapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sitemapPanel.Location = new System.Drawing.Point(3, 3);
            this.sitemapPanel.Name = "sitemapPanel";
            this.sitemapPanel.Size = new System.Drawing.Size(943, 802);
            this.sitemapPanel.TabIndex = 0;
            // 
            // sitemapControl
            // 
            this.sitemapControl.AutoScroll = true;
            this.sitemapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sitemapControl.Location = new System.Drawing.Point(0, 0);
            this.sitemapControl.Name = "sitemapControl";
            this.sitemapControl.Size = new System.Drawing.Size(943, 802);
            this.sitemapControl.TabIndex = 0;
            // 
            // tabFormato
            // 
            this.tabFormato.Controls.Add(this.formatoPanel);
            this.tabFormato.Location = new System.Drawing.Point(4, 25);
            this.tabFormato.Name = "tabFormato";
            this.tabFormato.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormato.Size = new System.Drawing.Size(949, 808);
            this.tabFormato.TabIndex = 2;
            this.tabFormato.Text = "Formato";
            this.tabFormato.UseVisualStyleBackColor = true;
            // 
            // formatoPanel
            // 
            this.formatoPanel.Controls.Add(this.formatoControl);
            this.formatoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatoPanel.Location = new System.Drawing.Point(3, 3);
            this.formatoPanel.Name = "formatoPanel";
            this.formatoPanel.Size = new System.Drawing.Size(943, 802);
            this.formatoPanel.TabIndex = 0;
            // 
            // formatoControl
            // 
            this.formatoControl.AutoScroll = true;
            this.formatoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatoControl.Location = new System.Drawing.Point(0, 0);
            this.formatoControl.Name = "formatoControl";
            this.formatoControl.Size = new System.Drawing.Size(943, 802);
            this.formatoControl.TabIndex = 0;
            // 
            // tabControlRemoto
            // 
            this.tabControlRemoto.Controls.Add(this.controlRemotoPanel);
            this.tabControlRemoto.Location = new System.Drawing.Point(4, 25);
            this.tabControlRemoto.Name = "tabControlRemoto";
            this.tabControlRemoto.Padding = new System.Windows.Forms.Padding(3);
            this.tabControlRemoto.Size = new System.Drawing.Size(949, 808);
            this.tabControlRemoto.TabIndex = 4;
            this.tabControlRemoto.Text = "Control_Remoto";
            this.tabControlRemoto.UseVisualStyleBackColor = true;
            // 
            // controlRemotoPanel
            // 
            this.controlRemotoPanel.Controls.Add(this.controlRemotoControl);
            this.controlRemotoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlRemotoPanel.Location = new System.Drawing.Point(3, 3);
            this.controlRemotoPanel.Name = "controlRemotoPanel";
            this.controlRemotoPanel.Size = new System.Drawing.Size(943, 802);
            this.controlRemotoPanel.TabIndex = 0;
            // 
            // controlRemotoControl
            // 
            this.controlRemotoControl.AutoScroll = true;
            this.controlRemotoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlRemotoControl.Location = new System.Drawing.Point(0, 0);
            this.controlRemotoControl.Name = "controlRemotoControl";
            this.controlRemotoControl.Size = new System.Drawing.Size(943, 802);
            this.controlRemotoControl.TabIndex = 0;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 837);
            this.Controls.Add(this.tabControlPrograms);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "S3Tools";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControlPrograms.ResumeLayout(false);
            this.tabAsinBatcher.ResumeLayout(false);
            this.asinBatcherPanel.ResumeLayout(false);
            this.tabSitemap.ResumeLayout(false);
            this.sitemapPanel.ResumeLayout(false);
            this.tabFormato.ResumeLayout(false);
            this.formatoPanel.ResumeLayout(false);
            this.tabControlRemoto.ResumeLayout(false);
            this.controlRemotoPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControlPrograms;
        private System.Windows.Forms.TabPage tabAsinBatcher;
        private System.Windows.Forms.TabPage tabSitemap;
        private System.Windows.Forms.TabPage tabFormato;
        private System.Windows.Forms.TabPage tabControlRemoto;
        private System.Windows.Forms.Panel asinBatcherPanel;
        private System.Windows.Forms.Panel sitemapPanel;
        private System.Windows.Forms.Panel formatoPanel;
        private System.Windows.Forms.Panel controlRemotoPanel;
        private AsinBatcherControl asinBatcherControl;
        private SitemapControl sitemapControl;
        private FormatoControl formatoControl;
        private ControlRemotoControl controlRemotoControl;
    }
}

