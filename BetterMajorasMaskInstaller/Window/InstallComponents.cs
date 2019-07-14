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

        public InstallerComponents Components { get; set; }

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
            string path = Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BetterMajorasMaskInstaller");

            string sevenZipExecutable = Path.Combine(path, "7za.exe");

            if (!File.Exists(sevenZipExecutable))
            {
                // extract 7za from executable
                File.WriteAllBytes(sevenZipExecutable, Properties.Resources._7za);
            }

            // aa
            string project64File = Path.Combine(path, "Project64.zip");

            // TODO, make this flexible
            string project64Path = Path.Combine(
                  Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                  "Project64");

            if (Directory.Exists(project64Path))
                Directory.CreateDirectory(project64Path);


            using (ArchiveFile archive = new ArchiveFile(project64File, sevenZipExecutable))
            {
                archive.OnProgressChange += (object source, int value) =>
                {
                    ChangeProgressBarValue(value);
                };
                archive.ExtractAll(project64Path);
            }

            foreach (InstallerComponent component in Components.Components)
            {
                string archiveFile = Path.Combine(path, component.Urls.First().FileName);

                Log($"Installing {component.Name}...");

                using (ArchiveFile archive = new ArchiveFile(archiveFile, sevenZipExecutable))
                {
                    foreach (KeyValuePair<string, string> extractFileInfo in component.Files)
                    {
                        archive.OnProgressChange += (object source, int value) =>
                        {
                            ChangeProgressBarValue(value);
                        };

                        string targetFile = Path.Combine(project64Path, extractFileInfo.Value);

                        bool extractSuccess = archive.ExtractFile(extractFileInfo.Key, targetFile);

                        if(!extractSuccess)
                        {
                            Log($"Installing {component.Name} Failed");
                            return;
                        }
                    }
                }
            }

            Log("Completed");

            new System.Media.SoundPlayer(Properties.Resources.MM_Notebook).PlaySync();
        }
    }
}
