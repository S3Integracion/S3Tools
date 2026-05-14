using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Tools
{
    internal sealed partial class AsinNoReportControl : UserControl
    {
        private readonly AsinNoReportEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;
        private string[] _lastMissingAsins = Array.Empty<string>();

        public AsinNoReportControl()
        {
            InitializeComponent();
            _engineClient = new AsinNoReportEngineClient();
            _inputControls = new List<Control>();

            PopulateInputControls();
            WireEvents();
            SetDefaults();
        }

        private void PopulateInputControls()
        {
            _inputControls.Add(_baseFileText);
            _inputControls.Add(_browseBaseButton);
            _inputControls.Add(_sheetCombo);
            _inputControls.Add(_reloadSheetsButton);
            _inputControls.Add(_importReportsButton);
            _inputControls.Add(_clearReportsButton);
            _inputControls.Add(_reportsList);
            _inputControls.Add(_analyzeButton);
            _inputControls.Add(_copyButton);
            _inputControls.Add(_exportButton);
            _inputControls.Add(_helpButton);
        }

        private void WireEvents()
        {
            _browseBaseButton.Click += BrowseBaseButton_Click;
            _reloadSheetsButton.Click += async (s, e) => await LoadSheetsAsync();
            _baseFileText.TextChanged += async (s, e) => await LoadSheetsAsync();
            _importReportsButton.Click += ImportReportsButton_Click;
            _clearReportsButton.Click += (s, e) =>
            {
                _reportsList.Items.Clear();
                UpdateReportsSummary();
            };
            _analyzeButton.Click += AnalyzeButton_Click;
            _copyButton.Click += (s, e) => CopyResultToClipboard();
            _exportButton.Click += (s, e) => ExportResultToTxt();
            _helpButton.Click += (s, e) => ShowHelp();
        }

        private void SetDefaults()
        {
            _summaryText.Text = "Importa archivo base y reportes para iniciar el análisis.";
            _resultText.Text = string.Empty;
            _sheetCombo.Enabled = false;
            _reloadSheetsButton.Enabled = false;
            UpdateReportsSummary();
            UpdateResultActions();
        }

        private async void BrowseBaseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Selecciona archivo base",
                Filter = "Base (.csv; .xlsx)|*.csv;*.xlsx|Todos (*.*)|*.*",
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _baseFileText.Text = dialog.FileName;
                    await LoadSheetsAsync();
                }
            }
        }

        private async Task LoadSheetsAsync()
        {
            if (_isBusy)
            {
                return;
            }

            var path = (_baseFileText.Text ?? string.Empty).Trim();
            var ext = Path.GetExtension(path).ToLowerInvariant();

            _sheetCombo.Items.Clear();
            _sheetCombo.Enabled = ext == ".xlsx";
            _reloadSheetsButton.Enabled = ext == ".xlsx";

            if (ext != ".xlsx" || !File.Exists(path))
            {
                return;
            }

            SetBusy(true);
            var response = await _engineClient.ListSheetsAsync(path);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo leer hojas del archivo base.", response.Error, response.Traceback);
                return;
            }

            foreach (var sheet in response.Sheets ?? Array.Empty<string>())
            {
                _sheetCombo.Items.Add(sheet);
            }

            if (_sheetCombo.Items.Count > 0)
            {
                _sheetCombo.SelectedIndex = 0;
            }
        }

        private void ImportReportsButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Selecciona reportes Amazon",
                Filter = "Reportes TXT (*.txt)|*.txt|Todos (*.*)|*.*",
                Multiselect = true,
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var existing = new HashSet<string>(
                    _reportsList.Items.OfType<FileItem>().Select(x => x.FullPath),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var file in dialog.FileNames)
                {
                    if (!File.Exists(file) || !existing.Add(file))
                    {
                        continue;
                    }
                    _reportsList.Items.Add(new FileItem(file));
                }

                UpdateReportsSummary();
            }
        }

        private async void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            var basePath = (_baseFileText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(basePath) || !File.Exists(basePath))
            {
                MessageBox.Show(this, "Selecciona un archivo base válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var reportPaths = _reportsList.Items
                .OfType<FileItem>()
                .Select(x => x.FullPath)
                .ToArray();

            if (reportPaths.Length == 0)
            {
                MessageBox.Show(this, "Debes importar al menos un reporte .txt.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var request = new AsinNoReportCompareRequest
            {
                BaseFilePath = basePath,
                BaseSheetName = _sheetCombo.Enabled ? _sheetCombo.SelectedItem as string : string.Empty,
                ReportPaths = reportPaths,
            };

            SetBusy(true);
            var response = await _engineClient.CompareAsync(request);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo completar el análisis.", response.Error, response.Traceback);
                return;
            }

            _lastMissingAsins = response.MissingAsins ?? Array.Empty<string>();
            _resultText.Text = response.MissingAsinsText ?? string.Empty;
            _summaryText.Text =
                "Filas con ASIN en base: " + response.BaseRowsWithAsin + Environment.NewLine +
                "ASIN únicos en base: " + response.BaseUniqueAsins + Environment.NewLine +
                "ASIN únicos en reportes: " + response.ReportsUniqueAsins + Environment.NewLine +
                "ASIN encontrados en reportes: " + response.FoundInReports + Environment.NewLine +
                "ASIN NO encontrados en reportes: " + response.MissingAsinsCount;

            UpdateResultActions();
        }

        private void CopyResultToClipboard()
        {
            if (string.IsNullOrWhiteSpace(_resultText.Text))
            {
                return;
            }

            try
            {
                Clipboard.SetText(_resultText.Text);
                MessageBox.Show(this, "Resultado copiado al portapapeles.", "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportResultToTxt()
        {
            if (string.IsNullOrWhiteSpace(_resultText.Text))
            {
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Title = "Guardar ASINs no encontrados",
                Filter = "TXT (*.txt)|*.txt|Todos (*.*)|*.*",
                FileName = "asins_no_en_reportes_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt",
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                File.WriteAllText(dialog.FileName, _resultText.Text);
                MessageBox.Show(this, "Archivo exportado:\n" + dialog.FileName, "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateReportsSummary()
        {
            _reportsSummaryLabel.Text = "Reportes: " + _reportsList.Items.Count;
        }

        private void UpdateResultActions()
        {
            var hasResult = _lastMissingAsins.Length > 0 && !string.IsNullOrWhiteSpace(_resultText.Text);
            _copyButton.Enabled = !_isBusy && hasResult;
            _exportButton.Enabled = !_isBusy && hasResult;
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            UseWaitCursor = busy;
            foreach (var control in _inputControls)
            {
                if (control == _copyButton || control == _exportButton)
                {
                    continue;
                }
                control.Enabled = !busy;
            }
            _sheetCombo.Enabled = !busy && string.Equals(Path.GetExtension((_baseFileText.Text ?? string.Empty).Trim()), ".xlsx", StringComparison.OrdinalIgnoreCase);
            _reloadSheetsButton.Enabled = _sheetCombo.Enabled;
            UpdateResultActions();
        }

        private void ShowEngineError(string title, string error, string traceback)
        {
            var message = error ?? "Error desconocido.";
            if (!string.IsNullOrWhiteSpace(traceback))
            {
                message += Environment.NewLine + Environment.NewLine + traceback;
            }

            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowHelp()
        {
            var msg =
                "Asin no Report\n\n" +
                "1) Importa un archivo base (.csv o .xlsx).\n" +
                "2) Si es .xlsx, elige la hoja a analizar.\n" +
                "3) Importa uno o varios reportes Amazon (.txt).\n" +
                "4) Presiona Analizar.\n" +
                "5) Copia o exporta el listado de ASINs que están en base, pero no aparecen en los reportes.";

            MessageBox.Show(this, msg, "Ayuda - Asin no Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
