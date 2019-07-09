using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
                if (Downloader.CurrentComponent.FileSizes != null)
                    fileSize = Downloader.CurrentComponent.FileSizes[Downloader.ComponentDownloadIndex];

            }
            catch(Exception)
            {
                // ignore
            }

            if(fileSize == null)
                ChangeProgressBarValue(a.ProgressPercentage);
            else
                ChangeProgressBarValue((int)(a.BytesReceived / 1024 / 1024), (int)(fileSize / 1024 / 1024));
           
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
        private DownloadComponents dc = new DownloadComponents();

        private void DownloadAllComponents()
        {
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "BetterMajorasMaskInstaller");

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            Downloader = new ComponentDownloader();
            Downloader.OnDownloadProgressChanged += OnDownloadProgressChanged;
            Downloader.RegisterEvents();

            string project64FileName = Path.Combine(downloadPath, "Project64.zip");


            dc.Components = new List<DownloadComponent>();
            dc.Components.Add(new DownloadComponent
            {
                Name = "Azimer's Audio Plugin",
                Urls = new Dictionary<string, string>()
                {
                    { "https://github.com/Azimer/AziAudio/files/1591596/AziAudio.v0.70.WIP10.zip", "AziAudio.zip" }
                },
                FileSizes = new int?[]
                {
                    96221
                },
                Files = new Dictionary<string, string>()
                {
                    { "AziAudio v0.70 WIP10.dll", @"Plugin\Audio\AziAudio v0.70 WIP10.dll" }
                }
            });
            dc.Components.Add(new DownloadComponent
            {
                Name = "GlideN64 GFX Plugin",
                Urls = new Dictionary<string, string>()
                {
                    { "gdrive:14fgffHHxnnNULRwf4c-4pY1scV6SOITc",
                        "GLideN64.7z" }
                },
                FileSizes = new int?[]
                {
                    5003466
                },
                Files = new Dictionary<string, string>()
                {
                    { @"GLideN64\Zilmar-specs\GLideN64.dll", @"Plugin\GFX\GLideN64.dll" },
                    { @"GLideN64\Zilmar-specs\GLideN64.custom.ini", @"Plugin\GFX\GLideN64.custom.ini" }
                }
            });
            dc.Components.Add(new DownloadComponent
            {
                Name = "MM HD Texture Pack",
                Urls = new Dictionary<string, string>()
                {
                    { "gdrive:1B_XetHFdS-Nx15rvzhxfVep81aSFJ0bJ",
                        "mmhd.zip.001" },
                    { "gdrive:103P0snXU3j1kmNMA0gELc2BkEL3Mq7sA",
                        "mmhd.zip.002" },
                    { "gdrive:1rBCOYsZnuqVIDoQk1TOjWfd6Zy0itcxB",
                        "mmhd.zip.003" }
                },
                FileSizes = new int?[]
                {
                    943718400,
                    943718400,
                    609021060
                },
                Files = new Dictionary<string, string>()
                {
                    { "ZELDA MAJORA'S MASK_HIRESTEXTURES.htc", @"Plugin\GFX\cache\ZELDA MAJORA'S MASK_HIRESTEXTURES.htc" }
                }
            });

            File.WriteAllText(downloadPath + @"\" + "config.json", JsonConvert.SerializeObject(dc));

            if (!File.Exists(project64FileName))
            {
                ChangeProgressBarValue(0);
                Log("Downloading Project64...");
                Downloader.Project64(project64FileName);
            }

            foreach (DownloadComponent component in dc.Components)
            {
                ChangeProgressBarValue(0);
                Log($"Downloading {component.Name}...");
                Downloader.DownloadComponent(component, downloadPath);
                    
                if (Downloader.Failed)
                {
                    Log($"Downloading {component.Name} Failed");
                    return;
                }
            }

            Log("Downloading completed");

            /*
            using(WebClient client = new WebClient())
            {
                Log(client.DownloadString("https://drive.google.com/uc?id=1B_XetHFdS-Nx15rvzhxfVep81aSFJ0bJ&export=download"));
            }
            */
           // LaunchInstallComponents();
        }
        private void LaunchInstallComponents()
        {
            if(this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { LaunchInstallComponents(); });
                return;
            }

            this.Hide();
            new InstallComponents() { DownloadComponents = dc,  StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void Log(string text)
        {   
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate () { Log(text); });
            else
                LogBox.Text += text + Environment.NewLine;
        }
        private void DownloadInstallComponents_Load(object sender, EventArgs e)
        {

        }

        private void LogBox_TextChanged(object sender, EventArgs e)
        {
                    
        }
    }
}
