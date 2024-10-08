namespace VideoGridProcessor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelectVideos = new System.Windows.Forms.Button();
            this.lstSelectedVideos = new System.Windows.Forms.ListBox();
            this.btnProcessVideos = new System.Windows.Forms.Button();
            this.txtFFmpegCommand = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectVideos
            // 
            this.btnSelectVideos.Location = new System.Drawing.Point(12, 12);
            this.btnSelectVideos.Name = "btnSelectVideos";
            this.btnSelectVideos.Size = new System.Drawing.Size(120, 30);
            this.btnSelectVideos.TabIndex = 0;
            this.btnSelectVideos.Text = "Select Videos";
            this.btnSelectVideos.UseVisualStyleBackColor = true;
            this.btnSelectVideos.Click += new System.EventHandler(this.btnSelectVideos_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(150, 12);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 30);
            this.btnMoveUp.TabIndex = 6;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(230, 12);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 30);
            this.btnMoveDown.TabIndex = 7;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // lstSelectedVideos
            // 
            this.lstSelectedVideos.FormattingEnabled = true;
            this.lstSelectedVideos.HorizontalScrollbar = true;
            this.lstSelectedVideos.ItemHeight = 15;
            this.lstSelectedVideos.Location = new System.Drawing.Point(12, 48);
            this.lstSelectedVideos.Name = "lstSelectedVideos";
            this.lstSelectedVideos.SelectionMode = System.Windows.Forms.SelectionMode.One; // Changed to One
            this.lstSelectedVideos.Size = new System.Drawing.Size(560, 109);
            this.lstSelectedVideos.TabIndex = 1;
            // 
            // btnProcessVideos
            // 
            this.btnProcessVideos.Location = new System.Drawing.Point(452, 12);
            this.btnProcessVideos.Name = "btnProcessVideos";
            this.btnProcessVideos.Size = new System.Drawing.Size(120, 30);
            this.btnProcessVideos.TabIndex = 2;
            this.btnProcessVideos.Text = "Process Videos";
            this.btnProcessVideos.UseVisualStyleBackColor = true;
            this.btnProcessVideos.Click += new System.EventHandler(this.btnProcessVideos_Click);
            // 
            // txtFFmpegCommand
            // 
            this.txtFFmpegCommand.Location = new System.Drawing.Point(12, 176);
            this.txtFFmpegCommand.Multiline = true;
            this.txtFFmpegCommand.Name = "txtFFmpegCommand";
            this.txtFFmpegCommand.ReadOnly = true;
            this.txtFFmpegCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFFmpegCommand.Size = new System.Drawing.Size(560, 100);
            this.txtFFmpegCommand.TabIndex = 3;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 290);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.TabIndex = 4;
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(12, 326);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(131, 15);
            this.lblOutputPath.TabIndex = 5;
            this.lblOutputPath.Text = "Output will be saved to:";
            // 
            // Form1
            // 
            this.AcceptButton = this.btnProcessVideos;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.txtFFmpegCommand);
            this.Controls.Add(this.btnProcessVideos);
            this.Controls.Add(this.lstSelectedVideos);
            this.Controls.Add(this.btnSelectVideos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Video Grid Processor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectVideos;
        private System.Windows.Forms.ListBox lstSelectedVideos;
        private System.Windows.Forms.Button btnProcessVideos;
        private System.Windows.Forms.TextBox txtFFmpegCommand;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
    }
}
