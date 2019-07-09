using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    public delegate void DownloadStatusChangedEventHandler(object source, DownloadStatusChangedEventArgs args);

    public class DownloadStatusChangedEventArgs : EventArgs
    {
        public long BytesReceived { get; set; }
        public int ProgressPercentage { get; set; }
        public DownloadStatusChangedEventArgs(long bytesReceived, int progressPercentage)
        {
            this.BytesReceived = bytesReceived;
            this.ProgressPercentage = progressPercentage;
        }
    }

    partial class ComponentDownloader : IDisposable
    {
        private static WebClient Client { get; set; }
        
        public DownloadStatusChangedEventHandler OnDownloadProgressChanged { get; set; }
        /// <summary>
        /// whether the download failed
        /// </summary>
        public bool Failed { get; set; }
        public ComponentDownloader()
        {
            Client = new WebClient();
        }

        public void RegisterEvents()
        {
            Client.DownloadProgressChanged += (object source, DownloadProgressChangedEventArgs args) =>
            {
                OnDownloadProgressChanged(source, new DownloadStatusChangedEventArgs(args.BytesReceived, args.ProgressPercentage));
            };
        }

        public int ComponentDownloadIndex { get; set; }
        public DownloadComponent CurrentComponent { get; set; }
        private bool VerifyFileSize(string fileName, int bytesLength)
        {
            using(FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                if (fileStream.Length == bytesLength)
                    return true;
            }

            return false;
        }
        public void DownloadComponent(DownloadComponent component, string directory)
        {
            ComponentDownloadIndex = -1;
            CurrentComponent = component;

            foreach (KeyValuePair<string, string> urlInfo in component.Urls)
            {
                ComponentDownloadIndex++;

                string url = urlInfo.Key;
                string file = Path.Combine(directory, urlInfo.Value);
                int? length = 0;

                if (File.Exists(file))
                {
                    if (CurrentComponent.FileSizes == null)
                        continue;

                    length = CurrentComponent.FileSizes[ComponentDownloadIndex];

                    if (length == null)
                        continue;

                    if (VerifyFileSize(file, (int)length))
                        continue;
                }

                if (IsGoogleDriveUrl(url))
                {
                    DownloadGoogleDriveFile(url, file);
                }
                else
                {
                    try
                    {
                        Client.DownloadFileAsync(new Uri(url), file);

                        while (Client.IsBusy)
                            Thread.Sleep(10);

                        Failed = VerifyFileSize(file, (int)length);
                    }
                    catch(Exception)
                    {
                        Failed = true;
                    }
                }

            }
        }
        public void Dispose()
        {
            Client = null;
            OnDownloadProgressChanged = null;
            GC.SuppressFinalize(this);
        }
    }
}
