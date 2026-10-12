using System;
using System.Drawing;
using System.Windows.Forms;

namespace S3Tools
{
    /// <summary>
    /// Diálogo modal que aloja <see cref="TemplateManagerControl"/>. Al cerrar pregunta por cambios sin guardar
    /// y expone <see cref="SelectedTemplateId"/> para que el llamador seleccione la plantilla creada o editada.
    /// </summary>
    internal sealed class TemplateManagerDialog : Form
    {
        private readonly TemplateManagerControl _manager;

        public TemplateManagerDialog(SitemapTemplateRepository repository, string initialTemplateId)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            Text = "Administrar plantillas";
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MinimumSize = new Size(900, 600);
            Size = new Size(1000, 680);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _manager = new TemplateManagerControl(repository) { Dock = DockStyle.Fill };
            root.Controls.Add(_manager, 0, 0);

            // DialogResult.Cancel cierra el formulario; OnFormClosing aplica la confirmación de cambios sin guardar.
            var close = new Button { Text = "Cerrar", AutoSize = true, MinimumSize = new Size(90, 0), DialogResult = DialogResult.Cancel };
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6, 0, 6, 6) };
            buttons.Controls.Add(close);
            root.Controls.Add(buttons, 0, 1);

            Controls.Add(root);
            AcceptButton = _manager.DefaultButton;
            CancelButton = close;

            if (initialTemplateId != null)
            {
                _manager.SelectTemplate(initialTemplateId);
            }
        }

        /// <summary>Plantilla mostrada al cerrar el diálogo; <c>null</c> si no hay ninguna (p. ej. una nueva sin guardar).</summary>
        public string SelectedTemplateId { get; private set; }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_manager.ConfirmLeave())
            {
                SelectedTemplateId = _manager.SelectedTemplateId;
            }
            else
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}
