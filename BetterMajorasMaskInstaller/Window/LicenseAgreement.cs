using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class LicenseAgreement : Form
    {
        public LicenseAgreement()
        {
            InitializeComponent();
        }

        private void DisagreeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AgreeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new SelectInstallComponents() { StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
    }
}
