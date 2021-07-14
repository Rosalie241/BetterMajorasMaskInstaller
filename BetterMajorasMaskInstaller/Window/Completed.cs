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
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using File = System.IO.File;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Completed : Form
    {
        public Completed()
        {
            InitializeComponent();
        }
        public void Welcome_Closing(object sender, CancelEventArgs e) => QuitButton_Click(this, null);
        public InstallerComponents InstallerComponents { get; set; }

        private void CreateShortcut(string path)
        {
            IWshShortcut shortcut = (IWshShortcut)new WshShell().CreateShortcut(path);

            shortcut.Description = "Project64 installed by BetterMajorasMaskInstaller";
            shortcut.TargetPath = Path.Combine(
                InstallerSettings.InstallDirectory,
                "PJ64Launcher.exe");

            shortcut.IconLocation = Path.Combine(
                InstallerSettings.InstallDirectory,
                "Project64.exe");

            shortcut.WorkingDirectory = InstallerSettings.InstallDirectory;

            shortcut.Save();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            // create desktop shortcut
            if (DesktopShortcutCheckBox.Checked)
            {
                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Project64 HD.lnk")
                );
            }

            // create start menu shortcut
            if (StartMenuShortcutCheckBox.Checked)
            {
                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Project64 HD.lnk")
                );
            }

            if (TemporaryFilesCheckBox.Checked)
            {
                // delete each file from each component,
                // then delete the directory if it's empty
                foreach (InstallerComponent component in InstallerComponents.Components)
                {
                    var urls = component.Urls.First().FileName == null ?
                                component.FallbackUrls :
                                component.Urls;

                    foreach (UrlInfo file in urls)
                    {
                        string targetFile = Path.Combine(InstallerSettings.DownloadDirectory,
                                    file.FileName);

                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                        }
                    }
                }

                // also delete extracted 7za.exe and project64.zip
                foreach (string file in new string[] { "7za.exe", "Project64.zip" })
                {
                    string targetFile = Path.Combine(InstallerSettings.DownloadDirectory, file);

                    if (File.Exists(targetFile))
                        File.Delete(targetFile);
                }

                // hmm, the dir can be gone? 
                if (Directory.Exists(InstallerSettings.DownloadDirectory))
                {
                    // make sure directory is empty
                    if (!Directory.EnumerateFileSystemEntries(InstallerSettings.DownloadDirectory).Any())
                    {
                        Directory.Delete(InstallerSettings.DownloadDirectory, true);
                    }
                }
            }

            Application.Exit();
        }
    }
}
