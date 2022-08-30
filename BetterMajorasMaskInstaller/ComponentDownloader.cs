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
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AppVeyorApi;

namespace BetterMajorasMaskInstaller
{
    public delegate void DownloadStatusChangedEventHandler(object source, DownloadStatusChangedEventArgs args);

    public class DownloadStatusChangedEventArgs : EventArgs
    {
        public long BytesReceived { get; set; }
        public int? ProgressPercentage { get; set; }
        public InstallerComponent CurrentComponent { get; set; }
        public int CurrentComponentDownloadIndex { get; set; }
        public DownloadStatusChangedEventArgs(long bytesReceived, int? progressPercentage, 
                InstallerComponent currentComponent, 
                int currentComponentDownloadIndex)
        {
            this.BytesReceived = bytesReceived;
            this.ProgressPercentage = progressPercentage;
            this.CurrentComponent = currentComponent;
            this.CurrentComponentDownloadIndex = currentComponentDownloadIndex;
        }
    }

    partial class ComponentDownloader : IDisposable
    {
        /// <summary>
        ///     Webclient for class
        /// </summary>
        private readonly WebClient _webClient;

        /// <summary>
        ///     Download Progress Changed event handler
        /// </summary>
        public DownloadStatusChangedEventHandler OnDownloadProgressChanged { get; set; }

        /// <summary>
        ///     Currently Downloading Component
        /// </summary>
        private InstallerComponent currentComponent { get; set; }

        /// <summary>
        ///     Current Component Download Index
        /// </summary>
        private int currentComponentDownloadIndex { get; set; }

        public ComponentDownloader()
        {
            // create WebClient
            _webClient = new WebClient();

            // register event
            _webClient.DownloadProgressChanged += (object source, DownloadProgressChangedEventArgs args) =>
            {
                OnDownloadProgressChanged(source, new DownloadStatusChangedEventArgs(args.BytesReceived, args.ProgressPercentage, currentComponent, currentComponentDownloadIndex));
            };
        }

        /// <summary>
        ///     Verifies MD5 hash
        /// </summary>
        private bool VerifyHash(string fileName, string fileHash, long fileSize)
        {
            // when there's no hash or filename, return false
            if (fileHash == null && 
                fileName == null)
            {
                return false;
            }

            // when the file doesn't exist, return false
            if (!File.Exists(fileName))
            {
                return false;
            }

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                {
                    // if there's no hash specified but
                    // there's a filesize specified, use that
                    if (fileHash == null)
                    {
                        return fileStream.Length == fileSize;
                    }

                    string md5Hash = BitConverter.ToString(md5.ComputeHash(fileStream))
                        .Replace("-", null)
                        .ToLower();

                    return md5Hash == fileHash;
                }
            }
        }
        /// <summary>
        ///     Downloads DownloadComponent in directory
        /// </summary>
        public async Task DownloadComponent(int componentIndex, string directory, bool fallback)
        {
            var component = InstallerSettings.InstallerConfiguration.Components[componentIndex];
            var urlList = fallback ? component.FallbackUrls : component.Urls;

            // reset global state
            currentComponent = component;
            currentComponentDownloadIndex = 0;

            // loop over each URL and download it
            for (int i = 0; i < urlList.Count; i++)
            {
                var urlInfo = urlList[i];

                // update global download index
                currentComponentDownloadIndex = i;

                // if it's an AppVeyor Url,
                // use the AppVeyor API to get file information
                // and change urlInfo according to that
                if (IsAppVeyorUrl(urlInfo.Url))
                {
                    urlInfo = AppVeyorUrlInfo(urlInfo);

                    // also update the actual InstallerComponent
                    if (fallback)
                    {
                        InstallerSettings.InstallerConfiguration.Components[componentIndex].FallbackUrls[0] = urlInfo;
                    }
                    else
                    {
                        InstallerSettings.InstallerConfiguration.Components[componentIndex].Urls[0] = urlInfo;
                    }
                }


                string url = urlInfo.Url;
                string file = urlInfo.FileName != null ?
                            Path.Combine(directory, urlInfo.FileName) :
                            null;
                string hash = urlInfo.FileHash;

                // if the hash matches, skip it
                if (VerifyHash(file, hash, urlInfo.FileSize))
                {
                    continue;
                }

                // try to download the file using WebClient
                // and verify the hash to make sure it matches
                await _webClient.DownloadFileTaskAsync(new Uri(url), file);

                if (!VerifyHash(file, hash, urlInfo.FileSize))
                {
                    throw new Exception("VerifyHash returned false!");
                }
            }
        }
        public void Dispose()
        {
            OnDownloadProgressChanged = null;
            GC.SuppressFinalize(this);
        }
    }
}
