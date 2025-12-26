// Formato tab UI.
// Normalizes the first two WebScraper headers in CSV/XLSX files.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class FormatoControl : UserControl
    {
        private readonly FormatoEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;

        private Button _importFilesButton;
        private Button _clearFilesButton;
        private RadioButton _modeAllRadio;
        private RadioButton _modeSelectRadio;
        private ListBox _filesList;
        private Label _summaryLabel;
        private RadioButton _templateAutoRadio;
        private RadioButton _templateTiendasRadio;
        private RadioButton _templateBbvsRadio;
        private Button _processButton;
        private Label _noteLabel;

        public FormatoControl()
        {
            InitializeComponent();
            _engineClient = new FormatoEngineClient();
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
                RowCount = 4,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 60f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildInputSection(), 0, 0);
            root.Controls.Add(BuildFilesSection(), 0, 1);
            root.Controls.Add(BuildTemplateSection(), 0, 2);
            root.Controls.Add(BuildProcessSection(), 0, 3);

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
                Text = "Archivos de entrada (.csv / .xlsx):",
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
                Text = "Archivos",
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

            _modeAllRadio = new RadioButton { Text = "Procesar todos", AutoSize = true };
            _modeSelectRadio = new RadioButton { Text = "Seleccionar archivos", AutoSize = true };

            var modePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };
            modePanel.Controls.Add(_modeAllRadio);
            modePanel.Controls.Add(_modeSelectRadio);

            layout.Controls.Add(modePanel, 0, 0);
            layout.SetColumnSpan(modePanel, 2);

            _filesList = new ListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.MultiExtended,
            };
            layout.Controls.Add(_filesList, 0, 1);
            layout.SetColumnSpan(_filesList, 2);

            _summaryLabel = new Label { Text = "Archivos: 0", AutoSize = true };
            layout.Controls.Add(_summaryLabel, 0, 2);
            layout.SetColumnSpan(_summaryLabel, 2);

            group.Controls.Add(layout);

            _inputControls.Add(_modeAllRadio);
            _inputControls.Add(_modeSelectRadio);
            _inputControls.Add(_filesList);

            return group;
        }

        private Control BuildTemplateSection()
        {
            var group = new GroupBox
            {
                Text = "Plantilla",
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            _templateAutoRadio = new RadioButton { Text = "Auto", AutoSize = true };
            _templateTiendasRadio = new RadioButton { Text = "Tiendas", AutoSize = true };
            _templateBbvsRadio = new RadioButton { Text = "BBvs", AutoSize = true };

            panel.Controls.Add(_templateAutoRadio);
            panel.Controls.Add(_templateTiendasRadio);
            panel.Controls.Add(_templateBbvsRadio);
            group.Controls.Add(panel);

            _inputControls.Add(_templateAutoRadio);
            _inputControls.Add(_templateTiendasRadio);
            _inputControls.Add(_templateBbvsRadio);

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
            _noteLabel = new Label
            {
                Text = "Solo se corrigen las dos primeras columnas; se actualiza en la misma carpeta.",
                AutoSize = true,
                Padding = new Padding(10, 8, 0, 0),
            };

            panel.Controls.Add(_processButton);
            panel.Controls.Add(_noteLabel);

            _inputControls.Add(_processButton);

            return panel;
        }

        private void WireEvents()
        {
            _importFilesButton.Click += ImportFilesButton_Click;
            _clearFilesButton.Click += (s, e) => ClearFiles();
            _modeAllRadio.CheckedChanged += (s, e) => UpdateMode();
            _modeSelectRadio.CheckedChanged += (s, e) => UpdateMode();
            _filesList.SelectedIndexChanged += (s, e) => UpdateSummary();
            _processButton.Click += ProcessButton_Click;
        }

        private void SetDefaults()
        {
            _modeAllRadio.Checked = true;
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

