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
                {
                    Exception = new Exception("Failed to get latest nightly URL!");
                    Failed = true;
                    return;
                }

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
