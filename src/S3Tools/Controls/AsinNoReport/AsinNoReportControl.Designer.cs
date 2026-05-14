namespace S3Tools
{
    partial class AsinNoReportControl
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
            this.baseGroup = new System.Windows.Forms.GroupBox();
            this.baseLayout = new System.Windows.Forms.TableLayoutPanel();
            this.baseLabel = new System.Windows.Forms.Label();
            this._baseFileText = new System.Windows.Forms.TextBox();
            this._browseBaseButton = new System.Windows.Forms.Button();
            this.sheetPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.sheetLabel = new System.Windows.Forms.Label();
            this._sheetCombo = new System.Windows.Forms.ComboBox();
            this._reloadSheetsButton = new System.Windows.Forms.Button();
            this.reportsGroup = new System.Windows.Forms.GroupBox();
            this.reportsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.reportsButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._importReportsButton = new System.Windows.Forms.Button();
            this._clearReportsButton = new System.Windows.Forms.Button();
            this._reportsList = new System.Windows.Forms.ListBox();
            this._reportsSummaryLabel = new System.Windows.Forms.Label();
            this.actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._analyzeButton = new System.Windows.Forms.Button();
            this._copyButton = new System.Windows.Forms.Button();
            this._exportButton = new System.Windows.Forms.Button();
            this._helpButton = new System.Windows.Forms.Button();
            this.resultsGroup = new System.Windows.Forms.GroupBox();
            this.resultsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._summaryText = new System.Windows.Forms.TextBox();
            this._resultText = new System.Windows.Forms.TextBox();
            this.rootLayout.SuspendLayout();
            this.baseGroup.SuspendLayout();
            this.baseLayout.SuspendLayout();
            this.sheetPanel.SuspendLayout();
            this.reportsGroup.SuspendLayout();
            this.reportsLayout.SuspendLayout();
            this.reportsButtonsPanel.SuspendLayout();
            this.actionsPanel.SuspendLayout();
            this.resultsGroup.SuspendLayout();
            this.resultsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.baseGroup, 0, 0);
            this.rootLayout.Controls.Add(this.reportsGroup, 0, 1);
            this.rootLayout.Controls.Add(this.actionsPanel, 0, 2);
            this.rootLayout.Controls.Add(this.resultsGroup, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.rootLayout.Size = new System.Drawing.Size(920, 760);
            this.rootLayout.TabIndex = 0;
            // 
            // baseGroup
            // 
            this.baseGroup.Controls.Add(this.baseLayout);
            this.baseGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baseGroup.Location = new System.Drawing.Point(13, 13);
            this.baseGroup.Name = "baseGroup";
            this.baseGroup.Size = new System.Drawing.Size(894, 96);
            this.baseGroup.TabIndex = 0;
            this.baseGroup.TabStop = false;
            this.baseGroup.Text = "Archivo base (.csv / .xlsx)";
            // 
            // baseLayout
            // 
            this.baseLayout.ColumnCount = 2;
            this.baseLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.baseLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.baseLayout.Controls.Add(this.baseLabel, 0, 0);
            this.baseLayout.Controls.Add(this._baseFileText, 0, 1);
            this.baseLayout.Controls.Add(this._browseBaseButton, 1, 1);
            this.baseLayout.Controls.Add(this.sheetPanel, 0, 2);
            this.baseLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baseLayout.Location = new System.Drawing.Point(3, 18);
            this.baseLayout.Name = "baseLayout";
            this.baseLayout.RowCount = 3;
            this.baseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.baseLayout.Size = new System.Drawing.Size(888, 75);
            this.baseLayout.TabIndex = 0;
            // 
            // baseLabel
            // 
            this.baseLabel.AutoSize = true;
            this.baseLayout.SetColumnSpan(this.baseLabel, 2);
            this.baseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.baseLabel.Location = new System.Drawing.Point(3, 0);
            this.baseLabel.Name = "baseLabel";
            this.baseLabel.Size = new System.Drawing.Size(234, 17);
            this.baseLabel.TabIndex = 0;
            this.baseLabel.Text = "Importa archivo con ASINs base";
            // 
            // _baseFileText
            // 
            this._baseFileText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._baseFileText.Location = new System.Drawing.Point(3, 20);
            this._baseFileText.Name = "_baseFileText";
            this._baseFileText.Size = new System.Drawing.Size(751, 22);
            this._baseFileText.TabIndex = 1;
            // 
            // _browseBaseButton
            // 
            this._browseBaseButton.AutoSize = true;
            this._browseBaseButton.Location = new System.Drawing.Point(760, 20);
            this._browseBaseButton.Name = "_browseBaseButton";
            this._browseBaseButton.Size = new System.Drawing.Size(125, 27);
            this._browseBaseButton.TabIndex = 2;
            this._browseBaseButton.Text = "Examinar base...";
            this._browseBaseButton.UseVisualStyleBackColor = true;
            // 
            // sheetPanel
            // 
            this.sheetPanel.AutoSize = false;
            this.baseLayout.SetColumnSpan(this.sheetPanel, 2);
            this.sheetPanel.Controls.Add(this.sheetLabel);
            this.sheetPanel.Controls.Add(this._sheetCombo);
            this.sheetPanel.Controls.Add(this._reloadSheetsButton);
            this.sheetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sheetPanel.Location = new System.Drawing.Point(3, 53);
            this.sheetPanel.Name = "sheetPanel";
            this.sheetPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.sheetPanel.Size = new System.Drawing.Size(882, 30);
            this.sheetPanel.TabIndex = 3;
            this.sheetPanel.WrapContents = false;
            // 
            // sheetLabel
            // 
            this.sheetLabel.AutoSize = true;
            this.sheetLabel.Location = new System.Drawing.Point(3, 3);
            this.sheetLabel.Margin = new System.Windows.Forms.Padding(3, 3, 6, 0);
            this.sheetLabel.Name = "sheetLabel";
            this.sheetLabel.Size = new System.Drawing.Size(131, 17);
            this.sheetLabel.TabIndex = 0;
            this.sheetLabel.Text = "Hoja a analizar (.xlsx):";
            // 
            // _sheetCombo
            // 
            this._sheetCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._sheetCombo.FormattingEnabled = true;
            this._sheetCombo.Location = new System.Drawing.Point(143, 6);
            this._sheetCombo.Name = "_sheetCombo";
            this._sheetCombo.Size = new System.Drawing.Size(340, 24);
            this._sheetCombo.TabIndex = 1;
            // 
            // _reloadSheetsButton
            // 
            this._reloadSheetsButton.AutoSize = true;
            this._reloadSheetsButton.Location = new System.Drawing.Point(489, 3);
            this._reloadSheetsButton.Name = "_reloadSheetsButton";
            this._reloadSheetsButton.Size = new System.Drawing.Size(121, 27);
            this._reloadSheetsButton.TabIndex = 2;
            this._reloadSheetsButton.Text = "Actualizar hojas";
            this._reloadSheetsButton.UseVisualStyleBackColor = true;
            // 
            // reportsGroup
            // 
            this.reportsGroup.Controls.Add(this.reportsLayout);
            this.reportsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsGroup.Location = new System.Drawing.Point(13, 115);
            this.reportsGroup.Name = "reportsGroup";
            this.reportsGroup.Size = new System.Drawing.Size(894, 218);
            this.reportsGroup.TabIndex = 1;
            this.reportsGroup.TabStop = false;
            this.reportsGroup.Text = "Reportes Amazon (.txt)";
            // 
            // reportsLayout
            // 
            this.reportsLayout.ColumnCount = 1;
            this.reportsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.reportsLayout.Controls.Add(this.reportsButtonsPanel, 0, 0);
            this.reportsLayout.Controls.Add(this._reportsList, 0, 1);
            this.reportsLayout.Controls.Add(this._reportsSummaryLabel, 0, 2);
            this.reportsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsLayout.Location = new System.Drawing.Point(3, 18);
            this.reportsLayout.Name = "reportsLayout";
            this.reportsLayout.RowCount = 3;
            this.reportsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.reportsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.reportsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.reportsLayout.Size = new System.Drawing.Size(888, 197);
            this.reportsLayout.TabIndex = 0;
            // 
            // reportsButtonsPanel
            // 
            this.reportsButtonsPanel.AutoSize = true;
            this.reportsButtonsPanel.Controls.Add(this._importReportsButton);
            this.reportsButtonsPanel.Controls.Add(this._clearReportsButton);
            this.reportsButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsButtonsPanel.Location = new System.Drawing.Point(3, 3);
            this.reportsButtonsPanel.Name = "reportsButtonsPanel";
            this.reportsButtonsPanel.Size = new System.Drawing.Size(882, 33);
            this.reportsButtonsPanel.TabIndex = 0;
            // 
            // _importReportsButton
            // 
            this._importReportsButton.AutoSize = true;
            this._importReportsButton.Location = new System.Drawing.Point(3, 3);
            this._importReportsButton.Name = "_importReportsButton";
            this._importReportsButton.Size = new System.Drawing.Size(146, 27);
            this._importReportsButton.TabIndex = 0;
            this._importReportsButton.Text = "Importar reportes...";
            this._importReportsButton.UseVisualStyleBackColor = true;
            // 
            // _clearReportsButton
            // 
            this._clearReportsButton.AutoSize = true;
            this._clearReportsButton.Location = new System.Drawing.Point(155, 3);
            this._clearReportsButton.Name = "_clearReportsButton";
            this._clearReportsButton.Size = new System.Drawing.Size(93, 27);
            this._clearReportsButton.TabIndex = 1;
            this._clearReportsButton.Text = "Limpiar lista";
            this._clearReportsButton.UseVisualStyleBackColor = true;
            // 
            // _reportsList
            // 
            this._reportsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._reportsList.FormattingEnabled = true;
            this._reportsList.ItemHeight = 16;
            this._reportsList.Location = new System.Drawing.Point(3, 42);
            this._reportsList.Name = "_reportsList";
            this._reportsList.Size = new System.Drawing.Size(882, 136);
            this._reportsList.TabIndex = 1;
            // 
            // _reportsSummaryLabel
            // 
            this._reportsSummaryLabel.AutoSize = true;
            this._reportsSummaryLabel.Location = new System.Drawing.Point(3, 181);
            this._reportsSummaryLabel.Name = "_reportsSummaryLabel";
            this._reportsSummaryLabel.Size = new System.Drawing.Size(84, 16);
            this._reportsSummaryLabel.TabIndex = 2;
            this._reportsSummaryLabel.Text = "Reportes: 0";
            // 
            // actionsPanel
            // 
            this.actionsPanel.AutoSize = true;
            this.actionsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.actionsPanel.Controls.Add(this._analyzeButton);
            this.actionsPanel.Controls.Add(this._copyButton);
            this.actionsPanel.Controls.Add(this._exportButton);
            this.actionsPanel.Controls.Add(this._helpButton);
            this.actionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsPanel.WrapContents = false;
            this.actionsPanel.Location = new System.Drawing.Point(13, 339);
            this.actionsPanel.Name = "actionsPanel";
            this.actionsPanel.Size = new System.Drawing.Size(894, 50);
            this.actionsPanel.TabIndex = 2;
            // 
            // _analyzeButton
            // 
            this._analyzeButton.AutoSize = true;
            this._analyzeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this._analyzeButton.Location = new System.Drawing.Point(3, 3);
            this._analyzeButton.Name = "_analyzeButton";
            this._analyzeButton.Padding = new System.Windows.Forms.Padding(16, 6, 16, 6);
            this._analyzeButton.Size = new System.Drawing.Size(121, 44);
            this._analyzeButton.TabIndex = 0;
            this._analyzeButton.Text = "Analizar";
            this._analyzeButton.UseVisualStyleBackColor = true;
            // 
            // _copyButton
            // 
            this._copyButton.AutoSize = true;
            this._copyButton.Enabled = false;
            this._copyButton.Location = new System.Drawing.Point(130, 3);
            this._copyButton.Name = "_copyButton";
            this._copyButton.Size = new System.Drawing.Size(128, 27);
            this._copyButton.TabIndex = 1;
            this._copyButton.Text = "Copiar resultado";
            this._copyButton.UseVisualStyleBackColor = true;
            // 
            // _exportButton
            // 
            this._exportButton.AutoSize = true;
            this._exportButton.Enabled = false;
            this._exportButton.Location = new System.Drawing.Point(264, 3);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(98, 27);
            this._exportButton.TabIndex = 2;
            this._exportButton.Text = "Exportar .txt";
            this._exportButton.UseVisualStyleBackColor = true;
            // 
            // _helpButton
            // 
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(368, 3);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 27);
            this._helpButton.TabIndex = 3;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            // 
            // resultsGroup
            // 
            this.resultsGroup.Controls.Add(this.resultsLayout);
            this.resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsGroup.Location = new System.Drawing.Point(13, 395);
            this.resultsGroup.Name = "resultsGroup";
            this.resultsGroup.Size = new System.Drawing.Size(894, 352);
            this.resultsGroup.TabIndex = 3;
            this.resultsGroup.TabStop = false;
            this.resultsGroup.Text = "Resultado";
            // 
            // resultsLayout
            // 
            this.resultsLayout.ColumnCount = 1;
            this.resultsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.resultsLayout.Controls.Add(this._summaryText, 0, 0);
            this.resultsLayout.Controls.Add(this._resultText, 0, 1);
            this.resultsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsLayout.Location = new System.Drawing.Point(3, 18);
            this.resultsLayout.Name = "resultsLayout";
            this.resultsLayout.RowCount = 2;
            this.resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.resultsLayout.Size = new System.Drawing.Size(888, 331);
            this.resultsLayout.TabIndex = 0;
            // 
            // _summaryText
            // 
            this._summaryText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._summaryText.Location = new System.Drawing.Point(3, 3);
            this._summaryText.Multiline = true;
            this._summaryText.Name = "_summaryText";
            this._summaryText.ReadOnly = true;
            this._summaryText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._summaryText.Size = new System.Drawing.Size(882, 62);
            this._summaryText.TabIndex = 0;
            // 
            // _resultText
            // 
            this._resultText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultText.Location = new System.Drawing.Point(3, 71);
            this._resultText.Multiline = true;
            this._resultText.Name = "_resultText";
            this._resultText.ReadOnly = true;
            this._resultText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._resultText.Size = new System.Drawing.Size(882, 257);
            this._resultText.TabIndex = 1;
            // 
            // AsinNoReportControl
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "AsinNoReportControl";
            this.Size = new System.Drawing.Size(920, 760);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.baseGroup.ResumeLayout(false);
            this.baseLayout.ResumeLayout(false);
            this.baseLayout.PerformLayout();
            this.sheetPanel.ResumeLayout(false);
            this.sheetPanel.PerformLayout();
            this.reportsGroup.ResumeLayout(false);
            this.reportsLayout.ResumeLayout(false);
            this.reportsLayout.PerformLayout();
            this.reportsButtonsPanel.ResumeLayout(false);
            this.reportsButtonsPanel.PerformLayout();
            this.actionsPanel.ResumeLayout(false);
            this.actionsPanel.PerformLayout();
            this.resultsGroup.ResumeLayout(false);
            this.resultsLayout.ResumeLayout(false);
            this.resultsLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.GroupBox baseGroup;
        private System.Windows.Forms.TableLayoutPanel baseLayout;
        private System.Windows.Forms.Label baseLabel;
        private System.Windows.Forms.TextBox _baseFileText;
        private System.Windows.Forms.Button _browseBaseButton;
        private System.Windows.Forms.FlowLayoutPanel sheetPanel;
        private System.Windows.Forms.Label sheetLabel;
        private System.Windows.Forms.ComboBox _sheetCombo;
        private System.Windows.Forms.Button _reloadSheetsButton;
        private System.Windows.Forms.GroupBox reportsGroup;
        private System.Windows.Forms.TableLayoutPanel reportsLayout;
        private System.Windows.Forms.FlowLayoutPanel reportsButtonsPanel;
        private System.Windows.Forms.Button _importReportsButton;
        private System.Windows.Forms.Button _clearReportsButton;
        private System.Windows.Forms.ListBox _reportsList;
        private System.Windows.Forms.Label _reportsSummaryLabel;
        private System.Windows.Forms.FlowLayoutPanel actionsPanel;
        private System.Windows.Forms.Button _analyzeButton;
        private System.Windows.Forms.Button _copyButton;
        private System.Windows.Forms.Button _exportButton;
        private System.Windows.Forms.Button _helpButton;
        private System.Windows.Forms.GroupBox resultsGroup;
        private System.Windows.Forms.TableLayoutPanel resultsLayout;
        private System.Windows.Forms.TextBox _summaryText;
        private System.Windows.Forms.TextBox _resultText;
    }
}
