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

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SevenZipSharp;
using System.Collections.Generic;
using System.Linq;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class InstallComponents : Form
    {
        public InstallComponents()
        {
            InitializeComponent();
        }

        public InstallerComponents InstallerComponents { get; set; }

        private void InstallComponents_Load(object sender, EventArgs args)
        {
            Task.Factory.StartNew(() => Install());
        }
        private void InstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();

        private void ChangeProgressBarValue(int value)
        {
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate () { ChangeProgressBarValue(value); });
            else
                InstallProgressBar.Value = value;
        }
        private void Log(string text)
        {
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate () { Log(text); });
            else
                LogBox.Text += text + Environment.NewLine;
        }
        private void Install()
        {
            string sevenZipExecutable = Path.Combine(InstallerSettings.DownloadDirectory, "7za.exe");

            if (!File.Exists(sevenZipExecutable))
            {
                // extract 7za from executable
                File.WriteAllBytes(sevenZipExecutable, Properties.Resources._7za);
            }

            // store the patches for later
            // we'll apply them when everything is installed
            Dictionary<string, KeyValuePair<string, string>> patchList
                = new Dictionary<string, KeyValuePair<string, string>>();

            foreach (InstallerComponent component in InstallerComponents.Components)
            {
                // if it's disabled,
                // skip it
                if (!component.Enabled)
                    continue;

                ChangeProgressBarValue(0);
                Log($"Installing {component.Name}...");

                if (component.Patches != null)
                {
                    // store patches in a dictionary, 
                    // we're gonna execute them when everything is installed
                    foreach (KeyValuePair<string, KeyValuePair<string, string>> patch in component.Patches)
                    {
                        // overwrite key value if already exists
                        if (patchList.ContainsKey(patch.Key))
                            patchList[patch.Key] = patch.Value;
                        else
                            patchList.Add(patch.Key, patch.Value);
                    }
                }

                // if it's not an archive, 
                // just copy the files
                if (!component.Archive)
                {
                    foreach (KeyValuePair<string, string> cFiles in component.Files)
                    {
                        string sourceFile = Path.Combine(InstallerSettings.DownloadDirectory,
                            cFiles.Key);
                        string targetFile = Path.Combine(InstallerSettings.InstallDirectory,
                            cFiles.Value);

                        try
                        {
                            File.Copy(sourceFile, targetFile, true);
                        }
                        catch (Exception e)
                        {
                            Log($"Installing {component.Name} Failed");
                            Log(e.Message);
                            Log(e.StackTrace);
                            return;
                        }
                    }

                    continue;
                }

                // if it's an archive, extract each file
                // in the right location

                string archiveFile = Path.Combine(InstallerSettings.DownloadDirectory,
                    component.Urls.First().FileName);

                using (ArchiveFile archive = new ArchiveFile(archiveFile, sevenZipExecutable))
                {
                    if (component.ExtractAll)
                    {
                        bool extractSuccess = archive.ExtractAll(InstallerSettings.InstallDirectory);

                        if (!extractSuccess)
                        {
                            Log($"Installing {component.Name} Failed");

                            if (archive.Exception != null)
                            {
                                Log(archive.Exception.Message);
                                Log(archive.Exception.StackTrace);
                            }

                            return;
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, string> extractFileInfo in component.Files)
                        {
                            archive.OnProgressChange += (object source, int value) =>
                            {
                                ChangeProgressBarValue(value);
                            };

                            string targetFile = Path.Combine(InstallerSettings.InstallDirectory,
                                extractFileInfo.Value);

                            bool extractSuccess = archive.ExtractFile(extractFileInfo.Key, targetFile);

                            if (extractSuccess)
                                continue;

                            Log($"Installing {component.Name} Failed");

                            if (archive.Exception != null)
                            {
                                Log(archive.Exception.Message);
                                Log(archive.Exception.StackTrace);
                            }

                            return;
                        }
                    }
                }
            }

            // execute the 'patches'
            // these 'patches' only allow us to replace text
            // so just read the file, replace the text and write the file
            try
            {
                foreach (KeyValuePair<string, KeyValuePair<string, string>> patch in patchList)
                {
                    string file = Path.Combine(InstallerSettings.InstallDirectory, patch.Key);

                    File.WriteAllText(file, File.ReadAllText(file)
                                                .Replace(patch.Value.Key, patch.Value.Value));
                }
            }
            catch (Exception e)
            {
                Log("Patching Failed");
                Log(e.Message);
                Log(e.StackTrace);
                return;
            }

            Log("Completed");

            // launch post-install screen
            LaunchCompleted();
        }

        public void LaunchCompleted()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { LaunchCompleted(); });
                return;
            }

            this.Hide();

            new Completed()
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                InstallerComponents = this.InstallerComponents
            }.Show();
        }
    }
}
