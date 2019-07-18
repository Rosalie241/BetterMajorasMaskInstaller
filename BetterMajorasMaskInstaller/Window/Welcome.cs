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
            InstallerSettings.InstallDirectory = InstallDirectoryTextBox.Text;
            InstallerSettings.DownloadDirectory = DownloadDirectoryTextBox.Text;

            // change download directory when install & download directory 
            // are the same
            if(InstallerSettings.InstallDirectory == InstallerSettings.DownloadDirectory)
            {
                InstallerSettings.DownloadDirectory = Path.Combine(InstallerSettings.DownloadDirectory, 
                    "download_cache");
            }

            if(!Directory.Exists(InstallerSettings.DownloadDirectory))
            {
                DialogResult result = MessageBox.Show("Download directory doesn't exist, do you want it to be created?", 
                    "Info",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result != DialogResult.Yes)
                    return;

                try
                {
                    Directory.CreateDirectory(InstallerSettings.DownloadDirectory);
                }
                catch(Exception)
                {
                    MessageBox.Show("Failed to create download directory", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!Directory.Exists(InstallerSettings.InstallDirectory))
            {
                DialogResult result = MessageBox.Show("Install directory doesn't exist, do you want it to be created?",
                   "Info",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result != DialogResult.Yes)
                    return;

                try
                {
                    Directory.CreateDirectory(InstallerSettings.InstallDirectory);
                }
                catch(Exception)
                {
                    MessageBox.Show("Failed to create install directory", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

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
                return;
            }

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
                Description = "Project64 Install Directory"
            };

            // return when cancelled
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            // make sure selected path isn't nothing
            if (String.IsNullOrEmpty(dialog.SelectedPath))
                return;

            // make sure we have access to the directory
            if(!IsDirectoryAccessible(dialog.SelectedPath))
            {
                MessageBox.Show("Unable to open directory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // make sure download & install directory aren't the same
            if (dialog.SelectedPath == InstallerSettings.DownloadDirectory)
            {
                DownloadDirectoryTextBox.Text = InstallerSettings.DownloadDirectory 
                    = Path.Combine(InstallerSettings.DownloadDirectory,
                    "download_cache");
                Directory.CreateDirectory(InstallerSettings.DownloadDirectory);
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

            // make sure selected path isn't nothing
            if (String.IsNullOrEmpty(dialog.SelectedPath))
                return;

            // make sure download & install directory aren't the same
            if (dialog.SelectedPath == InstallerSettings.InstallDirectory)
            {
                dialog.SelectedPath = Path.Combine(dialog.SelectedPath, "download_cache");
                Directory.CreateDirectory(dialog.SelectedPath);
            }

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
