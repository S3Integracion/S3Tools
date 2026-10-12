// Sitemap tab UI.
// Imports URL batch files and generates WebScraper sitemap JSONs.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S3Tools
{
    internal sealed partial class SitemapControl : UserControl
    {
        private const string TemplateContext = "sitemap";
        private const string SuggestedTemplateHint = "Plantilla sugerida para {0}";
        private const string MissingTemplateHint = "La plantilla anterior ya no existe; se seleccionó «{0}».";
        private static readonly string[] InputExtensions = { ".txt", ".csv", ".xlsx", ".json" };
        private static readonly string[] AsinBatcherExtensions = { ".txt" };
        private static readonly Regex UrlRegex = new Regex("https?://[^\\s\"']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const double UrlsPerHourEstimate = 600d;
        private const double TimeEstimateRangeFactor = 1.5d;

        private readonly SitemapEngineClient _engineClient;
        private readonly List<Control> _inputControls;
        private bool _isBusy;
        private bool _suppressStoreSync;

        /// <summary>Verdadero mientras el combo de plantillas se llena o selecciona por código (no es una elección del usuario).</summary>
        private bool _suppressTemplateSync;

        /// <summary>Repositorio de plantillas; <c>null</c> en el diseñador de WinForms.</summary>
        private SitemapTemplateRepository _repository;

        private RadioButton[] _storeRadios;
        private string _namePrefix1 = string.Empty;
        private string _namePrefix2 = string.Empty;

        public SitemapControl()
        {
            InitializeComponent();
            _engineClient = new SitemapEngineClient();
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
            _storeRadios = new[]
            {
                _storeProductosTxRadio,
                _storeHolaproductoRadio,
                _storeAltinorRadio,
                _storeHervazTradeRadio,
                _storeBbvsTemplateRadio,
                _storeBbvs2daRadio,
                _storeBbvsRadio,
            };

            _inputControls.Add(_importFilesButton);
            _inputControls.Add(_clearFilesButton);
            _inputControls.Add(_modeAllRadio);
            _inputControls.Add(_modeSelectRadio);
            _inputControls.Add(_refreshButton);
            _inputControls.Add(_filesList);
            _inputControls.Add(_nameConfigButton);
            _inputControls.AddRange(_storeRadios);
            _inputControls.Add(_templateCombo);
            _inputControls.Add(_manageTemplatesButton);
            _inputControls.Add(_baseNameText);
            _inputControls.Add(_outputText);
            _inputControls.Add(_downloadsButton);
            _inputControls.Add(_desktopButton);
            _inputControls.Add(_chooseOutputButton);
            _inputControls.Add(_zipCheck);
            _inputControls.Add(_processButton);
            _inputControls.Add(_helpButton);
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
            _templateCombo.SelectedIndexChanged += TemplateCombo_SelectedIndexChanged;
            _manageTemplatesButton.Click += ManageTemplatesButton_Click;
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
            if (_modeSelectRadio.Checked)
            {
                _filesList.SelectionMode = SelectionMode.MultiExtended;
            }
            else
            {
                _filesList.SelectionMode = SelectionMode.None;
                _filesList.ClearSelected();
            }
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

            LoadFilesFromFolder(folder, replace, AsinBatcherExtensions);
        }

        private void LoadFilesFromFolder(string folder, bool replace, IEnumerable<string> extensions)
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
            foreach (var ext in extensions ?? InputExtensions)
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
                UpdateBatchEstimateLabels(0, 0);
                return;
            }

            var urlCount = 0;
            foreach (var file in files)
            {
                urlCount += CountUrlsInFile(file);
            }

            _summaryLabel.Text = "Archivos: " + files.Count + " | URLs: " + urlCount;
            UpdateBatchEstimateLabels(urlCount, files.Count);
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

            var templateId = GetSelectedTemplateId();
            AppState.SetLastTemplateId(TemplateContext, templateId);

            var request = new SitemapEngineRequest
            {
                InputFiles = files,
                OutputDir = outputDir,
                BaseName = null,
                Store = storeName,
                StoreName = storeName,
                ZipOutput = _zipCheck.Checked,
                TemplateId = templateId,
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
            _filesList.Enabled = !busy;
        }

        // ---------------------------------------------------------------- Plantillas

        /// <summary>
        /// Llena el combo y elige la plantilla inicial: la última recordada (si sigue existiendo), si no la
        /// vinculada a la tienda marcada, si no la primera. Se suscribe a los cambios del repositorio.
        /// </summary>
        private void InitializeTemplates(SitemapTemplateRepository repository)
        {
            _repository = repository;
            PopulateTemplateCombo();

            // La elección recordada gana al arrancar; la sugerencia por tienda aplica en los cambios posteriores.
            if (!AppState.TryGetLastTemplateId(TemplateContext, out var rememberedId) || !SelectTemplateItem(rememberedId))
            {
                if (!ApplyStoreSuggestion(GetCheckedStoreRadio()))
                {
                    SelectFirstTemplateItem();
                }
            }

            _repository.Changed += Repository_Changed;
            Disposed += (s, e) => _repository.Changed -= Repository_Changed;
        }

        private string GetSelectedTemplateId()
        {
            return (_templateCombo.SelectedItem as TemplateItem)?.Id;
        }

        /// <summary>Reconstruye los elementos del combo sin disparar los efectos de una elección del usuario.</summary>
        private void PopulateTemplateCombo()
        {
            _suppressTemplateSync = true;
            _templateCombo.BeginUpdate();
            try
            {
                _templateCombo.Items.Clear();
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

        /// <summary>Selecciona por código la plantilla con ese id. Devuelve <c>false</c> si no está en el combo.</summary>
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
                    SetTemplateIndex(i);
                    return true;
                }
            }

            return false;
        }

        private void SelectFirstTemplateItem()
        {
            SetTemplateIndex(_templateCombo.Items.Count > 0 ? 0 : -1);
        }

        private void SetTemplateIndex(int index)
        {
            _suppressTemplateSync = true;
            try
            {
                _templateCombo.SelectedIndex = index;
            }
            finally
            {
                _suppressTemplateSync = false;
            }
        }

        private RadioButton GetCheckedStoreRadio()
        {
            return _storeRadios.FirstOrDefault(r => r.Checked);
        }

        /// <summary>
        /// Si la tienda tiene una plantilla vinculada, la selecciona y muestra la pista «Plantilla sugerida».
        /// Si no, conserva la plantilla y quita la pista (ya no corresponde). Devuelve si hubo sugerencia.
        /// </summary>
        private bool ApplyStoreSuggestion(RadioButton storeRadio)
        {
            var linked = storeRadio == null ? null : _repository.FindByStore(storeRadio.Text);
            if (linked != null && SelectTemplateItem(linked.Id))
            {
                SetTemplateHint(string.Format(CultureInfo.CurrentCulture, SuggestedTemplateHint, storeRadio.Text), false);
                return true;
            }

            SetTemplateHint(null, false);
            return false;
        }

        private void SetTemplateHint(string text, bool warning)
        {
            _templateHintLabel.Text = text ?? string.Empty;
            _templateHintLabel.ForeColor = warning ? SystemColors.ControlText : SystemColors.GrayText;
        }

        /// <summary>Pista «La plantilla anterior ya no existe» con el nombre de la plantilla que quedó seleccionada.</summary>
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
            AppState.SetLastTemplateId(TemplateContext, GetSelectedTemplateId());
        }

        /// <summary>
        /// Repuebla el combo conservando la plantilla elegida; si ya no existe, cae en la vinculada a la
        /// tienda marcada o en la primera, y lo indica con la pista.
        /// </summary>
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

            var previousId = GetSelectedTemplateId();
            PopulateTemplateCombo();
            if (SelectTemplateItem(previousId))
            {
                return;
            }

            var radio = GetCheckedStoreRadio();
            var linked = radio == null ? null : _repository.FindByStore(radio.Text);
            if (linked == null || !SelectTemplateItem(linked.Id))
            {
                SelectFirstTemplateItem();
            }

            if (previousId != null)
            {
                ShowMissingTemplateHint();
            }
        }

        private void ManageTemplatesButton_Click(object sender, EventArgs e)
        {
            if (_repository == null)
            {
                return;
            }

            var previousId = GetSelectedTemplateId();
            string chosenId;
            using (var dialog = new TemplateManagerDialog(_repository, previousId))
            {
                dialog.ShowDialog(this);
                chosenId = dialog.SelectedTemplateId;
            }

            // Solo es una elección si el diálogo terminó en otra plantilla (creada, importada o elegida en la lista).
            if (chosenId == null || chosenId == previousId || !SelectTemplateItem(chosenId))
            {
                return;
            }

            // Si la anterior se eliminó en el diálogo, la pista lo sigue indicando con el nombre de la nueva.
            if (previousId != null && _repository.Find(previousId) == null)
            {
                ShowMissingTemplateHint();
            }
            else
            {
                SetTemplateHint(null, false);
            }

            AppState.SetLastTemplateId(TemplateContext, chosenId);
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
                "4) Elige la plantilla (o adminístralas con «Administrar plantillas…»).\n" +
                "5) Configura los prefijos si aplica.\n" +
                "6) Elige carpeta destino (opcional ZIP).\n" +
                "7) Presiona Procesar.";
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

            if (_repository != null)
            {
                ApplyStoreSuggestion(radio);
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
            return StoreCatalog.StoresLeft[0];
        }

        private void UpdateBatchEstimateLabels(int totalUrls, int batches)
        {
            if (_urlsPerBatchLabel == null || _timeRangeLabel == null)
            {
                return;
            }

            if (totalUrls <= 0 || batches <= 0)
            {
                _urlsPerBatchLabel.Text = "URLs por lote: -";
                _timeRangeLabel.Text = "Rango de tiempo aproximado: -";
                return;
            }

            var urlsPerBatch = totalUrls / (double)batches;
            var approxHours = urlsPerBatch / UrlsPerHourEstimate;
            var approxText = FormatHours(approxHours);
            var rangeText = FormatHours(approxHours * TimeEstimateRangeFactor);

            _urlsPerBatchLabel.Text = "URLs por lote: " + FormatUrlCount(urlsPerBatch);
            _timeRangeLabel.Text = "Rango de tiempo aproximado: " + approxText + " : " + rangeText;
        }

        private static string FormatUrlCount(double value)
        {
            if (Math.Abs(value - Math.Round(value)) < 0.005)
            {
                return ((int)Math.Round(value)).ToString();
            }
            return value.ToString("0.##");
        }

        private static string FormatHours(double hours)
        {
            if (hours < 0)
            {
                hours = 0;
            }

            var totalMinutes = (int)Math.Round(hours * 60, MidpointRounding.AwayFromZero);
            if (totalMinutes < 0)
            {
                totalMinutes = 0;
            }

            var displayHours = totalMinutes / 60;
            var minutes = totalMinutes % 60;
            return displayHours + ":" + minutes.ToString("00") + " hr";
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

        /// <summary>Elemento del combo de plantillas: muestra el nombre y conserva el id.</summary>
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

        private void inputLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
