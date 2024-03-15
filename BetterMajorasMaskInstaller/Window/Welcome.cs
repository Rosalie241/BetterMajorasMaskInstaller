/*
 BetterMajorasMaskInstaller - https://github.com/Rosalie241/BetterMajorasMaskInstaller
 Copyright (C) 2020 Rosalie Wanders <rosalie@mailbox.org>
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
using WK.Libraries.BetterFolderBrowserNS;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            InstallDirectoryTextBox.Text = InstallerSettings.InstallDirectory;
            DownloadDirectoryTextBox.Text = InstallerSettings.DownloadDirectory;
            ConfigurationUrlTextBox.Text = InstallerSettings.ConfigurationUrl;
            DeveloperModeCheckBox.Checked = InstallerSettings.DeveloperMode;
            CleanupInstallationFilesOnFailureCheckBox.Checked = InstallerSettings.CleanupInstallationFilesOnFailure;
        }

        private void QuitButton_Click(object sender, EventArgs e) => Application.Exit();

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            InstallerSettings.InstallDirectory = Path.GetFullPath(InstallDirectoryTextBox.Text);
            InstallerSettings.DownloadDirectory = Path.GetFullPath(DownloadDirectoryTextBox.Text);
            InstallerSettings.ConfigurationUrl = ConfigurationUrlTextBox.Text;

            // change download directory when install & download directory 
            // are the same
            if (InstallerSettings.InstallDirectory ==
                InstallerSettings.DownloadDirectory)
                InstallerSettings.DownloadDirectory = Path.Combine(InstallerSettings.DownloadDirectory,
                    "temporary_download_cache");

            // make sure the installation directory is empty
            if (!IsDirectoryEmpty(InstallerSettings.InstallDirectory))
            {
                MessageBox.Show("Please choose an empty installation directory", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ask the user if they want to create the download & install directory
            // if they don't exist yet
            if (!AskCreateDirectory("Temporary Download", InstallerSettings.DownloadDirectory) ||
                !AskCreateDirectory("Project64 Install", InstallerSettings.InstallDirectory))
            {
                return;
            }

            // fetch configuration file
#if !DEBUG
            try
            {
#endif
            if (InstallerSettings.InstallerConfiguration == null || InstallerSettings.DeveloperMode)
            {
                InstallerSettings.InstallerConfiguration = JsonConvert.DeserializeObject<InstallerConfiguration>(
                                                            new WebClient().DownloadString(
                                                                InstallerSettings.ConfigurationUrl
                                                        ));

                if (DumpConfigFileCheckBox.Checked)
                {
                    File.WriteAllText("config.json", JsonConvert.SerializeObject(InstallerSettings.InstallerConfiguration, Formatting.Indented));
                }
            }
#if !DEBUG
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to fetch configuration file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif
            this.Hide();

            new SelectInstallComponents()
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            }.Show();
        }

        private void Welcome_Closing(object sender, CancelEventArgs args) => Application.Exit();

        /// <summary>
        ///     When the directory doesn't exist yet, ask the user if they want to create it
        /// </summary>
        private bool AskCreateDirectory(string directoryName, string directory)
        {
            // return when the directory already exists
            if (Directory.Exists(directory))
            {
                return true;
            }

            DialogResult result = MessageBox.Show($"{directoryName} directory doesn't exist, do you want it to be created?",
                   "Info",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
            {
                return false;
            }

            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to create {directoryName} directory", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks whether given directory is empty
        /// </summary>
        private bool IsDirectoryEmpty(string directory)
        {
            try
            {
                // we need to make sure that the download directory
                // isn't inside the installation directory, if it is
                // just ignore that 1 directory
                int maxDirectoriesAllowed = 0;
                if (InstallerSettings.DownloadDirectory ==
                    Path.Combine(directory, "temporary_download_cache"))
                {
                    maxDirectoriesAllowed++;
                }

                DirectoryInfo dInfo = new DirectoryInfo(directory);

                return dInfo.GetFiles().Length == 0 &&
                    dInfo.GetDirectories().Length == maxDirectoriesAllowed;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        ///     Checks whether we can write to the given directory
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
            catch (Exception)
            {
                return false;
            }
        }


        private void ChangeInstallDirectoryButton_Click(object sender, EventArgs e)
        {
            var betterFolderBrowser = new BetterFolderBrowser();

            betterFolderBrowser.Title = "Project64 Install Directory";
            betterFolderBrowser.RootFolder = "C:\\"; // Adjust the root folder as needed

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false; // Adjust according to your needs

            // If you'd prefer blocking the main UI thread when calling the dialog,
            // specify the window owner of the dialog using the ShowDialog(IWin32Window) method:
            if (betterFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                string selectedFolder = betterFolderBrowser.SelectedFolder;

                // make sure selected path isn't nothing
                if (String.IsNullOrEmpty(selectedFolder))
                {
                    return;
                }

                // make sure we have access to the directory
                if (!IsDirectoryAccessible(selectedFolder))
                {
                    MessageBox.Show("Unable to open directory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // make sure download & install directory aren't the same
                if (selectedFolder == InstallerSettings.DownloadDirectory)
                {
                    DownloadDirectoryTextBox.Text = InstallerSettings.DownloadDirectory
                        = Path.Combine(InstallerSettings.DownloadDirectory,
                        "temporary_download_cache");
                    Directory.CreateDirectory(InstallerSettings.DownloadDirectory);
                }

                InstallDirectoryTextBox.Text = InstallerSettings.InstallDirectory = selectedFolder;
            }
        }

        private void ChangeDownloadDirectoryButton_Click(object sender, EventArgs e)
        {
            var betterFolderBrowser = new BetterFolderBrowser();

            betterFolderBrowser.Title = "Temporary Download Directory";
            betterFolderBrowser.RootFolder = "C:\\"; // Adjust the root folder as needed

            // Allow multi-selection of folders.
            betterFolderBrowser.Multiselect = false; // Adjust according to your needs

            // If you'd prefer blocking the main UI thread when calling the dialog,
            // specify the window owner of the dialog using the ShowDialog(IWin32Window) method:
            if (betterFolderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                string selectedFolder = betterFolderBrowser.SelectedFolder;

                // make sure selected path isn't nothing
                if (String.IsNullOrEmpty(selectedFolder))
                {
                    return;
                }

                // make sure download & install directory aren't the same
                if (selectedFolder == InstallerSettings.InstallDirectory)
                {
                    selectedFolder = Path.Combine(selectedFolder, "temporary_download_cache");
                    Directory.CreateDirectory(selectedFolder);
                }

                // make sure we have access to the directory
                if (!IsDirectoryAccessible(selectedFolder))
                {
                    MessageBox.Show("Unable to open directory!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DownloadDirectoryTextBox.Text = InstallerSettings.DownloadDirectory = selectedFolder;
            }
        }

        private void DeveloperModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool visible = InstallerSettings.DeveloperMode = DeveloperModeCheckBox.Checked;

            ConfigurationUrlTextBox.Visible = visible;
            ConfigurationUrlLabel.Visible = visible;
            DumpConfigFileCheckBox.Visible = visible;
            CleanupInstallationFilesOnFailureCheckBox.Visible = visible;
        }

        private void CleanupInstallationFilesOnFailureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            InstallerSettings.CleanupInstallationFilesOnFailure = CleanupInstallationFilesOnFailureCheckBox.Checked;
        }
    }
}
