namespace S3Integración_programs
{
    partial class AsinBatcherControl
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
            this._inputText = new System.Windows.Forms.TextBox();
            this._browseButton = new System.Windows.Forms.Button();
            this.previewGroup = new System.Windows.Forms.GroupBox();
            this.previewLayout = new System.Windows.Forms.TableLayoutPanel();
            this._previewText = new System.Windows.Forms.TextBox();
            this.previewInfoLayout = new System.Windows.Forms.TableLayoutPanel();
            this._urlsPerBatchLabel = new System.Windows.Forms.Label();
            this._timeRangeLabel = new System.Windows.Forms.Label();
            this.previewButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._exportDuplicatesButton = new System.Windows.Forms.Button();
            this.optionsLayout = new System.Windows.Forms.TableLayoutPanel();
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
            this.sellerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._showSellerOnOpenCheck = new System.Windows.Forms.CheckBox();
            this.fileNameLayout = new System.Windows.Forms.TableLayoutPanel();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this._fileNameText = new System.Windows.Forms.TextBox();
            this.marketPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.marketLabel = new System.Windows.Forms.Label();
            this._marketCombo = new System.Windows.Forms.ComboBox();
            this.batchesLabel = new System.Windows.Forms.Label();
            this._batchesNumeric = new System.Windows.Forms.NumericUpDown();
            this.orderLabel = new System.Windows.Forms.Label();
            this._orderCombo = new System.Windows.Forms.ComboBox();
            this.outputGroup = new System.Windows.Forms.GroupBox();
            this.outputLayout = new System.Windows.Forms.TableLayoutPanel();
            this.outputRow = new System.Windows.Forms.TableLayoutPanel();
            this._outputText = new System.Windows.Forms.TextBox();
            this.outputButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
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
            this.previewGroup.SuspendLayout();
            this.previewLayout.SuspendLayout();
            this.previewInfoLayout.SuspendLayout();
            this.previewButtonPanel.SuspendLayout();
            this.optionsLayout.SuspendLayout();
            this.storeGroup.SuspendLayout();
            this.storeOuterLayout.SuspendLayout();
            this.storeHeaderPanel.SuspendLayout();
            this.storeGrid.SuspendLayout();
            this.sellerPanel.SuspendLayout();
            this.fileNameLayout.SuspendLayout();
            this.marketPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._batchesNumeric)).BeginInit();
            this.outputGroup.SuspendLayout();
            this.outputLayout.SuspendLayout();
            this.outputRow.SuspendLayout();
            this.outputButtonPanel.SuspendLayout();
            this.processPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.inputLayout, 0, 0);
            this.rootLayout.Controls.Add(this.previewGroup, 0, 1);
            this.rootLayout.Controls.Add(this.optionsLayout, 0, 2);
            this.rootLayout.Controls.Add(this.outputGroup, 0, 3);
            this.rootLayout.Controls.Add(this.processPanel, 1, 4);
            this.rootLayout.Controls.Add(this.helpPanel, 0, 5);
            this.rootLayout.Controls.Add(this._helpButton, 0, 6);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 7;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.rootLayout.Size = new System.Drawing.Size(900, 740);
            this.rootLayout.TabIndex = 0;
            // 
            // inputLayout
            // 
            this.inputLayout.AutoSize = true;
            this.inputLayout.ColumnCount = 2;
            this.inputLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.inputLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.inputLayout.Controls.Add(this.inputLabel, 0, 0);
            this.inputLayout.Controls.Add(this._inputText, 0, 1);
            this.inputLayout.Controls.Add(this._browseButton, 1, 1);
            this.inputLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputLayout.Location = new System.Drawing.Point(13, 13);
            this.inputLayout.Name = "inputLayout";
            this.inputLayout.RowCount = 2;
            this.inputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.inputLayout.Size = new System.Drawing.Size(874, 50);
            this.inputLayout.TabIndex = 0;
            // 
            // inputLabel
            // 
            this.inputLabel.AutoSize = true;
            this.inputLayout.SetColumnSpan(this.inputLabel, 2);
            this.inputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.inputLabel.Location = new System.Drawing.Point(3, 0);
            this.inputLabel.Name = "inputLabel";
            this.inputLabel.Size = new System.Drawing.Size(236, 17);
            this.inputLabel.TabIndex = 0;
            this.inputLabel.Text = "Archivo de entrada (.txt / .xlsx):";
            // 
            // _inputText
            // 
            this._inputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._inputText.Location = new System.Drawing.Point(3, 20);
            this._inputText.Name = "_inputText";
            this._inputText.Size = new System.Drawing.Size(774, 22);
            this._inputText.TabIndex = 1;
            // 
            // _browseButton
            // 
            this._browseButton.AutoSize = true;
            this._browseButton.Location = new System.Drawing.Point(783, 20);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(88, 27);
            this._browseButton.TabIndex = 2;
            this._browseButton.Text = "Examinar...";
            this._browseButton.UseVisualStyleBackColor = true;
            // 
            // previewGroup
            // 
            this.previewGroup.Controls.Add(this.previewLayout);
            this.previewGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewGroup.Location = new System.Drawing.Point(13, 69);
            this.previewGroup.Name = "previewGroup";
            this.previewGroup.Size = new System.Drawing.Size(874, 175);
            this.previewGroup.TabIndex = 1;
            this.previewGroup.TabStop = false;
            this.previewGroup.Text = "Previsualizacion";
            // 
            // previewLayout
            // 
            this.previewLayout.ColumnCount = 1;
            this.previewLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.previewLayout.Controls.Add(this._previewText, 0, 0);
            this.previewLayout.Controls.Add(this.previewInfoLayout, 0, 1);
            this.previewLayout.Controls.Add(this.previewButtonPanel, 0, 2);
            this.previewLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewLayout.Location = new System.Drawing.Point(3, 18);
            this.previewLayout.Name = "previewLayout";
            this.previewLayout.RowCount = 3;
            this.previewLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.previewLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.previewLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.previewLayout.Size = new System.Drawing.Size(868, 154);
            this.previewLayout.TabIndex = 0;
            // 
            // _previewText
            // 
            this._previewText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewText.Location = new System.Drawing.Point(3, 3);
            this._previewText.Multiline = true;
            this._previewText.Name = "_previewText";
            this._previewText.ReadOnly = true;
            this._previewText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._previewText.Size = new System.Drawing.Size(862, 78);
            this._previewText.TabIndex = 0;
            // 
            // previewInfoLayout
            // 
            this.previewInfoLayout.AutoSize = true;
            this.previewInfoLayout.ColumnCount = 1;
            this.previewInfoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.previewInfoLayout.Controls.Add(this._urlsPerBatchLabel, 0, 0);
            this.previewInfoLayout.Controls.Add(this._timeRangeLabel, 0, 1);
            this.previewInfoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewInfoLayout.Location = new System.Drawing.Point(3, 84);
            this.previewInfoLayout.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.previewInfoLayout.Name = "previewInfoLayout";
            this.previewInfoLayout.RowCount = 2;
            this.previewInfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.previewInfoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.previewInfoLayout.Size = new System.Drawing.Size(862, 34);
            this.previewInfoLayout.TabIndex = 1;
            // 
            // _urlsPerBatchLabel
            // 
            this._urlsPerBatchLabel.AutoSize = true;
            this._urlsPerBatchLabel.Location = new System.Drawing.Point(3, 0);
            this._urlsPerBatchLabel.Name = "_urlsPerBatchLabel";
            this._urlsPerBatchLabel.Size = new System.Drawing.Size(108, 17);
            this._urlsPerBatchLabel.TabIndex = 0;
            this._urlsPerBatchLabel.Text = "URLs por lote: -";
            // 
            // _timeRangeLabel
            // 
            this._timeRangeLabel.AutoSize = true;
            this._timeRangeLabel.Location = new System.Drawing.Point(3, 17);
            this._timeRangeLabel.Name = "_timeRangeLabel";
            this._timeRangeLabel.Size = new System.Drawing.Size(206, 17);
            this._timeRangeLabel.TabIndex = 1;
            this._timeRangeLabel.Text = "Rango de tiempo aproximado: -";
            // 
            // previewButtonPanel
            // 
            this.previewButtonPanel.AutoSize = true;
            this.previewButtonPanel.Controls.Add(this._exportDuplicatesButton);
            this.previewButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewButtonPanel.Location = new System.Drawing.Point(3, 118);
            this.previewButtonPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.previewButtonPanel.Name = "previewButtonPanel";
            this.previewButtonPanel.Size = new System.Drawing.Size(862, 33);
            this.previewButtonPanel.TabIndex = 2;
            // 
            // _exportDuplicatesButton
            // 
            this._exportDuplicatesButton.AutoSize = true;
            this._exportDuplicatesButton.Enabled = false;
            this._exportDuplicatesButton.Location = new System.Drawing.Point(3, 3);
            this._exportDuplicatesButton.Name = "_exportDuplicatesButton";
            this._exportDuplicatesButton.Size = new System.Drawing.Size(143, 27);
            this._exportDuplicatesButton.TabIndex = 0;
            this._exportDuplicatesButton.Text = "Exportar duplicados";
            this._exportDuplicatesButton.UseVisualStyleBackColor = true;
            // 
            // optionsLayout
            // 
            this.optionsLayout.AutoSize = true;
            this.optionsLayout.ColumnCount = 1;
            this.optionsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.optionsLayout.Controls.Add(this.storeGroup, 0, 0);
            this.optionsLayout.Controls.Add(this.sellerPanel, 0, 1);
            this.optionsLayout.Controls.Add(this.fileNameLayout, 0, 2);
            this.optionsLayout.Controls.Add(this.marketPanel, 0, 3);
            this.optionsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsLayout.Location = new System.Drawing.Point(13, 250);
            this.optionsLayout.Name = "optionsLayout";
            this.optionsLayout.RowCount = 4;
            this.optionsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayout.Size = new System.Drawing.Size(874, 283);
            this.optionsLayout.TabIndex = 2;
            // 
            // storeGroup
            // 
            this.storeGroup.AutoSize = true;
            this.storeGroup.Controls.Add(this.storeOuterLayout);
            this.storeGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeGroup.Location = new System.Drawing.Point(3, 3);
            this.storeGroup.Name = "storeGroup";
            this.storeGroup.Size = new System.Drawing.Size(868, 174);
            this.storeGroup.TabIndex = 0;
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
            this.storeOuterLayout.Location = new System.Drawing.Point(3, 18);
            this.storeOuterLayout.Name = "storeOuterLayout";
            this.storeOuterLayout.RowCount = 2;
            this.storeOuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeOuterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.storeOuterLayout.Size = new System.Drawing.Size(862, 153);
            this.storeOuterLayout.TabIndex = 0;
            // 
            // storeHeaderPanel
            // 
            this.storeHeaderPanel.AutoSize = true;
            this.storeHeaderPanel.Controls.Add(this._nameConfigButton);
            this.storeHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storeHeaderPanel.Location = new System.Drawing.Point(3, 3);
            this.storeHeaderPanel.Name = "storeHeaderPanel";
            this.storeHeaderPanel.Size = new System.Drawing.Size(856, 33);
            this.storeHeaderPanel.TabIndex = 0;
            this.storeHeaderPanel.WrapContents = false;
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
            this.storeGrid.Size = new System.Drawing.Size(856, 108);
            this.storeGrid.TabIndex = 1;
            // 
            // _storeProductosTxRadio
            // 
            this._storeProductosTxRadio.AutoSize = true;
            this._storeProductosTxRadio.Checked = true;
            this._storeProductosTxRadio.Location = new System.Drawing.Point(3, 3);
            this._storeProductosTxRadio.Name = "_storeProductosTxRadio";
            this._storeProductosTxRadio.Size = new System.Drawing.Size(111, 21);
            this._storeProductosTxRadio.TabIndex = 0;
            this._storeProductosTxRadio.TabStop = true;
            this._storeProductosTxRadio.Text = "ProductosTX";
            this._storeProductosTxRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvsTemplateRadio
            // 
            this._storeBbvsTemplateRadio.AutoSize = true;
            this._storeBbvsTemplateRadio.Location = new System.Drawing.Point(431, 3);
            this._storeBbvsTemplateRadio.Name = "_storeBbvsTemplateRadio";
            this._storeBbvsTemplateRadio.Size = new System.Drawing.Size(128, 21);
            this._storeBbvsTemplateRadio.TabIndex = 1;
            this._storeBbvsTemplateRadio.Text = "BBvs_Template";
            this._storeBbvsTemplateRadio.UseVisualStyleBackColor = true;
            // 
            // _storeHolaproductoRadio
            // 
            this._storeHolaproductoRadio.AutoSize = true;
            this._storeHolaproductoRadio.Location = new System.Drawing.Point(3, 30);
            this._storeHolaproductoRadio.Name = "_storeHolaproductoRadio";
            this._storeHolaproductoRadio.Size = new System.Drawing.Size(114, 21);
            this._storeHolaproductoRadio.TabIndex = 2;
            this._storeHolaproductoRadio.Text = "Holaproducto";
            this._storeHolaproductoRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvs2daRadio
            // 
            this._storeBbvs2daRadio.AutoSize = true;
            this._storeBbvs2daRadio.Location = new System.Drawing.Point(431, 30);
            this._storeBbvs2daRadio.Name = "_storeBbvs2daRadio";
            this._storeBbvs2daRadio.Size = new System.Drawing.Size(119, 21);
            this._storeBbvs2daRadio.TabIndex = 3;
            this._storeBbvs2daRadio.Text = "BBvsBB2_2da";
            this._storeBbvs2daRadio.UseVisualStyleBackColor = true;
            // 
            // _storeAltinorRadio
            // 
            this._storeAltinorRadio.AutoSize = true;
            this._storeAltinorRadio.Location = new System.Drawing.Point(3, 57);
            this._storeAltinorRadio.Name = "_storeAltinorRadio";
            this._storeAltinorRadio.Size = new System.Drawing.Size(69, 21);
            this._storeAltinorRadio.TabIndex = 4;
            this._storeAltinorRadio.Text = "Altinor";
            this._storeAltinorRadio.UseVisualStyleBackColor = true;
            // 
            // _storeBbvsRadio
            // 
            this._storeBbvsRadio.AutoSize = true;
            this._storeBbvsRadio.Location = new System.Drawing.Point(431, 57);
            this._storeBbvsRadio.Name = "_storeBbvsRadio";
            this._storeBbvsRadio.Size = new System.Drawing.Size(87, 21);
            this._storeBbvsRadio.TabIndex = 5;
            this._storeBbvsRadio.Text = "BBvsBB2";
            this._storeBbvsRadio.UseVisualStyleBackColor = true;
            // 
            // _storeHervazTradeRadio
            // 
            this._storeHervazTradeRadio.AutoSize = true;
            this._storeHervazTradeRadio.Location = new System.Drawing.Point(3, 84);
            this._storeHervazTradeRadio.Name = "_storeHervazTradeRadio";
            this._storeHervazTradeRadio.Size = new System.Drawing.Size(112, 21);
            this._storeHervazTradeRadio.TabIndex = 6;
            this._storeHervazTradeRadio.Text = "HervazTrade";
            this._storeHervazTradeRadio.UseVisualStyleBackColor = true;
            // 
            // sellerPanel
            // 
            this.sellerPanel.AutoSize = true;
            this.sellerPanel.Controls.Add(this._showSellerOnOpenCheck);
            this.sellerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sellerPanel.Location = new System.Drawing.Point(3, 183);
            this.sellerPanel.Name = "sellerPanel";
            this.sellerPanel.Size = new System.Drawing.Size(868, 27);
            this.sellerPanel.TabIndex = 1;
            // 
            // _showSellerOnOpenCheck
            // 
            this._showSellerOnOpenCheck.AutoSize = true;
            this._showSellerOnOpenCheck.Location = new System.Drawing.Point(3, 3);
            this._showSellerOnOpenCheck.Name = "_showSellerOnOpenCheck";
            this._showSellerOnOpenCheck.Size = new System.Drawing.Size(190, 21);
            this._showSellerOnOpenCheck.TabIndex = 0;
            this._showSellerOnOpenCheck.Text = "Mostrar vendedor al abrir";
            this._showSellerOnOpenCheck.UseVisualStyleBackColor = true;
            // 
            // fileNameLayout
            // 
            this.fileNameLayout.AutoSize = true;
            this.fileNameLayout.ColumnCount = 2;
            this.fileNameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.fileNameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fileNameLayout.Controls.Add(this.fileNameLabel, 0, 0);
            this.fileNameLayout.Controls.Add(this._fileNameText, 1, 0);
            this.fileNameLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileNameLayout.Location = new System.Drawing.Point(3, 216);
            this.fileNameLayout.Name = "fileNameLayout";
            this.fileNameLayout.RowCount = 1;
            this.fileNameLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.fileNameLayout.Size = new System.Drawing.Size(868, 28);
            this.fileNameLayout.TabIndex = 2;
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(3, 6);
            this.fileNameLabel.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(135, 17);
            this.fileNameLabel.TabIndex = 0;
            this.fileNameLabel.Text = "Nombre del archivo:";
            // 
            // _fileNameText
            // 
            this._fileNameText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._fileNameText.Location = new System.Drawing.Point(147, 3);
            this._fileNameText.Name = "_fileNameText";
            this._fileNameText.Size = new System.Drawing.Size(718, 22);
            this._fileNameText.TabIndex = 1;
            // 
            // marketPanel
            // 
            this.marketPanel.AutoSize = true;
            this.marketPanel.Controls.Add(this.marketLabel);
            this.marketPanel.Controls.Add(this._marketCombo);
            this.marketPanel.Controls.Add(this.batchesLabel);
            this.marketPanel.Controls.Add(this._batchesNumeric);
            this.marketPanel.Controls.Add(this.orderLabel);
            this.marketPanel.Controls.Add(this._orderCombo);
            this.marketPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.marketPanel.Location = new System.Drawing.Point(3, 250);
            this.marketPanel.Name = "marketPanel";
            this.marketPanel.Size = new System.Drawing.Size(868, 30);
            this.marketPanel.TabIndex = 3;
            this.marketPanel.WrapContents = false;
            // 
            // marketLabel
            // 
            this.marketLabel.AutoSize = true;
            this.marketLabel.Location = new System.Drawing.Point(3, 6);
            this.marketLabel.Margin = new System.Windows.Forms.Padding(3, 6, 4, 0);
            this.marketLabel.Name = "marketLabel";
            this.marketLabel.Size = new System.Drawing.Size(67, 17);
            this.marketLabel.TabIndex = 0;
            this.marketLabel.Text = "Mercado:";
            // 
            // _marketCombo
            // 
            this._marketCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._marketCombo.FormattingEnabled = true;
            this._marketCombo.Items.AddRange(new object[] {
            "MX",
            "US"});
            this._marketCombo.Location = new System.Drawing.Point(77, 3);
            this._marketCombo.Name = "_marketCombo";
            this._marketCombo.Size = new System.Drawing.Size(70, 24);
            this._marketCombo.TabIndex = 1;
            // 
            // batchesLabel
            // 
            this.batchesLabel.AutoSize = true;
            this.batchesLabel.Location = new System.Drawing.Point(160, 6);
            this.batchesLabel.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.batchesLabel.Name = "batchesLabel";
            this.batchesLabel.Size = new System.Drawing.Size(47, 17);
            this.batchesLabel.TabIndex = 2;
            this.batchesLabel.Text = "Lotes:";
            // 
            // _batchesNumeric
            // 
            this._batchesNumeric.Location = new System.Drawing.Point(214, 3);
            this._batchesNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._batchesNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._batchesNumeric.Name = "_batchesNumeric";
            this._batchesNumeric.Size = new System.Drawing.Size(80, 22);
            this._batchesNumeric.TabIndex = 3;
            this._batchesNumeric.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // orderLabel
            // 
            this.orderLabel.AutoSize = true;
            this.orderLabel.Location = new System.Drawing.Point(307, 6);
            this.orderLabel.Margin = new System.Windows.Forms.Padding(10, 6, 4, 0);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(52, 17);
            this.orderLabel.TabIndex = 4;
            this.orderLabel.Text = "Orden:";
            // 
            // _orderCombo
            // 
            this._orderCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderCombo.FormattingEnabled = true;
            this._orderCombo.Items.AddRange(new object[] {
            "Ordenado",
            "Inverso",
            "Aleatorio"});
            this._orderCombo.Location = new System.Drawing.Point(366, 3);
            this._orderCombo.Name = "_orderCombo";
            this._orderCombo.Size = new System.Drawing.Size(110, 24);
            this._orderCombo.TabIndex = 5;
            // 
            // outputGroup
            // 
            this.outputGroup.AutoSize = true;
            this.outputGroup.Controls.Add(this.outputLayout);
            this.outputGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputGroup.Location = new System.Drawing.Point(13, 539);
            this.outputGroup.Name = "outputGroup";
            this.outputGroup.Size = new System.Drawing.Size(874, 93);
            this.outputGroup.TabIndex = 3;
            this.outputGroup.TabStop = false;
            this.outputGroup.Text = "Carpeta destino";
            // 
            // outputLayout
            // 
            this.outputLayout.AutoSize = true;
            this.outputLayout.ColumnCount = 1;
            this.outputLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputLayout.Controls.Add(this.outputRow, 0, 0);
            this.outputLayout.Controls.Add(this._zipCheck, 0, 1);
            this.outputLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputLayout.Location = new System.Drawing.Point(3, 18);
            this.outputLayout.Name = "outputLayout";
            this.outputLayout.RowCount = 2;
            this.outputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputLayout.Size = new System.Drawing.Size(868, 72);
            this.outputLayout.TabIndex = 0;
            // 
            // outputRow
            // 
            this.outputRow.AutoSize = true;
            this.outputRow.ColumnCount = 2;
            this.outputRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outputRow.Controls.Add(this._outputText, 0, 0);
            this.outputRow.Controls.Add(this.outputButtonPanel, 1, 0);
            this.outputRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputRow.Location = new System.Drawing.Point(3, 3);
            this.outputRow.Name = "outputRow";
            this.outputRow.RowCount = 1;
            this.outputRow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outputRow.Size = new System.Drawing.Size(862, 39);
            this.outputRow.TabIndex = 0;
            // 
            // _outputText
            // 
            this._outputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._outputText.Location = new System.Drawing.Point(3, 3);
            this._outputText.Name = "_outputText";
            this._outputText.Size = new System.Drawing.Size(602, 22);
            this._outputText.TabIndex = 0;
            // 
            // outputButtonPanel
            // 
            this.outputButtonPanel.AutoSize = true;
            this.outputButtonPanel.Controls.Add(this._downloadsButton);
            this.outputButtonPanel.Controls.Add(this._desktopButton);
            this.outputButtonPanel.Controls.Add(this._chooseOutputButton);
            this.outputButtonPanel.Location = new System.Drawing.Point(611, 3);
            this.outputButtonPanel.Name = "outputButtonPanel";
            this.outputButtonPanel.Size = new System.Drawing.Size(248, 33);
            this.outputButtonPanel.TabIndex = 1;
            this.outputButtonPanel.WrapContents = false;
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
            this._zipCheck.Size = new System.Drawing.Size(146, 21);
            this._zipCheck.TabIndex = 1;
            this._zipCheck.Text = "Exportar como ZIP";
            this._zipCheck.UseVisualStyleBackColor = true;
            // 
            // processPanel
            // 
            this.processPanel.AutoSize = true;
            this.processPanel.Controls.Add(this._processButton);
            this.processPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processPanel.Location = new System.Drawing.Point(13, 638);
            this.processPanel.Name = "processPanel";
            this.processPanel.Size = new System.Drawing.Size(874, 50);
            this.processPanel.TabIndex = 4;
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
            this.helpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.helpPanel.Location = new System.Drawing.Point(13, 694);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Size = new System.Drawing.Size(874, 1);
            this.helpPanel.TabIndex = 5;
            // 
            // _helpButton
            // 
            this._helpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(819, 700);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 27);
            this._helpButton.TabIndex = 0;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            // 
            // AsinBatcherControl
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "AsinBatcherControl";
            this.Size = new System.Drawing.Size(900, 740);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.inputLayout.ResumeLayout(false);
            this.inputLayout.PerformLayout();
            this.previewGroup.ResumeLayout(false);
            this.previewLayout.ResumeLayout(false);
            this.previewLayout.PerformLayout();
            this.previewInfoLayout.ResumeLayout(false);
            this.previewInfoLayout.PerformLayout();
            this.previewButtonPanel.ResumeLayout(false);
            this.previewButtonPanel.PerformLayout();
            this.optionsLayout.ResumeLayout(false);
            this.optionsLayout.PerformLayout();
            this.storeGroup.ResumeLayout(false);
            this.storeGroup.PerformLayout();
            this.storeOuterLayout.ResumeLayout(false);
            this.storeOuterLayout.PerformLayout();
            this.storeHeaderPanel.ResumeLayout(false);
            this.storeHeaderPanel.PerformLayout();
            this.storeGrid.ResumeLayout(false);
            this.storeGrid.PerformLayout();
            this.sellerPanel.ResumeLayout(false);
            this.sellerPanel.PerformLayout();
            this.fileNameLayout.ResumeLayout(false);
            this.fileNameLayout.PerformLayout();
            this.marketPanel.ResumeLayout(false);
            this.marketPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._batchesNumeric)).EndInit();
            this.outputGroup.ResumeLayout(false);
            this.outputGroup.PerformLayout();
            this.outputLayout.ResumeLayout(false);
            this.outputLayout.PerformLayout();
            this.outputRow.ResumeLayout(false);
            this.outputRow.PerformLayout();
            this.outputButtonPanel.ResumeLayout(false);
            this.outputButtonPanel.PerformLayout();
            this.processPanel.ResumeLayout(false);
            this.processPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel inputLayout;
        private System.Windows.Forms.Label inputLabel;
        private System.Windows.Forms.GroupBox previewGroup;
        private System.Windows.Forms.TableLayoutPanel previewLayout;
        private System.Windows.Forms.TableLayoutPanel previewInfoLayout;
        private System.Windows.Forms.FlowLayoutPanel previewButtonPanel;
        private System.Windows.Forms.TableLayoutPanel optionsLayout;
        private System.Windows.Forms.GroupBox storeGroup;
        private System.Windows.Forms.TableLayoutPanel storeOuterLayout;
        private System.Windows.Forms.FlowLayoutPanel storeHeaderPanel;
        private System.Windows.Forms.TableLayoutPanel storeGrid;
        private System.Windows.Forms.FlowLayoutPanel sellerPanel;
        private System.Windows.Forms.TableLayoutPanel fileNameLayout;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.FlowLayoutPanel marketPanel;
        private System.Windows.Forms.Label marketLabel;
        private System.Windows.Forms.Label batchesLabel;
        private System.Windows.Forms.Label orderLabel;
        private System.Windows.Forms.GroupBox outputGroup;
        private System.Windows.Forms.TableLayoutPanel outputLayout;
        private System.Windows.Forms.TableLayoutPanel outputRow;
        private System.Windows.Forms.FlowLayoutPanel outputButtonPanel;
        private System.Windows.Forms.FlowLayoutPanel processPanel;
        private System.Windows.Forms.FlowLayoutPanel helpPanel;
        private System.Windows.Forms.TextBox _inputText;
        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.TextBox _previewText;
        private System.Windows.Forms.Label _urlsPerBatchLabel;
        private System.Windows.Forms.Label _timeRangeLabel;
        private System.Windows.Forms.Button _exportDuplicatesButton;
        private System.Windows.Forms.Button _nameConfigButton;
        private System.Windows.Forms.RadioButton _storeProductosTxRadio;
        private System.Windows.Forms.RadioButton _storeHolaproductoRadio;
        private System.Windows.Forms.RadioButton _storeAltinorRadio;
        private System.Windows.Forms.RadioButton _storeHervazTradeRadio;
        private System.Windows.Forms.RadioButton _storeBbvsTemplateRadio;
        private System.Windows.Forms.RadioButton _storeBbvs2daRadio;
        private System.Windows.Forms.RadioButton _storeBbvsRadio;
        private System.Windows.Forms.CheckBox _showSellerOnOpenCheck;
        private System.Windows.Forms.TextBox _fileNameText;
        private System.Windows.Forms.ComboBox _marketCombo;
        private System.Windows.Forms.NumericUpDown _batchesNumeric;
        private System.Windows.Forms.ComboBox _orderCombo;
        private System.Windows.Forms.TextBox _outputText;
        private System.Windows.Forms.Button _downloadsButton;
        private System.Windows.Forms.Button _desktopButton;
        private System.Windows.Forms.Button _chooseOutputButton;
        private System.Windows.Forms.CheckBox _zipCheck;
        private System.Windows.Forms.Button _processButton;
        private System.Windows.Forms.Button _helpButton;
    }
}

