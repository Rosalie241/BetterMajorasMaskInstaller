using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SevenZipSharp
{
    public class ArchiveFile
    {
        private string FileName { get; set; }
        private List<string> FileList = new List<string>();
        public ArchiveFile(string fileName)
        {
            this.FileName = fileName;

            if (!File.Exists(FileName))
                throw new InvalidDataException("Archive doesn't exist!");

            if (!IsValidArchive())
                throw new InvalidDataException("Invalid Archive!");

            GetArchiveInfo();
        }

        private void GetArchiveInfo()
        {
            using(Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "7za",
                    Arguments = $"l -r {FileName}",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                p.OutputDataReceived += (object source, DataReceivedEventArgs args) =>
                {
                    
                };
            }
        }

        private bool IsValidArchive()
        {
            using(Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = "7za",
                    Arguments = $"t {FileName}",
                    CreateNoWindow = true,                    
                };

                p.Start();
                p.WaitForExit();

                return p.ExitCode == 0;
            }
        }
    }
}
