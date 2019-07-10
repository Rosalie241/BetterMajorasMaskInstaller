using System;
using System.Text.RegularExpressions;
using System.Threading;

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
                // if the current line doesn't contain '/file/'
                // skip it, because it's useless for us
                if (!line.Contains("/file/"))
                    continue;

                Regex regex = new Regex("<a href=\"(.*?)\"");
                MatchCollection mCollection = regex.Matches(line);

                // loop over each regex match
                // if the match wasn't successful, skip it
                // if the match contains 'setup', skip it
                foreach (Match match in mCollection)
                {
                    if (!match.Success)
                        continue;

                    if (match.Value.Contains("setup"))
                        continue;

                    return @"https://www.pj64-emu.com" + match.Value.Split('"')[1];
                }

            }

            // if we found nothing
            // return nothing :)
            return null;
        }

        /// <summary>
        /// Downloads latest Project64 nightly to fileName
        /// </summary>
        public void Project64(string fileName)
        {
            try
            {
                string url = GetLatestNightlyUrl();

                if (url == null)
                    throw new Exception("Couldn't find latest nightly URL!");

                Client.DownloadFileAsync(new System.Uri(url), fileName);

                // we use Thread.Sleep here because
                // Thread.Yield has high cpu usage
                while (Client.IsBusy)
                    Thread.Sleep(10);

                Failed = false;
            }
            catch(Exception e)
            {
                Exception = e;
                Failed = true;
            }
        }
    }
}
