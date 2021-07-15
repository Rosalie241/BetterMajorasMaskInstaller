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

namespace BetterMajorasMaskInstaller
{
    public static class InstallerSettings
    {
        /// <summary>
        ///     Install Directory
        /// </summary>
        public static string InstallDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Project64");

        /// <summary>
        ///     Download Directory
        /// </summary>
        public static string DownloadDirectory = Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BetterMajorasMaskInstaller");
        /// <summary>
        ///     Configuration File Url
        /// </summary>
        public static string ConfigurationUrl = 
            "https://raw.githubusercontent.com/Rosalie241/BetterMajorasMaskInstaller-cfg/master/config.json";
        /// <summary>
        ///     Whether DeveloperMode has been enabled
        /// </summary>
        public static bool DeveloperMode = false;
        /// <summary>
        ///     Global Installer Components
        /// </summary>
        public static InstallerComponents InstallerComponents { get; set; }
    }
}
