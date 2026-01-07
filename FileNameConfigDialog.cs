using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace S3Integración_programs
{
    internal sealed class FileNameConfigDialog : Form
    {
        private static readonly (string Label, string Value)[] Prefix1Options =
        {
            ("Vacio", ""),
            ("01_", "01_"),
            ("02_", "02_"),
            ("03_", "03_"),
            ("04_", "04_"),
            ("05_", "05_"),
        };

        private static readonly (string Label, string Value)[] Prefix2Options =
        {
            ("Vacio", ""),
            ("1er_Vuelta_", "1er_Vuelta_"),
            ("2da_Vuelta_", "2da_Vuelta_"),
            ("3er_Vuelta_", "3er_Vuelta_"),
            ("4ta_Vuelta_", "4ta_Vuelta_"),
        };

        private readonly List<RadioButton> _prefix1Radios = new List<RadioButton>();
        private readonly List<RadioButton> _prefix2Radios = new List<RadioButton>();

        public FileNameConfigDialog(string prefix1, string prefix2)
        {
            Prefix1 = prefix1 ?? string.Empty;
            Prefix2 = prefix2 ?? string.Empty;

            Text = "Configuracion de nombre";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var root = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildGroup("1ra parte", Prefix1Options, _prefix1Radios, Prefix1), 0, 0);
            root.Controls.Add(BuildGroup("2da parte", Prefix2Options, _prefix2Radios, Prefix2), 0, 1);
            root.Controls.Add(BuildButtons(), 0, 2);

            Controls.Add(root);
        }

        public string Prefix1 { get; private set; }

        public string Prefix2 { get; private set; }

        private static Control BuildGroup(
            string title,
            (string Label, string Value)[] options,
            List<RadioButton> radios,
            string selectedValue)
        {
            var group = new GroupBox
            {
                Text = title,
                Dock = DockStyle.Fill,
                AutoSize = true,
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
            };

            foreach (var option in options)
            {
                var rb = new RadioButton
                {
                    Text = option.Label,
                    AutoSize = true,
                    Tag = option.Value,
                };
                if (string.Equals(option.Value, selectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    rb.Checked = true;
                }
                panel.Controls.Add(rb);
                radios.Add(rb);
            }

            if (!radios.Any(r => r.Checked) && radios.Count > 0)
            {
                radios[0].Checked = true;
            }

            group.Controls.Add(panel);
            return group;
        }

        private Control BuildButtons()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
            };

            var ok = new Button
            {
                Text = "Aceptar",
                AutoSize = true,
                DialogResult = DialogResult.OK,
            };
            var cancel = new Button
            {
                Text = "Cancelar",
                AutoSize = true,
                DialogResult = DialogResult.Cancel,
            };

            AcceptButton = ok;
            CancelButton = cancel;

            panel.Controls.Add(ok);
            panel.Controls.Add(cancel);
            return panel;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                Prefix1 = GetSelectedValue(_prefix1Radios);
                Prefix2 = GetSelectedValue(_prefix2Radios);
            }
            base.OnFormClosing(e);
        }

        private static string GetSelectedValue(IEnumerable<RadioButton> radios)
        {
            foreach (var rb in radios)
            {
                if (rb.Checked)
                {
                    return rb.Tag as string ?? string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
