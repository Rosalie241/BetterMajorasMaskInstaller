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
        private Queue<double> downloadQueue = new Queue<double>();
        private Stopwatch downloadStopWatch = new Stopwatch();
        private double oldAverage = -1;
        private double previousTotal = -1;

        private long GetAverageDownloadSpeed(long currentValue)
        {
            if (!downloadStopWatch.IsRunning)
            {
                downloadStopWatch.Start();
            }

            if (downloadStopWatch.ElapsedMilliseconds < 1000)
            {
                return (long)oldAverage;
            }

            if (downloadQueue.Count >= 20)
            {
                downloadQueue.Dequeue();
            }

            if (previousTotal == -1)
            {
                previousTotal = currentValue;

                return (long)oldAverage;
            }

            // Enqueue average download speed since last average was enqueued
            downloadQueue.Enqueue(((currentValue - previousTotal) / downloadStopWatch.Elapsed.TotalSeconds));
            previousTotal = currentValue;

            // Total average for component is the average of average download speeds
            double average = downloadQueue.Sum() / downloadQueue.Count;

            oldAverage = average;
            downloadStopWatch.Restart();

            return (long)average;
        }

        private Dictionary<UrlInfo, long> componentProgress = new Dictionary<UrlInfo, long>();
        private void OnDownloadProgressChanged(object source, DownloadStatusChangedEventArgs a)
        {
            // get total filesize of *all* the files of the InstallComponent
            // then get the total download progress instead of the progress per file
            // and change the progressbar
            // if it fails for whatever reason, ignore and fallback to given percentage
            try
            {
                var enabledComponents = InstallerSettings.InstallerComponents.Components.Where(c => c.Enabled);
                var currentUrlInfo = a.CurrentComponent.Urls[a.CurrentComponentDownloadIndex];
                var totalDownloadSize = enabledComponents.Select(c =>
                {
                    // use fallback urls if needed
                    if (c.Urls[a.CurrentComponentDownloadIndex].FileSize == 0 && c.FallbackUrls != null)
                        return c.FallbackUrls.Select(u => u.FileSize).Sum();

                    return c.Urls.Select(u => u.FileSize).Sum();
                }).Sum();

                if (!componentProgress.ContainsKey(currentUrlInfo) ||
                    (componentProgress.ContainsKey(currentUrlInfo) &&
                    componentProgress[currentUrlInfo] < a.BytesReceived))
                {
                    componentProgress[currentUrlInfo] = a.BytesReceived;
                }

                long totalBytesReceived = componentProgress.Values.Sum();
                int percentage = (int)((double)(totalBytesReceived / (double)totalDownloadSize) * 100);

                ChangeProgressBarValue(percentage);
                ChangeProgressLabel($"{(totalBytesReceived / 1024 / 1024)} MiB / {(totalDownloadSize / 1024 / 1024)} MiB @ {GetAverageDownloadSpeed((totalBytesReceived / 1024))} KiB/s");

                return;

                // this is rather hacky sadly
                // since what if i.e part 2 isn't downloaded but part 3 is?
                // it'll display it incorrectly,
                // we don't care for now
                // since users wont touch the files (I hope..)
                /*for (int i = 0; i < a.CurrentComponentDownloadIndex; i++)
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
                // ChangeProgressLabel($"{megaBytesReceived} MiB / {fileSizeInMegaBytes} MiB @ {GetAverageDownloadSpeed((bytesReceived / 1024), a.CurrentComponent)} KiB/s");
                */
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

        private async Task DownloadComponentAsync(ComponentDownloader downloader, InstallerComponent component, int index, bool fallback)
        {
            Task downloadTask = null;
            string taskIdString = "";

            try
            {
                downloadTask = downloader.DownloadComponent(index, InstallerSettings.DownloadDirectory, fallback);
                taskIdString = downloadTask.Id.ToString().PadLeft(4, '0');
                Log($"[{taskIdString}] Downloading {component.Name}...");
                await downloadTask;
            }
            catch (Exception e)
            {
                Log($"[{taskIdString}] Downloading {component.Name} Failed{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}");

                if (component.FallbackUrls != null &&
                    !fallback)
                {
                    Log($"[{taskIdString}] Retrying With Fallback...");
                    downloadTask = DownloadComponentAsync(downloader, component, index, true);
                    await downloadTask;
                    return;
                }
            }

            // re-throw task exception when needed
            if (downloadTask.IsFaulted)
            {
                throw downloadTask.Exception;
            }
            else
            { // successful download
                Log($"[{taskIdString}] Finished Downloading {component.Name}...");
            }
        }


        private async Task DownloadAllComponents()
        {
            var downloader = new ComponentDownloader();
            downloader.OnDownloadProgressChanged += OnDownloadProgressChanged;

            var components = InstallerSettings.InstallerComponents.Components;
            var downloadTasks = new List<Task>();

        retry:
            for (int i = 0; i < components.Count; i++)
            {
                InstallerComponent component = components[i];

                // make sure component is enabled
                if (!component.Enabled)
                {
                    continue;
                }

                var downloadTask = DownloadComponentAsync(downloader, component, i, false);
                downloadTasks.Add(downloadTask);
            }

            bool downloadFailed = false;

            while (downloadTasks.Count > 0)
            {
                var task = await Task.WhenAny(downloadTasks);

                if (task.IsFaulted)
                {
                    downloadFailed = true;
                    // fall through and wait for other tasks to complete..
                }

                downloadTasks.Remove(task);
            }

            if (downloadFailed)
            {
                DialogResult ret = MessageBox.Show("Download Failed, Try Again?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (ret == DialogResult.Yes)
                {
                    // reset download progress
                    componentProgress.Clear();
                    goto retry;
                }

                return;
            }

            //LaunchInstallComponents();
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
