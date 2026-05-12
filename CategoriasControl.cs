using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class CategoriasControl : UserControl
    {
        private const int PreviewPage = 2;

        private readonly CategoriasEngineClient _engineClient;
        private readonly List<CategoriaAmazon> _allCategorias = new List<CategoriaAmazon>();
        private readonly HashSet<string> _checkedNames = new HashSet<string>(StringComparer.Ordinal);
        private readonly List<UrlGenerada> _resultsData = new List<UrlGenerada>();
        private readonly List<UrlGenerada> _previewData = new List<UrlGenerada>();

        private string _detectedStore = string.Empty;
        private bool _suppressItemCheck;
        private bool _isBusy;
        private bool _categoriesLoaded;

        public CategoriasControl()
        {
            InitializeComponent();
            _engineClient = new CategoriasEngineClient();

            ConfigureGrids();
            WireEvents();
            UpdateActionState();
        }

        private void ConfigureGrids()
        {
            BuildGridColumns(_verificationGrid);
            BuildGridColumns(_resultsGrid);
        }

        private static void BuildGridColumns(DataGridView grid)
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Categoria",
                Name = "colCategoria",
                DataPropertyName = nameof(UrlGenerada.Categoria),
                FillWeight = 22,
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Paginas",
                Name = "colPagina",
                DataPropertyName = nameof(UrlGenerada.Pagina),
                FillWeight = 8,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tienda",
                Name = "colTienda",
                DataPropertyName = nameof(UrlGenerada.Tienda),
                FillWeight = 12,
            });
            var urlColumn = new DataGridViewLinkColumn
            {
                HeaderText = "URL (doble clic o Ctrl+clic para abrir)",
                Name = "colUrl",
                DataPropertyName = nameof(UrlGenerada.Url),
                FillWeight = 60,
                LinkBehavior = LinkBehavior.HoverUnderline,
                TrackVisitedState = false,
            };
            grid.Columns.Add(urlColumn);
        }

        private void WireEvents()
        {
            Load += async (s, e) => await EnsureCategoriesLoadedAsync();

            _analyzeUrlButton.Click += async (s, e) => await AnalyzeUrlAsync();
            _urlText.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    await AnalyzeUrlAsync();
                }
            };
            _clearUrlButton.Click += (s, e) =>
            {
                _urlText.Text = string.Empty;
                SetDetectedStore(string.Empty);
                RefreshPreview();
                SetStatus("URL limpiada.");
            };

            _selectAllButton.Click += (s, e) => CheckAllVisible(true);
            _selectNoneButton.Click += (s, e) => CheckAllVisible(false);
            _reloadCategoriesButton.Click += async (s, e) => await ReloadCategoriesAsync();

            _categoriasList.ItemCheck += CategoriasList_ItemCheck;

            _generateButton.Click += async (s, e) => await GenerateAsync();

            _copyAllButton.Click += (s, e) => CopyResultsToClipboard(false);
            _copySelectionButton.Click += (s, e) => CopyResultsToClipboard(true);
            _exportTxtButton.Click += (s, e) => ExportResults("txt");
            _exportCsvButton.Click += (s, e) => ExportResults("csv");
            _clearResultsButton.Click += (s, e) =>
            {
                ClearResults();
                SetStatus("Resultados limpiados.");
            };
            _helpButton.Click += (s, e) => ShowHelp();

            _verificationGrid.CellDoubleClick += (s, e) => OpenGridUrl(_verificationGrid, e.RowIndex, e.ColumnIndex);
            _verificationGrid.CellContentClick += (s, e) => OpenGridUrl(_verificationGrid, e.RowIndex, e.ColumnIndex);
            _verificationGrid.CellClick += (s, e) =>
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    OpenGridUrl(_verificationGrid, e.RowIndex, e.ColumnIndex);
                }
            };

            _resultsGrid.CellDoubleClick += (s, e) => OpenGridUrl(_resultsGrid, e.RowIndex, e.ColumnIndex);
            _resultsGrid.CellContentClick += (s, e) => OpenGridUrl(_resultsGrid, e.RowIndex, e.ColumnIndex);
            _resultsGrid.CellClick += (s, e) =>
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    OpenGridUrl(_resultsGrid, e.RowIndex, e.ColumnIndex);
                }
            };
        }

        private async System.Threading.Tasks.Task EnsureCategoriesLoadedAsync()
        {
            if (_categoriesLoaded || _isBusy)
            {
                return;
            }

            await ReloadCategoriesAsync();
        }

        private async System.Threading.Tasks.Task ReloadCategoriesAsync()
        {
            SetBusy(true);
            var response = await _engineClient.LoadCategoriesAsync();
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudieron cargar las categorias.", response.Error, response.Traceback);
                return;
            }

            _allCategorias.Clear();
            _allCategorias.AddRange(response.Categorias ?? Array.Empty<CategoriaAmazon>());
            _categoriesLoaded = true;
            RebuildCategoriesList();

            var status = "Categorias cargadas: " + _allCategorias.Count;
            if (response.PlantillasInvalidas != null && response.PlantillasInvalidas.Length > 0)
            {
                status += " (omitidas por plantilla invalida: " + string.Join(", ", response.PlantillasInvalidas) + ")";
            }
            SetStatus(status);
        }

        private void RebuildCategoriesList()
        {
            _suppressItemCheck = true;
            try
            {
                _categoriasList.BeginUpdate();
                _categoriasList.Items.Clear();

                foreach (var cat in _allCategorias)
                {
                    var checkedNow = _checkedNames.Contains(cat.Nombre);
                    _categoriasList.Items.Add(new CategoryItem(cat), checkedNow);
                }
            }
            finally
            {
                _categoriasList.EndUpdate();
                _suppressItemCheck = false;
            }

            UpdateCategoriesCount();
        }

        private void CategoriasList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressItemCheck)
            {
                return;
            }

            var item = _categoriasList.Items[e.Index] as CategoryItem;
            if (item == null)
            {
                return;
            }

            if (e.NewValue == CheckState.Checked)
            {
                _checkedNames.Add(item.Categoria.Nombre);
            }
            else
            {
                _checkedNames.Remove(item.Categoria.Nombre);
            }

            BeginInvoke(new Action(() =>
            {
                UpdateCategoriesCount();
                RefreshPreview();
            }));
        }

        private void CheckAllVisible(bool value)
        {
            _suppressItemCheck = true;
            try
            {
                for (var i = 0; i < _categoriasList.Items.Count; i++)
                {
                    var item = _categoriasList.Items[i] as CategoryItem;
                    if (item == null)
                    {
                        continue;
                    }

                    _categoriasList.SetItemChecked(i, value);
                    if (value)
                    {
                        _checkedNames.Add(item.Categoria.Nombre);
                    }
                    else
                    {
                        _checkedNames.Remove(item.Categoria.Nombre);
                    }
                }
            }
            finally
            {
                _suppressItemCheck = false;
            }

            UpdateCategoriesCount();
            RefreshPreview();
        }

        private void UpdateCategoriesCount()
        {
            _categoriasCount.Text = "Seleccionadas: " + _checkedNames.Count + " / " + _allCategorias.Count;
            UpdateActionState();
        }

        private async System.Threading.Tasks.Task AnalyzeUrlAsync()
        {
            if (_isBusy)
            {
                return;
            }

            var url = (_urlText.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show(this, "Ingresa una URL para analizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetBusy(true);
            var response = await _engineClient.AnalyzeUrlAsync(url);
            SetBusy(false);

            if (!response.Ok)
            {
                SetDetectedStore(string.Empty);
                RefreshPreview();
                SetStatus("Error: " + response.Error);
                MessageBox.Show(this, response.Error ?? "URL invalida.", "URL invalida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetDetectedStore(response.Tienda);
            RefreshPreview();
            SetStatus("Identificador detectado: " + response.Tienda);
        }

        private void SetDetectedStore(string store)
        {
            _detectedStore = store ?? string.Empty;
            _tiendaValue.Text = string.IsNullOrWhiteSpace(_detectedStore) ? "(no detectado)" : _detectedStore;
            UpdateActionState();
        }

        private void RefreshPreview()
        {
            _previewData.Clear();

            if (!string.IsNullOrWhiteSpace(_detectedStore))
            {
                var seleccionadas = GetSelectedCategorias();
                foreach (var cat in seleccionadas)
                {
                    _previewData.Add(new UrlGenerada
                    {
                        Categoria = cat.Nombre,
                        Pagina = PreviewPage.ToString(),
                        Tienda = _detectedStore,
                        Url = CategoriasDotNetEngine.BuildUrl(cat.Plantilla, _detectedStore, PreviewPage),
                    });
                }
            }

            BindGrid(_verificationGrid, _previewData);
        }

        private async System.Threading.Tasks.Task GenerateAsync()
        {
            if (_isBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_detectedStore))
            {
                MessageBox.Show(this, "Primero analiza una URL valida para extraer el identificador de tienda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var seleccionadas = GetSelectedCategorias();
            if (seleccionadas.Length == 0)
            {
                MessageBox.Show(this, "Selecciona al menos una categoria para generar URLs.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var n = (int)_pagesNumeric.Value;
            if (n < 1)
            {
                MessageBox.Show(this, "El numero de paginas debe ser mayor o igual a 1.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var request = new CategoriasGenerateRequest
            {
                Tienda = _detectedStore,
                CategoriasSeleccionadas = seleccionadas,
                PaginaInicial = 1,
                PaginaFinal = n,
            };

            SetBusy(true);
            var response = await _engineClient.GenerateAsync(request);
            SetBusy(false);

            if (!response.Ok)
            {
                ShowEngineError("No se pudieron generar las URLs.", response.Error, response.Traceback);
                return;
            }

            _resultsData.Clear();
            _resultsData.AddRange(response.Urls ?? Array.Empty<UrlGenerada>());
            BindGrid(_resultsGrid, _resultsData);

            SetStatus(
                "URLs generadas: " + _resultsData.Count +
                " (1 por categoria, rango de paginas [1-" + n + "])");
            UpdateActionState();
        }

        private CategoriaAmazon[] GetSelectedCategorias()
        {
            return _allCategorias
                .Where(c => _checkedNames.Contains(c.Nombre))
                .OrderBy(c => c.Orden)
                .ThenBy(c => c.Nombre, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }

        private static void BindGrid(DataGridView grid, IList<UrlGenerada> data)
        {
            grid.DataSource = null;
            grid.DataSource = new BindingSource { DataSource = data ?? new List<UrlGenerada>() };
        }

        private void OpenGridUrl(DataGridView grid, int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            var urlColumn = grid.Columns["colUrl"];
            if (urlColumn == null || columnIndex != urlColumn.Index)
            {
                return;
            }

            var row = grid.Rows[rowIndex];
            var url = row.Cells[urlColumn.Index].Value as string;
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo abrir la URL: " + ex.Message, "Abrir URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CopyResultsToClipboard(bool selectionOnly)
        {
            IEnumerable<UrlGenerada> source;
            if (selectionOnly)
            {
                source = _resultsGrid.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Where(r => !r.IsNewRow)
                    .OrderBy(r => r.Index)
                    .Select(r => r.DataBoundItem as UrlGenerada)
                    .Where(x => x != null);
            }
            else
            {
                source = _resultsData;
            }

            var lines = source.Select(u => u.Url ?? string.Empty).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
            if (lines.Length == 0)
            {
                MessageBox.Show(this, "No hay URLs para copiar.", "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Clipboard.SetText(string.Join(Environment.NewLine, lines));
                SetStatus("Copiadas " + lines.Length + " URL(s) al portapapeles.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Copiar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ExportResults(string format)
        {
            if (_resultsData.Count == 0)
            {
                MessageBox.Show(this, "No hay URLs para exportar.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filter;
            string ext;
            switch (format)
            {
                case "csv":
                    filter = "CSV (*.csv)|*.csv|Todos (*.*)|*.*";
                    ext = "csv";
                    break;
                case "json":
                    filter = "JSON (*.json)|*.json|Todos (*.*)|*.*";
                    ext = "json";
                    break;
                default:
                    filter = "TXT (*.txt)|*.txt|Todos (*.*)|*.*";
                    ext = "txt";
                    break;
            }

            using (var dialog = new SaveFileDialog
            {
                Title = "Exportar URLs",
                Filter = filter,
                FileName = "urls_amazon_tienda_" + (string.IsNullOrWhiteSpace(_detectedStore) ? "sin_id" : _detectedStore) + "." + ext,
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var request = new CategoriasExportRequest
                {
                    Urls = _resultsData.ToArray(),
                    FilePath = dialog.FileName,
                    Format = ext,
                };

                SetBusy(true);
                var response = await _engineClient.ExportAsync(request);
                SetBusy(false);

                if (!response.Ok)
                {
                    ShowEngineError("No fue posible exportar el archivo.", response.Error, response.Traceback);
                    return;
                }

                SetStatus("Archivo exportado: " + response.FilePath);
                MessageBox.Show(this, "Archivo exportado:\n" + response.FilePath, "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ClearResults()
        {
            _resultsData.Clear();
            BindGrid(_resultsGrid, _resultsData);
            UpdateActionState();
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            UseWaitCursor = busy;
            _analyzeUrlButton.Enabled = !busy;
            _clearUrlButton.Enabled = !busy;
            _selectAllButton.Enabled = !busy;
            _selectNoneButton.Enabled = !busy;
            _reloadCategoriesButton.Enabled = !busy;
            _categoriasList.Enabled = !busy;
            _generateButton.Enabled = !busy;
            UpdateActionState();
        }

        private void UpdateActionState()
        {
            var hasResults = _resultsData.Count > 0;
            _copyAllButton.Enabled = !_isBusy && hasResults;
            _copySelectionButton.Enabled = !_isBusy && hasResults;
            _exportTxtButton.Enabled = !_isBusy && hasResults;
            _exportCsvButton.Enabled = !_isBusy && hasResults;
            _clearResultsButton.Enabled = !_isBusy && hasResults;

            var canGenerate = !_isBusy
                && !string.IsNullOrWhiteSpace(_detectedStore)
                && _checkedNames.Count > 0;
            _generateButton.Enabled = canGenerate;
        }

        private void SetStatus(string message)
        {
            _statusLabel.Text = message ?? string.Empty;
        }

        private void ShowEngineError(string title, string error, string traceback)
        {
            var message = error ?? "Error desconocido.";
            if (!string.IsNullOrWhiteSpace(traceback))
            {
                message += Environment.NewLine + Environment.NewLine + traceback;
            }

            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Error: " + (error ?? "desconocido"));
        }

        private void ShowHelp()
        {
            var msg =
                "Categorias - Generador de URLs de Amazon Mexico\n\n" +
                "1) Pega una URL de tienda o categoria de amazon.com.mx y presiona Analizar.\n" +
                "2) El sistema extrae el identificador de tienda (p_6).\n" +
                "3) Marca una o varias categorias en la lista.\n" +
                "   Mientras seleccionas, la tabla de Verificacion muestra una URL\n" +
                "   por categoria con page=2 (clickeable para abrir en navegador).\n" +
                "4) Indica el numero N de paginas y presiona Generar URLs.\n" +
                "   Se genera UNA URL por categoria con el placeholder de rango [1-N]\n" +
                "   en page y ref=sr_pg_, listo para que el scraper expanda el rango.\n" +
                "5) Copia o exporta los resultados (TXT/CSV).\n\n" +
                "Doble clic o Ctrl+clic sobre una URL la abre en el navegador.\n\n" +
                "Las plantillas se cargan de PlantillaCategoriasAmazon.json.";

            MessageBox.Show(this, msg, "Ayuda - Categorias", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private sealed class CategoryItem
        {
            public CategoryItem(CategoriaAmazon categoria)
            {
                Categoria = categoria;
            }

            public CategoriaAmazon Categoria { get; }

            public override string ToString()
            {
                return Categoria?.Nombre ?? string.Empty;
            }
        }
    }
}
