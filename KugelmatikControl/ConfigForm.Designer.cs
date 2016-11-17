namespace KugelmatikControl
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TabControl tabControl1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.clientTabPage = new System.Windows.Forms.TabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.clusterTabPage = new System.Windows.Forms.TabPage();
            this.clusterPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.saveButton = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabControl1.SuspendLayout();
            this.clientTabPage.SuspendLayout();
            this.clusterTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(this.clientTabPage);
            tabControl1.Controls.Add(this.clusterTabPage);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(425, 454);
            tabControl1.TabIndex = 2;
            // 
            // clientTabPage
            // 
            this.clientTabPage.Controls.Add(this.propertyGrid);
            this.clientTabPage.Location = new System.Drawing.Point(4, 22);
            this.clientTabPage.Name = "clientTabPage";
            this.clientTabPage.Size = new System.Drawing.Size(417, 428);
            this.clientTabPage.TabIndex = 0;
            this.clientTabPage.Text = "Client";
            this.clientTabPage.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            this.propertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(417, 428);
            this.propertyGrid.TabIndex = 0;
            // 
            // clusterTabPage
            // 
            this.clusterTabPage.Controls.Add(this.clusterPropertyGrid);
            this.clusterTabPage.Location = new System.Drawing.Point(4, 22);
            this.clusterTabPage.Name = "clusterTabPage";
            this.clusterTabPage.Size = new System.Drawing.Size(417, 428);
            this.clusterTabPage.TabIndex = 1;
            this.clusterTabPage.Text = "Cluster";
            this.clusterTabPage.UseVisualStyleBackColor = true;
            // 
            // clusterPropertyGrid
            // 
            this.clusterPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.clusterPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.clusterPropertyGrid.Name = "clusterPropertyGrid";
            this.clusterPropertyGrid.Size = new System.Drawing.Size(417, 428);
            this.clusterPropertyGrid.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(344, 423);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 26);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 454);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.Text = "Config";
            tabControl1.ResumeLayout(false);
            this.clientTabPage.ResumeLayout(false);
            this.clusterTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.TabPage clientTabPage;
        private System.Windows.Forms.TabPage clusterTabPage;
        private System.Windows.Forms.PropertyGrid clusterPropertyGrid;
        private System.Windows.Forms.Button saveButton;
    }
}