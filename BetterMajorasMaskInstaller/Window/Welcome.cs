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
using System.Linq;
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
            DownloadDirectoryTextBox.Text = InstallerSettings.DownloadDirectory;
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
            catch(Exception ex)
            {
                // log exception
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace);
                MessageBox.Show("Failed to fetch configuration file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // log configuration
            Logger.Log(JsonConvert.SerializeObject(InstallerComponents));

            // disk space in mb
            long diskSpaceRequired = 0;

            // calculate disk space required in mb
            foreach(InstallerComponent component in InstallerComponents.Components)
            {
                foreach (int size in component.Urls.Select(x => x.FileSize))
                    diskSpaceRequired += size / 1024 / 1024 * 4;
            }
            
            // verify disk space in mb
            foreach (DriveInfo driveInfo in new DriveInfo[] {
                new DriveInfo(InstallerSettings.DownloadDirectory),
                new DriveInfo(InstallerSettings.InstallDirectory)})
            {
                if ((driveInfo.TotalFreeSpace / 1024 / 1024) <= diskSpaceRequired)
                {
                    MessageBox.Show("Not enough free disk space!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
           
            this.Hide();

            new SelectInstallComponents(InstallerComponents)
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            }.Show();
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
                return;
            }

            InstallDirectoryTextBox.Text = 
                InstallerSettings.InstallDirectory = dialog.SelectedPath;
        }

        private void ChangeDownloadDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = "Download Cache Directory"
            };

            // return when cancelled
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            // make sure we have access to the directory
            if (!IsDirectoryAccessible(dialog.SelectedPath))
            {
                MessageBox.Show("Unable to open directory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DownloadDirectoryTextBox.Text =
                InstallerSettings.DownloadDirectory = dialog.SelectedPath;
        }
    }
}
