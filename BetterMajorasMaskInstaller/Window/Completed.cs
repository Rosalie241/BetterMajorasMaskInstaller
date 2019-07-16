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

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Completed : Form
    {
        public Completed()
        {
            InitializeComponent();
        }
        public void Welcome_Closing(object sender, CancelEventArgs e) => QuitButton_Click(this, null);

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
                if(Directory.Exists(InstallerSettings.DownloadDirectory))
                    Directory.Delete(InstallerSettings.DownloadDirectory, true);
            }

            Application.Exit();
        }
    }
}
