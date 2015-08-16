namespace KugelmatikControl
{
    partial class StepperControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.heightNumber = new System.Windows.Forms.NumericUpDown();
            this.heightTrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.heightNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // heightNumber
            // 
            this.heightNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.heightNumber.Location = new System.Drawing.Point(0, 0);
            this.heightNumber.Name = "heightNumber";
            this.heightNumber.Size = new System.Drawing.Size(82, 20);
            this.heightNumber.TabIndex = 0;
            this.heightNumber.ValueChanged += new System.EventHandler(this.heightNumber_ValueChanged);
            // 
            // heightTrackBar
            // 
            this.heightTrackBar.AutoSize = false;
            this.heightTrackBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.heightTrackBar.LargeChange = 100;
            this.heightTrackBar.Location = new System.Drawing.Point(0, 20);
            this.heightTrackBar.Name = "heightTrackBar";
            this.heightTrackBar.Size = new System.Drawing.Size(82, 26);
            this.heightTrackBar.SmallChange = 10;
            this.heightTrackBar.TabIndex = 1;
            this.heightTrackBar.TickFrequency = 100;
            this.heightTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.heightTrackBar.ValueChanged += new System.EventHandler(this.heightTrackBar_ValueChanged);
            // 
            // StepperControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heightTrackBar);
            this.Controls.Add(this.heightNumber);
            this.Name = "StepperControl";
            this.Size = new System.Drawing.Size(82, 46);
            ((System.ComponentModel.ISupportInitialize)(this.heightNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown heightNumber;
        private System.Windows.Forms.TrackBar heightTrackBar;
    }
}
