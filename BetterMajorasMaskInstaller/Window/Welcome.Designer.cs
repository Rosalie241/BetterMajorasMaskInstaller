namespace BetterMajorasMaskInstaller.Window
{
    partial class Welcome
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
            this.ContinueButton = new System.Windows.Forms.Button();
            this.QuitButton = new System.Windows.Forms.Button();
            this.InstallDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ChangeInstallDirectoryButton = new System.Windows.Forms.Button();
            this.DownloadDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ChangeDownloadDirectoryButton = new System.Windows.Forms.Button();
            this.ConfigurationUrlTextBox = new System.Windows.Forms.TextBox();
            this.DeveloperModeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ContinueButton
            // 
            this.ContinueButton.Location = new System.Drawing.Point(497, 326);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(75, 23);
            this.ContinueButton.TabIndex = 0;
            this.ContinueButton.Text = "Continue";
            this.ContinueButton.UseVisualStyleBackColor = true;
            this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
            // 
            // QuitButton
            // 
            this.QuitButton.Location = new System.Drawing.Point(12, 326);
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new System.Drawing.Size(75, 23);
            this.QuitButton.TabIndex = 1;
            this.QuitButton.Text = "Quit";
            this.QuitButton.UseVisualStyleBackColor = true;
            this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // InstallDirectoryTextBox
            // 
            this.InstallDirectoryTextBox.Location = new System.Drawing.Point(12, 40);
            this.InstallDirectoryTextBox.Name = "InstallDirectoryTextBox";
            this.InstallDirectoryTextBox.Size = new System.Drawing.Size(350, 20);
            this.InstallDirectoryTextBox.TabIndex = 2;
            // 
            // ChangeInstallDirectoryButton
            // 
            this.ChangeInstallDirectoryButton.Location = new System.Drawing.Point(368, 39);
            this.ChangeInstallDirectoryButton.Name = "ChangeInstallDirectoryButton";
            this.ChangeInstallDirectoryButton.Size = new System.Drawing.Size(204, 21);
            this.ChangeInstallDirectoryButton.TabIndex = 3;
            this.ChangeInstallDirectoryButton.Text = "Change Project64 Install Directory";
            this.ChangeInstallDirectoryButton.UseVisualStyleBackColor = true;
            this.ChangeInstallDirectoryButton.Click += new System.EventHandler(this.ChangeInstallDirectoryButton_Click);
            // 
            // DownloadDirectoryTextBox
            // 
            this.DownloadDirectoryTextBox.Location = new System.Drawing.Point(12, 12);
            this.DownloadDirectoryTextBox.Name = "DownloadDirectoryTextBox";
            this.DownloadDirectoryTextBox.Size = new System.Drawing.Size(350, 20);
            this.DownloadDirectoryTextBox.TabIndex = 4;
            // 
            // ChangeDownloadDirectoryButton
            // 
            this.ChangeDownloadDirectoryButton.Location = new System.Drawing.Point(368, 12);
            this.ChangeDownloadDirectoryButton.Name = "ChangeDownloadDirectoryButton";
            this.ChangeDownloadDirectoryButton.Size = new System.Drawing.Size(204, 21);
            this.ChangeDownloadDirectoryButton.TabIndex = 5;
            this.ChangeDownloadDirectoryButton.Text = "Change Temporary Download Directory";
            this.ChangeDownloadDirectoryButton.UseVisualStyleBackColor = true;
            this.ChangeDownloadDirectoryButton.Click += new System.EventHandler(this.ChangeDownloadDirectoryButton_Click);
            // 
            // ConfigurationUrlTextBox
            // 
            this.ConfigurationUrlTextBox.Location = new System.Drawing.Point(12, 66);
            this.ConfigurationUrlTextBox.Name = "ConfigurationUrlTextBox";
            this.ConfigurationUrlTextBox.Size = new System.Drawing.Size(560, 20);
            this.ConfigurationUrlTextBox.TabIndex = 6;
            this.ConfigurationUrlTextBox.Visible = false;
            // 
            // DeveloperModeCheckBox
            // 
            this.DeveloperModeCheckBox.AutoSize = true;
            this.DeveloperModeCheckBox.Location = new System.Drawing.Point(12, 303);
            this.DeveloperModeCheckBox.Name = "DeveloperModeCheckBox";
            this.DeveloperModeCheckBox.Size = new System.Drawing.Size(105, 17);
            this.DeveloperModeCheckBox.TabIndex = 7;
            this.DeveloperModeCheckBox.Text = "Developer Mode";
            this.DeveloperModeCheckBox.UseVisualStyleBackColor = true;
            this.DeveloperModeCheckBox.CheckedChanged += new System.EventHandler(this.DeveloperModeCheckBox_CheckedChanged);
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.DeveloperModeCheckBox);
            this.Controls.Add(this.ConfigurationUrlTextBox);
            this.Controls.Add(this.ChangeDownloadDirectoryButton);
            this.Controls.Add(this.DownloadDirectoryTextBox);
            this.Controls.Add(this.ChangeInstallDirectoryButton);
            this.Controls.Add(this.InstallDirectoryTextBox);
            this.Controls.Add(this.QuitButton);
            this.Controls.Add(this.ContinueButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Welcome";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Welcome_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ContinueButton;
        private System.Windows.Forms.Button QuitButton;
        private System.Windows.Forms.TextBox InstallDirectoryTextBox;
        private System.Windows.Forms.Button ChangeInstallDirectoryButton;
        private System.Windows.Forms.TextBox DownloadDirectoryTextBox;
        private System.Windows.Forms.Button ChangeDownloadDirectoryButton;
        private System.Windows.Forms.TextBox ConfigurationUrlTextBox;
        private System.Windows.Forms.CheckBox DeveloperModeCheckBox;
    }
}