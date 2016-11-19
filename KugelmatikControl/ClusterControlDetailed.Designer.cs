namespace KugelmatikControl
{
    partial class ClusterControlDetailed
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.clusterInfoGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.steppersPanel = new System.Windows.Forms.Panel();
            this.clusterHeightTrackBar = new System.Windows.Forms.TrackBar();
            this.clusterHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.setDataButton = new System.Windows.Forms.Button();
            this.blinkButton = new System.Windows.Forms.Button();
            this.configButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.infoButton = new System.Windows.Forms.Button();
            this.moveToTopButton = new System.Windows.Forms.Button();
            this.getDataButton = new System.Windows.Forms.Button();
            this.homeButton = new System.Windows.Forms.Button();
            this.stepperBox = new System.Windows.Forms.GroupBox();
            this.setKugelmatikButton = new System.Windows.Forms.Button();
            this.setClusterButton = new System.Windows.Forms.Button();
            this.fixStepperButton = new System.Windows.Forms.Button();
            this.homeStepperButton = new System.Windows.Forms.Button();
            this.clusterBox = new System.Windows.Forms.GroupBox();
            this.blinkContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.greenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clusterHeightTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clusterHeight)).BeginInit();
            this.stepperBox.SuspendLayout();
            this.clusterBox.SuspendLayout();
            this.blinkContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new System.Drawing.Point(3, 19);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.setDataButton);
            splitContainer1.Panel2.Controls.Add(this.blinkButton);
            splitContainer1.Panel2.Controls.Add(this.configButton);
            splitContainer1.Panel2.Controls.Add(this.stopButton);
            splitContainer1.Panel2.Controls.Add(this.infoButton);
            splitContainer1.Panel2.Controls.Add(this.moveToTopButton);
            splitContainer1.Panel2.Controls.Add(this.getDataButton);
            splitContainer1.Panel2.Controls.Add(this.homeButton);
            splitContainer1.Panel2.Controls.Add(this.stepperBox);
            splitContainer1.Size = new System.Drawing.Size(633, 416);
            splitContainer1.SplitterDistance = 319;
            splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.clusterInfoGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(633, 319);
            this.splitContainer2.SplitterDistance = 248;
            this.splitContainer2.TabIndex = 1;
            // 
            // clusterInfoGrid
            // 
            this.clusterInfoGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.clusterInfoGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterInfoGrid.HelpVisible = false;
            this.clusterInfoGrid.Location = new System.Drawing.Point(0, 0);
            this.clusterInfoGrid.Name = "clusterInfoGrid";
            this.clusterInfoGrid.Size = new System.Drawing.Size(248, 319);
            this.clusterInfoGrid.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.steppersPanel);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.clusterHeightTrackBar);
            this.splitContainer3.Panel2.Controls.Add(this.clusterHeight);
            this.splitContainer3.Panel2.Controls.Add(this.label1);
            this.splitContainer3.Size = new System.Drawing.Size(381, 319);
            this.splitContainer3.SplitterDistance = 253;
            this.splitContainer3.TabIndex = 1;
            // 
            // steppersPanel
            // 
            this.steppersPanel.AutoScroll = true;
            this.steppersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.steppersPanel.Location = new System.Drawing.Point(0, 0);
            this.steppersPanel.Margin = new System.Windows.Forms.Padding(10);
            this.steppersPanel.Name = "steppersPanel";
            this.steppersPanel.Size = new System.Drawing.Size(381, 253);
            this.steppersPanel.TabIndex = 0;
            // 
            // clusterHeightTrackBar
            // 
            this.clusterHeightTrackBar.Location = new System.Drawing.Point(211, 21);
            this.clusterHeightTrackBar.Name = "clusterHeightTrackBar";
            this.clusterHeightTrackBar.Size = new System.Drawing.Size(242, 45);
            this.clusterHeightTrackBar.TabIndex = 2;
            this.clusterHeightTrackBar.TickFrequency = 100;
            this.clusterHeightTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.clusterHeightTrackBar.ValueChanged += new System.EventHandler(this.clusterHeightTrackBar_ValueChanged);
            // 
            // clusterHeight
            // 
            this.clusterHeight.Location = new System.Drawing.Point(82, 21);
            this.clusterHeight.Name = "clusterHeight";
            this.clusterHeight.Size = new System.Drawing.Size(120, 23);
            this.clusterHeight.TabIndex = 1;
            this.clusterHeight.ValueChanged += new System.EventHandler(this.clusterHeight_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cluster";
            // 
            // setDataButton
            // 
            this.setDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.setDataButton.Location = new System.Drawing.Point(165, 66);
            this.setDataButton.Name = "setDataButton";
            this.setDataButton.Size = new System.Drawing.Size(78, 23);
            this.setDataButton.TabIndex = 7;
            this.setDataButton.Text = "Set data";
            this.setDataButton.UseVisualStyleBackColor = true;
            this.setDataButton.Click += new System.EventHandler(this.setDataButton_Click);
            // 
            // blinkButton
            // 
            this.blinkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.blinkButton.Location = new System.Drawing.Point(165, 41);
            this.blinkButton.Name = "blinkButton";
            this.blinkButton.Size = new System.Drawing.Size(78, 23);
            this.blinkButton.TabIndex = 6;
            this.blinkButton.Text = "Blink";
            this.blinkButton.UseVisualStyleBackColor = true;
            this.blinkButton.Click += new System.EventHandler(this.blinkButton_Click);
            // 
            // configButton
            // 
            this.configButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.configButton.Location = new System.Drawing.Point(165, 14);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(78, 23);
            this.configButton.TabIndex = 5;
            this.configButton.Text = "Config";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.BackColor = System.Drawing.Color.DarkRed;
            this.stopButton.ForeColor = System.Drawing.Color.White;
            this.stopButton.Location = new System.Drawing.Point(84, 66);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // infoButton
            // 
            this.infoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoButton.Location = new System.Drawing.Point(84, 41);
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(75, 23);
            this.infoButton.TabIndex = 3;
            this.infoButton.Text = "Info";
            this.infoButton.UseVisualStyleBackColor = true;
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // moveToTopButton
            // 
            this.moveToTopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.moveToTopButton.Location = new System.Drawing.Point(3, 14);
            this.moveToTopButton.Name = "moveToTopButton";
            this.moveToTopButton.Size = new System.Drawing.Size(75, 23);
            this.moveToTopButton.TabIndex = 0;
            this.moveToTopButton.Text = "Move top";
            this.moveToTopButton.UseVisualStyleBackColor = true;
            this.moveToTopButton.Click += new System.EventHandler(this.moveToTopButton_Click);
            // 
            // getDataButton
            // 
            this.getDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.getDataButton.Location = new System.Drawing.Point(84, 14);
            this.getDataButton.Name = "getDataButton";
            this.getDataButton.Size = new System.Drawing.Size(75, 23);
            this.getDataButton.TabIndex = 2;
            this.getDataButton.Text = "Get data";
            this.getDataButton.UseVisualStyleBackColor = true;
            this.getDataButton.Click += new System.EventHandler(this.getDataButton_Click);
            // 
            // homeButton
            // 
            this.homeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.homeButton.Location = new System.Drawing.Point(3, 66);
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(75, 23);
            this.homeButton.TabIndex = 1;
            this.homeButton.Text = "Home";
            this.homeButton.UseVisualStyleBackColor = true;
            this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
            // 
            // stepperBox
            // 
            this.stepperBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stepperBox.Controls.Add(this.setKugelmatikButton);
            this.stepperBox.Controls.Add(this.setClusterButton);
            this.stepperBox.Controls.Add(this.fixStepperButton);
            this.stepperBox.Controls.Add(this.homeStepperButton);
            this.stepperBox.Location = new System.Drawing.Point(298, 9);
            this.stepperBox.Name = "stepperBox";
            this.stepperBox.Size = new System.Drawing.Size(332, 75);
            this.stepperBox.TabIndex = 2;
            this.stepperBox.TabStop = false;
            this.stepperBox.Text = "Stepper";
            // 
            // setKugelmatikButton
            // 
            this.setKugelmatikButton.Location = new System.Drawing.Point(168, 46);
            this.setKugelmatikButton.Name = "setKugelmatikButton";
            this.setKugelmatikButton.Size = new System.Drawing.Size(154, 23);
            this.setKugelmatikButton.TabIndex = 3;
            this.setKugelmatikButton.Text = "Set Kugelmatik";
            this.setKugelmatikButton.UseVisualStyleBackColor = true;
            this.setKugelmatikButton.Click += new System.EventHandler(this.setKugelmatikButton_Click);
            // 
            // setClusterButton
            // 
            this.setClusterButton.Location = new System.Drawing.Point(6, 46);
            this.setClusterButton.Name = "setClusterButton";
            this.setClusterButton.Size = new System.Drawing.Size(156, 23);
            this.setClusterButton.TabIndex = 1;
            this.setClusterButton.Text = "Set cluster";
            this.setClusterButton.UseVisualStyleBackColor = true;
            this.setClusterButton.Click += new System.EventHandler(this.setClusterButton_Click);
            // 
            // fixStepperButton
            // 
            this.fixStepperButton.Location = new System.Drawing.Point(87, 22);
            this.fixStepperButton.Name = "fixStepperButton";
            this.fixStepperButton.Size = new System.Drawing.Size(75, 23);
            this.fixStepperButton.TabIndex = 2;
            this.fixStepperButton.Text = "Fix";
            this.fixStepperButton.UseVisualStyleBackColor = true;
            this.fixStepperButton.Click += new System.EventHandler(this.fixStepperButton_Click);
            // 
            // homeStepperButton
            // 
            this.homeStepperButton.Location = new System.Drawing.Point(6, 22);
            this.homeStepperButton.Name = "homeStepperButton";
            this.homeStepperButton.Size = new System.Drawing.Size(75, 23);
            this.homeStepperButton.TabIndex = 0;
            this.homeStepperButton.Text = "Home";
            this.homeStepperButton.UseVisualStyleBackColor = true;
            this.homeStepperButton.Click += new System.EventHandler(this.homeStepperButton_Click);
            // 
            // clusterBox
            // 
            this.clusterBox.Controls.Add(splitContainer1);
            this.clusterBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clusterBox.Location = new System.Drawing.Point(0, 0);
            this.clusterBox.Name = "clusterBox";
            this.clusterBox.Size = new System.Drawing.Size(639, 438);
            this.clusterBox.TabIndex = 2;
            this.clusterBox.TabStop = false;
            this.clusterBox.Text = "Cluster";
            // 
            // blinkContextMenu
            // 
            this.blinkContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.greenToolStripMenuItem,
            this.redToolStripMenuItem});
            this.blinkContextMenu.Name = "blinkContextMenu";
            this.blinkContextMenu.Size = new System.Drawing.Size(106, 48);
            // 
            // greenToolStripMenuItem
            // 
            this.greenToolStripMenuItem.Name = "greenToolStripMenuItem";
            this.greenToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.greenToolStripMenuItem.Text = "Green";
            this.greenToolStripMenuItem.Click += new System.EventHandler(this.greenToolStripMenuItem_Click);
            // 
            // redToolStripMenuItem
            // 
            this.redToolStripMenuItem.Name = "redToolStripMenuItem";
            this.redToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.redToolStripMenuItem.Text = "Red";
            this.redToolStripMenuItem.Click += new System.EventHandler(this.redToolStripMenuItem_Click);
            // 
            // ClusterControlDetailed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clusterBox);
            this.Name = "ClusterControlDetailed";
            this.Size = new System.Drawing.Size(639, 438);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.clusterHeightTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clusterHeight)).EndInit();
            this.stepperBox.ResumeLayout(false);
            this.clusterBox.ResumeLayout(false);
            this.blinkContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox clusterBox;
        private System.Windows.Forms.GroupBox stepperBox;
        private System.Windows.Forms.Button homeButton;
        private System.Windows.Forms.Button fixStepperButton;
        private System.Windows.Forms.Button homeStepperButton;
        private System.Windows.Forms.Button setKugelmatikButton;
        private System.Windows.Forms.Button setClusterButton;
        private System.Windows.Forms.Button getDataButton;
        private System.Windows.Forms.Button moveToTopButton;
        private System.Windows.Forms.Button infoButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button blinkButton;
        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.ContextMenuStrip blinkContextMenu;
        private System.Windows.Forms.ToolStripMenuItem greenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid clusterInfoGrid;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Panel steppersPanel;
        private System.Windows.Forms.TrackBar clusterHeightTrackBar;
        private System.Windows.Forms.NumericUpDown clusterHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button setDataButton;
    }
}
