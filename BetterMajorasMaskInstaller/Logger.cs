using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetterMajorasMaskInstaller
{
    public static class Logger
    {
        /// <summary>
        /// Main log buffer
        /// </summary>
        private static List<string> Buffer = new List<string>();
        /// <summary>
        /// Whether to write directly when there's data in the buffer
        /// </summary>
        public static bool AggressiveWriting { get; set; }

        public static void Log(string content,
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            Buffer.Add($"[{sourceFilePath}.{memberName}] {content}");
        }

        public static void StartLoggingAsync() => Task.Factory.StartNew(() => StartLogging());

        private static void StartLogging()
        {
            if (File.Exists(InstallerSettings.LogFile))
                File.Delete(InstallerSettings.LogFile);

            string directory = Path.GetDirectoryName(InstallerSettings.LogFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            while(true)
            {                
                while (Buffer.Count == 0)
                    Thread.Sleep(10);

                // write buffer to file
                File.AppendAllText(InstallerSettings.LogFile, String.Join(Environment.NewLine, Buffer) + Environment.NewLine);

                // clear the buffer
                Buffer.Clear();

                if (!AggressiveWriting)
                {
                    // wait 1 second
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
