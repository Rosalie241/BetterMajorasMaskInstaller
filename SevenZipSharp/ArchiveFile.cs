using System;

namespace SevenZipSharp
{
    public class ArchiveFile
    {
        private string File { get; set; }
        public ArchiveFile(string fileName)
        {
            this.File = fileName;
        }
    }
}
