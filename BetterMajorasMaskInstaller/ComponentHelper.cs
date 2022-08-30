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
using System.IO;
using System.Linq;

namespace BetterMajorasMaskInstaller
{
    public static class ComponentHelper
    {
        /// <summary>
        ///     Cleans Installation Files
        /// </summary>
        public static void CleanupInstallationFiles()
        {
            // developer mode option
            if (!InstallerSettings.CleanupInstallationFilesOnFailure)
            {
                return;
            }

            // installation directory is required to be empty..
            // so we can wipe it safely (hopefully)
            Directory.Delete(InstallerSettings.InstallDirectory, true);
        }

        /// <summary>
        ///     Cleans Downloaded Files
        /// </summary>
        /// <param name="components"></param>
        public static void CleanupDownloadFiles(InstallerConfiguration components)
        {
            foreach (InstallerComponent component in components.Components)
            {
                var urls = component.Urls.First().FileName == null ?
                            component.FallbackUrls :
                            component.Urls;

                foreach (UrlInfo file in urls)
                {
                    string targetFile = Path.Combine(InstallerSettings.DownloadDirectory,
                                file.FileName);

                    if (File.Exists(targetFile))
                    {
                        File.Delete(targetFile);
                    }
                }
            }

            // also delete extracted 7za.exe and project64.zip
            foreach (string file in new string[] { "7za.exe", "Project64.zip" })
            {
                string targetFile = Path.Combine(InstallerSettings.DownloadDirectory, file);

                if (File.Exists(targetFile))
                    File.Delete(targetFile);
            }

            // hmm, the dir can be gone? 
            if (Directory.Exists(InstallerSettings.DownloadDirectory))
            {
                // make sure directory is empty
                if (!Directory.EnumerateFileSystemEntries(InstallerSettings.DownloadDirectory).Any())
                {
                    Directory.Delete(InstallerSettings.DownloadDirectory, true);
                }
            }
        }
    }
}
