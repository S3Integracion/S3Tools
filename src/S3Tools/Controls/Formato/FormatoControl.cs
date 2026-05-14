// Formato tab UI.
// Normalizes the first two WebScraper headers in CSV/XLSX files.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S3Tools
{
    internal sealed partial class FormatoControl : UserControl
    {
        private readonly FormatoEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;

        public FormatoControl()
        {
            InitializeComponent();
            _engineClient = new FormatoEngineClient();
            _inputControls = new List<Control>();

            PopulateInputControls();
            WireEvents();
            SetDefaults();
        }

        private void PopulateInputControls()
        {
            _inputControls.Add(_importFilesButton);
            _inputControls.Add(_clearFilesButton);
            _inputControls.Add(_modeAllRadio);
            _inputControls.Add(_modeSelectRadio);
            _inputControls.Add(_headerFormatHyphenRadio);
            _inputControls.Add(_headerFormatUnderscoreRadio);
            _inputControls.Add(_filesList);
            _inputControls.Add(_templateAutoRadio);
            _inputControls.Add(_templateTiendasRadio);
            _inputControls.Add(_templateBbvsRadio);
            _inputControls.Add(_processButton);
            _inputControls.Add(_helpButton);
        }


        private void WireEvents()
        {
            _importFilesButton.Click += ImportFilesButton_Click;
            _clearFilesButton.Click += (s, e) => ClearFiles();
            _modeAllRadio.CheckedChanged += (s, e) => UpdateMode();
            _modeSelectRadio.CheckedChanged += (s, e) => UpdateMode();
            _filesList.SelectedIndexChanged += (s, e) => UpdateSummary();
            _processButton.Click += ProcessButton_Click;
            _helpButton.Click += (s, e) => ShowHelp();
        }

        private void SetDefaults()
        {
            _modeAllRadio.Checked = true;
            _headerFormatUnderscoreRadio.Checked = true;
            _templateAutoRadio.Checked = true;
            UpdateMode();
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
                Title = "Selecciona archivos",
                Filter = "CSV/Excel (*.csv;*.xlsx)|*.csv;*.xlsx|Todos (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = GetDownloadsPath(),
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    AddFiles(dialog.FileNames, false);
                }
            }
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
            _summaryLabel.Text = "Archivos: " + files.Count;
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

        private string GetSelectedTemplate()
        {
            if (_templateTiendasRadio.Checked)
            {
                return "tiendas";
            }
            if (_templateBbvsRadio.Checked)
            {
                return "bbvs";
            }
            return "auto";
        }

        private string GetSelectedHeaderFormat()
        {
            if (_headerFormatHyphenRadio.Checked)
            {
                return "hyphen";
            }
            return "underscore";
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

            var request = new FormatoEngineRequest
            {
                InputFiles = files,
                Template = GetSelectedTemplate(),
                HeaderFormat = GetSelectedHeaderFormat(),
            };

            SetBusy(true);
            var response = await _engineClient.ProcessAsync(request);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudo actualizar los archivos.", response);
                return;
            }

            UpdateSummary();

            var updated = response.UpdatedFiles?.Length ?? 0;
            var message = "Listo!\nArchivos actualizados: " + updated;
            if (response.TemplateCounts != null && response.TemplateCounts.Count > 0)
            {
                var details = response.TemplateCounts
                    .Where(kv => kv.Value > 0)
                    .Select(kv => kv.Key + ": " + kv.Value);
                var detailText = string.Join(", ", details);
                if (!string.IsNullOrWhiteSpace(detailText))
                {
                    message += "\nPlantillas: " + detailText;
                }
            }

            MessageBox.Show(this, message, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            var msg =
                "Formato\n\n" +
                "1) Importa archivos .csv o .xlsx.\n" +
                "2) Elige modo: Procesar todos o Seleccionar archivos.\n" +
                "3) Elige formato de headers: Medio - o Bajo _.\n" +
                "4) Elige plantilla: Auto, Tiendas o BBvs.\n" +
                "5) Presiona Procesar.\n\n" +
                "Se actualizan solo las dos primeras columnas con el formato elegido.";
            MessageBox.Show(this, msg, "Ayuda - Formato", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ShowEngineError(string title, FormatoEngineResponse response)
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

