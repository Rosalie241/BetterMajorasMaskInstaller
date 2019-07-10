using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class SelectInstallComponents : Form
    {
        private Dictionary<string, bool> InstallComponentsItems = new Dictionary<string, bool>()
        {
            { "Project64 + Plugins", true }
        }; 

        public SelectInstallComponents()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, bool> installComponent in InstallComponentsItems)
            {
                InstallComponentsList.Items.Add(installComponent.Key);
                InstallComponentsList.SetItemChecked(InstallComponentsList.Items.IndexOf(installComponent.Key), installComponent.Value);
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InstallComponentsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstallButton.Enabled = InstallComponentsList.CheckedItems.Count != 0;
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new DownloadInstallComponents() { StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void SelectInstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
