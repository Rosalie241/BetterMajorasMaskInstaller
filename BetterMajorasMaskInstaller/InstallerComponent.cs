/*
 BetterMajorasMaskInstaller - https://github.com/tim241/BetterMajorasMaskInstaller
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
using System.Collections.Generic;

namespace BetterMajorasMaskInstaller
{
    public class InstallerComponent
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Whether this component is enabled
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Whether this component is readonly
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// What components need to be disabled when this one is selected
        /// </summary>
        public List<string> DisableOnSelected { get; set; }
        /// <summary>
        /// Whether this is an archive
        /// </summary>
        public bool Archive { get; set; }
        /// <summary>
        /// Installed File Size
        /// </summary>
        public Int64 InstalledSize { get; set;  }
        /// <summary>
        /// Whether we should extract everything
        /// </summary>
        public bool ExtractAll { get; set; }
        /// <summary>
        /// Url(s) (url, filename, filesize, md5 hash)
        /// </summary>
        public List<UrlInfo> Urls { get; set; }
        /// <summary>
        /// Files (source, target)
        /// </summary>
        public Dictionary<string, string> Files { get; set; }
        /// <summary>
        /// Patches (file, source text, replacement text)
        /// </summary>
        public Dictionary<string, KeyValuePair<string, string>> Patches { get; set; }
    }
}
