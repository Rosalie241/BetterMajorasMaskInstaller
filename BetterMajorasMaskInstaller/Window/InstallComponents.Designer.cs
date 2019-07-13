namespace BetterMajorasMaskInstaller.Window
{
    partial class InstallComponents
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
            this.InstallProgressBar = new System.Windows.Forms.ProgressBar();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // InstallProgressBar
            // 
            this.InstallProgressBar.Location = new System.Drawing.Point(12, 326);
            this.InstallProgressBar.Name = "InstallProgressBar";
            this.InstallProgressBar.Size = new System.Drawing.Size(560, 23);
            this.InstallProgressBar.TabIndex = 0;
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(81, 20);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogBox.Size = new System.Drawing.Size(420, 300);
            this.LogBox.TabIndex = 1;
            // 
            // InstallComponents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.InstallProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallComponents";
            this.Text = "InstallComponents";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallComponents_Closing);
            this.Load += new System.EventHandler(this.InstallComponents_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar InstallProgressBar;
        private System.Windows.Forms.TextBox LogBox;
    }
}