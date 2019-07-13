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
        public bool IsGoogleDriveUrl(string url) => url.StartsWith("gdrive:");

        /// <summary>
        /// Downloads file(from URL) using the Google Drive API
        /// </summary>
        public void DownloadGoogleDriveFile(string url, string fileName, string fileHash)
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
                                break;

                            case DownloadStatus.Failed:
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
