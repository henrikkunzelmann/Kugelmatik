namespace KugelmatikControl.PingPong
{
    partial class PingPongForm
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
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.resetButton = new System.Windows.Forms.Button();
            this.scoreText = new System.Windows.Forms.Label();
            this.playerTopTextBox = new System.Windows.Forms.TextBox();
            this.playerBottomTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // gameTimer
            // 
            this.gameTimer.Enabled = true;
            this.gameTimer.Interval = 32;
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(13, 13);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 0;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // scoreText
            // 
            this.scoreText.AutoSize = true;
            this.scoreText.Location = new System.Drawing.Point(208, 20);
            this.scoreText.Name = "scoreText";
            this.scoreText.Size = new System.Drawing.Size(35, 13);
            this.scoreText.TabIndex = 1;
            this.scoreText.Text = "Score";
            this.scoreText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // playerTopTextBox
            // 
            this.playerTopTextBox.Location = new System.Drawing.Point(125, 17);
            this.playerTopTextBox.Name = "playerTopTextBox";
            this.playerTopTextBox.Size = new System.Drawing.Size(77, 20);
            this.playerTopTextBox.TabIndex = 2;
            this.playerTopTextBox.Text = "Bot Walt";
            this.playerTopTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // playerBottomTextBox
            // 
            this.playerBottomTextBox.Location = new System.Drawing.Point(249, 17);
            this.playerBottomTextBox.Name = "playerBottomTextBox";
            this.playerBottomTextBox.Size = new System.Drawing.Size(73, 20);
            this.playerBottomTextBox.TabIndex = 3;
            this.playerBottomTextBox.Text = "Bot Cecil";
            this.playerBottomTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(344, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "top       A  D\r\nbottom    J  L\r\n";
            // 
            // PingPongForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 52);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerBottomTextBox);
            this.Controls.Add(this.playerTopTextBox);
            this.Controls.Add(this.scoreText);
            this.Controls.Add(this.resetButton);
            this.KeyPreview = true;
            this.Name = "PingPongForm";
            this.Text = "PingPong";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PingPongForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PingPongForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label scoreText;
        private System.Windows.Forms.TextBox playerTopTextBox;
        private System.Windows.Forms.TextBox playerBottomTextBox;
        private System.Windows.Forms.Label label1;
    }
}