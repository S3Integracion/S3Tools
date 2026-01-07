// Asin Batcher tab UI.
// Loads ASIN files, previews counts/duplicates, and generates URL batches.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class AsinBatcherControl : UserControl
    {
        private static readonly string[] StoresLeft = { "ProductosTX", "Holaproducto", "Altinor", "HervazTrade" };
        private static readonly string[] StoresRight = { "BBvs_Template", "BBvsBB2_2da", "BBvsBB2" };
        private static readonly string[] Markets = { "MX", "US" };
        private static readonly string[] OrderChoices = { "Ordenado", "Inverso", "Aleatorio" };
        private const int DefaultBatches = 30;

        private readonly AsinBatcherEngineClient _engineClient;
        private readonly Timer _previewTimer;
        private readonly List<Control> _inputControls;
        private int _previewRequestId;
        private int _lastDuplicates;
        private int _lastUnique;
        private string _lastPreviewPath;
        private bool _isBusy;
        private bool _suppressStoreSync;

        private TextBox _inputText;
        private Button _browseButton;
        private TextBox _previewText;
        private Button _exportDuplicatesButton;
        private TextBox _fileNameText;
        private ComboBox _marketCombo;
        private ComboBox _orderCombo;
        private NumericUpDown _batchesNumeric;
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

        public AsinBatcherControl()
        {
            InitializeComponent();
            _engineClient = new AsinBatcherEngineClient();
            // Debounce preview requests while the user edits the input.
            _previewTimer = new Timer { Interval = 350 };
            _previewTimer.Tick += PreviewTimer_Tick;
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
                RowCount = 6,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildInputSection(), 0, 0);
            root.Controls.Add(BuildPreviewSection(), 0, 1);
            root.Controls.Add(BuildOptionsSection(), 0, 2);
            root.Controls.Add(BuildOutputSection(), 0, 3);
            root.Controls.Add(BuildProcessSection(), 0, 4);
            root.Controls.Add(BuildHelpSection(), 0, 5);

            Controls.Add(root);
            ResumeLayout();
        }

        private Control BuildInputSection()
        {
            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = "Archivo de entrada (.txt / .xlsx):",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
            };
            layout.Controls.Add(label, 0, 0);
            layout.SetColumnSpan(label, 2);

            _inputText = new TextBox { Dock = DockStyle.Fill };
            _browseButton = new Button { Text = "Examinar...", AutoSize = true };

            layout.Controls.Add(_inputText, 0, 1);
            layout.Controls.Add(_browseButton, 1, 1);

            _inputControls.Add(_inputText);
            _inputControls.Add(_browseButton);

            return layout;
        }

        private Control BuildPreviewSection()
        {
            var group = new GroupBox
            {
                Text = "Previsualizacion",
                Dock = DockStyle.Fill,
            };

            var layout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _previewText = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
            };
            _exportDuplicatesButton = new Button
            {
                Text = "Exportar duplicados",
                AutoSize = true,
                Enabled = false,
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };
            buttonPanel.Controls.Add(_exportDuplicatesButton);

            layout.Controls.Add(_previewText, 0, 0);
            layout.Controls.Add(buttonPanel, 0, 1);
            group.Controls.Add(layout);

            _inputControls.Add(_exportDuplicatesButton);

            return group;
        }

        private Control BuildOptionsSection()
        {
            var panel = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            panel.Controls.Add(BuildStoreGroup(), 0, 0);
            panel.Controls.Add(BuildFileNameRow(), 0, 1);
            panel.Controls.Add(BuildMarketRow(), 0, 2);

            return panel;
        }

        private Control BuildStoreGroup()
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

