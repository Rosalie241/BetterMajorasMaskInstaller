using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

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
        /// <summary>
        /// When Failed, the exception will be here
        /// </summary>
        public Exception Exception { get; set; }
        public ComponentDownloader()
        {
            Client = new WebClient
            {
                CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
            };
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
        private bool VerifyFileSize(string fileName, int? bytesLength)
        {
            if (bytesLength == null)
                return true;

            using(FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                if (fileStream.Length == bytesLength)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Downloads DownloadComponent in directory
        /// </summary>
        public void DownloadComponent(DownloadComponent component, string directory)
        {
            ComponentDownloadIndex = -1;
            CurrentComponent = component;

            // loop over each URL and download it
            foreach (KeyValuePair<string, string> urlInfo in component.Urls)
            {
                ComponentDownloadIndex++;

                string url = urlInfo.Key;
                string file = Path.Combine(directory, urlInfo.Value);
                int? length = 0;

                if(CurrentComponent.FileSizes != null)
                    length = CurrentComponent.FileSizes[ComponentDownloadIndex];

                // if the file exists
                // make sure the size is the same as the specified size
                // if so, skip this item
                if (File.Exists(file))
                {
                    if (VerifyFileSize(file, length))
                        continue;
                }

                // if the URL is a Google Drive URL,
                // download it using the Google API
                // and return
                if (IsGoogleDriveUrl(url))
                {
                    DownloadGoogleDriveFile(url, file);
                    return;
                }

                // try to download the file using WebClient
                // also verify the size when it's done.
                // since we also want the events to work
                // we'll download it async
                // and wait for the WebClient to be done
                try
                {
                    Client.DownloadFileAsync(new Uri(url), file);

                    // we use Thread.Sleep here because
                    // Thread.Yield has high cpu usage
                    while (Client.IsBusy)
                        Thread.Sleep(10);

                    Failed = !VerifyFileSize(file, length);
                }
                catch (Exception e)
                {
                    Exception = e;
                    Failed = true;
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
