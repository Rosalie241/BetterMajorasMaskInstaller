using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LicenseAgreement() { StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void Welcome_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
