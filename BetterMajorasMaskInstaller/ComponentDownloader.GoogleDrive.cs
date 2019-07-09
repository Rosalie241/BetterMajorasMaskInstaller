using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
//using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    partial class ComponentDownloader
    {
        public bool IsGoogleDriveUrl(string url) => url.Contains("gdrive:");

        public void DownloadGoogleDriveFile(string url, string fileName)
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
                                OnDownloadProgressChanged(this, new DownloadStatusChangedEventArgs(progress.BytesDownloaded, 0));
                                fileStream.Flush();
                                Failed = false;
                                break;

                            case DownloadStatus.Failed:
                                File.WriteAllText("exception.txt", progress.Exception.Message);
                                Failed = true;
                                return;

                            case DownloadStatus.Completed:
                                break;
                        }

                    };

                    request.Download(fileStream);
                    fileStream.Close();
                }
            }
        }
    }
}
