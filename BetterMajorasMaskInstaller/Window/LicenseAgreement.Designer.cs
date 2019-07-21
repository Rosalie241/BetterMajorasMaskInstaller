namespace BetterMajorasMaskInstaller.Window
{
    partial class LicenseAgreement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseAgreement));
            this.AgreeButton = new System.Windows.Forms.Button();
            this.DisagreeButton = new System.Windows.Forms.Button();
            this.LicenseBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // AgreeButton
            // 
            this.AgreeButton.Location = new System.Drawing.Point(497, 326);
            this.AgreeButton.Name = "AgreeButton";
            this.AgreeButton.Size = new System.Drawing.Size(75, 23);
            this.AgreeButton.TabIndex = 1;
            this.AgreeButton.Text = "I Agree";
            this.AgreeButton.UseVisualStyleBackColor = true;
            this.AgreeButton.Click += new System.EventHandler(this.AgreeButton_Click);
            // 
            // DisagreeButton
            // 
            this.DisagreeButton.Location = new System.Drawing.Point(12, 326);
            this.DisagreeButton.Name = "DisagreeButton";
            this.DisagreeButton.Size = new System.Drawing.Size(75, 23);
            this.DisagreeButton.TabIndex = 2;
            this.DisagreeButton.Text = "I Disagree";
            this.DisagreeButton.UseVisualStyleBackColor = true;
            this.DisagreeButton.Click += new System.EventHandler(this.DisagreeButton_Click);
            // 
            // LicenseBox
            // 
            this.LicenseBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.LicenseBox.Location = new System.Drawing.Point(81, 20);
            this.LicenseBox.Multiline = true;
            this.LicenseBox.Name = "LicenseBox";
            this.LicenseBox.ReadOnly = true;
            this.LicenseBox.ShortcutsEnabled = false;
            this.LicenseBox.Size = new System.Drawing.Size(420, 300);
            this.LicenseBox.TabIndex = 0;
            this.LicenseBox.Text = resources.GetString("LicenseBox.Text");
            this.LicenseBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LicenseAgreement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.DisagreeButton);
            this.Controls.Add(this.AgreeButton);
            this.Controls.Add(this.LicenseBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseAgreement";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LicenseAgreement_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button AgreeButton;
        private System.Windows.Forms.Button DisagreeButton;
        private System.Windows.Forms.TextBox LicenseBox;
    }
}