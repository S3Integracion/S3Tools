// Sitemap tab UI.
// Imports URL batch files and generates WebScraper sitemap JSONs.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class SitemapControl : UserControl
    {
        private static readonly string[] StoresLeft = { "ProductosTX", "Holaproducto", "Altinor", "HervazTrade" };
        private static readonly string[] StoresRight = { "BBvs_Template", "BBvsBB2_2da", "BBvsBB2" };
        private static readonly string[] InputExtensions = { ".txt", ".csv", ".xlsx", ".json" };
        private static readonly Regex UrlRegex = new Regex("https?://[^\\s\"']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly SitemapEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;
        private bool _suppressStoreSync;

        private Button _importFilesButton;
        private Button _clearFilesButton;
        private RadioButton _modeAllRadio;
        private RadioButton _modeSelectRadio;
        private ListBox _filesList;
        private Button _refreshButton;
        private Label _summaryLabel;
        private TextBox _baseNameText;
        private TextBox _outputText;
        private Button _downloadsButton;
        private Button _desktopButton;
        private Button _chooseOutputButton;
        private CheckBox _zipCheck;
        private Button _processButton;
        private Button _nameConfigButton;
        private Button _helpButton;
        private RadioButton[] _storeRadios;
        private string _namePrefix1 = string.Empty;
        private string _namePrefix2 = string.Empty;

        public SitemapControl()
        {
            InitializeComponent();
            _engineClient = new SitemapEngineClient();
            _inputControls = new List<Control>();

            BuildLayout();
            WireEvents();
            SetDefaults();
        }

        private void BuildLayout()
        {
            SuspendLayout();
            Dock = DockStyle.Fill;
            AutoScroll = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildInputSection(), 0, 0);
            root.Controls.Add(BuildFilesSection(), 0, 1);
            root.Controls.Add(BuildStoreSection(), 0, 2);
            root.Controls.Add(BuildBaseNameSection(), 0, 3);
            root.Controls.Add(BuildOutputSection(), 0, 4);
            root.Controls.Add(BuildProcessSection(), 0, 5);
            root.Controls.Add(BuildHelpSection(), 0, 6);

            Controls.Add(root);
            ResumeLayout();
        }

        private Control BuildInputSection()
        {
            var layout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = "Archivos de entrada (.txt / .csv / .xlsx / .json):",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
            };
            layout.Controls.Add(label, 0, 0);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            _importFilesButton = new Button { Text = "Importar archivos...", AutoSize = true };
            _clearFilesButton = new Button { Text = "Limpiar lista", AutoSize = true };

            buttonPanel.Controls.Add(_importFilesButton);
            buttonPanel.Controls.Add(_clearFilesButton);

            layout.Controls.Add(buttonPanel, 0, 1);

            _inputControls.Add(_importFilesButton);
            _inputControls.Add(_clearFilesButton);

            return layout;
        }

        private Control BuildFilesSection()
        {
            var group = new GroupBox
            {
                Text = "Lotes de links",
                Dock = DockStyle.Fill,
            };

            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Fill,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _modeAllRadio = new RadioButton { Text = "Convertir todos", AutoSize = true };
            _modeSelectRadio = new RadioButton { Text = "Seleccionar lotes", AutoSize = true };

            var modePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };
            modePanel.Controls.Add(_modeAllRadio);
            modePanel.Controls.Add(_modeSelectRadio);

            _refreshButton = new Button { Text = "Cargar ultimo lote", AutoSize = true };

            var headerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };
            headerPanel.Controls.Add(modePanel);
            headerPanel.Controls.Add(_refreshButton);

            layout.Controls.Add(headerPanel, 0, 0);
            layout.SetColumnSpan(headerPanel, 2);

            _filesList = new ListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.MultiExtended,
            };
            layout.Controls.Add(_filesList, 0, 1);
            layout.SetColumnSpan(_filesList, 2);

            _summaryLabel = new Label { Text = "Archivos: 0 | URLs: 0", AutoSize = true };
            layout.Controls.Add(_summaryLabel, 0, 2);
            layout.SetColumnSpan(_summaryLabel, 2);

            group.Controls.Add(layout);

            _inputControls.Add(_modeAllRadio);
            _inputControls.Add(_modeSelectRadio);
            _inputControls.Add(_refreshButton);
            _inputControls.Add(_filesList);

            return group;
        }

        private Control BuildStoreSection()
        {
            var group = new GroupBox
            {
                Text = "Tienda",
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            var outer = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
            };
            _nameConfigButton = new Button { Text = "Configurar nombre...", AutoSize = true };
            header.Controls.Add(_nameConfigButton);
            outer.Controls.Add(header, 0, 0);

            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = Math.Max(StoresLeft.Length, StoresRight.Length),
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            var radios = new List<RadioButton>();

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            for (var i = 0; i < layout.RowCount; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                if (i < StoresLeft.Length)
                {
                    var rb = new RadioButton { Text = StoresLeft[i], AutoSize = true, Anchor = AnchorStyles.Left };
                    layout.Controls.Add(rb, 0, i);
                    radios.Add(rb);
                }

                if (i < StoresRight.Length)
                {
                    var rb = new RadioButton { Text = StoresRight[i], AutoSize = true, Anchor = AnchorStyles.Left };
                    layout.Controls.Add(rb, 1, i);
                    radios.Add(rb);
                }
            }

            if (radios.Count > 0)
            {
                radios[0].Checked = true;
            }

            outer.Controls.Add(layout, 0, 1);
            group.Controls.Add(outer);

            _storeRadios = radios.ToArray();
            _inputControls.Add(_nameConfigButton);
            _inputControls.AddRange(_storeRadios);

            return group;
        }

        private Control BuildBaseNameSection()
        {
            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var label = new Label
            {
                Text = "Nombre para sitemaps:",
                AutoSize = true,
                Padding = new Padding(0, 6, 6, 0),
            };
            _baseNameText = new TextBox { Dock = DockStyle.Fill };

            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(_baseNameText, 1, 0);

            _inputControls.Add(_baseNameText);

            return layout;
        }

        private Control BuildOutputSection()
        {
            var group = new GroupBox
            {
                Text = "Carpeta destino",
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            var layout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var row = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _outputText = new TextBox { Dock = DockStyle.Fill };
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
            };
            _downloadsButton = new Button { Text = "Descargas", AutoSize = true };
            _desktopButton = new Button { Text = "Escritorio", AutoSize = true };
            _chooseOutputButton = new Button { Text = "Otra...", AutoSize = true };

            buttonPanel.Controls.Add(_downloadsButton);
            buttonPanel.Controls.Add(_desktopButton);
            buttonPanel.Controls.Add(_chooseOutputButton);

            row.Controls.Add(_outputText, 0, 0);
            row.Controls.Add(buttonPanel, 1, 0);

            _zipCheck = new CheckBox { Text = "Exportar como ZIP", AutoSize = true };

            layout.Controls.Add(row, 0, 0);
            layout.Controls.Add(_zipCheck, 0, 1);

            group.Controls.Add(layout);

            _inputControls.Add(_outputText);
            _inputControls.Add(_downloadsButton);
            _inputControls.Add(_desktopButton);
            _inputControls.Add(_chooseOutputButton);
            _inputControls.Add(_zipCheck);

            return group;
        }

        private Control BuildProcessSection()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            _processButton = new Button
            {
                Text = "Procesar",
                AutoSize = true,
                Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
                Padding = new Padding(16, 6, 16, 6),
            };
            panel.Controls.Add(_processButton);

            _inputControls.Add(_processButton);

            return panel;
        }

        private Control BuildHelpSection()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
            };

            _helpButton = new Button
            {
                Text = "Ayuda",
                AutoSize = true,
            };

            panel.Controls.Add(_helpButton);

            _inputControls.Add(_helpButton);

            return panel;
        }

        private void WireEvents()
        {
            _importFilesButton.Click += ImportFilesButton_Click;
            _clearFilesButton.Click += (s, e) => ClearFiles();
            _refreshButton.Click += (s, e) => LoadLastAsinBatcherFiles(true);
            _modeAllRadio.CheckedChanged += (s, e) => UpdateMode();
            _modeSelectRadio.CheckedChanged += (s, e) => UpdateMode();
            _filesList.SelectedIndexChanged += (s, e) => UpdateSummary();
            _downloadsButton.Click += (s, e) => _outputText.Text = GetDownloadsPath();
            _desktopButton.Click += (s, e) => _outputText.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            _chooseOutputButton.Click += ChooseOutputButton_Click;
            _processButton.Click += ProcessButton_Click;
            _nameConfigButton.Click += (s, e) => ShowNameConfigDialog();
            _helpButton.Click += (s, e) => ShowHelp();
            _baseNameText.TextChanged += BaseNameText_TextChanged;
            foreach (var radio in _storeRadios)
            {
                radio.CheckedChanged += StoreRadio_CheckedChanged;
            }
            VisibleChanged += (s, e) =>
            {
                // Auto-load last Asin Batcher output on first show.
                if (Visible && _filesList.Items.Count == 0)
                {
                    LoadLastAsinBatcherFiles(true);
                }
            };
        }

        private void SetDefaults()
        {
            _modeAllRadio.Checked = true;
            _outputText.Text = GetDownloadsPath();
            UpdateMode();
            LoadLastAsinBatcherFiles(true);
        }

        private void UpdateMode()
        {
            var allowSelection = _modeSelectRadio.Checked;
            _filesList.Enabled = allowSelection;
            UpdateSummary();
        }

        private void ImportFilesButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Selecciona archivos de links",
                Filter = "Links (*.txt;*.csv;*.xlsx;*.json)|*.txt;*.csv;*.xlsx;*.json|Todos (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = GetDefaultInputDirectory(),
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    AddFiles(dialog.FileNames, false);
                }
            }
        }

        private void ChooseOutputButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = (_outputText.Text ?? string.Empty).Trim();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _outputText.Text = dialog.SelectedPath;
                }
            }
        }

        private void LoadLastAsinBatcherFiles(bool replace)
        {
            if (!AppState.TryGetLastAsinOutputDir(out var folder))
            {
                if (replace)
                {
                    ClearFiles();
                }
                return;
            }

            LoadFilesFromFolder(folder, replace);
        }

        private void LoadFilesFromFolder(string folder, bool replace)
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                if (replace)
                {
                    ClearFiles();
                }
                return;
            }

            var files = new List<string>();
            foreach (var ext in InputExtensions)
            {
                files.AddRange(Directory.GetFiles(folder, "*" + ext));
            }
            files = files.OrderBy(f => f).ToList();
            AddFiles(files, replace);
        }

        private void AddFiles(IEnumerable<string> files, bool replace)
        {
            if (replace)
            {
                _filesList.Items.Clear();
            }

            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in _filesList.Items.OfType<FileItem>())
            {
                existing.Add(item.FullPath);
            }

            foreach (var file in files ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
                {
                    continue;
                }
                if (!existing.Add(file))
                {
                    continue;
                }
                _filesList.Items.Add(new FileItem(file));
            }

            if (_modeAllRadio.Checked)
            {
                _filesList.ClearSelected();
            }

            UpdateSummary();
        }

        private void ClearFiles()
        {
            _filesList.Items.Clear();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            var files = GetSelectedFiles().ToList();
            if (files.Count == 0)
            {
                _summaryLabel.Text = "Archivos: 0 | URLs: 0";
                return;
            }

            var urlCount = 0;
            foreach (var file in files)
            {
                urlCount += CountUrlsInFile(file);
            }

            _summaryLabel.Text = "Archivos: " + files.Count + " | URLs: " + urlCount;
        }

        private IEnumerable<string> GetSelectedFiles()
        {
            if (_modeAllRadio.Checked)
            {
                foreach (var item in _filesList.Items.OfType<FileItem>())
                {
                    yield return item.FullPath;
                }
                yield break;
            }

            foreach (var item in _filesList.SelectedItems.OfType<FileItem>())
            {
                yield return item.FullPath;
            }
        }

        private static int CountUrlsInFile(string path)
        {
            try
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext == ".xlsx")
                {
                    return 0;
                }
                var content = File.ReadAllText(path);
                return UrlRegex.Matches(content).Count;
            }
            catch
            {
                return 0;
            }
        }

        private string GetDefaultInputDirectory()
        {
            if (AppState.TryGetLastAsinOutputDir(out var folder))
            {
                return folder;
            }
            return GetDownloadsPath();
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            var files = GetSelectedFiles().ToArray();
            if (files.Length == 0)
            {
                MessageBox.Show(this, "No hay archivos seleccionados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var storeName = GetStoreNameForFiles();
            if (string.IsNullOrWhiteSpace(storeName))
            {
                MessageBox.Show(this, "Selecciona una tienda o escribe un nombre.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var outputDir = (_outputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(outputDir))
            {
                outputDir = GetDownloadsPath();
                _outputText.Text = outputDir;
            }

            var request = new SitemapEngineRequest
            {
                InputFiles = files,
                OutputDir = outputDir,
                BaseName = null,
                Store = storeName,
                StoreName = storeName,
                ZipOutput = _zipCheck.Checked,
                NamePrefix1 = _namePrefix1,
                NamePrefix2 = _namePrefix2,
            };

            SetBusy(true);
            var response = await _engineClient.ProcessAsync(request);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo generar los sitemaps.", response);
                return;
            }

            UpdateSummary();

            var message = "Listo!\n";
            if (!string.IsNullOrWhiteSpace(response.ZipPath))
            {
                message += "ZIP creado:\n" + response.ZipPath;
                OpenInExplorer(Path.GetDirectoryName(response.ZipPath));
            }
            else
            {
                message += "Carpeta creada:\n" + response.OutputFolder;
                OpenInExplorer(response.OutputFolder);
            }

            MessageBox.Show(this, message, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            UseWaitCursor = busy;
            foreach (var control in _inputControls)
            {
                control.Enabled = !busy;
            }
            if (!_modeSelectRadio.Checked)
            {
                _filesList.Enabled = false;
            }
        }

        private void ShowNameConfigDialog()
        {
            using (var dialog = new FileNameConfigDialog(_namePrefix1, _namePrefix2))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _namePrefix1 = dialog.Prefix1;
                    _namePrefix2 = dialog.Prefix2;
                }
            }
        }

        private void ShowHelp()
        {
            var msg =
                "Sitemap\n\n" +
                "1) Importa archivos .txt/.csv/.xlsx/.json.\n" +
                "2) Elige modo: Convertir todos o Seleccionar lotes.\n" +
                "3) Elige una tienda o escribe un nombre manual.\n" +
                "4) Configura los prefijos si aplica.\n" +
                "5) Elige carpeta destino (opcional ZIP).\n" +
                "6) Presiona Procesar.";
            MessageBox.Show(this, msg, "Ayuda - Sitemap", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BaseNameText_TextChanged(object sender, EventArgs e)
        {
            if (_suppressStoreSync)
            {
                return;
            }

            var manual = (_baseNameText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(manual))
            {
                if (_storeRadios.Length > 0 && Array.TrueForAll(_storeRadios, r => !r.Checked))
                {
                    _suppressStoreSync = true;
                    _storeRadios[0].Checked = true;
                    _suppressStoreSync = false;
                }
                return;
            }

            _suppressStoreSync = true;
            foreach (var radio in _storeRadios)
            {
                radio.Checked = false;
            }
            _suppressStoreSync = false;
        }

        private void StoreRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressStoreSync)
            {
                return;
            }

            var radio = sender as RadioButton;
            if (radio == null || !radio.Checked)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_baseNameText.Text))
            {
                return;
            }

            _suppressStoreSync = true;
            _baseNameText.Text = string.Empty;
            _suppressStoreSync = false;
        }

        private string GetStoreNameForFiles()
        {
            var manual = (_baseNameText.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(manual))
            {
                return manual;
            }

            foreach (var radio in _storeRadios)
            {
                if (radio.Checked)
                {
                    return radio.Text;
                }
            }
            return StoresLeft[0];
        }

        private void ShowEngineError(string title, SitemapEngineResponse response)
        {
            var message = response.Error ?? "Error desconocido.";
            if (!string.IsNullOrWhiteSpace(response.Traceback))
            {
                message += Environment.NewLine + Environment.NewLine + response.Traceback;
            }
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string GetDownloadsPath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, "Downloads");
        }

        private static void OpenInExplorer(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            var target = path;
            if (File.Exists(path))
            {
                target = Path.GetDirectoryName(path);
            }
            if (string.IsNullOrWhiteSpace(target))
            {
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = target,
                UseShellExecute = true,
            });
        }

        private sealed class FileItem
        {
            public FileItem(string fullPath)
            {
                FullPath = fullPath;
                Name = Path.GetFileName(fullPath);
            }

            public string FullPath { get; }
            public string Name { get; }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
