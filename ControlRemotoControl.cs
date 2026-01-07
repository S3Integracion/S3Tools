// Control Remoto tab UI placeholder.
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class ControlRemotoControl : UserControl
    {
        private Button _helpButton;

        public ControlRemotoControl()
        {
            InitializeComponent();
            BuildLayout();
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
                RowCount = 2,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, 0);
            root.Controls.Add(BuildHelpSection(), 0, 1);

            Controls.Add(root);
            ResumeLayout();
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
            _helpButton.Click += (s, e) => ShowHelp();

            panel.Controls.Add(_helpButton);
            return panel;
        }

        private void ShowHelp()
        {
            var msg =
                "Control Remoto\n\n" +
                "Esta pestaña esta en desarrollo y no tiene logica activa en esta version.";
            MessageBox.Show(this, msg, "Ayuda - Control Remoto", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

