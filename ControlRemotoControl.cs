// Control Remoto tab UI placeholder.
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed partial class ControlRemotoControl : UserControl
    {
        public ControlRemotoControl()
        {
            InitializeComponent();
            _helpButton.Click += (s, e) => ShowHelp();
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

