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

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace AppVeyorApi
{
    public class AppVeyor : IDisposable
    {
        private HttpClient client { get; set; }
        private string project { get; set; }
        private string branch { get; set; }
        private readonly string apiUrl = "https://ci.appveyor.com/api";
        public AppVeyor(string project, string branch = "master")
        {
            this.project = project;
            this.branch = branch;
        }

        /// <summary>
        /// Returns json result from url
        /// </summary>
        private string getApiData(string url)
        {
            // init HttpClient
            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            return client.GetStringAsync(url).Result;
        }

        /// <summary>
        /// Grabs the latest successful build artifacts
        /// </summary>
        public Artifact[] GetLatestArtifacts(string artifactName = null)
        {
            string url = $"{apiUrl}/buildjobs/{getJobId(artifactName)}/artifacts";

            Artifact[] ret = JsonConvert.DeserializeObject<Artifact[]>(getApiData(url));

            foreach(Artifact a in ret)
            {
                a.Url = $"{url}/{a.FileName}";
                a.FileName = Path.GetFileName(a.FileName);
            }

            return ret;
        }

        private string getJobId(string artifactName = null)
        {
            string url = $"{apiUrl}/projects/{project}/history?recordsNumber=5&branch={branch}";

            ProjectHistory history = JsonConvert.DeserializeObject<ProjectHistory>(getApiData(url));

            // find successful build
            foreach (Build b in history.Builds)
            {
                if (b.Status == "success")
                {
                    string url2 = $"{apiUrl}/projects/{project}/build/{b.Version}";

                    // find artifact with artifact name, 
                    // else just return the first artifact
                    if (artifactName != null)
                    {
                        foreach(Jobs j in JsonConvert.DeserializeObject<BuildInfo>(getApiData(url2)).Build.Jobs)
                        {
                            if(j.Name == artifactName)
                                return j.JobId;
                        }

                        throw new Exception($"No artifact with name '{artifactName}' was found!");
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<BuildInfo>(getApiData(url2)).Build.Jobs[0].JobId;
                    }
                }
            }

            throw new Exception($"No successful builds found for '{project}'!");
        }

        private bool _Disposed { get; set; }
        public void Dispose()
        {
            if (!_Disposed)
            {
                client = null;
                project = null;

                _Disposed = true;
            }
        }
    }
}
