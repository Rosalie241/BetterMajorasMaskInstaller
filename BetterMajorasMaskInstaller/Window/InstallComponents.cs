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
            {
                this.Invoke((MethodInvoker)delegate () { ChangeProgressBarValue(value); });
                return;
            }

            InstallProgressBar.Value = value;
        }
        private void Install()
        {
            string path = @"C:\Users\Tim\AppData\Local\BetterMajorasMaskInstaller";
            string project64File = Path.Combine(path, "Project64.zip");

            // TODO, make this flexible
            string project64Path = Path.Combine(
                  Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                  "Project64");

            if (Directory.Exists(project64Path))
                Directory.CreateDirectory(project64Path);


            using(ArchiveFile archive = new ArchiveFile(project64File))
            {
                archive.OnProgressChange += (object source, int value) =>
                {
                    ChangeProgressBarValue(value);
                };
                archive.ExtractAll(project64Path);
            }

            foreach(InstallerComponent component in Components.Components)
            {
                string archiveFile = Path.Combine(path, component.Urls.First().Value);
                using (ArchiveFile archive = new ArchiveFile(archiveFile))
                {
                    foreach (KeyValuePair<string, string> extractFileInfo in component.Files)
                    {
                        archive.OnProgressChange += (object source, int value) =>
                        {
                            ChangeProgressBarValue(value);
                        };
                        string targetFile = Path.Combine(project64Path, extractFileInfo.Value);
                        archive.ExtractFile(extractFileInfo.Key, targetFile);
                    }
                }
            }

        }
    }
}
