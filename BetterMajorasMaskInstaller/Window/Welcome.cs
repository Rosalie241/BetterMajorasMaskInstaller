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

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            InstallDirectoryTextBox.Text = InstallerSettings.InstallDirectory;
        }
        public InstallerComponents InstallerComponents { get; set; }
        private void QuitButton_Click(object sender, EventArgs e) => Application.Exit();

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            // fetch configuration file
            try
            {
                InstallerComponents = JsonConvert.DeserializeObject<InstallerComponents>(
                                                    new WebClient().DownloadString(
                                                        InstallerSettings.ConfigurationUrl));
            }
            catch(Exception)
            {
                MessageBox.Show("Failed to fetch configuration file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            this.Hide();
            new LicenseAgreement() { InstallerComponents = InstallerComponents,
                StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void Welcome_Closing(object sender, CancelEventArgs args) => Application.Exit();

        /// <summary>
        /// Checks whether we can write to the given directory
        /// </summary>
        private bool IsDirectoryAccessible(string directory)
        {
            try
            {
                File.Create(
                    Path.Combine(directory,
                        Path.GetRandomFileName()), 
                        1, 
                        FileOptions.DeleteOnClose
                        ).Close();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        private void ChangeInstallDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "MM HD Install Directory"
            };

            // return when cancelled
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            // make sure we have access to the directory
            if(!IsDirectoryAccessible(dialog.SelectedPath))
            {
                MessageBox.Show("Unable to open directory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ContinueButton.Enabled = false;
                return;
            }

            InstallDirectoryTextBox.Text = 
                InstallerSettings.InstallDirectory = dialog.SelectedPath;

            ContinueButton.Enabled = true;

        }
    }
}
