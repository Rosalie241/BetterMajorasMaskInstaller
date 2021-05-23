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

            // we can use 'appveyor:example/project#Win32#develop'
            // to find an artifact which contains 'Win32'
            // in branch 'develop'
            if (urlInfo.Url.Contains("#"))
            {
                artifactFileName = urlInfo.Url.Split('#')[1];
                project = project.Split('#')[0];

                if (urlInfo.Url.Split('#').Length > 2)
                {
                    branch = urlInfo.Url.Split('#')[2];
                }
            }

            using (AppVeyor appVeyor = new AppVeyor(project, branch))
            {
                Artifact[] artifacts = appVeyor.GetLatestArtifacts();
                Artifact artifact = null;

                foreach(Artifact a in artifacts)
                {
                    if (a.FileName.Contains(artifactFileName))
                        artifact = a;
                }

                if (artifact == null)
                    throw new Exception($"Cannot find artifact containing \"{artifactFileName}\" in the filename");

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
