namespace KugelmatikControl
{
    partial class SearchForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.clusterList = new System.Windows.Forms.ListBox();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // clusterList
            // 
            this.clusterList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterList.FormattingEnabled = true;
            this.clusterList.Location = new System.Drawing.Point(0, 0);
            this.clusterList.Name = "clusterList";
            this.clusterList.Size = new System.Drawing.Size(254, 298);
            this.clusterList.TabIndex = 0;
            // 
            // searchTimer
            // 
            this.searchTimer.Enabled = true;
            this.searchTimer.Interval = 20000;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 298);
            this.Controls.Add(this.clusterList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchForm";
            this.Text = "Cluster Search";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox clusterList;
        private System.Windows.Forms.Timer searchTimer;
    }
}