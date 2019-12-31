using AppVeyorApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    partial class ComponentDownloader
    {
        /// <summary>
        /// Checks whether URL is a AppVeyor URL (using the 'special' format)
        /// </summary>
        private bool IsAppVeyorUrl(string url) => url.StartsWith("appveyor:");

        private UrlInfo AppVeyorUrlInfo(UrlInfo urlInfo)
        {
            string project = urlInfo.Url.Split(':')[1];
            string branch = "master";
            string artifactFileName = null;

            using (AppVeyor appVeyor = new AppVeyor(project, branch))
            {
                Artifact artifact = appVeyor.GetLatestArtifacts(artifactFileName)[0];

                UrlInfo info = new UrlInfo()
                {
                    FileName = artifact.FileName,
                    FileSize = artifact.Size,
                    Url = artifact.Url
                };

                // update
                urlInfo = info;
            }

            return urlInfo;
        }
    }
}
