namespace S3Integración_programs
{
    partial class CategoriasControl
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
            this.leftLayout = new System.Windows.Forms.TableLayoutPanel();
            this.urlGroup = new System.Windows.Forms.GroupBox();
            this.urlLayout = new System.Windows.Forms.TableLayoutPanel();
            this.urlLabel = new System.Windows.Forms.Label();
            this._urlText = new System.Windows.Forms.TextBox();
            this.urlButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._analyzeUrlButton = new System.Windows.Forms.Button();
            this._clearUrlButton = new System.Windows.Forms.Button();
            this.tiendaPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tiendaLabel = new System.Windows.Forms.Label();
            this._tiendaValue = new System.Windows.Forms.Label();
            this.categoriasGroup = new System.Windows.Forms.GroupBox();
            this.categoriasLayout = new System.Windows.Forms.TableLayoutPanel();
            this.catActionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._selectAllButton = new System.Windows.Forms.Button();
            this._selectNoneButton = new System.Windows.Forms.Button();
            this._reloadCategoriesButton = new System.Windows.Forms.Button();
            this._categoriasCount = new System.Windows.Forms.Label();
            this._categoriasList = new System.Windows.Forms.CheckedListBox();
            this.verificationGroup = new System.Windows.Forms.GroupBox();
            this._verificationGrid = new System.Windows.Forms.DataGridView();
            this.rightLayout = new System.Windows.Forms.TableLayoutPanel();
            this.generateGroup = new System.Windows.Forms.GroupBox();
            this.generateLayout = new System.Windows.Forms.TableLayoutPanel();
            this.generateInputsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pagesLabel = new System.Windows.Forms.Label();
            this._pagesNumeric = new System.Windows.Forms.NumericUpDown();
            this._generateButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
            this.resultsGroup = new System.Windows.Forms.GroupBox();
            this._resultsGrid = new System.Windows.Forms.DataGridView();
            this.actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._copyAllButton = new System.Windows.Forms.Button();
            this._copySelectionButton = new System.Windows.Forms.Button();
            this._exportTxtButton = new System.Windows.Forms.Button();
            this._exportCsvButton = new System.Windows.Forms.Button();
            this._clearResultsButton = new System.Windows.Forms.Button();
            this._helpButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.leftLayout.SuspendLayout();
            this.urlGroup.SuspendLayout();
            this.urlLayout.SuspendLayout();
            this.urlButtonsPanel.SuspendLayout();
            this.tiendaPanel.SuspendLayout();
            this.categoriasGroup.SuspendLayout();
            this.categoriasLayout.SuspendLayout();
            this.catActionsPanel.SuspendLayout();
            this.verificationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verificationGrid)).BeginInit();
            this.rightLayout.SuspendLayout();
            this.generateGroup.SuspendLayout();
            this.generateLayout.SuspendLayout();
            this.generateInputsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pagesNumeric)).BeginInit();
            this.resultsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._resultsGrid)).BeginInit();
            this.actionsPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.Controls.Add(this.leftLayout, 0, 0);
            this.rootLayout.Controls.Add(this.rightLayout, 1, 0);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 1;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(920, 900);
            this.rootLayout.TabIndex = 0;
            //
            // leftLayout
            //
            this.leftLayout.ColumnCount = 1;
            this.leftLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayout.Controls.Add(this.urlGroup, 0, 0);
            this.leftLayout.Controls.Add(this.categoriasGroup, 0, 1);
            this.leftLayout.Controls.Add(this.verificationGroup, 0, 2);
            this.leftLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftLayout.Location = new System.Drawing.Point(13, 13);
            this.leftLayout.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.leftLayout.Name = "leftLayout";
            this.leftLayout.RowCount = 3;
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.leftLayout.Size = new System.Drawing.Size(444, 874);
            this.leftLayout.TabIndex = 0;
            //
            // urlGroup
            //
            this.urlGroup.Controls.Add(this.urlLayout);
            this.urlGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlGroup.Location = new System.Drawing.Point(3, 3);
            this.urlGroup.Name = "urlGroup";
            this.urlGroup.Size = new System.Drawing.Size(438, 124);
            this.urlGroup.TabIndex = 0;
            this.urlGroup.TabStop = false;
            this.urlGroup.Text = "URL de origen (Amazon Mexico)";
            //
            // urlLayout
            //
            this.urlLayout.ColumnCount = 2;
            this.urlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.urlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.urlLayout.Controls.Add(this.urlLabel, 0, 0);
            this.urlLayout.Controls.Add(this._urlText, 0, 1);
            this.urlLayout.Controls.Add(this.urlButtonsPanel, 1, 1);
            this.urlLayout.Controls.Add(this.tiendaPanel, 0, 2);
            this.urlLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlLayout.Location = new System.Drawing.Point(3, 18);
            this.urlLayout.Name = "urlLayout";
            this.urlLayout.RowCount = 3;
            this.urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.urlLayout.Size = new System.Drawing.Size(432, 103);
            this.urlLayout.TabIndex = 0;
            //
            // urlLabel
            //
            this.urlLabel.AutoSize = true;
            this.urlLayout.SetColumnSpan(this.urlLabel, 2);
            this.urlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.urlLabel.Location = new System.Drawing.Point(3, 0);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(220, 17);
            this.urlLabel.TabIndex = 0;
            this.urlLabel.Text = "Pega una URL de tienda de Amazon";
            //
            // _urlText
            //
            this._urlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._urlText.Location = new System.Drawing.Point(3, 20);
            this._urlText.Multiline = false;
            this._urlText.Name = "_urlText";
            this._urlText.Size = new System.Drawing.Size(243, 22);
            this._urlText.TabIndex = 1;
            //
            // urlButtonsPanel
            //
            this.urlButtonsPanel.AutoSize = true;
            this.urlButtonsPanel.Controls.Add(this._analyzeUrlButton);
            this.urlButtonsPanel.Controls.Add(this._clearUrlButton);
            this.urlButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlButtonsPanel.Location = new System.Drawing.Point(252, 17);
            this.urlButtonsPanel.Name = "urlButtonsPanel";
            this.urlButtonsPanel.Size = new System.Drawing.Size(177, 44);
            this.urlButtonsPanel.TabIndex = 2;
            this.urlButtonsPanel.WrapContents = false;
            //
            // _analyzeUrlButton
            //
            this._analyzeUrlButton.AutoSize = true;
            this._analyzeUrlButton.Location = new System.Drawing.Point(3, 3);
            this._analyzeUrlButton.Name = "_analyzeUrlButton";
            this._analyzeUrlButton.Size = new System.Drawing.Size(81, 27);
            this._analyzeUrlButton.TabIndex = 0;
            this._analyzeUrlButton.Text = "Analizar";
            this._analyzeUrlButton.UseVisualStyleBackColor = true;
            //
            // _clearUrlButton
            //
            this._clearUrlButton.AutoSize = true;
            this._clearUrlButton.Location = new System.Drawing.Point(90, 3);
            this._clearUrlButton.Name = "_clearUrlButton";
            this._clearUrlButton.Size = new System.Drawing.Size(81, 27);
            this._clearUrlButton.TabIndex = 1;
            this._clearUrlButton.Text = "Limpiar";
            this._clearUrlButton.UseVisualStyleBackColor = true;
            //
            // tiendaPanel
            //
            this.urlLayout.SetColumnSpan(this.tiendaPanel, 2);
            this.tiendaPanel.Controls.Add(this.tiendaLabel);
            this.tiendaPanel.Controls.Add(this._tiendaValue);
            this.tiendaPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tiendaPanel.Location = new System.Drawing.Point(3, 67);
            this.tiendaPanel.Name = "tiendaPanel";
            this.tiendaPanel.Size = new System.Drawing.Size(426, 33);
            this.tiendaPanel.TabIndex = 3;
            this.tiendaPanel.WrapContents = false;
            //
            // tiendaLabel
            //
            this.tiendaLabel.AutoSize = true;
            this.tiendaLabel.Location = new System.Drawing.Point(3, 8);
            this.tiendaLabel.Margin = new System.Windows.Forms.Padding(3, 8, 6, 0);
            this.tiendaLabel.Name = "tiendaLabel";
            this.tiendaLabel.Size = new System.Drawing.Size(140, 17);
            this.tiendaLabel.TabIndex = 0;
            this.tiendaLabel.Text = "Identificador de tienda:";
            //
            // _tiendaValue
            //
            this._tiendaValue.AutoSize = true;
            this._tiendaValue.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this._tiendaValue.Location = new System.Drawing.Point(152, 8);
            this._tiendaValue.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this._tiendaValue.Name = "_tiendaValue";
            this._tiendaValue.Size = new System.Drawing.Size(83, 17);
            this._tiendaValue.TabIndex = 1;
            this._tiendaValue.Text = "(no detectado)";
            //
            // categoriasGroup
            //
            this.categoriasGroup.Controls.Add(this.categoriasLayout);
            this.categoriasGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoriasGroup.Location = new System.Drawing.Point(3, 133);
            this.categoriasGroup.Name = "categoriasGroup";
            this.categoriasGroup.Size = new System.Drawing.Size(438, 404);
            this.categoriasGroup.TabIndex = 1;
            this.categoriasGroup.TabStop = false;
            this.categoriasGroup.Text = "Categorias";
            //
            // categoriasLayout
            //
            this.categoriasLayout.ColumnCount = 1;
            this.categoriasLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.categoriasLayout.Controls.Add(this.catActionsPanel, 0, 0);
            this.categoriasLayout.Controls.Add(this._categoriasList, 0, 1);
            this.categoriasLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoriasLayout.Location = new System.Drawing.Point(3, 18);
            this.categoriasLayout.Name = "categoriasLayout";
            this.categoriasLayout.RowCount = 2;
            this.categoriasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.categoriasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.categoriasLayout.Size = new System.Drawing.Size(432, 383);
            this.categoriasLayout.TabIndex = 0;
            //
            // catActionsPanel
            //
            this.catActionsPanel.AutoSize = true;
            this.catActionsPanel.Controls.Add(this._selectAllButton);
            this.catActionsPanel.Controls.Add(this._selectNoneButton);
            this.catActionsPanel.Controls.Add(this._reloadCategoriesButton);
            this.catActionsPanel.Controls.Add(this._categoriasCount);
            this.catActionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.catActionsPanel.Location = new System.Drawing.Point(3, 3);
            this.catActionsPanel.Name = "catActionsPanel";
            this.catActionsPanel.Size = new System.Drawing.Size(426, 33);
            this.catActionsPanel.TabIndex = 0;
            this.catActionsPanel.WrapContents = true;
            //
            // _selectAllButton
            //
            this._selectAllButton.AutoSize = true;
            this._selectAllButton.Location = new System.Drawing.Point(3, 3);
            this._selectAllButton.Name = "_selectAllButton";
            this._selectAllButton.Size = new System.Drawing.Size(125, 27);
            this._selectAllButton.TabIndex = 0;
            this._selectAllButton.Text = "Seleccionar todas";
            this._selectAllButton.UseVisualStyleBackColor = true;
            //
            // _selectNoneButton
            //
            this._selectNoneButton.AutoSize = true;
            this._selectNoneButton.Location = new System.Drawing.Point(134, 3);
            this._selectNoneButton.Name = "_selectNoneButton";
            this._selectNoneButton.Size = new System.Drawing.Size(75, 27);
            this._selectNoneButton.TabIndex = 1;
            this._selectNoneButton.Text = "Ninguna";
            this._selectNoneButton.UseVisualStyleBackColor = true;
            //
            // _reloadCategoriesButton
            //
            this._reloadCategoriesButton.AutoSize = true;
            this._reloadCategoriesButton.Location = new System.Drawing.Point(215, 3);
            this._reloadCategoriesButton.Name = "_reloadCategoriesButton";
            this._reloadCategoriesButton.Size = new System.Drawing.Size(150, 27);
            this._reloadCategoriesButton.TabIndex = 2;
            this._reloadCategoriesButton.Text = "Recargar categorias";
            this._reloadCategoriesButton.UseVisualStyleBackColor = true;
            //
            // _categoriasCount
            //
            this._categoriasCount.AutoSize = true;
            this._categoriasCount.Location = new System.Drawing.Point(371, 8);
            this._categoriasCount.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this._categoriasCount.Name = "_categoriasCount";
            this._categoriasCount.Size = new System.Drawing.Size(110, 17);
            this._categoriasCount.TabIndex = 3;
            this._categoriasCount.Text = "Seleccionadas: 0";
            //
            // _categoriasList
            //
            this._categoriasList.CheckOnClick = true;
            this._categoriasList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._categoriasList.FormattingEnabled = true;
            this._categoriasList.IntegralHeight = false;
            this._categoriasList.ItemHeight = 17;
            this._categoriasList.Location = new System.Drawing.Point(3, 42);
            this._categoriasList.Name = "_categoriasList";
            this._categoriasList.Size = new System.Drawing.Size(426, 338);
            this._categoriasList.TabIndex = 1;
            //
            // verificationGroup
            //
            this.verificationGroup.Controls.Add(this._verificationGrid);
            this.verificationGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verificationGroup.Location = new System.Drawing.Point(3, 543);
            this.verificationGroup.Name = "verificationGroup";
            this.verificationGroup.Size = new System.Drawing.Size(438, 328);
            this.verificationGroup.TabIndex = 2;
            this.verificationGroup.TabStop = false;
            this.verificationGroup.Text = "Verificacion (page=2, doble clic o Ctrl+clic para abrir)";
            //
            // _verificationGrid
            //
            this._verificationGrid.AllowUserToAddRows = false;
            this._verificationGrid.AllowUserToDeleteRows = false;
            this._verificationGrid.AllowUserToResizeRows = false;
            this._verificationGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._verificationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._verificationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._verificationGrid.Location = new System.Drawing.Point(3, 18);
            this._verificationGrid.MultiSelect = false;
            this._verificationGrid.Name = "_verificationGrid";
            this._verificationGrid.ReadOnly = true;
            this._verificationGrid.RowHeadersVisible = false;
            this._verificationGrid.RowTemplate.Height = 24;
            this._verificationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._verificationGrid.Size = new System.Drawing.Size(432, 307);
            this._verificationGrid.TabIndex = 0;
            //
            // rightLayout
            //
            this.rightLayout.ColumnCount = 1;
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightLayout.Controls.Add(this.generateGroup, 0, 0);
            this.rightLayout.Controls.Add(this.resultsGroup, 0, 1);
            this.rightLayout.Controls.Add(this.actionsPanel, 0, 2);
            this.rightLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightLayout.Location = new System.Drawing.Point(463, 13);
            this.rightLayout.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.rightLayout.Name = "rightLayout";
            this.rightLayout.RowCount = 3;
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.rightLayout.Size = new System.Drawing.Size(444, 874);
            this.rightLayout.TabIndex = 1;
            //
            // generateGroup
            //
            this.generateGroup.Controls.Add(this.generateLayout);
            this.generateGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateGroup.Location = new System.Drawing.Point(3, 3);
            this.generateGroup.Name = "generateGroup";
            this.generateGroup.Size = new System.Drawing.Size(438, 84);
            this.generateGroup.TabIndex = 0;
            this.generateGroup.TabStop = false;
            this.generateGroup.Text = "Generacion";
            //
            // generateLayout
            //
            this.generateLayout.ColumnCount = 1;
            this.generateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.generateLayout.Controls.Add(this.generateInputsPanel, 0, 0);
            this.generateLayout.Controls.Add(this._statusLabel, 0, 1);
            this.generateLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateLayout.Location = new System.Drawing.Point(3, 18);
            this.generateLayout.Name = "generateLayout";
            this.generateLayout.RowCount = 2;
            this.generateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.generateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.generateLayout.Size = new System.Drawing.Size(432, 63);
            this.generateLayout.TabIndex = 0;
            //
            // generateInputsPanel
            //
            this.generateInputsPanel.AutoSize = true;
            this.generateInputsPanel.Controls.Add(this.pagesLabel);
            this.generateInputsPanel.Controls.Add(this._pagesNumeric);
            this.generateInputsPanel.Controls.Add(this._generateButton);
            this.generateInputsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateInputsPanel.Location = new System.Drawing.Point(3, 3);
            this.generateInputsPanel.Name = "generateInputsPanel";
            this.generateInputsPanel.Size = new System.Drawing.Size(426, 33);
            this.generateInputsPanel.TabIndex = 0;
            this.generateInputsPanel.WrapContents = false;
            //
            // pagesLabel
            //
            this.pagesLabel.AutoSize = true;
            this.pagesLabel.Location = new System.Drawing.Point(3, 8);
            this.pagesLabel.Margin = new System.Windows.Forms.Padding(3, 8, 6, 0);
            this.pagesLabel.Name = "pagesLabel";
            this.pagesLabel.Size = new System.Drawing.Size(160, 17);
            this.pagesLabel.TabIndex = 0;
            this.pagesLabel.Text = "Numero maximo de paginas:";
            //
            // _pagesNumeric
            //
            this._pagesNumeric.Location = new System.Drawing.Point(172, 5);
            this._pagesNumeric.Margin = new System.Windows.Forms.Padding(3, 5, 12, 3);
            this._pagesNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this._pagesNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this._pagesNumeric.Name = "_pagesNumeric";
            this._pagesNumeric.Size = new System.Drawing.Size(80, 22);
            this._pagesNumeric.TabIndex = 1;
            this._pagesNumeric.Value = new decimal(new int[] { 5, 0, 0, 0 });
            //
            // _generateButton
            //
            this._generateButton.AutoSize = true;
            this._generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this._generateButton.Location = new System.Drawing.Point(267, 3);
            this._generateButton.Name = "_generateButton";
            this._generateButton.Padding = new System.Windows.Forms.Padding(12, 2, 12, 2);
            this._generateButton.Size = new System.Drawing.Size(155, 27);
            this._generateButton.TabIndex = 2;
            this._generateButton.Text = "Generar URLs (rango [1-N])";
            this._generateButton.UseVisualStyleBackColor = true;
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this._statusLabel.Location = new System.Drawing.Point(3, 39);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(72, 13);
            this._statusLabel.TabIndex = 1;
            this._statusLabel.Text = "Listo.";
            //
            // resultsGroup
            //
            this.resultsGroup.Controls.Add(this._resultsGrid);
            this.resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsGroup.Location = new System.Drawing.Point(3, 93);
            this.resultsGroup.Name = "resultsGroup";
            this.resultsGroup.Size = new System.Drawing.Size(438, 698);
            this.resultsGroup.TabIndex = 1;
            this.resultsGroup.TabStop = false;
            this.resultsGroup.Text = "Resultados (doble clic o Ctrl+clic para abrir)";
            //
            // _resultsGrid
            //
            this._resultsGrid.AllowUserToAddRows = false;
            this._resultsGrid.AllowUserToDeleteRows = false;
            this._resultsGrid.AllowUserToResizeRows = false;
            this._resultsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGrid.Location = new System.Drawing.Point(3, 18);
            this._resultsGrid.Name = "_resultsGrid";
            this._resultsGrid.ReadOnly = true;
            this._resultsGrid.RowHeadersVisible = false;
            this._resultsGrid.RowTemplate.Height = 24;
            this._resultsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._resultsGrid.Size = new System.Drawing.Size(432, 677);
            this._resultsGrid.TabIndex = 0;
            //
            // actionsPanel
            //
            this.actionsPanel.AutoSize = true;
            this.actionsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.actionsPanel.Controls.Add(this._copyAllButton);
            this.actionsPanel.Controls.Add(this._copySelectionButton);
            this.actionsPanel.Controls.Add(this._exportTxtButton);
            this.actionsPanel.Controls.Add(this._exportCsvButton);
            this.actionsPanel.Controls.Add(this._clearResultsButton);
            this.actionsPanel.Controls.Add(this._helpButton);
            this.actionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsPanel.Location = new System.Drawing.Point(3, 797);
            this.actionsPanel.Name = "actionsPanel";
            this.actionsPanel.Size = new System.Drawing.Size(438, 74);
            this.actionsPanel.TabIndex = 2;
            this.actionsPanel.WrapContents = true;
            //
            // _copyAllButton
            //
            this._copyAllButton.AutoSize = true;
            this._copyAllButton.Enabled = false;
            this._copyAllButton.Location = new System.Drawing.Point(3, 3);
            this._copyAllButton.Name = "_copyAllButton";
            this._copyAllButton.Size = new System.Drawing.Size(115, 27);
            this._copyAllButton.TabIndex = 0;
            this._copyAllButton.Text = "Copiar todas";
            this._copyAllButton.UseVisualStyleBackColor = true;
            //
            // _copySelectionButton
            //
            this._copySelectionButton.AutoSize = true;
            this._copySelectionButton.Enabled = false;
            this._copySelectionButton.Location = new System.Drawing.Point(124, 3);
            this._copySelectionButton.Name = "_copySelectionButton";
            this._copySelectionButton.Size = new System.Drawing.Size(124, 27);
            this._copySelectionButton.TabIndex = 1;
            this._copySelectionButton.Text = "Copiar seleccion";
            this._copySelectionButton.UseVisualStyleBackColor = true;
            //
            // _exportTxtButton
            //
            this._exportTxtButton.AutoSize = true;
            this._exportTxtButton.Enabled = false;
            this._exportTxtButton.Location = new System.Drawing.Point(254, 3);
            this._exportTxtButton.Name = "_exportTxtButton";
            this._exportTxtButton.Size = new System.Drawing.Size(98, 27);
            this._exportTxtButton.TabIndex = 2;
            this._exportTxtButton.Text = "Exportar TXT";
            this._exportTxtButton.UseVisualStyleBackColor = true;
            //
            // _exportCsvButton
            //
            this._exportCsvButton.AutoSize = true;
            this._exportCsvButton.Enabled = false;
            this._exportCsvButton.Location = new System.Drawing.Point(3, 36);
            this._exportCsvButton.Name = "_exportCsvButton";
            this._exportCsvButton.Size = new System.Drawing.Size(98, 27);
            this._exportCsvButton.TabIndex = 3;
            this._exportCsvButton.Text = "Exportar CSV";
            this._exportCsvButton.UseVisualStyleBackColor = true;
            //
            // _clearResultsButton
            //
            this._clearResultsButton.AutoSize = true;
            this._clearResultsButton.Enabled = false;
            this._clearResultsButton.Location = new System.Drawing.Point(107, 36);
            this._clearResultsButton.Name = "_clearResultsButton";
            this._clearResultsButton.Size = new System.Drawing.Size(122, 27);
            this._clearResultsButton.TabIndex = 4;
            this._clearResultsButton.Text = "Limpiar resultados";
            this._clearResultsButton.UseVisualStyleBackColor = true;
            //
            // _helpButton
            //
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(235, 36);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 27);
            this._helpButton.TabIndex = 5;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            //
            // CategoriasControl
            //
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "CategoriasControl";
            this.Size = new System.Drawing.Size(920, 900);
            this.rootLayout.ResumeLayout(false);
            this.leftLayout.ResumeLayout(false);
            this.urlGroup.ResumeLayout(false);
            this.urlLayout.ResumeLayout(false);
            this.urlLayout.PerformLayout();
            this.urlButtonsPanel.ResumeLayout(false);
            this.urlButtonsPanel.PerformLayout();
            this.tiendaPanel.ResumeLayout(false);
            this.tiendaPanel.PerformLayout();
            this.categoriasGroup.ResumeLayout(false);
            this.categoriasLayout.ResumeLayout(false);
            this.categoriasLayout.PerformLayout();
            this.catActionsPanel.ResumeLayout(false);
            this.catActionsPanel.PerformLayout();
            this.verificationGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._verificationGrid)).EndInit();
            this.rightLayout.ResumeLayout(false);
            this.generateGroup.ResumeLayout(false);
            this.generateLayout.ResumeLayout(false);
            this.generateLayout.PerformLayout();
            this.generateInputsPanel.ResumeLayout(false);
            this.generateInputsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pagesNumeric)).EndInit();
            this.resultsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._resultsGrid)).EndInit();
            this.actionsPanel.ResumeLayout(false);
            this.actionsPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel leftLayout;
        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private System.Windows.Forms.GroupBox urlGroup;
        private System.Windows.Forms.TableLayoutPanel urlLayout;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.TextBox _urlText;
        private System.Windows.Forms.FlowLayoutPanel urlButtonsPanel;
        private System.Windows.Forms.Button _analyzeUrlButton;
        private System.Windows.Forms.Button _clearUrlButton;
        private System.Windows.Forms.FlowLayoutPanel tiendaPanel;
        private System.Windows.Forms.Label tiendaLabel;
        private System.Windows.Forms.Label _tiendaValue;
        private System.Windows.Forms.GroupBox categoriasGroup;
        private System.Windows.Forms.TableLayoutPanel categoriasLayout;
        private System.Windows.Forms.FlowLayoutPanel catActionsPanel;
        private System.Windows.Forms.Button _selectAllButton;
        private System.Windows.Forms.Button _selectNoneButton;
        private System.Windows.Forms.Button _reloadCategoriesButton;
        private System.Windows.Forms.Label _categoriasCount;
        private System.Windows.Forms.CheckedListBox _categoriasList;
        private System.Windows.Forms.GroupBox verificationGroup;
        private System.Windows.Forms.DataGridView _verificationGrid;
        private System.Windows.Forms.GroupBox generateGroup;
        private System.Windows.Forms.TableLayoutPanel generateLayout;
        private System.Windows.Forms.FlowLayoutPanel generateInputsPanel;
        private System.Windows.Forms.Label pagesLabel;
        private System.Windows.Forms.NumericUpDown _pagesNumeric;
        private System.Windows.Forms.Button _generateButton;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.GroupBox resultsGroup;
        private System.Windows.Forms.DataGridView _resultsGrid;
        private System.Windows.Forms.FlowLayoutPanel actionsPanel;
        private System.Windows.Forms.Button _copyAllButton;
        private System.Windows.Forms.Button _copySelectionButton;
        private System.Windows.Forms.Button _exportTxtButton;
        private System.Windows.Forms.Button _exportCsvButton;
        private System.Windows.Forms.Button _clearResultsButton;
        private System.Windows.Forms.Button _helpButton;
    }
}
