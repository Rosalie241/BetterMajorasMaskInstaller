using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    partial class ComponentDownloader
    {
        private string GetLatestNightlyUrl()
        {
            // use Regex to get latest URL
            // we're going to try to find the string between the first '<a href="' and '"'
            // then we return that string
            string text = Client.DownloadString("https://www.pj64-emu.com/nightly-builds");
            foreach (string line in text.Split('\n'))
            {
                if (line.Contains("/file/"))
                {
                    Regex reg = new Regex("<a href=\"(.*?)\"");
                    MatchCollection mCollection = reg.Matches(line);

                    foreach (Match m in mCollection)
                    {
                        if (m.Success)
                        {
                            if (!m.Value.Contains("setup"))
                            {
                                return @"https://www.pj64-emu.com" + m.Value.Split('"')[1];
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void Project64(string fileName)
        {
            string url = GetLatestNightlyUrl();
            Client.DownloadFileAsync(new System.Uri(url), fileName);

            while (Client.IsBusy)
                Thread.Yield();
        }
    }
}
