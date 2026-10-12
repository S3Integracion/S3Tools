using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>
    /// Panel para administrar las plantillas de sitemap: lista, editor con validación en vivo y acciones
    /// (nueva, duplicar, importar, exportar, eliminar, restaurar predeterminadas). Se usa en el diálogo
    /// <see cref="TemplateManagerDialog"/> y como pestaña de la ventana principal.
    /// </summary>
    internal sealed partial class TemplateManagerControl : UserControl
    {
        private const string Source = nameof(TemplateManagerControl);
        private const string BuiltInSuffix = " · Predeterminada";
        private const string DeleteMessage = "¿Eliminar la plantilla «{0}»? Esta acción no se puede deshacer.";
        private const string RestoreMessage = "Se restaurarán las plantillas predeterminadas (Tiendas y BBvs) a su contenido original. Tus plantillas personalizadas no se modifican. ¿Continuar?";
        private const string UnsavedMessage = "La plantilla «{0}» tiene cambios sin guardar. ¿Deseas guardarlos?";
        private const string MoveStoreMessage = "La tienda {0} está vinculada a «{1}». ¿Vincularla a esta plantilla?";
        private const string FutureSchemaBanner = "El archivo de plantillas fue creado por una versión más reciente de S3Tools. Las plantillas están en modo de solo lectura.";
        private const string IoErrorBanner = "No se pudo leer el archivo de plantillas. Las plantillas están en modo de solo lectura hasta reiniciar S3Tools.";
        private const string JsonFilter = "JSON (*.json)|*.json";

        private readonly SitemapTemplateRepository _repository;
        private readonly TemplateEditorState _state = new TemplateEditorState();
        private readonly System.Windows.Forms.Timer _validationTimer = new System.Windows.Forms.Timer { Interval = 300 };
        private readonly ToolTip _toolTip = new ToolTip();
        private readonly Font _editorFont = new Font("Consolas", 10f);
        private readonly bool _readOnly;

        private TemplateValidationResult _validation;
        private TemplateIssue _statusTarget;

        /// <summary>Verdadero mientras se cargan valores en los campos (los eventos de cambio no deben tocar el estado).</summary>
        private bool _loading;

        /// <summary>Verdadero mientras se cambia la selección de la lista por código (no es una navegación del usuario).</summary>
        private bool _suppressSelection;

        /// <summary>Evita navegaciones anidadas mientras se muestra el aviso de cambios sin guardar.</summary>
        private bool _navigating;

        public TemplateManagerControl(SitemapTemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _readOnly = repository.IsReadOnly;

            BuildLayout();
            ShowBanners();
            ApplyReadOnly();

            _validationTimer.Tick += (s, e) => RunValidation();
            ReloadList();
            ShowFirstTemplate();

            _repository.Changed += OnRepositoryChanged;
        }

        /// <summary>Verdadero cuando el editor tiene cambios sin guardar.</summary>
        public bool HasUnsavedChanges
        {
            get { return _state.IsDirty; }
        }

        /// <summary>Id de la plantilla mostrada en el editor; <c>null</c> si es una plantilla nueva sin guardar.</summary>
        public string SelectedTemplateId
        {
            get { return _state.IsNew ? null : _state.EditingId; }
        }

        /// <summary>Botón «Guardar», para usarlo como <see cref="Form.AcceptButton"/> del formulario anfitrión.</summary>
        public IButtonControl DefaultButton
        {
            get { return _saveButton; }
        }

        /// <summary>
        /// Si hay cambios sin guardar, pregunta Sí / No / Cancelar. Sí guarda (si falla, no se puede salir);
        /// No descarta los cambios. Devuelve <c>true</c> si se puede salir.
        /// </summary>
        public bool ConfirmLeave()
        {
            if (!_state.IsDirty)
            {
                return true;
            }

            var answer = MessageBox.Show(
                this,
                string.Format(CultureInfo.CurrentCulture, UnsavedMessage, DisplayName()),
                "Cambios sin guardar",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);
            switch (answer)
            {
                case DialogResult.Yes:
                    return SaveChanges();
                case DialogResult.No:
                    DiscardChanges();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Muestra la plantilla indicada (preguntando por cambios sin guardar). No hace nada si el id no existe.</summary>
        public void SelectTemplate(string id)
        {
            if (_repository.Find(id) != null)
            {
                NavigateTo(id);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                FlushValidation();
                if (_saveButton.Enabled)
                {
                    SaveChanges();
                }

                return true;
            }

            if (keyData == (Keys.Control | Keys.N))
            {
                if (_newButton.Enabled)
                {
                    CreateNew();
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Changed -= OnRepositoryChanged;
                _validationTimer.Dispose();
                _toolTip.Dispose();
                _editorFont.Dispose();
            }

            base.Dispose(disposing);
        }

        // ---------------------------------------------------------------- Lista y navegación

        private void ReloadList()
        {
            _suppressSelection = true;
            _list.BeginUpdate();
            try
            {
                _list.Items.Clear();
                foreach (var template in _repository.GetAll())
                {
                    var text = template.IsBuiltIn ? template.Name + BuiltInSuffix : template.Name;
                    var stores = string.Join(", ", template.Stores);
                    var item = new ListViewItem(text) { Tag = template.Id, ToolTipText = stores.Length == 0 ? text : text + Environment.NewLine + stores };
                    item.SubItems.Add(SitemapTemplateValidator.Validate(template.SitemapJson).Selectors.Count.ToString(CultureInfo.CurrentCulture));
                    item.SubItems.Add(stores);
                    _list.Items.Add(item);
                }
            }
            finally
            {
                _list.EndUpdate();
                _suppressSelection = false;
            }

            SelectListItem(SelectedTemplateId);
            RefreshStoreItems();
        }

        /// <summary>Selecciona en la lista el elemento con ese id (o ninguno) sin disparar navegación.</summary>
        private void SelectListItem(string id)
        {
            _suppressSelection = true;
            try
            {
                foreach (ListViewItem item in _list.Items)
                {
                    var match = id != null && (string)item.Tag == id;
                    item.Selected = match;
                    if (match)
                    {
                        item.Focused = true;
                        item.EnsureVisible();
                    }
                }
            }
            finally
            {
                _suppressSelection = false;
            }
        }

        private void OnListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (_suppressSelection || !e.IsSelected)
            {
                return;
            }

            // Se difiere hasta que la lista termine de procesar el clic o la tecla, para poder revertir la selección.
            var id = (string)e.Item.Tag;
            if (IsHandleCreated)
            {
                BeginInvoke(new Action(() => NavigateTo(id)));
            }
            else
            {
                NavigateTo(id);
            }
        }

        private void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                e.Handled = true;
                if (_deleteButton.Enabled)
                {
                    DeleteCurrent();
                }
            }
        }

        private void NavigateTo(string id)
        {
            if (_navigating || IsDisposed || (!_state.IsNew && id == _state.EditingId))
            {
                return;
            }

            _navigating = true;
            try
            {
                if (!ConfirmLeave())
                {
                    SelectListItem(SelectedTemplateId);
                    return;
                }

                var template = _repository.Find(id);
                if (template == null)
                {
                    ReloadList();
                    return;
                }

                ShowTemplate(template);
            }
            finally
            {
                _navigating = false;
            }
        }

        private void ShowTemplate(SitemapTemplate template)
        {
            _state.Load(template);
            PushStateToFields();
            SelectListItem(template.Id);
        }

        private void ShowFirstTemplate()
        {
            var first = _repository.GetAll().FirstOrDefault();
            if (first != null)
            {
                ShowTemplate(first);
            }
            else
            {
                ShowNewTemplate();
            }
        }

        private void ShowNewTemplate()
        {
            _state.LoadNew();
            PushStateToFields();
            SelectListItem(null);
        }

        private void OnRepositoryChanged(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnRepositoryChanged(sender, e)));
                return;
            }

            // Con cambios sin guardar (o una plantilla nueva) solo se refresca la lista; el editor se conserva.
            if (_state.IsDirty || _state.IsNew)
            {
                ReloadList();
                UpdateCommands();
                return;
            }

            var current = _repository.Find(_state.EditingId);
            ReloadList();
            if (current != null)
            {
                ShowTemplate(current);
            }
            else
            {
                ShowFirstTemplate();
            }
        }

        // ---------------------------------------------------------------- Editor

        /// <summary>Copia el estado a los campos sin marcar cambios y valida de inmediato.</summary>
        private void PushStateToFields()
        {
            _loading = true;
            try
            {
                _nameBox.Text = _state.Name;
                _descriptionBox.Text = _state.Description;
                _jsonBox.Text = _state.SitemapJson;
                _jsonBox.Select(0, 0);
                RefreshStoreItems();
            }
            finally
            {
                _loading = false;
            }

            RunValidation();
        }

        /// <summary>Reconstruye la lista de tiendas: marcas según el estado y «(en: …)» para las vinculadas a otra plantilla.</summary>
        private void RefreshStoreItems()
        {
            var wasLoading = _loading;
            _loading = true;
            _storesList.BeginUpdate();
            try
            {
                _storesList.Items.Clear();
                foreach (var store in StoreCatalog.StoresLeft.Concat(StoreCatalog.StoresRight))
                {
                    var owner = _repository.FindByStore(store);
                    var ownerName = owner == null || owner.Id == _state.EditingId ? null : owner.Name;
                    var isChecked = _state.Stores.Contains(store, StringComparer.OrdinalIgnoreCase);
                    _storesList.Items.Add(new StoreItem(store, ownerName), isChecked);
                }
            }
            finally
            {
                _storesList.EndUpdate();
                _loading = wasLoading;
            }
        }

        private void OnNameChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _state.Name = _nameBox.Text;
                UpdateCommands();
            }
        }

        private void OnDescriptionChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _state.Description = _descriptionBox.Text;
                UpdateCommands();
            }
        }

        private void OnJsonChanged(object sender, EventArgs e)
        {
            if (!_loading)
            {
                _state.SitemapJson = _jsonBox.Text;
                _validationTimer.Stop();
                _validationTimer.Start();
                UpdateCommands();
            }
        }

        private void OnStoreItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading)
            {
                return;
            }

            // ItemCheck llega antes de aplicar la marca: se usa NewValue para el elemento que cambia.
            var stores = new List<string>();
            for (var i = 0; i < _storesList.Items.Count; i++)
            {
                var isChecked = i == e.Index ? e.NewValue == CheckState.Checked : _storesList.GetItemChecked(i);
                if (isChecked)
                {
                    stores.Add(((StoreItem)_storesList.Items[i]).Store);
                }
            }

            _state.Stores = stores;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            var dirty = _state.IsDirty;
            var valid = _validation.IsValid;
            _newButton.Enabled = !_readOnly;
            _importButton.Enabled = !_readOnly;
            _restoreLink.Enabled = !_readOnly;
            _duplicateButton.Enabled = !_readOnly && !_state.IsNew;
            _deleteButton.Enabled = !_readOnly && !_state.IsNew && _list.Items.Count > 1;
            _formatButton.Enabled = !_readOnly && _validation.Sitemap != null;
            _exportButton.Enabled = valid;
            _discardButton.Enabled = !_readOnly && dirty;
            _saveButton.Enabled = !_readOnly && dirty && valid;
        }

        // ---------------------------------------------------------------- Validación y línea de estado

        private void FlushValidation()
        {
            if (_validationTimer.Enabled)
            {
                RunValidation();
            }
        }

        private void RunValidation()
        {
            _validationTimer.Stop();
            _validation = SitemapTemplateValidator.Validate(_jsonBox.Text);
            ShowValidationStatus();
            FillSelectorsGrid();
            UpdateCommands();
        }

        private void ShowValidationStatus()
        {
            var errors = _validation.Issues.Where(i => i.Severity == TemplateIssueSeverity.Error).ToList();
            var warnings = _validation.Issues.Where(i => i.Severity == TemplateIssueSeverity.Warning).ToList();
            if (errors.Count > 0)
            {
                var first = errors.FirstOrDefault(i => i.Line.HasValue) ?? errors[0];
                SetStatus(FormatIssue(first) + More(errors.Count), Color.Firebrick, first, errors);
            }
            else if (warnings.Count > 0)
            {
                var first = warnings.FirstOrDefault(i => i.Line.HasValue) ?? warnings[0];
                SetStatus(Count(warnings.Count, "advertencia", "advertencias") + ": " + FormatIssue(first) + More(warnings.Count), Color.DarkOrange, first, warnings);
            }
            else
            {
                SetStatus("✓ Válida · " + Count(_validation.Selectors.Count, "selector", "selectores"), Color.ForestGreen, null, Array.Empty<TemplateIssue>());
            }
        }

        /// <summary>Muestra el texto de estado; si <paramref name="target"/> tiene línea, el texto es un enlace hacia ella.</summary>
        private void SetStatus(string text, Color color, TemplateIssue target, IReadOnlyList<TemplateIssue> details)
        {
            _statusTarget = target != null && target.Line.HasValue ? target : null;
            _statusLabel.LinkArea = new LinkArea(0, 0);
            _statusLabel.Text = text;
            _statusLabel.ForeColor = color;
            _statusLabel.LinkColor = color;
            _statusLabel.ActiveLinkColor = color;
            _statusLabel.VisitedLinkColor = color;
            _statusLabel.LinkArea = _statusTarget == null ? new LinkArea(0, 0) : new LinkArea(0, text.Length);
            _statusLabel.TabStop = _statusTarget != null;
            _toolTip.SetToolTip(_statusLabel, details.Count > 1 ? string.Join(Environment.NewLine, details.Select(FormatIssue)) : null);
        }

        private void OnStatusLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_statusTarget == null)
            {
                return;
            }

            var line = (int)Math.Max(0, _statusTarget.Line.Value - 1);
            var start = _jsonBox.GetFirstCharIndexFromLine(line);
            if (start < 0)
            {
                start = _jsonBox.TextLength;
            }

            var next = _jsonBox.GetFirstCharIndexFromLine(line + 1);
            var end = next < 0 ? _jsonBox.TextLength : Math.Max(start, next - Environment.NewLine.Length);
            var column = _statusTarget.Column.HasValue ? (int)Math.Max(0, _statusTarget.Column.Value - 1) : 0;

            _tabs.SelectedTab = _jsonPage;
            _jsonBox.Focus();
            _jsonBox.Select(Math.Min(start + column, end), 0);
            _jsonBox.ScrollToCaret();
        }

        private void FillSelectorsGrid()
        {
            _selectorsGrid.SuspendLayout();
            _selectorsGrid.Rows.Clear();
            foreach (var selector in _validation.Selectors)
            {
                _selectorsGrid.Rows.Add(
                    selector.Id,
                    selector.Type,
                    selector.Selector,
                    selector.Multiple ? "Sí" : "No",
                    string.Join(", ", selector.ParentSelectors));
            }

            _selectorsGrid.ResumeLayout();
        }

        // ---------------------------------------------------------------- Acciones

        private void CreateNew()
        {
            if (_readOnly || !ConfirmLeave())
            {
                return;
            }

            ShowNewTemplate();
            _nameBox.Focus();
        }

        private bool SaveChanges()
        {
            FlushValidation();
            if (_readOnly)
            {
                return false;
            }

            ConfirmStoreMoves();
            var draft = _state.ToDraft();
            var result = _state.IsNew ? _repository.Create(draft) : _repository.Update(_state.EditingId, draft);
            if (!result.Ok)
            {
                ShowIssues("No se pudo guardar", result.Issues);
                return false;
            }

            ShowTemplate(result.Template);
            ShowResultWarnings("Guardada", result);
            return true;
        }

        /// <summary>Por cada tienda marcada que pertenece a otra plantilla, pregunta si se mueve; «No» la desmarca.</summary>
        private void ConfirmStoreMoves()
        {
            for (var i = 0; i < _storesList.Items.Count; i++)
            {
                if (!_storesList.GetItemChecked(i))
                {
                    continue;
                }

                var store = ((StoreItem)_storesList.Items[i]).Store;
                var owner = _repository.FindByStore(store);
                if (owner == null || owner.Id == _state.EditingId)
                {
                    continue;
                }

                var answer = MessageBox.Show(
                    this,
                    string.Format(CultureInfo.CurrentCulture, MoveStoreMessage, store, owner.Name),
                    "Mover tienda",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (answer == DialogResult.No)
                {
                    _storesList.SetItemChecked(i, false);
                }
            }
        }

        private void DiscardChanges()
        {
            if (_state.IsNew)
            {
                ShowNewTemplate();
                return;
            }

            var template = _repository.Find(_state.EditingId);
            if (template != null)
            {
                ShowTemplate(template);
            }
            else
            {
                ReloadList();
                ShowFirstTemplate();
            }
        }

        private void DuplicateCurrent()
        {
            if (_readOnly || _state.IsNew || !ConfirmLeave())
            {
                return;
            }

            var result = _repository.Duplicate(_state.EditingId);
            if (!result.Ok)
            {
                ShowIssues("No se pudo duplicar", result.Issues);
                return;
            }

            ShowTemplate(result.Template);
            _nameBox.Focus();
            _nameBox.SelectAll();
        }

        private void DeleteCurrent()
        {
            var template = _state.IsNew ? null : _repository.Find(_state.EditingId);
            if (_readOnly || template == null)
            {
                return;
            }

            var answer = MessageBox.Show(
                this,
                string.Format(CultureInfo.CurrentCulture, DeleteMessage, template.Name),
                "Eliminar plantilla",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (answer != DialogResult.Yes)
            {
                return;
            }

            var index = _repository.GetAll().ToList().FindIndex(t => t.Id == template.Id);
            var result = _repository.Delete(template.Id);
            if (!result.Ok)
            {
                ShowIssues("No se pudo eliminar", result.Issues);
                return;
            }

            // Se muestra la plantilla que ocupó su lugar (o la última), descartando lo que hubiera en el editor.
            var remaining = _repository.GetAll();
            ShowTemplate(remaining[Math.Min(Math.Max(index, 0), remaining.Count - 1)]);
        }

        private void ImportTemplate()
        {
            if (_readOnly)
            {
                return;
            }

            string path;
            using (var dialog = new OpenFileDialog { Filter = JsonFilter, Title = "Importar plantilla" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                path = dialog.FileName;
            }

            if (!ConfirmLeave())
            {
                return;
            }

            string text;
            try
            {
                text = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                FileLogger.Error(Source, "No se pudo leer el archivo a importar: " + path, ex);
                MessageBox.Show(this, "No se pudo leer el archivo: " + ex.Message, "Importar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = _repository.Import(text, Path.GetFileNameWithoutExtension(path));
            if (!result.Ok)
            {
                ShowIssues("No se pudo importar", result.Issues);
                return;
            }

            ShowTemplate(result.Template);
            ShowResultWarnings("Importada", result);
        }

        private void ExportTemplate()
        {
            FlushValidation();
            if (!_validation.IsValid)
            {
                return;
            }

            var json = SitemapTemplateValidator.Normalize(_validation.Sitemap);
            using (var dialog = new SaveFileDialog
            {
                Filter = JsonFilter,
                Title = "Exportar plantilla",
                DefaultExt = "json",
                FileName = SafeFileName(DisplayName()) + ".json",
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    File.WriteAllText(dialog.FileName, json, new UTF8Encoding(false));
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    FileLogger.Error(Source, "No se pudo exportar la plantilla a " + dialog.FileName, ex);
                    MessageBox.Show(this, "No se pudo exportar: " + ex.Message, "Exportar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormatJson()
        {
            FlushValidation();
            if (_readOnly || _validation.Sitemap == null)
            {
                return;
            }

            // Asignar Text dispara TextChanged: el estado se actualiza (queda con cambios si el texto difiere).
            _jsonBox.Text = SitemapTemplateValidator.Normalize(_validation.Sitemap);
            _jsonBox.Select(0, 0);
            RunValidation();
        }

        private void RestoreDefaults()
        {
            if (_readOnly)
            {
                return;
            }

            var answer = MessageBox.Show(this, RestoreMessage, "Restaurar predeterminadas", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes || !ConfirmLeave())
            {
                return;
            }

            // Si funciona, Changed recarga la lista y la plantilla en edición.
            var result = _repository.RestoreDefaults();
            if (!result.Ok)
            {
                ShowIssues("No se pudieron restaurar las plantillas", result.Issues);
            }
        }

        // ---------------------------------------------------------------- Utilidades

        private void ShowResultWarnings(string verb, TemplateSaveResult result)
        {
            var warnings = result.Issues.Where(i => i.Severity == TemplateIssueSeverity.Warning).ToList();
            if (warnings.Count > 0)
            {
                var text = verb + " con " + Count(warnings.Count, "advertencia", "advertencias") + ": " + string.Join(" ", warnings.Select(FormatIssue));
                SetStatus(text, Color.DarkOrange, null, warnings);
            }
        }

        private void ShowIssues(string title, IReadOnlyList<TemplateIssue> issues)
        {
            var errors = issues.Where(i => i.Severity == TemplateIssueSeverity.Error).ToList();
            var shown = errors.Count > 0 ? errors : issues.ToList();
            MessageBox.Show(this, string.Join(Environment.NewLine, shown.Select(FormatIssue)), title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string DisplayName()
        {
            var name = (_state.Name ?? string.Empty).Trim();
            return name.Length == 0 ? "Nueva plantilla" : name;
        }

        private static string FormatIssue(TemplateIssue issue)
        {
            if (issue.Line.HasValue && issue.Column.HasValue)
            {
                return string.Format(CultureInfo.CurrentCulture, "Línea {0}, col {1}: {2}", issue.Line.Value, issue.Column.Value, issue.Message);
            }

            if (issue.Line.HasValue)
            {
                return string.Format(CultureInfo.CurrentCulture, "Línea {0}: {1}", issue.Line.Value, issue.Message);
            }

            return issue.Message;
        }

        private static string Count(int count, string singular, string plural)
        {
            return count.ToString(CultureInfo.CurrentCulture) + " " + (count == 1 ? singular : plural);
        }

        private static string More(int count)
        {
            return count > 1 ? " (+" + (count - 1).ToString(CultureInfo.CurrentCulture) + " más)" : string.Empty;
        }

        private static string SafeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        }

        /// <summary>Elemento de la lista de tiendas; muestra «(en: plantilla)» si otra plantilla la tiene vinculada.</summary>
        private sealed class StoreItem
        {
            public StoreItem(string store, string ownerName)
            {
                Store = store;
                OwnerName = ownerName;
            }

            public string Store { get; }

            public string OwnerName { get; }

            public override string ToString()
            {
                return OwnerName == null ? Store : Store + " (en: " + OwnerName + ")";
            }
        }
    }
}
