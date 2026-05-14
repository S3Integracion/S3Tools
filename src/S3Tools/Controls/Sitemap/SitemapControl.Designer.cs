namespace S3Tools
{
    partial class SitemapControl
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
            this.filesHeaderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.modePanel = new System.Windows.Forms.FlowLayoutPanel();
            this._modeAllRadio = new System.Windows.Forms.RadioButton();
            this._modeSelectRadio = new System.Windows.Forms.RadioButton();
            this._refreshButton = new System.Windows.Forms.Button();
            this._filesList = new System.Windows.Forms.ListBox();
            this._summaryLabel = new System.Windows.Forms.Label();
            this.filesInfoLayout = new System.Windows.Forms.TableLayoutPanel();
            this._urlsPerBatchLabel = new System.Windows.Forms.Label();
            this._timeRangeLabel = new System.Windows.Forms.Label();
            this.storeGroup = new System.Windows.Forms.GroupBox();
            this.storeOuterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.storeHeaderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._nameConfigButton = new System.Windows.Forms.Button();
            this.storeGrid = new System.Windows.Forms.TableLayoutPanel();
            this._storeProductosTxRadio = new System.Windows.Forms.RadioButton();
            this._storeBbvsTemplateRadio = new System.Windows.Forms.RadioButton();
            this._storeHolaproductoRadio = new System.Windows.Forms.RadioButton();
            this._storeBbvs2daRadio = new System.Windows.Forms.RadioButton();
            this._storeAltinorRadio = new System.Windows.Forms.RadioButton();
            this._storeBbvsRadio = new System.Windows.Forms.RadioButton();
            this._storeHervazTradeRadio = new System.Windows.Forms.RadioButton();
            this.templateGroup = new System.Windows.Forms.GroupBox();
            this.templatePanel = new System.Windows.Forms.FlowLayoutPanel();
            this._templateNormalRadio = new System.Windows.Forms.RadioButton();
            this._templateNubeRadio = new System.Windows.Forms.RadioButton();
            this.baseNameLayout = new System.Windows.Forms.TableLayoutPanel();
            this.baseNameLabel = new System.Windows.Forms.Label();
            this._baseNameText = new System.Windows.Forms.TextBox();
            this.outputGroup = new System.Windows.Forms.GroupBox();
            this.outputLayout = new System.Windows.Forms.TableLayoutPanel();
            this.outputRowLayout = new System.Windows.Forms.TableLayoutPanel();
            this._outputText = new System.Windows.Forms.TextBox();
            this.outputButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._downloadsButton = new System.Windows.Forms.Button();
            this._desktopButton = new System.Windows.Forms.Button();
            this._chooseOutputButton = new System.Windows.Forms.Button();
            this._zipCheck = new System.Windows.Forms.CheckBox();
            this.processPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._processButton = new System.Windows.Forms.Button();
            this.helpPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._helpButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.inputLayout.SuspendLayout();
            this.inputButtonsPanel.SuspendLayout();
            this.filesGroup.SuspendLayout();
            this.filesLayout.SuspendLayout();
            this.filesHeaderPanel.SuspendLayout();
            this.modePanel.SuspendLayout();
            this.filesInfoLayout.SuspendLayout();
            this.storeGroup.SuspendLayout();
            this.storeOuterLayout.SuspendLayout();
            this.storeHeaderPanel.SuspendLayout();
            this.storeGrid.SuspendLayout();
            this.templateGroup.SuspendLayout();
            this.templatePanel.SuspendLayout();
            this.baseNameLayout.SuspendLayout();
            this.outputGroup.SuspendLayout();
            this.outputLayout.SuspendLayout();
            this.outputRowLayout.SuspendLayout();
            this.outputButtonsPanel.SuspendLayout();
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
            this.rootLayout.Controls.Add(this.storeGroup, 0, 2);
            this.rootLayout.Controls.Add(this.templateGroup, 0, 3);
            this.rootLayout.Controls.Add(this.baseNameLayout, 0, 4);
            this.rootLayout.Controls.Add(this.outputGroup, 0, 5);
            this.rootLayout.Controls.Add(this.processPanel, 0, 6);
            this.rootLayout.Controls.Add(this.helpPanel, 0, 7);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 8;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.Size = new System.Drawing.Size(1020, 633);
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
            this.inputLayout.Size = new System.Drawing.Size(994, 52);
            this.inputLayout.TabIndex = 0;
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.inputLabel.Location = new System.Drawing.Point(3, 0);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(273, 13);
            this.inputLabel.TabIndex = 0;
            this.inputLabel.Text = "Archivos de entrada (.txt / .csv / .xlsx / .json):";
            this.inputLabel.Click += new System.EventHandler(this.inputLabel_Click);
            // 
            // inputButtonsPanel
            // 
            this.inputButtonsPanel.AutoSize = true;
            this.inputButtonsPanel.Controls.Add(this._importFilesButton);
            this.inputButtonsPanel.Controls.Add(this._clearFilesButton);
            this.inputButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputButtonsPanel.Location = new System.Drawing.Point(3, 16);
            this.inputButtonsPanel.Name = "inputButtonsPanel";
            this.inputButtonsPanel.Size = new System.Drawing.Size(988, 33);
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
            this.filesGroup.Location = new System.Drawing.Point(13, 71);
            this.filesGroup.Name = "filesGroup";
            this.filesGroup.Size = new System.Drawing.Size(994, 119);
            this.filesGroup.TabIndex = 1;
            this.filesGroup.TabStop = false;
            this.filesGroup.Text = "Lotes de links";
            // 
            // filesLayout
            // 
            this.filesLayout.ColumnCount = 2;
            this.filesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.filesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filesLayout.Controls.Add(this.filesHeaderPanel, 0, 0);
            this.filesLayout.Controls.Add(this._filesList, 0, 1);
            this.filesLayout.Controls.Add(this._summaryLabel, 0, 2);
            this.filesLayout.Controls.Add(this.filesInfoLayout, 0, 3);
            this.filesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesLayout.Location = new System.Drawing.Point(3, 16);
            this.filesLayout.Name = "filesLayout";
            this.filesLayout.RowCount = 4;
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesLayout.Size = new System.Drawing.Size(988, 100);
            this.filesLayout.TabIndex = 0;
            // 
            // filesHeaderPanel
            // 
            this.filesHeaderPanel.AutoSize = true;
            this.filesLayout.SetColumnSpan(this.filesHeaderPanel, 2);
            this.filesHeaderPanel.Controls.Add(this.modePanel);
            this.filesHeaderPanel.Controls.Add(this._refreshButton);
            this.filesHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesHeaderPanel.Location = new System.Drawing.Point(3, 3);
            this.filesHeaderPanel.Name = "filesHeaderPanel";
            this.filesHeaderPanel.Size = new System.Drawing.Size(982, 33);
            this.filesHeaderPanel.TabIndex = 0;
            // 
            // modePanel
            // 
            this.modePanel.AutoSize = true;
            this.modePanel.Controls.Add(this._modeAllRadio);
            this.modePanel.Controls.Add(this._modeSelectRadio);
            this.modePanel.Location = new System.Drawing.Point(3, 3);
            this.modePanel.Name = "modePanel";
            this.modePanel.Size = new System.Drawing.Size(214, 23);
            this.modePanel.TabIndex = 0;
            // 
            // _modeAllRadio
            // 
            this._modeAllRadio.AutoSize = true;
            this._modeAllRadio.Location = new System.Drawing.Point(3, 3);
            this._modeAllRadio.Name = "_modeAllRadio";
            this._modeAllRadio.Size = new System.Drawing.Size(96, 17);
            this._modeAllRadio.TabIndex = 0;
            this._modeAllRadio.TabStop = true;
            this._modeAllRadio.Text = "Convertir todos";
            this._modeAllRadio.UseVisualStyleBackColor = true;
            // 
            // _modeSelectRadio
            // 
            this._modeSelectRadio.AutoSize = true;
            this._modeSelectRadio.Location = new System.Drawing.Point(105, 3);
            this._modeSelectRadio.Name = "_modeSelectRadio";
            this._modeSelectRadio.Size = new System.Drawing.Size(106, 17);
            this._modeSelectRadio.TabIndex = 1;
            this._modeSelectRadio.TabStop = true;
            this._modeSelectRadio.Text = "Seleccionar lotes";
            this._modeSelectRadio.UseVisualStyleBackColor = true;
            // 
            // _refreshButton
            // 
            this._refreshButton.AutoSize = true;
            this._refreshButton.Location = new System.Drawing.Point(223, 3);
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(129, 27);
            this._refreshButton.TabIndex = 1;
            this._refreshButton.Text = "Cargar ultimo lote";
            this._refreshButton.UseVisualStyleBackColor = true;
            // 
            // _filesList
            // 
            this.filesLayout.SetColumnSpan(this._filesList, 2);
            this._filesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filesList.FormattingEnabled = true;
            this._filesList.Location = new System.Drawing.Point(3, 42);
            this._filesList.Name = "_filesList";
            this._filesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._filesList.Size = new System.Drawing.Size(982, 10);
            this._filesList.TabIndex = 1;
            // 
            // _summaryLabel
            // 
            this._summaryLabel.AutoSize = true;
            this.filesLayout.SetColumnSpan(this._summaryLabel, 2);
            this._summaryLabel.Location = new System.Drawing.Point(3, 55);
            this._summaryLabel.Name = "_summaryLabel";
            this._summaryLabel.Size = new System.Drawing.Size(107, 13);
            this._summaryLabel.TabIndex = 2;
            this._summaryLabel.Text = "Archivos: 0 | URLs: 0";
            // 
            // filesInfoLayout
            // 
            this.filesInfoLayout.AutoSize = true;
            this.filesInfoLayout.ColumnCount = 1;
            this.filesLayout.SetColumnSpan(this.filesInfoLayout, 2);
            this.filesInfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filesInfoLayout.Controls.Add(this._urlsPerBatchLabel, 0, 0);
            this.filesInfoLayout.Controls.Add(this._timeRangeLabel, 0, 1);
            this.filesInfoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesInfoLayout.Location = new System.Drawing.Point(3, 71);
            this.filesInfoLayout.Name = "filesInfoLayout";
            this.filesInfoLayout.RowCount = 2;
            this.filesInfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesInfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.filesInfoLayout.Size = new System.Drawing.Size(982, 26);
            this.filesInfoLayout.TabIndex = 3;
            // 
            // _urlsPerBatchLabel
            // 
            this._urlsPerBatchLabel.AutoSize = true;
            this._urlsPerBatchLabel.Location = new System.Drawing.Point(3, 0);
            this._urlsPerBatchLabel.Name = "_urlsPerBatchLabel";
            this._urlsPerBatchLabel.Size = new System.Drawing.Size(81, 13);
            this._urlsPerBatchLabel.TabIndex = 0;
            this._urlsPerBatchLabel.Text = "URLs por lote: -";
            // 
            // _timeRangeLabel
            // 
            this._timeRangeLabel.AutoSize = true;
            this._timeRangeLabel.Location = new System.Drawing.Point(3, 13);
            this._timeRangeLabel.Name = "_timeRangeLabel";
            this._timeRangeLabel.Size = new System.Drawing.Size(154, 13);
            this._timeRangeLabel.TabIndex = 1;
            this._timeRangeLabel.Text = "Rango de tiempo aproximado: -";
            // 
            // storeGroup
            // 
            this.storeGroup.AutoSize = true;
            this.storeGroup.Controls.Add(this.storeOuterLayout);
            this.storeGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeGroup.Location = new System.Drawing.Point(13, 196);
            this.storeGroup.Name = "storeGroup";
            this.storeGroup.Size = new System.Drawing.Size(994, 156);
            this.storeGroup.TabIndex = 2;
            this.storeGroup.TabStop = false;
            this.storeGroup.Text = "Tienda";
            // 
            // storeOuterLayout
            // 
            this.storeOuterLayout.AutoSize = true;
            this.storeOuterLayout.ColumnCount = 1;
            this.storeOuterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.storeOuterLayout.Controls.Add(this.storeHeaderPanel, 0, 0);
            this.storeOuterLayout.Controls.Add(this.storeGrid, 0, 1);
            this.storeOuterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeOuterLayout.Location = new System.Drawing.Point(3, 16);
            this.storeOuterLayout.Name = "storeOuterLayout";
            this.storeOuterLayout.RowCount = 2;
            this.storeOuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeOuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeOuterLayout.Size = new System.Drawing.Size(988, 137);
            this.storeOuterLayout.TabIndex = 0;
            // 
            // storeHeaderPanel
            // 
            this.storeHeaderPanel.AutoSize = true;
            this.storeHeaderPanel.Controls.Add(this._nameConfigButton);
            this.storeHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeHeaderPanel.Location = new System.Drawing.Point(3, 3);
            this.storeHeaderPanel.Name = "storeHeaderPanel";
            this.storeHeaderPanel.Size = new System.Drawing.Size(982, 33);
            this.storeHeaderPanel.TabIndex = 0;
            // 
            // _nameConfigButton
            // 
            this._nameConfigButton.AutoSize = true;
            this._nameConfigButton.Location = new System.Drawing.Point(3, 3);
            this._nameConfigButton.Name = "_nameConfigButton";
            this._nameConfigButton.Size = new System.Drawing.Size(148, 27);
            this._nameConfigButton.TabIndex = 0;
            this._nameConfigButton.Text = "Configurar nombre...";
            this._nameConfigButton.UseVisualStyleBackColor = true;
            // 
            // storeGrid
            // 
            this.storeGrid.AutoSize = true;
            this.storeGrid.ColumnCount = 2;
            this.storeGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.storeGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.storeGrid.Controls.Add(this._storeProductosTxRadio, 0, 0);
            this.storeGrid.Controls.Add(this._storeBbvsTemplateRadio, 1, 0);
            this.storeGrid.Controls.Add(this._storeHolaproductoRadio, 0, 1);
            this.storeGrid.Controls.Add(this._storeBbvs2daRadio, 1, 1);
            this.storeGrid.Controls.Add(this._storeAltinorRadio, 0, 2);
            this.storeGrid.Controls.Add(this._storeBbvsRadio, 1, 2);
            this.storeGrid.Controls.Add(this._storeHervazTradeRadio, 0, 3);
            this.storeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeGrid.Location = new System.Drawing.Point(3, 42);
            this.storeGrid.Name = "storeGrid";
            this.storeGrid.RowCount = 4;
            this.storeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeGrid.Size = new System.Drawing.Size(982, 92);
            this.storeGrid.TabIndex = 1;
            // 
            // _storeProductosTxRadio
            // 
            this._storeProductosTxRadio.AutoSize = true;
            this._storeProductosTxRadio.Checked = true;
            this._storeProductosTxRadio.Location = new System.Drawing.Point(3, 3);
            this._storeProductosTxRadio.Name = "_storeProductosTxRadio";
            this._storeProductosTxRadio.Size = new System.Drawing.Size(87, 17);
            this._storeProductosTxRadio.TabIndex = 0;
            this._storeProductosTxRadio.TabStop = true;
            this._storeProductosTxRadio.Text = "ProductosTX";
            this._storeProductosTxRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvsTemplateRadio
            // 
            this._storeBbvsTemplateRadio.AutoSize = true;
            this._storeBbvsTemplateRadio.Location = new System.Drawing.Point(494, 3);
            this._storeBbvsTemplateRadio.Name = "_storeBbvsTemplateRadio";
            this._storeBbvsTemplateRadio.Size = new System.Drawing.Size(100, 17);
            this._storeBbvsTemplateRadio.TabIndex = 1;
            this._storeBbvsTemplateRadio.Text = "BBvs_Template";
            this._storeBbvsTemplateRadio.UseVisualStyleBackColor = true;
            // 
            // _storeHolaproductoRadio
            // 
            this._storeHolaproductoRadio.AutoSize = true;
            this._storeHolaproductoRadio.Location = new System.Drawing.Point(3, 26);
            this._storeHolaproductoRadio.Name = "_storeHolaproductoRadio";
            this._storeHolaproductoRadio.Size = new System.Drawing.Size(89, 17);
            this._storeHolaproductoRadio.TabIndex = 2;
            this._storeHolaproductoRadio.Text = "Holaproducto";
            this._storeHolaproductoRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvs2daRadio
            // 
            this._storeBbvs2daRadio.AutoSize = true;
            this._storeBbvs2daRadio.Location = new System.Drawing.Point(494, 26);
            this._storeBbvs2daRadio.Name = "_storeBbvs2daRadio";
            this._storeBbvs2daRadio.Size = new System.Drawing.Size(94, 17);
            this._storeBbvs2daRadio.TabIndex = 3;
            this._storeBbvs2daRadio.Text = "BBvsBB2_2da";
            this._storeBbvs2daRadio.UseVisualStyleBackColor = true;
            // 
            // _storeAltinorRadio
            // 
            this._storeAltinorRadio.AutoSize = true;
            this._storeAltinorRadio.Location = new System.Drawing.Point(3, 49);
            this._storeAltinorRadio.Name = "_storeAltinorRadio";
            this._storeAltinorRadio.Size = new System.Drawing.Size(54, 17);
            this._storeAltinorRadio.TabIndex = 4;
            this._storeAltinorRadio.Text = "Altinor";
            this._storeAltinorRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvsRadio
            // 
            this._storeBbvsRadio.AutoSize = true;
            this._storeBbvsRadio.Location = new System.Drawing.Point(494, 49);
            this._storeBbvsRadio.Name = "_storeBbvsRadio";
            this._storeBbvsRadio.Size = new System.Drawing.Size(70, 17);
            this._storeBbvsRadio.TabIndex = 5;
            this._storeBbvsRadio.Text = "BBvsBB2";
            this._storeBbvsRadio.UseVisualStyleBackColor = true;
            // 
            // _storeHervazTradeRadio
            // 
            this._storeHervazTradeRadio.AutoSize = true;
            this._storeHervazTradeRadio.Location = new System.Drawing.Point(3, 72);
            this._storeHervazTradeRadio.Name = "_storeHervazTradeRadio";
            this._storeHervazTradeRadio.Size = new System.Drawing.Size(87, 17);
            this._storeHervazTradeRadio.TabIndex = 6;
            this._storeHervazTradeRadio.Text = "HervazTrade";
            this._storeHervazTradeRadio.UseVisualStyleBackColor = true;
            // 
            // templateGroup
            // 
            this.templateGroup.AutoSize = true;
            this.templateGroup.Controls.Add(this.templatePanel);
            this.templateGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templateGroup.Location = new System.Drawing.Point(13, 358);
            this.templateGroup.Name = "templateGroup";
            this.templateGroup.Size = new System.Drawing.Size(994, 42);
            this.templateGroup.TabIndex = 3;
            this.templateGroup.TabStop = false;
            this.templateGroup.Text = "Plantilla de sitemap";
            // 
            // templatePanel
            // 
            this.templatePanel.AutoSize = true;
            this.templatePanel.Controls.Add(this._templateNormalRadio);
            this.templatePanel.Controls.Add(this._templateNubeRadio);
            this.templatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.templatePanel.Location = new System.Drawing.Point(3, 16);
            this.templatePanel.Name = "templatePanel";
            this.templatePanel.Size = new System.Drawing.Size(988, 23);
            this.templatePanel.TabIndex = 0;
            // 
            // _templateNormalRadio
            // 
            this._templateNormalRadio.AutoSize = true;
            this._templateNormalRadio.Location = new System.Drawing.Point(3, 3);
            this._templateNormalRadio.Name = "_templateNormalRadio";
            this._templateNormalRadio.Size = new System.Drawing.Size(58, 17);
            this._templateNormalRadio.TabIndex = 0;
            this._templateNormalRadio.TabStop = true;
            this._templateNormalRadio.Text = "Normal";
            this._templateNormalRadio.UseVisualStyleBackColor = true;
            // 
            // _templateNubeRadio
            // 
            this._templateNubeRadio.AutoSize = true;
            this._templateNubeRadio.Location = new System.Drawing.Point(67, 3);
            this._templateNubeRadio.Name = "_templateNubeRadio";
            this._templateNubeRadio.Size = new System.Drawing.Size(51, 17);
            this._templateNubeRadio.TabIndex = 1;
            this._templateNubeRadio.TabStop = true;
            this._templateNubeRadio.Text = "Nube";
            this._templateNubeRadio.UseVisualStyleBackColor = true;
            // 
            // baseNameLayout
            // 
            this.baseNameLayout.AutoSize = true;
            this.baseNameLayout.ColumnCount = 2;
            this.baseNameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.baseNameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.baseNameLayout.Controls.Add(this.baseNameLabel, 0, 0);
            this.baseNameLayout.Controls.Add(this._baseNameText, 1, 0);
            this.baseNameLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baseNameLayout.Location = new System.Drawing.Point(13, 406);
            this.baseNameLayout.Name = "baseNameLayout";
            this.baseNameLayout.RowCount = 1;
            this.baseNameLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.baseNameLayout.Size = new System.Drawing.Size(994, 26);
            this.baseNameLayout.TabIndex = 4;
            // 
            // baseNameLabel
            // 
            this.baseNameLabel.AutoSize = true;
            this.baseNameLabel.Location = new System.Drawing.Point(3, 6);
            this.baseNameLabel.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.baseNameLabel.Name = "baseNameLabel";
            this.baseNameLabel.Size = new System.Drawing.Size(115, 13);
            this.baseNameLabel.TabIndex = 0;
            this.baseNameLabel.Text = "Nombre para sitemaps:";
            // 
            // _baseNameText
            // 
            this._baseNameText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._baseNameText.Location = new System.Drawing.Point(127, 3);
            this._baseNameText.Name = "_baseNameText";
            this._baseNameText.Size = new System.Drawing.Size(864, 20);
            this._baseNameText.TabIndex = 1;
            // 
            // outputGroup
            // 
            this.outputGroup.AutoSize = true;
            this.outputGroup.Controls.Add(this.outputLayout);
            this.outputGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputGroup.Location = new System.Drawing.Point(13, 438);
            this.outputGroup.Name = "outputGroup";
            this.outputGroup.Size = new System.Drawing.Size(994, 87);
            this.outputGroup.TabIndex = 5;
            this.outputGroup.TabStop = false;
            this.outputGroup.Text = "Carpeta destino";
            // 
            // outputLayout
            // 
            this.outputLayout.AutoSize = true;
            this.outputLayout.ColumnCount = 1;
            this.outputLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputLayout.Controls.Add(this.outputRowLayout, 0, 0);
            this.outputLayout.Controls.Add(this._zipCheck, 0, 1);
            this.outputLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputLayout.Location = new System.Drawing.Point(3, 16);
            this.outputLayout.Name = "outputLayout";
            this.outputLayout.RowCount = 2;
            this.outputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputLayout.Size = new System.Drawing.Size(988, 68);
            this.outputLayout.TabIndex = 0;
            // 
            // outputRowLayout
            // 
            this.outputRowLayout.AutoSize = true;
            this.outputRowLayout.ColumnCount = 2;
            this.outputRowLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputRowLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outputRowLayout.Controls.Add(this._outputText, 0, 0);
            this.outputRowLayout.Controls.Add(this.outputButtonsPanel, 1, 0);
            this.outputRowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputRowLayout.Location = new System.Drawing.Point(3, 3);
            this.outputRowLayout.Name = "outputRowLayout";
            this.outputRowLayout.RowCount = 1;
            this.outputRowLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputRowLayout.Size = new System.Drawing.Size(982, 39);
            this.outputRowLayout.TabIndex = 0;
            // 
            // _outputText
            // 
            this._outputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._outputText.Location = new System.Drawing.Point(3, 3);
            this._outputText.Name = "_outputText";
            this._outputText.Size = new System.Drawing.Size(722, 20);
            this._outputText.TabIndex = 0;
            // 
            // outputButtonsPanel
            // 
            this.outputButtonsPanel.AutoSize = true;
            this.outputButtonsPanel.Controls.Add(this._downloadsButton);
            this.outputButtonsPanel.Controls.Add(this._desktopButton);
            this.outputButtonsPanel.Controls.Add(this._chooseOutputButton);
            this.outputButtonsPanel.Location = new System.Drawing.Point(731, 3);
            this.outputButtonsPanel.Name = "outputButtonsPanel";
            this.outputButtonsPanel.Size = new System.Drawing.Size(248, 33);
            this.outputButtonsPanel.TabIndex = 1;
            this.outputButtonsPanel.WrapContents = false;
            // 
            // _downloadsButton
            // 
            this._downloadsButton.AutoSize = true;
            this._downloadsButton.Location = new System.Drawing.Point(3, 3);
            this._downloadsButton.Name = "_downloadsButton";
            this._downloadsButton.Size = new System.Drawing.Size(89, 27);
            this._downloadsButton.TabIndex = 0;
            this._downloadsButton.Text = "Descargas";
            this._downloadsButton.UseVisualStyleBackColor = true;
            // 
            // _desktopButton
            // 
            this._desktopButton.AutoSize = true;
            this._desktopButton.Location = new System.Drawing.Point(98, 3);
            this._desktopButton.Name = "_desktopButton";
            this._desktopButton.Size = new System.Drawing.Size(83, 27);
            this._desktopButton.TabIndex = 1;
            this._desktopButton.Text = "Escritorio";
            this._desktopButton.UseVisualStyleBackColor = true;
            // 
            // _chooseOutputButton
            // 
            this._chooseOutputButton.AutoSize = true;
            this._chooseOutputButton.Location = new System.Drawing.Point(187, 3);
            this._chooseOutputButton.Name = "_chooseOutputButton";
            this._chooseOutputButton.Size = new System.Drawing.Size(58, 27);
            this._chooseOutputButton.TabIndex = 2;
            this._chooseOutputButton.Text = "Otra...";
            this._chooseOutputButton.UseVisualStyleBackColor = true;
            // 
            // _zipCheck
            // 
            this._zipCheck.AutoSize = true;
            this._zipCheck.Location = new System.Drawing.Point(3, 48);
            this._zipCheck.Name = "_zipCheck";
            this._zipCheck.Size = new System.Drawing.Size(114, 17);
            this._zipCheck.TabIndex = 1;
            this._zipCheck.Text = "Exportar como ZIP";
            this._zipCheck.UseVisualStyleBackColor = true;
            // 
            // processPanel
            // 
            this.processPanel.AutoSize = true;
            this.processPanel.Controls.Add(this._processButton);
            this.processPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processPanel.Location = new System.Drawing.Point(13, 531);
            this.processPanel.Name = "processPanel";
            this.processPanel.Size = new System.Drawing.Size(994, 50);
            this.processPanel.TabIndex = 6;
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
            // helpPanel
            // 
            this.helpPanel.AutoSize = true;
            this.helpPanel.Controls.Add(this._helpButton);
            this.helpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.helpPanel.Location = new System.Drawing.Point(13, 587);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Size = new System.Drawing.Size(994, 33);
            this.helpPanel.TabIndex = 7;
            // 
            // _helpButton
            // 
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(923, 3);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 27);
            this._helpButton.TabIndex = 0;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            // 
            // SitemapControl
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "SitemapControl";
            this.Size = new System.Drawing.Size(1020, 633);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.inputLayout.ResumeLayout(false);
            this.inputLayout.PerformLayout();
            this.inputButtonsPanel.ResumeLayout(false);
            this.inputButtonsPanel.PerformLayout();
            this.filesGroup.ResumeLayout(false);
            this.filesLayout.ResumeLayout(false);
            this.filesLayout.PerformLayout();
            this.filesHeaderPanel.ResumeLayout(false);
            this.filesHeaderPanel.PerformLayout();
            this.modePanel.ResumeLayout(false);
            this.modePanel.PerformLayout();
            this.filesInfoLayout.ResumeLayout(false);
            this.filesInfoLayout.PerformLayout();
            this.storeGroup.ResumeLayout(false);
            this.storeGroup.PerformLayout();
            this.storeOuterLayout.ResumeLayout(false);
            this.storeOuterLayout.PerformLayout();
            this.storeHeaderPanel.ResumeLayout(false);
            this.storeHeaderPanel.PerformLayout();
            this.storeGrid.ResumeLayout(false);
            this.storeGrid.PerformLayout();
            this.templateGroup.ResumeLayout(false);
            this.templateGroup.PerformLayout();
            this.templatePanel.ResumeLayout(false);
            this.templatePanel.PerformLayout();
            this.baseNameLayout.ResumeLayout(false);
            this.baseNameLayout.PerformLayout();
            this.outputGroup.ResumeLayout(false);
            this.outputGroup.PerformLayout();
            this.outputLayout.ResumeLayout(false);
            this.outputLayout.PerformLayout();
            this.outputRowLayout.ResumeLayout(false);
            this.outputRowLayout.PerformLayout();
            this.outputButtonsPanel.ResumeLayout(false);
            this.outputButtonsPanel.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel filesHeaderPanel;
        private System.Windows.Forms.FlowLayoutPanel modePanel;
        private System.Windows.Forms.TableLayoutPanel filesInfoLayout;
        private System.Windows.Forms.GroupBox storeGroup;
        private System.Windows.Forms.TableLayoutPanel storeOuterLayout;
        private System.Windows.Forms.FlowLayoutPanel storeHeaderPanel;
        private System.Windows.Forms.TableLayoutPanel storeGrid;
        private System.Windows.Forms.GroupBox templateGroup;
        private System.Windows.Forms.FlowLayoutPanel templatePanel;
        private System.Windows.Forms.TableLayoutPanel baseNameLayout;
        private System.Windows.Forms.Label baseNameLabel;
        private System.Windows.Forms.GroupBox outputGroup;
        private System.Windows.Forms.TableLayoutPanel outputLayout;
        private System.Windows.Forms.TableLayoutPanel outputRowLayout;
        private System.Windows.Forms.FlowLayoutPanel outputButtonsPanel;
        private System.Windows.Forms.FlowLayoutPanel processPanel;
        private System.Windows.Forms.FlowLayoutPanel helpPanel;
        private System.Windows.Forms.Button _importFilesButton;
        private System.Windows.Forms.Button _clearFilesButton;
        private System.Windows.Forms.RadioButton _modeAllRadio;
        private System.Windows.Forms.RadioButton _modeSelectRadio;
        private System.Windows.Forms.ListBox _filesList;
        private System.Windows.Forms.Button _refreshButton;
        private System.Windows.Forms.Label _summaryLabel;
        private System.Windows.Forms.Label _urlsPerBatchLabel;
        private System.Windows.Forms.Label _timeRangeLabel;
        private System.Windows.Forms.RadioButton _templateNormalRadio;
        private System.Windows.Forms.RadioButton _templateNubeRadio;
        private System.Windows.Forms.TextBox _baseNameText;
        private System.Windows.Forms.TextBox _outputText;
        private System.Windows.Forms.Button _downloadsButton;
        private System.Windows.Forms.Button _desktopButton;
        private System.Windows.Forms.Button _chooseOutputButton;
        private System.Windows.Forms.CheckBox _zipCheck;
        private System.Windows.Forms.Button _processButton;
        private System.Windows.Forms.Button _nameConfigButton;
        private System.Windows.Forms.Button _helpButton;
        private System.Windows.Forms.RadioButton _storeProductosTxRadio;
        private System.Windows.Forms.RadioButton _storeHolaproductoRadio;
        private System.Windows.Forms.RadioButton _storeAltinorRadio;
        private System.Windows.Forms.RadioButton _storeHervazTradeRadio;
        private System.Windows.Forms.RadioButton _storeBbvsTemplateRadio;
        private System.Windows.Forms.RadioButton _storeBbvs2daRadio;
        private System.Windows.Forms.RadioButton _storeBbvsRadio;
    }
}

