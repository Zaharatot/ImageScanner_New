namespace ImageScanner_New
{
    partial class main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.scanButton = new System.Windows.Forms.Button();
            this.scanPathTextBox = new System.Windows.Forms.TextBox();
            this.duplicatesList = new System.Windows.Forms.ListView();
            this.ImagesList = new System.Windows.Forms.ImageList(this.components);
            this.removeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(635, 15);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(75, 20);
            this.scanButton.TabIndex = 1;
            this.scanButton.Text = "Scan";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // scanPathTextBox
            // 
            this.scanPathTextBox.Location = new System.Drawing.Point(12, 15);
            this.scanPathTextBox.Name = "scanPathTextBox";
            this.scanPathTextBox.Size = new System.Drawing.Size(617, 20);
            this.scanPathTextBox.TabIndex = 2;
            this.scanPathTextBox.Text = "E:\\БЕКАП\\newFolder\\aaa\\";
            // 
            // duplicatesList
            // 
            this.duplicatesList.CheckBoxes = true;
            this.duplicatesList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.duplicatesList.LargeImageList = this.ImagesList;
            this.duplicatesList.Location = new System.Drawing.Point(0, 83);
            this.duplicatesList.Name = "duplicatesList";
            this.duplicatesList.Size = new System.Drawing.Size(722, 548);
            this.duplicatesList.TabIndex = 3;
            this.duplicatesList.UseCompatibleStateImageBehavior = false;
            // 
            // ImagesList
            // 
            this.ImagesList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ImagesList.ImageSize = new System.Drawing.Size(128, 128);
            this.ImagesList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(215, 41);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(308, 36);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "Remove Selected";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 631);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.duplicatesList);
            this.Controls.Add(this.scanPathTextBox);
            this.Controls.Add(this.scanButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageScanner_New";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.TextBox scanPathTextBox;
        private System.Windows.Forms.ListView duplicatesList;
        private System.Windows.Forms.ImageList ImagesList;
        private System.Windows.Forms.Button removeButton;
    }
}

