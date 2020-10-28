namespace BetterMajorasMaskInstaller.Window
{
    partial class Completed
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
            this.QuitButton = new System.Windows.Forms.Button();
            this.DesktopShortcutCheckBox = new System.Windows.Forms.CheckBox();
            this.TemporaryFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.StartMenuShortcutCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // QuitButton
            // 
            this.QuitButton.Location = new System.Drawing.Point(358, 326);
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new System.Drawing.Size(75, 23);
            this.QuitButton.TabIndex = 0;
            this.QuitButton.Text = "Quit";
            this.QuitButton.UseVisualStyleBackColor = true;
            this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // DesktopShortcutCheckBox
            // 
            this.DesktopShortcutCheckBox.AutoSize = true;
            this.DesktopShortcutCheckBox.Checked = true;
            this.DesktopShortcutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DesktopShortcutCheckBox.Location = new System.Drawing.Point(13, 13);
            this.DesktopShortcutCheckBox.Name = "DesktopShortcutCheckBox";
            this.DesktopShortcutCheckBox.Size = new System.Drawing.Size(139, 17);
            this.DesktopShortcutCheckBox.TabIndex = 1;
            this.DesktopShortcutCheckBox.Text = "Create desktop shortcut";
            this.DesktopShortcutCheckBox.UseVisualStyleBackColor = true;
            // 
            // TemporaryFilesCheckBox
            // 
            this.TemporaryFilesCheckBox.AutoSize = true;
            this.TemporaryFilesCheckBox.Checked = true;
            this.TemporaryFilesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TemporaryFilesCheckBox.Location = new System.Drawing.Point(12, 59);
            this.TemporaryFilesCheckBox.Name = "TemporaryFilesCheckBox";
            this.TemporaryFilesCheckBox.Size = new System.Drawing.Size(215, 17);
            this.TemporaryFilesCheckBox.TabIndex = 2;
            this.TemporaryFilesCheckBox.Text = "Remove Temporary Download Directory";
            this.TemporaryFilesCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartMenuShortcutCheckBox
            // 
            this.StartMenuShortcutCheckBox.AutoSize = true;
            this.StartMenuShortcutCheckBox.Checked = true;
            this.StartMenuShortcutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartMenuShortcutCheckBox.Location = new System.Drawing.Point(12, 36);
            this.StartMenuShortcutCheckBox.Name = "StartMenuShortcutCheckBox";
            this.StartMenuShortcutCheckBox.Size = new System.Drawing.Size(150, 17);
            this.StartMenuShortcutCheckBox.TabIndex = 3;
            this.StartMenuShortcutCheckBox.Text = "Create start menu shortcut";
            this.StartMenuShortcutCheckBox.UseVisualStyleBackColor = true;
            // 
            // Completed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 361);
            this.Controls.Add(this.StartMenuShortcutCheckBox);
            this.Controls.Add(this.TemporaryFilesCheckBox);
            this.Controls.Add(this.DesktopShortcutCheckBox);
            this.Controls.Add(this.QuitButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Completed";
            this.ShowIcon = false;
            this.Text = "Completed";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Welcome_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button QuitButton;
        private System.Windows.Forms.CheckBox DesktopShortcutCheckBox;
        private System.Windows.Forms.CheckBox TemporaryFilesCheckBox;
        private System.Windows.Forms.CheckBox StartMenuShortcutCheckBox;
    }
}