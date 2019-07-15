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
using System.ComponentModel;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class SelectInstallComponents : Form
    {
        public InstallerComponents InstallerComponents { get; set; }

        public SelectInstallComponents(InstallerComponents installerComponents)
        {
            InitializeComponent();

            // we sadly need InstallerComponents really early
            // so we set it here
            this.InstallerComponents = installerComponents;

            // readonly 'placeholder' item
            InstallComponentsList.Items.Add("Project64", CheckState.Indeterminate);

            foreach (InstallerComponent component in InstallerComponents.Components)
            {
                InstallComponentsList.Items.Add(component.Name, component.Enabled);
            }
        }

        private void QuitButton_Click(object sender, EventArgs e) => Application.Exit();

        private void InstallComponentsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if it's 'readonly', keep it 'readonly'
            if (e.CurrentValue == CheckState.Indeterminate)
            {
                e.NewValue = CheckState.Indeterminate;
                return;
            }

            foreach(InstallerComponent component in InstallerComponents.Components)
            {
                if (component.Name ==
                    InstallComponentsList.Items[e.Index].ToString())
                {
                    component.Enabled = e.NewValue == CheckState.Checked;
                    break;
                }
            }

            InstallButton.Enabled = true;

        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new DownloadInstallComponents() { InstallerComponents = InstallerComponents,
                StartPosition = FormStartPosition.Manual, Location = this.Location }.Show();
        }
        private void SelectInstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
