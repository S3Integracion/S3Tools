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
            rootLayout = new System.Windows.Forms.TableLayoutPanel();
            leftLayout = new System.Windows.Forms.TableLayoutPanel();
            urlGroup = new System.Windows.Forms.GroupBox();
            urlLayout = new System.Windows.Forms.TableLayoutPanel();
            urlLabel = new System.Windows.Forms.Label();
            _urlText = new System.Windows.Forms.TextBox();
            urlButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            _analyzeUrlButton = new System.Windows.Forms.Button();
            _clearUrlButton = new System.Windows.Forms.Button();
            tiendaPanel = new System.Windows.Forms.FlowLayoutPanel();
            tiendaLabel = new System.Windows.Forms.Label();
            _tiendaValue = new System.Windows.Forms.Label();
            categoriasGroup = new System.Windows.Forms.GroupBox();
            categoriasLayout = new System.Windows.Forms.TableLayoutPanel();
            catActionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            _selectAllButton = new System.Windows.Forms.Button();
            _selectNoneButton = new System.Windows.Forms.Button();
            _reloadCategoriesButton = new System.Windows.Forms.Button();
            _categoriasCount = new System.Windows.Forms.Label();
            _categoriasList = new System.Windows.Forms.CheckedListBox();
            verificationGroup = new System.Windows.Forms.GroupBox();
            _verificationGrid = new System.Windows.Forms.DataGridView();
            rightLayout = new System.Windows.Forms.TableLayoutPanel();
            generateGroup = new System.Windows.Forms.GroupBox();
            generateLayout = new System.Windows.Forms.TableLayoutPanel();
            generateInputsPanel = new System.Windows.Forms.FlowLayoutPanel();
            _generateButton = new System.Windows.Forms.Button();
            _statusLabel = new System.Windows.Forms.Label();
            resultsGroup = new System.Windows.Forms.GroupBox();
            _resultsGrid = new System.Windows.Forms.DataGridView();
            actionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            _copyAllButton = new System.Windows.Forms.Button();
            _copySelectionButton = new System.Windows.Forms.Button();
            _exportTxtButton = new System.Windows.Forms.Button();
            _exportCsvButton = new System.Windows.Forms.Button();
            _clearResultsButton = new System.Windows.Forms.Button();
            _helpButton = new System.Windows.Forms.Button();
            rootLayout.SuspendLayout();
            leftLayout.SuspendLayout();
            urlGroup.SuspendLayout();
            urlLayout.SuspendLayout();
            urlButtonsPanel.SuspendLayout();
            tiendaPanel.SuspendLayout();
            categoriasGroup.SuspendLayout();
            categoriasLayout.SuspendLayout();
            catActionsPanel.SuspendLayout();
            verificationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_verificationGrid).BeginInit();
            rightLayout.SuspendLayout();
            generateGroup.SuspendLayout();
            generateLayout.SuspendLayout();
            generateInputsPanel.SuspendLayout();
            resultsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_resultsGrid).BeginInit();
            actionsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // rootLayout
            // 
            rootLayout.ColumnCount = 2;
            rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            rootLayout.Controls.Add(leftLayout, 0, 0);
            rootLayout.Controls.Add(rightLayout, 1, 0);
            rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            rootLayout.Location = new System.Drawing.Point(0, 0);
            rootLayout.Name = "rootLayout";
            rootLayout.Padding = new System.Windows.Forms.Padding(10);
            rootLayout.RowCount = 1;
            rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rootLayout.Size = new System.Drawing.Size(1849, 900);
            rootLayout.TabIndex = 0;
            // 
            // leftLayout
            // 
            leftLayout.ColumnCount = 1;
            leftLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftLayout.Controls.Add(urlGroup, 0, 0);
            leftLayout.Controls.Add(categoriasGroup, 0, 1);
            leftLayout.Controls.Add(verificationGroup, 0, 2);
            leftLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            leftLayout.Location = new System.Drawing.Point(10, 10);
            leftLayout.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            leftLayout.Name = "leftLayout";
            leftLayout.RowCount = 3;
            leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            leftLayout.Size = new System.Drawing.Size(908, 880);
            leftLayout.TabIndex = 0;
            // 
            // urlGroup
            // 
            urlGroup.Controls.Add(urlLayout);
            urlGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            urlGroup.Location = new System.Drawing.Point(3, 3);
            urlGroup.Name = "urlGroup";
            urlGroup.Size = new System.Drawing.Size(902, 124);
            urlGroup.TabIndex = 0;
            urlGroup.TabStop = false;
            urlGroup.Text = "URL de origen (Amazon)";
            // 
            // urlLayout
            // 
            urlLayout.ColumnCount = 2;
            urlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            urlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            urlLayout.Controls.Add(urlLabel, 0, 0);
            urlLayout.Controls.Add(_urlText, 0, 1);
            urlLayout.Controls.Add(urlButtonsPanel, 1, 1);
            urlLayout.Controls.Add(tiendaPanel, 0, 2);
            urlLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            urlLayout.Location = new System.Drawing.Point(3, 19);
            urlLayout.Name = "urlLayout";
            urlLayout.RowCount = 3;
            urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            urlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            urlLayout.Size = new System.Drawing.Size(896, 102);
            urlLayout.TabIndex = 0;
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLayout.SetColumnSpan(urlLabel, 2);
            urlLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            urlLabel.Location = new System.Drawing.Point(3, 0);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new System.Drawing.Size(213, 13);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "Pega una URL de tienda de Amazon";
            // 
            // _urlText
            // 
            _urlText.Dock = System.Windows.Forms.DockStyle.Fill;
            _urlText.Location = new System.Drawing.Point(3, 16);
            _urlText.Name = "_urlText";
            _urlText.Size = new System.Drawing.Size(710, 23);
            _urlText.TabIndex = 1;
            // 
            // urlButtonsPanel
            // 
            urlButtonsPanel.AutoSize = true;
            urlButtonsPanel.Controls.Add(_analyzeUrlButton);
            urlButtonsPanel.Controls.Add(_clearUrlButton);
            urlButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            urlButtonsPanel.Location = new System.Drawing.Point(719, 16);
            urlButtonsPanel.Name = "urlButtonsPanel";
            urlButtonsPanel.Size = new System.Drawing.Size(174, 44);
            urlButtonsPanel.TabIndex = 2;
            urlButtonsPanel.WrapContents = false;
            // 
            // _analyzeUrlButton
            // 
            _analyzeUrlButton.AutoSize = true;
            _analyzeUrlButton.Location = new System.Drawing.Point(3, 3);
            _analyzeUrlButton.Name = "_analyzeUrlButton";
            _analyzeUrlButton.Size = new System.Drawing.Size(81, 27);
            _analyzeUrlButton.TabIndex = 0;
            _analyzeUrlButton.Text = "Analizar";
            _analyzeUrlButton.UseVisualStyleBackColor = true;
            // 
            // _clearUrlButton
            // 
            _clearUrlButton.AutoSize = true;
            _clearUrlButton.Location = new System.Drawing.Point(90, 3);
            _clearUrlButton.Name = "_clearUrlButton";
            _clearUrlButton.Size = new System.Drawing.Size(81, 27);
            _clearUrlButton.TabIndex = 1;
            _clearUrlButton.Text = "Limpiar";
            _clearUrlButton.UseVisualStyleBackColor = true;
            // 
            // tiendaPanel
            // 
            urlLayout.SetColumnSpan(tiendaPanel, 2);
            tiendaPanel.Controls.Add(tiendaLabel);
            tiendaPanel.Controls.Add(_tiendaValue);
            tiendaPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tiendaPanel.Location = new System.Drawing.Point(3, 66);
            tiendaPanel.Name = "tiendaPanel";
            tiendaPanel.Size = new System.Drawing.Size(890, 33);
            tiendaPanel.TabIndex = 3;
            tiendaPanel.WrapContents = false;
            // 
            // tiendaLabel
            // 
            tiendaLabel.AutoSize = true;
            tiendaLabel.Location = new System.Drawing.Point(3, 8);
            tiendaLabel.Margin = new System.Windows.Forms.Padding(3, 8, 6, 0);
            tiendaLabel.Name = "tiendaLabel";
            tiendaLabel.Size = new System.Drawing.Size(129, 15);
            tiendaLabel.TabIndex = 0;
            tiendaLabel.Text = "Identificador de tienda:";
            // 
            // _tiendaValue
            // 
            _tiendaValue.AutoSize = true;
            _tiendaValue.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            _tiendaValue.Location = new System.Drawing.Point(141, 8);
            _tiendaValue.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            _tiendaValue.Name = "_tiendaValue";
            _tiendaValue.Size = new System.Drawing.Size(120, 17);
            _tiendaValue.TabIndex = 1;
            _tiendaValue.Text = "(no detectado)";
            // 
            // categoriasGroup
            // 
            categoriasGroup.Controls.Add(categoriasLayout);
            categoriasGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            categoriasGroup.Location = new System.Drawing.Point(3, 133);
            categoriasGroup.Name = "categoriasGroup";
            categoriasGroup.Size = new System.Drawing.Size(902, 406);
            categoriasGroup.TabIndex = 1;
            categoriasGroup.TabStop = false;
            categoriasGroup.Text = "Categorias";
            // 
            // categoriasLayout
            // 
            categoriasLayout.ColumnCount = 1;
            categoriasLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            categoriasLayout.Controls.Add(catActionsPanel, 0, 0);
            categoriasLayout.Controls.Add(_categoriasList, 0, 1);
            categoriasLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            categoriasLayout.Location = new System.Drawing.Point(3, 19);
            categoriasLayout.Name = "categoriasLayout";
            categoriasLayout.RowCount = 2;
            categoriasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            categoriasLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            categoriasLayout.Size = new System.Drawing.Size(896, 384);
            categoriasLayout.TabIndex = 0;
            // 
            // catActionsPanel
            // 
            catActionsPanel.AutoSize = true;
            catActionsPanel.Controls.Add(_selectAllButton);
            catActionsPanel.Controls.Add(_selectNoneButton);
            catActionsPanel.Controls.Add(_reloadCategoriesButton);
            catActionsPanel.Controls.Add(_categoriasCount);
            catActionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            catActionsPanel.Location = new System.Drawing.Point(3, 3);
            catActionsPanel.Name = "catActionsPanel";
            catActionsPanel.Size = new System.Drawing.Size(890, 33);
            catActionsPanel.TabIndex = 0;
            // 
            // _selectAllButton
            // 
            _selectAllButton.AutoSize = true;
            _selectAllButton.Location = new System.Drawing.Point(3, 3);
            _selectAllButton.Name = "_selectAllButton";
            _selectAllButton.Size = new System.Drawing.Size(125, 27);
            _selectAllButton.TabIndex = 0;
            _selectAllButton.Text = "Seleccionar todas";
            _selectAllButton.UseVisualStyleBackColor = true;
            // 
            // _selectNoneButton
            // 
            _selectNoneButton.AutoSize = true;
            _selectNoneButton.Location = new System.Drawing.Point(134, 3);
            _selectNoneButton.Name = "_selectNoneButton";
            _selectNoneButton.Size = new System.Drawing.Size(75, 27);
            _selectNoneButton.TabIndex = 1;
            _selectNoneButton.Text = "Ninguna";
            _selectNoneButton.UseVisualStyleBackColor = true;
            // 
            // _reloadCategoriesButton
            // 
            _reloadCategoriesButton.AutoSize = true;
            _reloadCategoriesButton.Location = new System.Drawing.Point(215, 3);
            _reloadCategoriesButton.Name = "_reloadCategoriesButton";
            _reloadCategoriesButton.Size = new System.Drawing.Size(150, 27);
            _reloadCategoriesButton.TabIndex = 2;
            _reloadCategoriesButton.Text = "Recargar categorias";
            _reloadCategoriesButton.UseVisualStyleBackColor = true;
            // 
            // _categoriasCount
            // 
            _categoriasCount.AutoSize = true;
            _categoriasCount.Location = new System.Drawing.Point(371, 8);
            _categoriasCount.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            _categoriasCount.Name = "_categoriasCount";
            _categoriasCount.Size = new System.Drawing.Size(93, 15);
            _categoriasCount.TabIndex = 3;
            _categoriasCount.Text = "Seleccionadas: 0";
            // 
            // _categoriasList
            // 
            _categoriasList.CheckOnClick = true;
            _categoriasList.Dock = System.Windows.Forms.DockStyle.Fill;
            _categoriasList.FormattingEnabled = true;
            _categoriasList.IntegralHeight = false;
            _categoriasList.Location = new System.Drawing.Point(3, 42);
            _categoriasList.Name = "_categoriasList";
            _categoriasList.Size = new System.Drawing.Size(890, 339);
            _categoriasList.TabIndex = 1;
            // 
            // verificationGroup
            // 
            verificationGroup.Controls.Add(_verificationGrid);
            verificationGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            verificationGroup.Location = new System.Drawing.Point(3, 545);
            verificationGroup.Name = "verificationGroup";
            verificationGroup.Size = new System.Drawing.Size(902, 332);
            verificationGroup.TabIndex = 2;
            verificationGroup.TabStop = false;
            verificationGroup.Text = "Verificacion (page=2, doble clic o Ctrl+clic para abrir)";
            // 
            // _verificationGrid
            // 
            _verificationGrid.AllowUserToAddRows = false;
            _verificationGrid.AllowUserToDeleteRows = false;
            _verificationGrid.AllowUserToResizeRows = false;
            _verificationGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            _verificationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _verificationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            _verificationGrid.Location = new System.Drawing.Point(3, 19);
            _verificationGrid.MultiSelect = false;
            _verificationGrid.Name = "_verificationGrid";
            _verificationGrid.ReadOnly = true;
            _verificationGrid.RowHeadersVisible = false;
            _verificationGrid.RowTemplate.Height = 24;
            _verificationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _verificationGrid.Size = new System.Drawing.Size(896, 310);
            _verificationGrid.TabIndex = 0;
            // 
            // rightLayout
            // 
            rightLayout.ColumnCount = 1;
            rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightLayout.Controls.Add(generateGroup, 0, 0);
            rightLayout.Controls.Add(resultsGroup, 0, 1);
            rightLayout.Controls.Add(actionsPanel, 0, 2);
            rightLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            rightLayout.Location = new System.Drawing.Point(930, 10);
            rightLayout.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            rightLayout.Name = "rightLayout";
            rightLayout.RowCount = 3;
            rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            rightLayout.Size = new System.Drawing.Size(909, 880);
            rightLayout.TabIndex = 1;
            // 
            // generateGroup
            // 
            generateGroup.Controls.Add(generateLayout);
            generateGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            generateGroup.Location = new System.Drawing.Point(3, 3);
            generateGroup.Name = "generateGroup";
            generateGroup.Size = new System.Drawing.Size(903, 84);
            generateGroup.TabIndex = 0;
            generateGroup.TabStop = false;
            generateGroup.Text = "Generacion";
            // 
            // generateLayout
            // 
            generateLayout.ColumnCount = 1;
            generateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            generateLayout.Controls.Add(generateInputsPanel, 0, 0);
            generateLayout.Controls.Add(_statusLabel, 0, 1);
            generateLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            generateLayout.Location = new System.Drawing.Point(3, 19);
            generateLayout.Name = "generateLayout";
            generateLayout.RowCount = 2;
            generateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            generateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            generateLayout.Size = new System.Drawing.Size(897, 62);
            generateLayout.TabIndex = 0;
            // 
            // generateInputsPanel
            // 
            generateInputsPanel.AutoSize = true;
            generateInputsPanel.Controls.Add(_generateButton);
            generateInputsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            generateInputsPanel.Location = new System.Drawing.Point(3, 3);
            generateInputsPanel.Name = "generateInputsPanel";
            generateInputsPanel.Size = new System.Drawing.Size(891, 35);
            generateInputsPanel.TabIndex = 0;
            generateInputsPanel.WrapContents = false;
            //
            // _generateButton
            //
            _generateButton.AutoSize = true;
            _generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            _generateButton.Location = new System.Drawing.Point(3, 3);
            _generateButton.Name = "_generateButton";
            _generateButton.Padding = new System.Windows.Forms.Padding(12, 2, 12, 2);
            _generateButton.Size = new System.Drawing.Size(260, 29);
            _generateButton.TabIndex = 0;
            _generateButton.Text = "Generar URLs (edita paginas por fila)";
            _generateButton.UseVisualStyleBackColor = true;
            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            _statusLabel.Location = new System.Drawing.Point(3, 41);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new System.Drawing.Size(35, 15);
            _statusLabel.TabIndex = 1;
            _statusLabel.Text = "Listo.";
            // 
            // resultsGroup
            // 
            resultsGroup.Controls.Add(_resultsGrid);
            resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            resultsGroup.Location = new System.Drawing.Point(3, 93);
            resultsGroup.Name = "resultsGroup";
            resultsGroup.Size = new System.Drawing.Size(903, 704);
            resultsGroup.TabIndex = 1;
            resultsGroup.TabStop = false;
            resultsGroup.Text = "Resultados (doble clic o Ctrl+clic para abrir)";
            // 
            // _resultsGrid
            // 
            _resultsGrid.AllowUserToAddRows = false;
            _resultsGrid.AllowUserToDeleteRows = false;
            _resultsGrid.AllowUserToResizeRows = false;
            _resultsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            _resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            _resultsGrid.Location = new System.Drawing.Point(3, 19);
            _resultsGrid.Name = "_resultsGrid";
            _resultsGrid.ReadOnly = false;
            _resultsGrid.RowHeadersVisible = false;
            _resultsGrid.RowTemplate.Height = 24;
            _resultsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _resultsGrid.Size = new System.Drawing.Size(897, 682);
            _resultsGrid.TabIndex = 0;
            // 
            // actionsPanel
            // 
            actionsPanel.AutoSize = true;
            actionsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            actionsPanel.Controls.Add(_copyAllButton);
            actionsPanel.Controls.Add(_copySelectionButton);
            actionsPanel.Controls.Add(_exportTxtButton);
            actionsPanel.Controls.Add(_exportCsvButton);
            actionsPanel.Controls.Add(_clearResultsButton);
            actionsPanel.Controls.Add(_helpButton);
            actionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            actionsPanel.Location = new System.Drawing.Point(3, 803);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Size = new System.Drawing.Size(903, 74);
            actionsPanel.TabIndex = 2;
            // 
            // _copyAllButton
            // 
            _copyAllButton.AutoSize = true;
            _copyAllButton.Enabled = false;
            _copyAllButton.Location = new System.Drawing.Point(3, 3);
            _copyAllButton.Name = "_copyAllButton";
            _copyAllButton.Size = new System.Drawing.Size(115, 27);
            _copyAllButton.TabIndex = 0;
            _copyAllButton.Text = "Copiar todas";
            _copyAllButton.UseVisualStyleBackColor = true;
            // 
            // _copySelectionButton
            // 
            _copySelectionButton.AutoSize = true;
            _copySelectionButton.Enabled = false;
            _copySelectionButton.Location = new System.Drawing.Point(124, 3);
            _copySelectionButton.Name = "_copySelectionButton";
            _copySelectionButton.Size = new System.Drawing.Size(124, 27);
            _copySelectionButton.TabIndex = 1;
            _copySelectionButton.Text = "Copiar seleccion";
            _copySelectionButton.UseVisualStyleBackColor = true;
            // 
            // _exportTxtButton
            // 
            _exportTxtButton.AutoSize = true;
            _exportTxtButton.Enabled = false;
            _exportTxtButton.Location = new System.Drawing.Point(254, 3);
            _exportTxtButton.Name = "_exportTxtButton";
            _exportTxtButton.Size = new System.Drawing.Size(98, 27);
            _exportTxtButton.TabIndex = 2;
            _exportTxtButton.Text = "Exportar TXT";
            _exportTxtButton.UseVisualStyleBackColor = true;
            // 
            // _exportCsvButton
            // 
            _exportCsvButton.AutoSize = true;
            _exportCsvButton.Enabled = false;
            _exportCsvButton.Location = new System.Drawing.Point(358, 3);
            _exportCsvButton.Name = "_exportCsvButton";
            _exportCsvButton.Size = new System.Drawing.Size(98, 27);
            _exportCsvButton.TabIndex = 3;
            _exportCsvButton.Text = "Exportar CSV";
            _exportCsvButton.UseVisualStyleBackColor = true;
            // 
            // _clearResultsButton
            // 
            _clearResultsButton.AutoSize = true;
            _clearResultsButton.Enabled = false;
            _clearResultsButton.Location = new System.Drawing.Point(462, 3);
            _clearResultsButton.Name = "_clearResultsButton";
            _clearResultsButton.Size = new System.Drawing.Size(122, 27);
            _clearResultsButton.TabIndex = 4;
            _clearResultsButton.Text = "Limpiar resultados";
            _clearResultsButton.UseVisualStyleBackColor = true;
            // 
            // _helpButton
            // 
            _helpButton.AutoSize = true;
            _helpButton.Location = new System.Drawing.Point(590, 3);
            _helpButton.Name = "_helpButton";
            _helpButton.Size = new System.Drawing.Size(68, 27);
            _helpButton.TabIndex = 5;
            _helpButton.Text = "Ayuda";
            _helpButton.UseVisualStyleBackColor = true;
            // 
            // CategoriasControl
            // 
            AutoScroll = true;
            Controls.Add(rootLayout);
            Name = "CategoriasControl";
            Size = new System.Drawing.Size(1849, 900);
            rootLayout.ResumeLayout(false);
            leftLayout.ResumeLayout(false);
            urlGroup.ResumeLayout(false);
            urlLayout.ResumeLayout(false);
            urlLayout.PerformLayout();
            urlButtonsPanel.ResumeLayout(false);
            urlButtonsPanel.PerformLayout();
            tiendaPanel.ResumeLayout(false);
            tiendaPanel.PerformLayout();
            categoriasGroup.ResumeLayout(false);
            categoriasLayout.ResumeLayout(false);
            categoriasLayout.PerformLayout();
            catActionsPanel.ResumeLayout(false);
            catActionsPanel.PerformLayout();
            verificationGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_verificationGrid).EndInit();
            rightLayout.ResumeLayout(false);
            rightLayout.PerformLayout();
            generateGroup.ResumeLayout(false);
            generateLayout.ResumeLayout(false);
            generateLayout.PerformLayout();
            generateInputsPanel.ResumeLayout(false);
            generateInputsPanel.PerformLayout();
            resultsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_resultsGrid).EndInit();
            actionsPanel.ResumeLayout(false);
            actionsPanel.PerformLayout();
            ResumeLayout(false);
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
