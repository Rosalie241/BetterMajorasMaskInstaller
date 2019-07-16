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

using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;

namespace BetterMajorasMaskInstaller
{
    partial class ComponentDownloader
    {
        /// <summary>
        /// Checks whether URL is a Google Drive URL (using the 'special' format)
        /// </summary>
        private bool IsGoogleDriveUrl(string url) => url.StartsWith("gdrive:");

        /// <summary>
        /// Downloads file(from URL) using the Google Drive API
        /// </summary>
        private void DownloadGoogleDriveFile(string url, string fileName, string fileHash)
        {
            string driveId = url.Split(':')[1];
            using (DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                ApiKey = GoogleApiInformation.ApiKey,
                ApplicationName = GoogleApiInformation.ApplicationName
            }))
            {
                FilesResource.GetRequest request = service.Files.Get(driveId);

                using(FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
                    {
                        switch(progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                Logger.Log($"{url} {fileName} {fileHash}: {progress.BytesDownloaded}");
                                OnDownloadProgressChanged(this, new DownloadStatusChangedEventArgs(progress.BytesDownloaded));
                                fileStream.Flush();
                                break;

                            case DownloadStatus.Failed:
                                Logger.Log($"{url} {fileName} {fileHash}: {progress.Exception}");
                                Exception = progress.Exception;
                                Failed = true;
                                return;

                            case DownloadStatus.Completed:
                                fileStream.Close();
                                Failed = !VerifyHash(fileName, fileHash);
                                break;
                        }

                    };

                    request.Download(fileStream);
                }
            }
        }
    }
}
