/*
 BetterMajorasMaskInstaller - https://github.com/tim241/BetterMajorasMaskInstaller
 Copyright (C) 2019 Tim Wanders <tim241@mailbox.org>
 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class LicenseAgreement : Form
    {
        public LicenseAgreement()
        {
            InitializeComponent();
        }
        public InstallerComponents InstallerComponents { get; set; }
        private void DisagreeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AgreeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new SelectInstallComponents(InstallerComponents) {
                StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void LicenseAgreement_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
