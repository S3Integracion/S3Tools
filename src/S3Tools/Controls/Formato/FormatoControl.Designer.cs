namespace S3Tools
{
    partial class FormatoControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.inputLayout = new System.Windows.Forms.TableLayoutPanel();
            this.inputLabel = new System.Windows.Forms.Label();
            this.inputButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._importFilesButton = new System.Windows.Forms.Button();
            this._clearFilesButton = new System.Windows.Forms.Button();
            this.filesGroup = new System.Windows.Forms.GroupBox();
            this.filesLayout = new System.Windows.Forms.TableLayoutPanel();
            this.modePanel = new System.Windows.Forms.FlowLayoutPanel();
            this._modeAllRadio = new System.Windows.Forms.RadioButton();
            this._modeSelectRadio = new System.Windows.Forms.RadioButton();
            this.headerFormatGroup = new System.Windows.Forms.GroupBox();
            this.headerFormatPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._headerFormatHyphenRadio = new System.Windows.Forms.RadioButton();
            this._headerFormatUnderscoreRadio = new System.Windows.Forms.RadioButton();
            this._filesList = new System.Windows.Forms.ListBox();
            this._summaryLabel = new System.Windows.Forms.Label();
            this.templateGroup = new System.Windows.Forms.GroupBox();
            this.templatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this._templateAutoRadio = new System.Windows.Forms.RadioButton();
            this._templateTiendasRadio = new System.Windows.Forms.RadioButton();
            this._templateBbvsRadio = new System.Windows.Forms.RadioButton();
            this.processPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._processButton = new System.Windows.Forms.Button();
            this._noteLabel = new System.Windows.Forms.Label();
            this.helpPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._helpButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.inputLayout.SuspendLayout();
            this.inputButtonsPanel.SuspendLayout();
            this.filesGroup.SuspendLayout();
            this.filesLayout.SuspendLayout();
            this.modePanel.SuspendLayout();
            this.headerFormatGroup.SuspendLayout();
            this.headerFormatPanel.SuspendLayout();
            this.templateGroup.SuspendLayout();
            this.templatePanel.SuspendLayout();
            this.processPanel.SuspendLayout();
            this.helpPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.inputLayout, 0, 0);
            this.rootLayout.Controls.Add(this.filesGroup, 0, 1);
            this.rootLayout.Controls.Add(this.templateGroup, 0, 2);
            this.rootLayout.Controls.Add(this.processPanel, 0, 3);
            this.rootLayout.Controls.Add(this.helpPanel, 0, 4);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.Size = new System.Drawing.Size(1013, 654);
            this.rootLayout.TabIndex = 0;
            // 
            // inputLayout
            // 
            this.inputLayout.AutoSize = true;
            this.inputLayout.ColumnCount = 1;
            this.inputLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inputLayout.Controls.Add(this.inputLabel, 0, 0);
            this.inputLayout.Controls.Add(this.inputButtonsPanel, 0, 1);
            this.inputLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputLayout.Location = new System.Drawing.Point(13, 13);
            this.inputLayout.Name = "inputLayout";
            this.inputLayout.RowCount = 2;
            this.inputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputLayout.Size = new System.Drawing.Size(987, 56);
            this.inputLayout.TabIndex = 0;
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.inputLabel.Location = new System.Drawing.Point(3, 0);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(251, 17);
            this.inputLabel.TabIndex = 0;
            this.inputLabel.Text = "Archivos de entrada (.csv / .xlsx):";
            // 
            // inputButtonsPanel
            // 
            this.inputButtonsPanel.AutoSize = true;
            this.inputButtonsPanel.Controls.Add(this._importFilesButton);
            this.inputButtonsPanel.Controls.Add(this._clearFilesButton);
            this.inputButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputButtonsPanel.Location = new System.Drawing.Point(3, 20);
            this.inputButtonsPanel.Name = "inputButtonsPanel";
            this.inputButtonsPanel.Size = new System.Drawing.Size(981, 33);
            this.inputButtonsPanel.TabIndex = 1;
            // 
            // _importFilesButton
            // 
            this._importFilesButton.AutoSize = true;
            this._importFilesButton.Location = new System.Drawing.Point(3, 3);
            this._importFilesButton.Name = "_importFilesButton";
            this._importFilesButton.Size = new System.Drawing.Size(139, 27);
            this._importFilesButton.TabIndex = 0;
            this._importFilesButton.Text = "Importar archivos...";
            this._importFilesButton.UseVisualStyleBackColor = true;
            // 
            // _clearFilesButton
            // 
            this._clearFilesButton.AutoSize = true;
            this._clearFilesButton.Location = new System.Drawing.Point(148, 3);
            this._clearFilesButton.Name = "_clearFilesButton";
            this._clearFilesButton.Size = new System.Drawing.Size(93, 27);
            this._clearFilesButton.TabIndex = 1;
            this._clearFilesButton.Text = "Limpiar lista";
            this._clearFilesButton.UseVisualStyleBackColor = true;
            // 
            // filesGroup
            // 
            this.filesGroup.Controls.Add(this.filesLayout);
            this.filesGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesGroup.Location = new System.Drawing.Point(13, 75);
            this.filesGroup.Name = "filesGroup";
            this.filesGroup.Size = new System.Drawing.Size(987, 417);
            this.filesGroup.TabIndex = 1;
            this.filesGroup.TabStop = false;
            this.filesGroup.Text = "Archivos";
            // 
            // filesLayout
            // 
            this.filesLayout.ColumnCount = 2;
            this.filesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.filesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filesLayout.Controls.Add(this.modePanel, 0, 0);
            this.filesLayout.Controls.Add(this.headerFormatGroup, 0, 1);
            this.filesLayout.Controls.Add(this._filesList, 0, 2);
            this.filesLayout.Controls.Add(this._summaryLabel, 0, 3);
            this.filesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesLayout.Location = new System.Drawing.Point(3, 18);
            this.filesLayout.Name = "filesLayout";
            this.filesLayout.RowCount = 4;
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.Size = new System.Drawing.Size(981, 396);
            this.filesLayout.TabIndex = 0;
            // 
            // modePanel
            // 
            this.modePanel.AutoSize = true;
            this.filesLayout.SetColumnSpan(this.modePanel, 2);
            this.modePanel.Controls.Add(this._modeAllRadio);
            this.modePanel.Controls.Add(this._modeSelectRadio);
            this.modePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modePanel.Location = new System.Drawing.Point(3, 3);
            this.modePanel.Name = "modePanel";
            this.modePanel.Size = new System.Drawing.Size(975, 27);
            this.modePanel.TabIndex = 0;
            // 
            // _modeAllRadio
            // 
            this._modeAllRadio.AutoSize = true;
            this._modeAllRadio.Location = new System.Drawing.Point(3, 3);
            this._modeAllRadio.Name = "_modeAllRadio";
            this._modeAllRadio.Size = new System.Drawing.Size(125, 21);
            this._modeAllRadio.TabIndex = 0;
            this._modeAllRadio.TabStop = true;
            this._modeAllRadio.Text = "Procesar todos";
            this._modeAllRadio.UseVisualStyleBackColor = true;
            // 
            // _modeSelectRadio
            // 
            this._modeSelectRadio.AutoSize = true;
            this._modeSelectRadio.Location = new System.Drawing.Point(134, 3);
            this._modeSelectRadio.Name = "_modeSelectRadio";
            this._modeSelectRadio.Size = new System.Drawing.Size(160, 21);
            this._modeSelectRadio.TabIndex = 1;
            this._modeSelectRadio.TabStop = true;
            this._modeSelectRadio.Text = "Seleccionar archivos";
            this._modeSelectRadio.UseVisualStyleBackColor = true;
            // 
            // headerFormatGroup
            // 
            this.headerFormatGroup.AutoSize = true;
            this.filesLayout.SetColumnSpan(this.headerFormatGroup, 2);
            this.headerFormatGroup.Controls.Add(this.headerFormatPanel);
            this.headerFormatGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerFormatGroup.Location = new System.Drawing.Point(3, 36);
            this.headerFormatGroup.Name = "headerFormatGroup";
            this.headerFormatGroup.Size = new System.Drawing.Size(975, 48);
            this.headerFormatGroup.TabIndex = 1;
            this.headerFormatGroup.TabStop = false;
            this.headerFormatGroup.Text = "Formato de Headers";
            // 
            // headerFormatPanel
            // 
            this.headerFormatPanel.AutoSize = true;
            this.headerFormatPanel.Controls.Add(this._headerFormatHyphenRadio);
            this.headerFormatPanel.Controls.Add(this._headerFormatUnderscoreRadio);
            this.headerFormatPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerFormatPanel.Location = new System.Drawing.Point(3, 18);
            this.headerFormatPanel.Name = "headerFormatPanel";
            this.headerFormatPanel.Size = new System.Drawing.Size(969, 27);
            this.headerFormatPanel.TabIndex = 0;
            // 
            // _headerFormatHyphenRadio
            // 
            this._headerFormatHyphenRadio.AutoSize = true;
            this._headerFormatHyphenRadio.Location = new System.Drawing.Point(3, 3);
            this._headerFormatHyphenRadio.Name = "_headerFormatHyphenRadio";
            this._headerFormatHyphenRadio.Size = new System.Drawing.Size(76, 21);
            this._headerFormatHyphenRadio.TabIndex = 0;
            this._headerFormatHyphenRadio.TabStop = true;
            this._headerFormatHyphenRadio.Text = "Medio -";
            this._headerFormatHyphenRadio.UseVisualStyleBackColor = true;
            // 
            // _headerFormatUnderscoreRadio
            // 
            this._headerFormatUnderscoreRadio.AutoSize = true;
            this._headerFormatUnderscoreRadio.Location = new System.Drawing.Point(85, 3);
            this._headerFormatUnderscoreRadio.Name = "_headerFormatUnderscoreRadio";
            this._headerFormatUnderscoreRadio.Size = new System.Drawing.Size(69, 21);
            this._headerFormatUnderscoreRadio.TabIndex = 1;
            this._headerFormatUnderscoreRadio.TabStop = true;
            this._headerFormatUnderscoreRadio.Text = "Bajo _";
            this._headerFormatUnderscoreRadio.UseVisualStyleBackColor = true;
            // 
            // _filesList
            // 
            this.filesLayout.SetColumnSpan(this._filesList, 2);
            this._filesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filesList.FormattingEnabled = true;
            this._filesList.ItemHeight = 16;
            this._filesList.Location = new System.Drawing.Point(3, 90);
            this._filesList.Name = "_filesList";
            this._filesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._filesList.Size = new System.Drawing.Size(975, 286);
            this._filesList.TabIndex = 2;
            // 
            // _summaryLabel
            // 
            this._summaryLabel.AutoSize = true;
            this.filesLayout.SetColumnSpan(this._summaryLabel, 2);
            this._summaryLabel.Location = new System.Drawing.Point(3, 379);
            this._summaryLabel.Name = "_summaryLabel";
            this._summaryLabel.Size = new System.Drawing.Size(78, 17);
            this._summaryLabel.TabIndex = 3;
            this._summaryLabel.Text = "Archivos: 0";
            // 
            // templateGroup
            // 
            this.templateGroup.AutoSize = true;
            this.templateGroup.Controls.Add(this.templatePanel);
            this.templateGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateGroup.Location = new System.Drawing.Point(13, 498);
            this.templateGroup.Name = "templateGroup";
            this.templateGroup.Size = new System.Drawing.Size(987, 48);
            this.templateGroup.TabIndex = 2;
            this.templateGroup.TabStop = false;
            this.templateGroup.Text = "Plantilla";
            // 
            // templatePanel
            // 
            this.templatePanel.AutoSize = true;
            this.templatePanel.Controls.Add(this._templateAutoRadio);
            this.templatePanel.Controls.Add(this._templateTiendasRadio);
            this.templatePanel.Controls.Add(this._templateBbvsRadio);
            this.templatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templatePanel.Location = new System.Drawing.Point(3, 18);
            this.templatePanel.Name = "templatePanel";
            this.templatePanel.Size = new System.Drawing.Size(981, 27);
            this.templatePanel.TabIndex = 0;
            // 
            // _templateAutoRadio
            // 
            this._templateAutoRadio.AutoSize = true;
            this._templateAutoRadio.Location = new System.Drawing.Point(3, 3);
            this._templateAutoRadio.Name = "_templateAutoRadio";
            this._templateAutoRadio.Size = new System.Drawing.Size(58, 21);
            this._templateAutoRadio.TabIndex = 0;
            this._templateAutoRadio.TabStop = true;
            this._templateAutoRadio.Text = "Auto";
            this._templateAutoRadio.UseVisualStyleBackColor = true;
            // 
            // _templateTiendasRadio
            // 
            this._templateTiendasRadio.AutoSize = true;
            this._templateTiendasRadio.Location = new System.Drawing.Point(67, 3);
            this._templateTiendasRadio.Name = "_templateTiendasRadio";
            this._templateTiendasRadio.Size = new System.Drawing.Size(80, 21);
            this._templateTiendasRadio.TabIndex = 1;
            this._templateTiendasRadio.TabStop = true;
            this._templateTiendasRadio.Text = "Tiendas";
            this._templateTiendasRadio.UseVisualStyleBackColor = true;
            // 
            // _templateBbvsRadio
            // 
            this._templateBbvsRadio.AutoSize = true;
            this._templateBbvsRadio.Location = new System.Drawing.Point(153, 3);
            this._templateBbvsRadio.Name = "_templateBbvsRadio";
            this._templateBbvsRadio.Size = new System.Drawing.Size(61, 21);
            this._templateBbvsRadio.TabIndex = 2;
            this._templateBbvsRadio.TabStop = true;
            this._templateBbvsRadio.Text = "BBvs";
            this._templateBbvsRadio.UseVisualStyleBackColor = true;
            // 
            // processPanel
            // 
            this.processPanel.AutoSize = true;
            this.processPanel.Controls.Add(this._processButton);
            this.processPanel.Controls.Add(this._noteLabel);
            this.processPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processPanel.Location = new System.Drawing.Point(13, 552);
            this.processPanel.Name = "processPanel";
            this.processPanel.Size = new System.Drawing.Size(987, 50);
            this.processPanel.TabIndex = 3;
            // 
            // _processButton
            // 
            this._processButton.AutoSize = true;
            this._processButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this._processButton.Location = new System.Drawing.Point(3, 3);
            this._processButton.Name = "_processButton";
            this._processButton.Padding = new System.Windows.Forms.Padding(16, 6, 16, 6);
            this._processButton.Size = new System.Drawing.Size(127, 44);
            this._processButton.TabIndex = 0;
            this._processButton.Text = "Procesar";
            this._processButton.UseVisualStyleBackColor = true;
            // 
            // _noteLabel
            // 
            this._noteLabel.AutoSize = true;
            this._noteLabel.Location = new System.Drawing.Point(136, 8);
            this._noteLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this._noteLabel.Name = "_noteLabel";
            this._noteLabel.Size = new System.Drawing.Size(618, 17);
            this._noteLabel.TabIndex = 1;
            this._noteLabel.Text = "Solo se corrigen las dos primeras columnas (Medio - o Bajo _); se actualiza en la" +
    " misma carpeta.";
            // 
            // helpPanel
            // 
            this.helpPanel.AutoSize = true;
            this.helpPanel.Controls.Add(this._helpButton);
            this.helpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.helpPanel.Location = new System.Drawing.Point(13, 608);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Size = new System.Drawing.Size(987, 33);
            this.helpPanel.TabIndex = 4;
            // 
            // _helpButton
            // 
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(916, 3);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 27);
            this._helpButton.TabIndex = 0;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            // 
            // FormatoControl
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "FormatoControl";
            this.Size = new System.Drawing.Size(1013, 654);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.inputLayout.ResumeLayout(false);
            this.inputLayout.PerformLayout();
            this.inputButtonsPanel.ResumeLayout(false);
            this.inputButtonsPanel.PerformLayout();
            this.filesGroup.ResumeLayout(false);
            this.filesLayout.ResumeLayout(false);
            this.filesLayout.PerformLayout();
            this.modePanel.ResumeLayout(false);
            this.modePanel.PerformLayout();
            this.headerFormatGroup.ResumeLayout(false);
            this.headerFormatGroup.PerformLayout();
            this.headerFormatPanel.ResumeLayout(false);
            this.headerFormatPanel.PerformLayout();
            this.templateGroup.ResumeLayout(false);
            this.templateGroup.PerformLayout();
            this.templatePanel.ResumeLayout(false);
            this.templatePanel.PerformLayout();
            this.processPanel.ResumeLayout(false);
            this.processPanel.PerformLayout();
            this.helpPanel.ResumeLayout(false);
            this.helpPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel inputLayout;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.FlowLayoutPanel inputButtonsPanel;
        private System.Windows.Forms.GroupBox filesGroup;
        private System.Windows.Forms.TableLayoutPanel filesLayout;
        private System.Windows.Forms.FlowLayoutPanel modePanel;
        private System.Windows.Forms.GroupBox headerFormatGroup;
        private System.Windows.Forms.FlowLayoutPanel headerFormatPanel;
        private System.Windows.Forms.GroupBox templateGroup;
        private System.Windows.Forms.FlowLayoutPanel templatePanel;
        private System.Windows.Forms.FlowLayoutPanel processPanel;
        private System.Windows.Forms.FlowLayoutPanel helpPanel;
        private System.Windows.Forms.Button _importFilesButton;
        private System.Windows.Forms.Button _clearFilesButton;
        private System.Windows.Forms.RadioButton _modeAllRadio;
        private System.Windows.Forms.RadioButton _modeSelectRadio;
        private System.Windows.Forms.ListBox _filesList;
        private System.Windows.Forms.Label _summaryLabel;
        private System.Windows.Forms.RadioButton _headerFormatHyphenRadio;
        private System.Windows.Forms.RadioButton _headerFormatUnderscoreRadio;
        private System.Windows.Forms.RadioButton _templateAutoRadio;
        private System.Windows.Forms.RadioButton _templateTiendasRadio;
        private System.Windows.Forms.RadioButton _templateBbvsRadio;
        private System.Windows.Forms.Button _processButton;
        private System.Windows.Forms.Button _helpButton;
        private System.Windows.Forms.Label _noteLabel;
    }
}

