using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    public class DownloadComponent
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Url(s) (url, filename)
        /// </summary>
        public Dictionary<string, string> Urls { get; set; }
        /// <summary>
        /// Url(s) FileSizes
        /// </summary>
        public int?[] FileSizes { get; set; }
        /// <summary>
        /// Files (source, target)
        /// </summary>
        public Dictionary<string, string> Files { get; set; }
    }
}
