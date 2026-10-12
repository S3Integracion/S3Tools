// Formato tab UI.
// Normalizes the first two WebScraper headers in CSV/XLSX files.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S3Tools
{
    internal sealed partial class FormatoControl : UserControl
    {
        private const string TemplateContext = "formato";
        private const string AutoTemplateName = "Detección automática";
        private const string MissingTemplateHint = "La plantilla anterior ya no existe; se seleccionó «{0}».";

        private readonly FormatoEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;

        /// <summary>Verdadero mientras el combo de plantillas se llena o selecciona por código (no es una elección del usuario).</summary>
        private bool _suppressTemplateSync;

        /// <summary>Repositorio de plantillas; <c>null</c> en el diseñador de WinForms.</summary>
        private SitemapTemplateRepository _repository;

        public FormatoControl()
        {
            InitializeComponent();
            _engineClient = new FormatoEngineClient();
            _inputControls = new List<Control>();

            PopulateInputControls();
            WireEvents();
            SetDefaults();

            // En tiempo de diseño no se toca el repositorio: cargarlo escribiría en %LocalAppData%.
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                InitializeTemplates(SitemapTemplateRepository.Default);
            }
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
            _inputControls.Add(_templateCombo);
            _inputControls.Add(_manageTemplatesButton);
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
            _templateCombo.SelectedIndexChanged += TemplateCombo_SelectedIndexChanged;
            _manageTemplatesButton.Click += ManageTemplatesButton_Click;
        }

        private void SetDefaults()
        {
            _modeAllRadio.Checked = true;
            _headerFormatUnderscoreRadio.Checked = true;
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

        // ---------------------------------------------------------------- Plantillas

        /// <summary>
        /// Llena el combo («Detección automática» y luego las plantillas) y elige la última recordada si
        /// sigue existiendo; si no, la detección automática. Se suscribe a los cambios del repositorio.
        /// </summary>
        private void InitializeTemplates(SitemapTemplateRepository repository)
        {
            _repository = repository;
            PopulateTemplateCombo();

            if (!AppState.TryGetLastTemplateId(TemplateContext, out var rememberedId) || !SelectTemplateItem(rememberedId))
            {
                SelectTemplateItem(TemplateResolver.AutoChoice);
            }

            _repository.Changed += Repository_Changed;
            Disposed += (s, e) => _repository.Changed -= Repository_Changed;
        }

        /// <summary>Id de la plantilla elegida o <c>"auto"</c>; es el valor que espera <see cref="FormatoEngine"/>.</summary>
        private string GetSelectedTemplate()
        {
            return (_templateCombo.SelectedItem as TemplateItem)?.Id ?? TemplateResolver.AutoChoice;
        }

        /// <summary>Reconstruye los elementos del combo sin disparar los efectos de una elección del usuario.</summary>
        private void PopulateTemplateCombo()
        {
            _suppressTemplateSync = true;
            _templateCombo.BeginUpdate();
            try
            {
                _templateCombo.Items.Clear();
                _templateCombo.Items.Add(new TemplateItem(TemplateResolver.AutoChoice, AutoTemplateName));
                foreach (var template in _repository.GetAll())
                {
                    _templateCombo.Items.Add(new TemplateItem(template.Id, template.Name));
                }
            }
            finally
            {
                _templateCombo.EndUpdate();
                _suppressTemplateSync = false;
            }
        }

        /// <summary>Selecciona por código el elemento con ese id. Devuelve <c>false</c> si no está en el combo.</summary>
        private bool SelectTemplateItem(string id)
        {
            if (id == null)
            {
                return false;
            }

            for (var i = 0; i < _templateCombo.Items.Count; i++)
            {
                if (((TemplateItem)_templateCombo.Items[i]).Id == id)
                {
                    _suppressTemplateSync = true;
                    try
                    {
                        _templateCombo.SelectedIndex = i;
                    }
                    finally
                    {
                        _suppressTemplateSync = false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void SetTemplateHint(string text, bool warning)
        {
            _templateHintLabel.Text = text ?? string.Empty;
            _templateHintLabel.ForeColor = warning ? SystemColors.ControlText : SystemColors.GrayText;
        }

        /// <summary>Pista «La plantilla anterior ya no existe» con el nombre del elemento que quedó seleccionado.</summary>
        private void ShowMissingTemplateHint()
        {
            var item = _templateCombo.SelectedItem as TemplateItem;
            SetTemplateHint(item == null ? null : string.Format(CultureInfo.CurrentCulture, MissingTemplateHint, item.Name), true);
        }

        private void TemplateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTemplateSync)
            {
                return;
            }

            SetTemplateHint(null, false);
            AppState.SetLastTemplateId(TemplateContext, GetSelectedTemplate());
        }

        /// <summary>Repuebla el combo conservando la elección; si la plantilla ya no existe, vuelve a la detección automática.</summary>
        private void Repository_Changed(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Repository_Changed(sender, e)));
                return;
            }

            var previousId = GetSelectedTemplate();
            PopulateTemplateCombo();
            if (!SelectTemplateItem(previousId))
            {
                SelectTemplateItem(TemplateResolver.AutoChoice);
                ShowMissingTemplateHint();
            }
        }

        private void ManageTemplatesButton_Click(object sender, EventArgs e)
        {
            if (_repository == null)
            {
                return;
            }

            var previousId = GetSelectedTemplate();
            var isAuto = previousId == TemplateResolver.AutoChoice;

            // Con «Detección automática» el diálogo abre en la primera plantilla; esa no cuenta como elección.
            var shownId = isAuto ? _repository.GetAll().FirstOrDefault()?.Id : previousId;
            string chosenId;
            using (var dialog = new TemplateManagerDialog(_repository, isAuto ? null : previousId))
            {
                dialog.ShowDialog(this);
                chosenId = dialog.SelectedTemplateId;
            }

            // Es una elección solo si el usuario terminó en otra plantilla; si la mostrada al abrir se eliminó
            // estando en automático, el diálogo pasó a la vecina por sí solo y se conserva la detección automática.
            if (chosenId == null || chosenId == shownId || (isAuto && _repository.Find(shownId) == null) || !SelectTemplateItem(chosenId))
            {
                return;
            }

            if (!isAuto && _repository.Find(previousId) == null)
            {
                ShowMissingTemplateHint();
            }
            else
            {
                SetTemplateHint(null, false);
            }

            AppState.SetLastTemplateId(TemplateContext, chosenId);
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

            var template = GetSelectedTemplate();
            AppState.SetLastTemplateId(TemplateContext, template);

            var request = new FormatoEngineRequest
            {
                InputFiles = files,
                Template = template,
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
                "4) Elige la plantilla: Detección automática o una específica (o adminístralas con «Administrar plantillas…»).\n" +
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

        /// <summary>Elemento del combo de plantillas: muestra el nombre y conserva el id (o <c>"auto"</c>).</summary>
        private sealed class TemplateItem
        {
            public TemplateItem(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id { get; }

            public string Name { get; }

            public override string ToString()
            {
                return Name;
            }
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

