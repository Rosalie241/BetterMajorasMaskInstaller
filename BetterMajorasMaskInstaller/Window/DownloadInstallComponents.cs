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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class DownloadInstallComponents : Form
    {
        public DownloadInstallComponents()
        {
            InitializeComponent();

            Task.Run(() => DownloadAllComponents());
        }

        //
        // Thanks to Jaxon on Discord for this code <3
        //
        Queue<long> downloadQueue = new Queue<long>();
        private Stopwatch downloadStopWatch = new Stopwatch();
        private long oldAverage = -1;
        private long previousTotal = -1;
        private InstallerComponent oldComponent;
        private long GetAverageDownloadSpeed(long currentTotal, InstallerComponent currentComponent)
        {
            // if we're at a different component,
            // reset everything
            if (currentComponent != oldComponent)
            {
                downloadQueue.Clear();
                oldComponent = currentComponent;
                downloadStopWatch.Restart();
                previousTotal = -1;
                oldAverage = -1;

                return oldAverage;
            }

            if (!downloadStopWatch.IsRunning)
            {
                downloadStopWatch.Start();
            }

            if (downloadStopWatch.ElapsedMilliseconds < 1000)
            {
                return oldAverage;
            }

            if (downloadQueue.Count >= 20)
            {
                downloadQueue.Dequeue();
            }

            if (previousTotal == -1)
            {
                previousTotal = currentTotal;

                return oldAverage;
            }

            // Enqueue average download speed since last average was enqueued
            downloadQueue.Enqueue((long)((currentTotal - previousTotal) / downloadStopWatch.Elapsed.TotalSeconds));
            previousTotal = currentTotal;

            // Total average for component is the average of average download speeds
            long average = downloadQueue.Sum() / downloadQueue.Count;

            oldAverage = average;
            downloadStopWatch.Restart();

            return average;
        }

        private void OnDownloadProgressChanged(object source, DownloadStatusChangedEventArgs a)
        {
            long fileSize = 0;
            long bytesReceived = a.BytesReceived;

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
                for (int i = 0; i < a.CurrentComponentDownloadIndex; i++)
                {
                    bytesReceived += a.CurrentComponent.Urls[i].FileSize;
                }

                // add each filesize to fileSize
                foreach (UrlInfo urlInfo in a.CurrentComponent.Urls)
                {
                    fileSize += urlInfo.FileSize;
                }

                // since we only accept int as argument,
                // just convert bytes to MB
                // since we'll be going over the maximum int value
                // if we don't do this
                int megaBytesReceived = (int)(bytesReceived / 1024 / 1024);
                int fileSizeInMegaBytes = (int)(fileSize / 1024 / 1024);

                ChangeProgressBarValue(megaBytesReceived, fileSizeInMegaBytes);
                ChangeProgressLabel($"{megaBytesReceived} MiB / {fileSizeInMegaBytes} MiB @ {GetAverageDownloadSpeed((bytesReceived / 1024), a.CurrentComponent)} KiB/s");
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

            // sanity check
            if (value > maxValue)
            {
                return;
            }

            progressBar1.Maximum = maxValue;
            progressBar1.Value = value;
        }

        private void ChangeProgressLabel(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { ChangeProgressLabel(text); });
            }
            else
            {
                progressLabel.Text = text;
            }
        }

        private async Task DownloadAllComponents()
        {
            var downloader = new ComponentDownloader();
            downloader.OnDownloadProgressChanged += OnDownloadProgressChanged;

            var components = InstallerSettings.InstallerConfiguration.Components;

            for (int i = 0; i < components.Count; i++)
            {
                InstallerComponent component = components[i];

                // make sure component is enabled
                if (!component.Enabled)
                {
                    continue;
                }

                bool useFallback = false;
            retry:
                try
                {
                    ChangeProgressBarValue(0);
                    Log($"Downloading {component.Name}...");

                    await downloader.DownloadComponent(i, InstallerSettings.DownloadDirectory, useFallback);
                }
                catch (Exception e)
                {
                    Log($"Downloading {component.Name} Failed");
                    Log(e.Message);
                    Log(e.StackTrace);

                    if (component.FallbackUrls != null &&
                        !useFallback)
                    {
                        Log("Retrying With Fallback...");
                        useFallback = true;
                        goto retry;
                    }

                    // ask the user if they want to retry
                    DialogResult ret = MessageBox.Show("Download Failed, Try Again?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (ret == DialogResult.Yes)
                    {
                        useFallback = false;
                        goto retry;
                    }

                    // early exit
                    return;
                }
            }

            Log("Downloading completed");
            LaunchInstallComponents();
        }

        private void LaunchInstallComponents()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { LaunchInstallComponents(); });
                return;
            }

            this.Hide();

            new InstallComponents()
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            }.Show();
        }

        private void Log(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { Log(text); });
            }
            else
            {
                LogBox.AppendText(text + Environment.NewLine);
            }
        }

        private void DownloadInstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
