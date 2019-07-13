using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class DownloadInstallComponents : Form
    {
        public DownloadInstallComponents()
        {
            InitializeComponent();

            Task.Factory.StartNew(() => DownloadAllComponents());
        }


        private void OnDownloadProgressChanged(object source, DownloadStatusChangedEventArgs a)
        {
            int? fileSize = null;

            // TODO
            try
            {
                if (Downloader.CurrentComponent.Urls != null)
                    fileSize = Downloader.CurrentComponent.Urls[Downloader.ComponentDownloadIndex].FileSize;

            }
            catch(Exception)
            {
                // ignore
            }

            if (fileSize == null)
                ChangeProgressBarValue(a.ProgressPercentage);
            else
                ChangeProgressBarValue((int)a.BytesReceived, (int)fileSize);
           
        }

        private void ChangeProgressBarValue(int value, int maxValue = 100)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { ChangeProgressBarValue(value, maxValue); });
                return;
            }

            // if this happens, something is wrong
            // I don't care though :)
            // not my problem
            if (value > maxValue)
                return;

            progressBar1.Maximum = maxValue;
            progressBar1.Value = value;
        }
        private ComponentDownloader Downloader { get; set; }
        private InstallerComponents InstallerComponents = new InstallerComponents();

        private void DownloadAllComponents()
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "BetterMajorasMaskInstaller");

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            Downloader = new ComponentDownloader();
            Downloader.OnDownloadProgressChanged += OnDownloadProgressChanged;

            string configUrl = "https://raw.githubusercontent.com/tim241/BetterMajorasMaskInstaller-cfg/master/config.json";

            // Download the Installer Configuration
            // and parse it
            try
            {
                Log("Downloading Installer Configuration...");
                InstallerComponents = JsonConvert.DeserializeObject<InstallerComponents>(
                                                new WebClient().DownloadString(configUrl));
            }
            catch (Exception e)
            {
                Log("Downloading Installer Configuration Failed");
                Log(e.Message);
                Log(e.StackTrace);
                return;
            }

            string project64FileName = Path.Combine(downloadPath, "Project64.zip");
            
            // we *sadly* need to do something special for Project64
            if (!File.Exists(project64FileName))
            {
                ChangeProgressBarValue(0);
                Log("Downloading Project64...");
                Downloader.Project64(project64FileName);

                if(Downloader.Failed)
                {
                    Log("Downloading Project64 Failed");
                    Log(Downloader.Exception.Message);
                    Log(Downloader.Exception.StackTrace);                    
                }
            }

            foreach (InstallerComponent component in InstallerComponents.Components)
            {
                ChangeProgressBarValue(0);
                Log($"Downloading {component.Name}...");
                Downloader.DownloadComponent(component, downloadPath);
                
                if (Downloader.Failed)
                {
                    Log($"Downloading {component.Name} Failed");

                    // log Exception aswell, if it exists
                    if (Downloader.Exception != null)
                    {
                        Log(Downloader.Exception.Message);
                        Log(Downloader.Exception.StackTrace);
                    }
                    
                    return;
                }
            }

            Log("Downloading completed");

            LaunchInstallComponents();
        }
        private void LaunchInstallComponents()
        {
            if(this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { LaunchInstallComponents(); });
                return;
            }

            this.Hide();
            new InstallComponents() { Components = InstallerComponents, StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void Log(string text)
        {   
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate () { Log(text); });
            else
                LogBox.Text += text + Environment.NewLine;
        }
        private void DownloadInstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
