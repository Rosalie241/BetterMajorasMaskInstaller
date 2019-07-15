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

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            double fileSize = 0;
            double bytesReceived = a.BytesReceived;

            // when we've been given the percentage
            // just use that and return
            if (a.ProgressPercentage != null)
            {
                ChangeProgressBarValue((int)a.ProgressPercentage);
                return;
            }

            // get total filesize of *all* the files of the InstallComponent
            // then get the total download progress instead of the progress per file
            // and change the progressbar
            // if it fails for whatever reason, ignore and fallback to given percentage
            try
            {
                // this is rather hacky sadly
                // since what if i.e part 2 isn't downloaded but part 3 is?
                // it'll display it incorrectly,
                // we don't care for now
                // since users wont touch the files (I hope..)
                for (int i = 0; i < Downloader.ComponentDownloadIndex; i++)
                    bytesReceived += Downloader.CurrentComponent.Urls[i].FileSize;

                // add each filesize to fileSize
                foreach (UrlInfo urlInfo in Downloader.CurrentComponent.Urls)
                    fileSize += urlInfo.FileSize;

                // since we only accept int as argument,
                // just convert bytes to MB and convert that to int
                // since we'll be going over the maximum int value
                // if we don't do this
                ChangeProgressBarValue((int)(bytesReceived / 1024 / 1024), (int)(fileSize / 1024 / 1024));
            }
            catch (Exception)
            {
                // ignore
            }
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
        public InstallerComponents InstallerComponents { get; set; }

        private void DownloadAllComponents()
        {
            if (!Directory.Exists(InstallerSettings.DownloadDirectory))
                Directory.CreateDirectory(InstallerSettings.DownloadDirectory);

            Downloader = new ComponentDownloader();
            Downloader.OnDownloadProgressChanged += OnDownloadProgressChanged;

            string project64FileName = Path.Combine(InstallerSettings.DownloadDirectory, "Project64.zip");

            // we *sadly* need to do something special for Project64
            Log("Downloading Project64...");
            Downloader.Project64(project64FileName);

            if (Downloader.Failed)
            {
                Log("Downloading Project64 Failed");
                Log(Downloader.Exception.Message);
                Log(Downloader.Exception.StackTrace);
                return;
            }

            foreach (InstallerComponent component in InstallerComponents.Components)
            {
                // if it's disabled,
                // skip it
                if (!component.Enabled)
                    continue;

                ChangeProgressBarValue(0);
                Log($"Downloading {component.Name}...");
                Downloader.DownloadComponent(component, InstallerSettings.DownloadDirectory);

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
            new InstallComponents() { InstallerComponents = InstallerComponents,
                StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
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
