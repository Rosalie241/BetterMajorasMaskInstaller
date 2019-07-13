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
        /// Url(s) (url, filename, filesize, md5 hash)
        /// </summary>
        public List<UrlInfo> Urls { get; set; }
        /// <summary>
        /// Files (source, target)
        /// </summary>
        public Dictionary<string, string> Files { get; set; }
    }
}
