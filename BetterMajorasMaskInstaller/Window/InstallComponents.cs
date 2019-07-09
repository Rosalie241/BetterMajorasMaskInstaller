using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class InstallComponents : Form
    {
        public InstallComponents()
        {
            InitializeComponent();
        }

        public DownloadComponents DownloadComponents { get; set; }

        private void InstallComponents_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => Install());
        }

        private void Install()
        {
            /*
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                  "BetterMajorasMaskInstaller");
            foreach (DownloadComponent component in DownloadComponents.Components)
            {
                ArchiveFile f = null;
                if (component.Urls.Values.Count > 1)
                {
                    List<Stream> tmp = new List<Stream>();

                    foreach (string aaaa in component.Urls.Values)
                    {
                        tmp.Add(new FileStream(Path.Combine(downloadPath, aaaa), FileMode.Open));
                    }
                    f = new ArchiveFile(new CombinationStream.CombinationStream(tmp));
                }
                else
                    continue;
                    // f = new ArchiveFile(Path.Combine(downloadPath, component.Urls.Values));

                    foreach(Entry e in f.Entries)
                    {
                        int index = 0;
                        foreach (string fileKey in component.Files.Keys)
                        {
                            if (e.FileName == fileKey)
                            {

                                e.Extract(Path.Combine(downloadPath, "test", component.Files[fileKey]));
                            }


                            index++;
                        }
                        //File.AppendAllText("test.txt", e.FileName + Environment.NewLine);
                    }

              // component.Files
            }
            */
        }
    }
}
