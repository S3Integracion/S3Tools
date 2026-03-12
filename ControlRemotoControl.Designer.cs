namespace S3Integración_programs
{
    partial class ControlRemotoControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.helpPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._helpButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.helpPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.contentPanel, 0, 0);
            this.rootLayout.Controls.Add(this.helpPanel, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.Size = new System.Drawing.Size(760, 520);
            this.rootLayout.TabIndex = 0;
            // 
            // contentPanel
            // 
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(13, 13);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(734, 461);
            this.contentPanel.TabIndex = 0;
            // 
            // helpPanel
            // 
            this.helpPanel.AutoSize = true;
            this.helpPanel.Controls.Add(this._helpButton);
            this.helpPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.helpPanel.Location = new System.Drawing.Point(13, 480);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Size = new System.Drawing.Size(734, 27);
            this.helpPanel.TabIndex = 1;
            // 
            // _helpButton
            // 
            this._helpButton.AutoSize = true;
            this._helpButton.Location = new System.Drawing.Point(663, 3);
            this._helpButton.Name = "_helpButton";
            this._helpButton.Size = new System.Drawing.Size(68, 26);
            this._helpButton.TabIndex = 0;
            this._helpButton.Text = "Ayuda";
            this._helpButton.UseVisualStyleBackColor = true;
            // 
            // ControlRemotoControl
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.rootLayout);
            this.Name = "ControlRemotoControl";
            this.Size = new System.Drawing.Size(760, 520);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.helpPanel.ResumeLayout(false);
            this.helpPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.FlowLayoutPanel helpPanel;
        private System.Windows.Forms.Button _helpButton;
    }
}

