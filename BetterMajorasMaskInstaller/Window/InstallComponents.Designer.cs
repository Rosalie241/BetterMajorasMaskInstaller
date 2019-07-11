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
            this.SuspendLayout();
            // 
            // InstallProgressBar
            // 
            this.InstallProgressBar.Location = new System.Drawing.Point(12, 326);
            this.InstallProgressBar.Name = "InstallProgressBar";
            this.InstallProgressBar.Size = new System.Drawing.Size(560, 23);
            this.InstallProgressBar.TabIndex = 0;
            // 
            // InstallComponents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.InstallProgressBar);
            this.Name = "InstallComponents";
            this.Text = "InstallComponents";
            this.Load += new System.EventHandler(this.InstallComponents_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallComponents_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar InstallProgressBar;
    }
}