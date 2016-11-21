namespace KugelmatikControl
{
    partial class HeightViewForm
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
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeightViewForm));
            this.findRangeCheckBox = new System.Windows.Forms.CheckBox();
            this.invertCheckBox = new System.Windows.Forms.CheckBox();
            this.scale = new System.Windows.Forms.NumericUpDown();
            this.maxHeight = new System.Windows.Forms.NumericUpDown();
            this.minHeight = new System.Windows.Forms.NumericUpDown();
            this.heightView = new System.Windows.Forms.PictureBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this.findRangeCheckBox);
            splitContainer1.Panel1.Controls.Add(this.invertCheckBox);
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1.Controls.Add(this.scale);
            splitContainer1.Panel1.Controls.Add(this.maxHeight);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(this.minHeight);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this.heightView);
            splitContainer1.Size = new System.Drawing.Size(896, 573);
            splitContainer1.SplitterDistance = 74;
            splitContainer1.TabIndex = 0;
            // 
            // findRangeCheckBox
            // 
            this.findRangeCheckBox.AutoSize = true;
            this.findRangeCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.findRangeCheckBox.Location = new System.Drawing.Point(289, 39);
            this.findRangeCheckBox.Name = "findRangeCheckBox";
            this.findRangeCheckBox.Size = new System.Drawing.Size(76, 17);
            this.findRangeCheckBox.TabIndex = 7;
            this.findRangeCheckBox.Text = "Find range";
            this.findRangeCheckBox.UseVisualStyleBackColor = true;
            // 
            // invertCheckBox
            // 
            this.invertCheckBox.AutoSize = true;
            this.invertCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.invertCheckBox.Location = new System.Drawing.Point(230, 39);
            this.invertCheckBox.Name = "invertCheckBox";
            this.invertCheckBox.Size = new System.Drawing.Size(53, 17);
            this.invertCheckBox.TabIndex = 6;
            this.invertCheckBox.Text = "Invert";
            this.invertCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(227, 14);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(34, 13);
            label3.TabIndex = 5;
            label3.Text = "Scale";
            // 
            // scale
            // 
            this.scale.Location = new System.Drawing.Point(267, 12);
            this.scale.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.scale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.scale.Name = "scale";
            this.scale.Size = new System.Drawing.Size(120, 20);
            this.scale.TabIndex = 4;
            this.scale.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // maxHeight
            // 
            this.maxHeight.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxHeight.Location = new System.Drawing.Point(76, 38);
            this.maxHeight.Name = "maxHeight";
            this.maxHeight.Size = new System.Drawing.Size(120, 20);
            this.maxHeight.TabIndex = 3;
            this.maxHeight.ValueChanged += new System.EventHandler(this.maxHeight_ValueChanged);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 40);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(59, 13);
            label2.TabIndex = 2;
            label2.Text = "Max height";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(56, 13);
            label1.TabIndex = 1;
            label1.Text = "Min height";
            // 
            // minHeight
            // 
            this.minHeight.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.minHeight.Location = new System.Drawing.Point(76, 12);
            this.minHeight.Name = "minHeight";
            this.minHeight.Size = new System.Drawing.Size(120, 20);
            this.minHeight.TabIndex = 0;
            this.minHeight.ValueChanged += new System.EventHandler(this.minHeight_ValueChanged);
            // 
            // heightView
            // 
            this.heightView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heightView.Location = new System.Drawing.Point(0, 0);
            this.heightView.Name = "heightView";
            this.heightView.Size = new System.Drawing.Size(896, 495);
            this.heightView.TabIndex = 0;
            this.heightView.TabStop = false;
            this.heightView.Paint += new System.Windows.Forms.PaintEventHandler(this.heightView_Paint);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 33;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // HeightViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 573);
            this.Controls.Add(splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HeightViewForm";
            this.Text = "Height view";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox heightView;
        private System.Windows.Forms.NumericUpDown maxHeight;
        private System.Windows.Forms.NumericUpDown minHeight;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.NumericUpDown scale;
        private System.Windows.Forms.CheckBox invertCheckBox;
        private System.Windows.Forms.CheckBox findRangeCheckBox;
    }
}