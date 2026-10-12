using System.Drawing;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>Construcción por código de la interfaz del panel de plantillas (sin diseñador).</summary>
    internal sealed partial class TemplateManagerControl
    {
        private TableLayoutPanel _bannerHost;
        private ListView _list;
        private Button _newButton;
        private Button _duplicateButton;
        private Button _importButton;
        private Button _deleteButton;
        private LinkLabel _restoreLink;
        private TextBox _nameBox;
        private TextBox _descriptionBox;
        private CheckedListBox _storesList;
        private TabControl _tabs;
        private TabPage _jsonPage;
        private TextBox _jsonBox;
        private DataGridView _selectorsGrid;
        private LinkLabel _statusLabel;
        private Button _formatButton;
        private Button _exportButton;
        private Button _discardButton;
        private Button _saveButton;

        /// <summary>Arma la interfaz. Los controles se agregan en orden visual para que la tabulación vaya de izquierda a derecha y de arriba abajo.</summary>
        private void BuildLayout()
        {
            SuspendLayout();

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(6),
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _bannerHost = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 1,
                Visible = false,
                Margin = new Padding(0, 0, 0, 6),
            };
            _bannerHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // Se dimensiona antes de fijar SplitterDistance para que el valor sea válido.
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(900, 500),
                FixedPanel = FixedPanel.Panel1,
                TabStop = false,
            };
            split.SplitterDistance = 280;
            split.Panel1MinSize = 220;
            split.Panel2MinSize = 380;
            split.Panel1.Controls.Add(BuildListPanel());
            split.Panel2.Controls.Add(BuildEditorPanel());

            root.Controls.Add(_bannerHost, 0, 0);
            root.Controls.Add(split, 0, 1);
            root.Controls.Add(BuildStatusBar(), 0, 2);

            Controls.Add(root);
            MinimumSize = new Size(700, 450);
            ResumeLayout(true);
        }

        private Control BuildListPanel()
        {
            var group = new GroupBox { Text = "Plantillas", Dock = DockStyle.Fill };

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false,
                ShowItemToolTips = true,
                AccessibleName = "Lista de plantillas",
            };
            _list.Columns.Add("Nombre", 165);
            _list.Columns.Add("Selectores", 65, HorizontalAlignment.Right);
            _list.Columns.Add("Tiendas", 160);
            _list.ItemSelectionChanged += OnListItemSelectionChanged;
            _list.KeyDown += OnListKeyDown;
            layout.Controls.Add(_list, 0, 0);
            layout.SetColumnSpan(_list, 2);

            _newButton = ListButton("Nueva", (s, e) => CreateNew());
            _duplicateButton = ListButton("Duplicar", (s, e) => DuplicateCurrent());
            _importButton = ListButton("Importar…", (s, e) => ImportTemplate());
            _deleteButton = ListButton("Eliminar", (s, e) => DeleteCurrent());
            layout.Controls.Add(_newButton, 0, 1);
            layout.Controls.Add(_duplicateButton, 1, 1);
            layout.Controls.Add(_importButton, 0, 2);
            layout.Controls.Add(_deleteButton, 1, 2);

            _restoreLink = new LinkLabel { Text = "Restaurar predeterminadas", AutoSize = true, Margin = new Padding(3, 6, 3, 3) };
            _restoreLink.LinkClicked += (s, e) => RestoreDefaults();
            layout.Controls.Add(_restoreLink, 0, 3);
            layout.SetColumnSpan(_restoreLink, 2);

            _toolTip.SetToolTip(_newButton, "Nueva plantilla (Ctrl+N)");
            _toolTip.SetToolTip(_deleteButton, "Eliminar la plantilla (Supr en la lista)");

            group.Controls.Add(layout);
            return group;
        }

        private static Button ListButton(string text, System.EventHandler onClick)
        {
            var button = new Button { Text = text, Dock = DockStyle.Fill, AutoSize = true };
            button.Click += onClick;
            return button;
        }

        private Control BuildEditorPanel()
        {
            var group = new GroupBox { Text = "Plantilla", Dock = DockStyle.Fill };

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _nameBox = new TextBox
            {
                Dock = DockStyle.Fill,
                MaxLength = SitemapTemplateRepository.MaxNameLength,
                AccessibleName = "Nombre de la plantilla",
            };
            _nameBox.TextChanged += OnNameChanged;
            AddField(layout, 0, "&Nombre:", _nameBox);

            _descriptionBox = new TextBox
            {
                Dock = DockStyle.Fill,
                MaxLength = SitemapTemplateRepository.MaxDescriptionLength,
                AccessibleName = "Descripción de la plantilla",
            };
            _descriptionBox.TextChanged += OnDescriptionChanged;
            AddField(layout, 1, "&Descripción:", _descriptionBox);

            _storesList = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                IntegralHeight = false,
                AccessibleName = "Tiendas vinculadas",
            };
            _storesList.Height = _storesList.ItemHeight * (StoreCatalog.StoresLeft.Length + StoreCatalog.StoresRight.Length) + 6;
            _storesList.ItemCheck += OnStoreItemCheck;
            AddField(layout, 2, "&Tiendas vinculadas:", _storesList);

            _tabs = new TabControl { Dock = DockStyle.Fill, AccessibleName = "Contenido de la plantilla" };
            _jsonPage = new TabPage("JSON") { Padding = new Padding(3) };
            _jsonBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsReturn = true,
                WordWrap = false,
                ScrollBars = ScrollBars.Both,
                MaxLength = SitemapTemplateValidator.MaxTemplateChars,
                Font = _editorFont,
                AccessibleName = "JSON del sitemap",
            };
            _jsonBox.TextChanged += OnJsonChanged;
            _jsonPage.Controls.Add(_jsonBox);

            var selectorsPage = new TabPage("Selectores") { Padding = new Padding(3) };
            _selectorsGrid = BuildSelectorsGrid();
            selectorsPage.Controls.Add(_selectorsGrid);

            _tabs.TabPages.Add(_jsonPage);
            _tabs.TabPages.Add(selectorsPage);
            layout.Controls.Add(_tabs, 0, 3);
            layout.SetColumnSpan(_tabs, 2);

            group.Controls.Add(layout);
            return group;
        }

        private static void AddField(TableLayoutPanel layout, int row, string caption, Control field)
        {
            // La etiqueta va antes que el campo para que su tecla de acceso (Alt+letra) enfoque el campo.
            var label = new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Margin = new Padding(3, 6, 3, 3) };
            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(field, 1, row);
        }

        private static DataGridView BuildSelectorsGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                AccessibleName = "Selectores del sitemap",
            };
            grid.Columns.Add("Id", "Id");
            grid.Columns.Add("Tipo", "Tipo");
            grid.Columns.Add("Selector", "Selector");
            grid.Columns.Add("Multiple", "Múltiple");
            grid.Columns.Add("Padres", "Padres");
            grid.Columns["Selector"].FillWeight = 200f;
            grid.Columns["Multiple"].FillWeight = 50f;
            return grid;
        }

        private Control BuildStatusBar()
        {
            var bar = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, RowCount = 1 };
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            bar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _statusLabel = new LinkLabel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                LinkBehavior = LinkBehavior.HoverUnderline,
                AccessibleName = "Estado de validación",
                Margin = new Padding(3, 8, 3, 3),
            };
            _statusLabel.LinkClicked += OnStatusLinkClicked;
            bar.Controls.Add(_statusLabel, 0, 0);

            var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Anchor = AnchorStyles.Right };
            _formatButton = BarButton(buttons, "Formatear JSON", (s, e) => FormatJson());
            _exportButton = BarButton(buttons, "Exportar…", (s, e) => ExportTemplate());
            _discardButton = BarButton(buttons, "Descartar cambios", (s, e) => DiscardChanges());
            _saveButton = BarButton(buttons, "Guardar", (s, e) => SaveChanges());
            _toolTip.SetToolTip(_saveButton, "Guardar (Ctrl+S)");
            bar.Controls.Add(buttons, 1, 0);
            return bar;
        }

        private static Button BarButton(FlowLayoutPanel host, string text, System.EventHandler onClick)
        {
            var button = new Button { Text = text, AutoSize = true, MinimumSize = new Size(90, 0) };
            button.Click += onClick;
            host.Controls.Add(button);
            return button;
        }

        /// <summary>
        /// Muestra el aviso de carga (una vez por instancia, descartable) o, en solo lectura, el aviso del motivo;
        /// en ese caso el aviso de carga describe lo mismo y se omite para no duplicarlo.
        /// </summary>
        private void ShowBanners()
        {
            var warning = _repository.LoadWarning;
            if (warning != null && !_readOnly)
            {
                AddBanner(warning, true);
            }

            switch (_repository.ReadOnlyReason)
            {
                case StoreReadOnlyReason.FutureSchema:
                    AddBanner(FutureSchemaBanner, false);
                    break;
                case StoreReadOnlyReason.IoError:
                    AddBanner(IoErrorBanner, false);
                    break;
            }
        }

        /// <summary>Agrega un aviso amarillo arriba del panel; si es descartable lleva un enlace «×» que lo quita.</summary>
        private void AddBanner(string text, bool dismissible)
        {
            var banner = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 2,
                BackColor = SystemColors.Info,
                ForeColor = SystemColors.InfoText,
                Padding = new Padding(4),
                Margin = new Padding(0, 0, 0, 3),
            };
            banner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            banner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            banner.Controls.Add(new Label { Text = text, AutoSize = true, Dock = DockStyle.Fill, AccessibleName = "Aviso" }, 0, 0);

            if (dismissible)
            {
                var close = new LinkLabel { Text = "×", AutoSize = true, AccessibleName = "Cerrar aviso", LinkBehavior = LinkBehavior.NeverUnderline };
                close.LinkClicked += (s, e) =>
                {
                    _bannerHost.Controls.Remove(banner);
                    banner.Dispose();
                    _bannerHost.Visible = _bannerHost.Controls.Count > 0;
                };
                banner.Controls.Add(close, 1, 0);
            }

            _bannerHost.Controls.Add(banner);
            _bannerHost.Visible = true;
        }

        /// <summary>En solo lectura los campos no se editan; los botones de mutación se deshabilitan en <c>UpdateCommands</c>.</summary>
        private void ApplyReadOnly()
        {
            _nameBox.ReadOnly = _readOnly;
            _descriptionBox.ReadOnly = _readOnly;
            _jsonBox.ReadOnly = _readOnly;
            _storesList.Enabled = !_readOnly;
        }
    }
}
