namespace ImageScanner_New
{
    partial class waiter
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
            this.scahHeader = new System.Windows.Forms.Label();
            this.scanProgress = new System.Windows.Forms.ProgressBar();
            this.visualizeProgress = new System.Windows.Forms.ProgressBar();
            this.visualizeHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // scahHeader
            // 
            this.scahHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.scahHeader.Location = new System.Drawing.Point(25, 25);
            this.scahHeader.Name = "scahHeader";
            this.scahHeader.Padding = new System.Windows.Forms.Padding(5);
            this.scahHeader.Size = new System.Drawing.Size(368, 27);
            this.scahHeader.TabIndex = 0;
            this.scahHeader.Text = "Прогресс сканирования файлов:";
            // 
            // scanProgress
            // 
            this.scanProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.scanProgress.Location = new System.Drawing.Point(25, 52);
            this.scanProgress.Name = "scanProgress";
            this.scanProgress.Size = new System.Drawing.Size(368, 23);
            this.scanProgress.TabIndex = 1;
            // 
            // visualizeProgress
            // 
            this.visualizeProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.visualizeProgress.Location = new System.Drawing.Point(25, 102);
            this.visualizeProgress.Name = "visualizeProgress";
            this.visualizeProgress.Size = new System.Drawing.Size(368, 23);
            this.visualizeProgress.TabIndex = 3;
            // 
            // visualizeHeader
            // 
            this.visualizeHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.visualizeHeader.Location = new System.Drawing.Point(25, 75);
            this.visualizeHeader.Name = "visualizeHeader";
            this.visualizeHeader.Padding = new System.Windows.Forms.Padding(5);
            this.visualizeHeader.Size = new System.Drawing.Size(368, 27);
            this.visualizeHeader.TabIndex = 2;
            this.visualizeHeader.Text = "Прогресс отображения результатов сканирования:";
            // 
            // waiter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 142);
            this.Controls.Add(this.visualizeProgress);
            this.Controls.Add(this.visualizeHeader);
            this.Controls.Add(this.scanProgress);
            this.Controls.Add(this.scahHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "waiter";
            this.Padding = new System.Windows.Forms.Padding(25);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "waiter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label scahHeader;
        private System.Windows.Forms.ProgressBar scanProgress;
        private System.Windows.Forms.ProgressBar visualizeProgress;
        private System.Windows.Forms.Label visualizeHeader;
    }
}