private Control BuildFileNameRow()
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
                Text = "Nombre del archivo:",
                AutoSize = true,
                Padding = new Padding(0, 6, 6, 0),
            };
            _fileNameText = new TextBox { Dock = DockStyle.Fill };

            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(_fileNameText, 1, 0);

            _inputControls.Add(_fileNameText);

            return layout;
        }

        private Control BuildMarketRow()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            panel.Controls.Add(new Label { Text = "Mercado:", AutoSize = true, Padding = new Padding(0, 6, 4, 0) });
            _marketCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 70,
            };
            _marketCombo.Items.AddRange(Markets);
            panel.Controls.Add(_marketCombo);

            panel.Controls.Add(new Label { Text = "Lotes:", AutoSize = true, Padding = new Padding(10, 6, 4, 0) });
            _batchesNumeric = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000,
                Width = 80,
                Value = DefaultBatches,
            };
            panel.Controls.Add(_batchesNumeric);

            panel.Controls.Add(new Label { Text = "Orden:", AutoSize = true, Padding = new Padding(10, 6, 4, 0) });
            _orderCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100,
            };
            _orderCombo.Items.AddRange(OrderChoices);
            panel.Controls.Add(_orderCombo);

            _inputControls.Add(_marketCombo);
            _inputControls.Add(_batchesNumeric);
            _inputControls.Add(_orderCombo);

            return panel;
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
            _inputText.TextChanged += (s, e) =>
            {
                if (_isBusy)
                {
                    return;
                }
                _previewTimer.Stop();
                _previewTimer.Start();
            };
            _browseButton.Click += BrowseButton_Click;
            _downloadsButton.Click += (s, e) => _outputText.Text = GetDownloadsPath();
            _desktopButton.Click += (s, e) => _outputText.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            _chooseOutputButton.Click += ChooseOutputButton_Click;
            _exportDuplicatesButton.Click += ExportDuplicatesButton_Click;
            _processButton.Click += ProcessButton_Click;
            _nameConfigButton.Click += (s, e) => ShowNameConfigDialog();
            _helpButton.Click += (s, e) => ShowHelp();
            _fileNameText.TextChanged += FileNameText_TextChanged;
            foreach (var radio in _storeRadios)
            {
                radio.CheckedChanged += StoreRadio_CheckedChanged;
            }
        }

        private void SetDefaults()
        {
            if (_marketCombo.Items.Contains("US"))
            {
                _marketCombo.SelectedItem = "US";
            }
            else if (_marketCombo.Items.Count > 0)
            {
                _marketCombo.SelectedIndex = 0;
            }
            if (_orderCombo.Items.Count > 0)
            {
                _orderCombo.SelectedIndex = 0;
            }
            _batchesNumeric.Value = DefaultBatches;
            _outputText.Text = GetDownloadsPath();
            _previewText.Text = string.Empty;
        }

        private async void PreviewTimer_Tick(object sender, EventArgs e)
        {
            _previewTimer.Stop();
            await RunPreviewAsync();
        }

        private async Task RunPreviewAsync()
        {
            var inputPath = (_inputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
            {
                _lastDuplicates = 0;
                _lastUnique = 0;
                _lastPreviewPath = null;
                _previewText.Text = string.Empty;
                UpdateExportDuplicatesButton();
                return;
            }

            var requestId = ++_previewRequestId;
            _previewText.Text = "Leyendo archivo...";

            var response = await _engineClient.PreviewAsync(inputPath);
            if (requestId != _previewRequestId)
            {
                return;
            }

            if (!response.Ok)
            {
                _lastDuplicates = 0;
                _lastUnique = 0;
                _lastPreviewPath = null;
                _previewText.Text = string.Empty;
                UpdateExportDuplicatesButton();
                ShowEngineError("No se pudo leer el archivo.", response);
                return;
            }

            _previewText.Text = FormatPreview(response);
            _lastDuplicates = response.Duplicates ?? 0;
            _lastUnique = response.Unique ?? 0;
            _lastPreviewPath = inputPath;
            UpdateExportDuplicatesButton();
        }

        private async void ExportDuplicatesButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            var inputPath = (_inputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
            {
                MessageBox.Show(this, "Selecciona un archivo valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var outputDir = (_outputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(outputDir))
            {
                outputDir = GetDownloadsPath();
                _outputText.Text = outputDir;
            }

            SetBusy(true);
            var response = await _engineClient.ExportDuplicatesAsync(inputPath, outputDir);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo exportar duplicados.", response);
                return;
            }

            _lastDuplicates = response.Duplicates ?? 0;
            UpdateExportDuplicatesButton();

            if (_lastDuplicates <= 0)
            {
                MessageBox.Show(this, "No se detectaron duplicados.", "Duplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!string.IsNullOrWhiteSpace(response.CsvPath))
            {
                MessageBox.Show(this, "CSV de duplicados creado:\n" + response.CsvPath, "Duplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenInExplorer(Path.GetDirectoryName(response.CsvPath));
            }
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            var inputPath = (_inputText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
            {
                MessageBox.Show(this, "Selecciona un archivo valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            var requestedBatches = (int)_batchesNumeric.Value;
            if (!await ValidateBatchCountAsync(inputPath, requestedBatches))
            {
                return;
            }

            var request = new EngineRequest
            {
                InputPath = inputPath,
                OutputDir = outputDir,
                Market = _marketCombo.SelectedItem as string,
                Store = storeName,
                StoreName = storeName,
                Order = _orderCombo.SelectedItem as string,
                Batches = requestedBatches,
                ZipOutput = _zipCheck.Checked,
                FileLabel = null,
                NamePrefix1 = _namePrefix1,
                NamePrefix2 = _namePrefix2,
            };

            SetBusy(true);
            var response = await _engineClient.ProcessAsync(request);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo procesar el archivo.", response);
                return;
            }

            _lastDuplicates = response.Duplicates ?? 0;
            _previewText.Text = FormatPreview(response);
            UpdateExportDuplicatesButton();

            if (!string.IsNullOrWhiteSpace(response.OutputFolder) && Directory.Exists(response.OutputFolder))
            {
                // Persist output folder for Sitemap defaults.
                AppState.SetLastAsinOutputDir(response.OutputFolder);
            }

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

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Selecciona archivo",
                Filter = "TXT (*.txt)|*.txt|Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Todos (*.*)|*.*",
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _inputText.Text = dialog.FileName;
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
                "ASIN Batcher\n\n" +
                "1) Selecciona un archivo .txt o .xlsx.\n" +
                "2) Revisa la previsualizacion (totales, unicos, duplicados).\n" +
                "3) Elige una tienda o escribe un nombre manual.\n" +
                "4) Configura los prefijos si aplica.\n" +
                "5) Define Mercado, Lotes y Orden.\n" +
                "6) Elige carpeta destino (opcional ZIP).\n" +
                "7) Presiona Procesar.\n\n" +
                "Extra: Exportar duplicados crea un CSV si hay repetidos.";
            MessageBox.Show(this, msg, "Ayuda - Asin Batcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FileNameText_TextChanged(object sender, EventArgs e)
        {
            if (_suppressStoreSync)
            {
                return;
            }

            var manual = (_fileNameText.Text ?? string.Empty).Trim();
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

            if (string.IsNullOrWhiteSpace(_fileNameText.Text))
            {
                return;
            }

            _suppressStoreSync = true;
            _fileNameText.Text = string.Empty;
            _suppressStoreSync = false;
        }

        private async Task<bool> ValidateBatchCountAsync(string inputPath, int batches)
        {
            if (!string.Equals(_lastPreviewPath, inputPath, StringComparison.OrdinalIgnoreCase))
            {
                SetBusy(true);
                var preview = await _engineClient.PreviewAsync(inputPath);
                SetBusy(false);

                if (!preview.Ok)
                {
                    ShowEngineError("No se pudo leer el archivo.", preview);
                    return false;
                }

                _previewText.Text = FormatPreview(preview);
                _lastDuplicates = preview.Duplicates ?? 0;
                _lastUnique = preview.Unique ?? 0;
                _lastPreviewPath = inputPath;
                UpdateExportDuplicatesButton();
            }

            if (_lastUnique > 0 && batches > _lastUnique)
            {
                MessageBox.Show(
                    this,
                    "La cantidad de lotes no puede ser mayor que la cantidad de URLs.\n" +
                    "URLs: " + _lastUnique + "\n" +
                    "Lotes: " + batches,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            UseWaitCursor = busy;
            foreach (var control in _inputControls)
            {
                if (control == _exportDuplicatesButton)
                {
                    control.Enabled = !busy && _lastDuplicates > 0;
                    continue;
                }
                control.Enabled = !busy;
            }
        }

        private void UpdateExportDuplicatesButton()
        {
            _exportDuplicatesButton.Enabled = !_isBusy && _lastDuplicates > 0;
        }

        private string GetStoreNameForFiles()
        {
            var manual = (_fileNameText.Text ?? string.Empty).Trim();
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

        private static string FormatPreview(EngineResponse response)
        {
            var total = response.Total ?? 0;
            var unique = response.Unique ?? 0;
            var duplicates = response.Duplicates ?? 0;
            return "ASIN totales (incl. duplicados): " + total + Environment.NewLine +
                   "Unicos: " + unique + Environment.NewLine +
                   "Duplicados: " + duplicates;
        }

        private void ShowEngineError(string title, EngineResponse response)
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
    }
}
