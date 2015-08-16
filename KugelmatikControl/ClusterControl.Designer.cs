namespace KugelmatikControl
{
    partial class ClusterControl
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
            this.clusterBox = new System.Windows.Forms.GroupBox();
            this.gridText = new System.Windows.Forms.Label();
            this.infoText = new System.Windows.Forms.Label();
            this.clusterBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // clusterBox
            // 
            this.clusterBox.Controls.Add(this.infoText);
            this.clusterBox.Controls.Add(this.gridText);
            this.clusterBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clusterBox.Location = new System.Drawing.Point(0, 0);
            this.clusterBox.Name = "clusterBox";
            this.clusterBox.Size = new System.Drawing.Size(273, 173);
            this.clusterBox.TabIndex = 2;
            this.clusterBox.TabStop = false;
            this.clusterBox.Text = "Cluster";
            // 
            // gridText
            // 
            this.gridText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridText.Location = new System.Drawing.Point(3, 19);
            this.gridText.Name = "gridText";
            this.gridText.Size = new System.Drawing.Size(267, 151);
            this.gridText.TabIndex = 0;
            this.gridText.Text = "gridText";
            this.gridText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infoText
            // 
            this.infoText.AutoSize = true;
            this.infoText.Location = new System.Drawing.Point(7, 19);
            this.infoText.Name = "infoText";
            this.infoText.Size = new System.Drawing.Size(63, 15);
            this.infoText.TabIndex = 1;
            this.infoText.Text = "infoText";
            // 
            // ClusterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clusterBox);
            this.Name = "ClusterControl";
            this.Size = new System.Drawing.Size(273, 173);
            this.clusterBox.ResumeLayout(false);
            this.clusterBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox clusterBox;
        private System.Windows.Forms.Label gridText;
        private System.Windows.Forms.Label infoText;
    }
}
