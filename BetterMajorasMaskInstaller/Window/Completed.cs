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

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            // create desktop shortcut
            if(DesktopShortcutCheckBox.Checked)
            {
                string desktopFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Project64.lnk");

                IWshShortcut shortcut = (IWshShortcut)new WshShell().CreateShortcut(desktopFilePath);

                shortcut.Description = "Project64 installed by MM Installer";
                shortcut.TargetPath = Path.Combine(
                    InstallerSettings.InstallDirectory,
                    "Project64.exe");

                shortcut.WorkingDirectory = InstallerSettings.InstallDirectory;

                shortcut.Save();  
            }

            if(TemporaryFilesCheckBox.Checked)
            {
                // delete each file from each component,
                // then delete the directory if it's empty
                foreach (InstallerComponent component in InstallerComponents.Components)
                {
                    foreach (UrlInfo file in component.Urls)
                    {
                        string targetFile = Path.Combine(InstallerSettings.DownloadDirectory,
                                    file.FileName);

                        if (File.Exists(targetFile))
                            File.Delete(targetFile);
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
