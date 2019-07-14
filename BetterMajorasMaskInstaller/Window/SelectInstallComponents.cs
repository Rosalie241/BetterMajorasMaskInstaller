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
