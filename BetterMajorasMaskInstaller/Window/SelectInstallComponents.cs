/*
 BetterMajorasMaskInstaller - https://github.com/Rosalie241/BetterMajorasMaskInstaller
 Copyright (C) 2020 Rosalie Wanders <rosalie@mailbox.org>
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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class SelectInstallComponents : Form
    {
        private readonly List<string> _descriptionList = new List<string>();

        public SelectInstallComponents()
        {
            InitializeComponent();

            foreach (InstallerComponent component in InstallerSettings.InstallerComponents.Components)
            {
                int componentIndex = InstallComponentsList.Items.Add(component.Name, component.Enabled);

                if (component.ReadOnly)
                    InstallComponentsList.SetItemCheckState(componentIndex, CheckState.Indeterminate);

                _descriptionList.Add(component.Description);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Welcome()
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            }.Show();
        }

        private void InstallComponentsList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if it's 'readonly', keep it 'readonly'
            if (e.CurrentValue == CheckState.Indeterminate)
            {
                e.NewValue = CheckState.Indeterminate;
                return;
            }

            foreach (InstallerComponent component in InstallerSettings.InstallerComponents.Components)
            {
                // skip the component if it's not the one we need
                if (component.Name !=
                    InstallComponentsList.Items[e.Index].ToString())
                    continue;

                component.Enabled = e.NewValue == CheckState.Checked || e.NewValue == CheckState.Indeterminate;

                // break if the DisableOnSelected list doesn't exist,
                // or if we're not getting checked
                if (component.DisableOnSelected == null
                    || e.NewValue != CheckState.Checked)
                    break;

                // disable required components
                foreach (string dComponent in component.DisableOnSelected)
                {
                    foreach (InstallerComponent c in InstallerSettings.InstallerComponents.Components)
                    {
                        // skip if not required
                        if (c.Name != dComponent)
                            continue;

                        // disable component
                        c.Enabled = false;

                        int dComponentIndex = InstallComponentsList.FindStringExact(dComponent);

                        // break when not found
                        if (dComponentIndex == -1)
                            break;

                        InstallComponentsList.SetItemCheckState(dComponentIndex, CheckState.Unchecked);
                    }
                }

                break;
            }

            InstallButton.Enabled = true;
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            // download disk space required in bytes
            // we start with 512 to make sure it has at least 512 mb available
            Int64 downloadDiskSpaceRequired = 512;
            Int64 installDiskSpaceRequired = 512;

            // calculate disk space required in mb
            foreach (InstallerComponent component in InstallerSettings.InstallerComponents.Components)
            {
                // skip item when it's disabled
                if (!component.Enabled)
                    continue;

                installDiskSpaceRequired += (component.InstalledSize / 1024 / 1024);

                foreach (Int64 size in component.Urls.Select(x => x.FileSize))
                {
                    downloadDiskSpaceRequired += (size / 1024 / 1024);
                }
            }

            DriveInfo downloadDriveInfo = new DriveInfo(InstallerSettings.DownloadDirectory);
            DriveInfo installDriveInfo = new DriveInfo(InstallerSettings.InstallDirectory);

            // if it's the same drive, download + install size
            if (downloadDriveInfo.Name == installDriveInfo.Name)
            {
                long diskSpaceRequired = downloadDiskSpaceRequired + installDiskSpaceRequired;
                if ((downloadDriveInfo.AvailableFreeSpace / 1024 / 1024) <= diskSpaceRequired)
                {
                    MessageBox.Show($"Not enough free disk space! {diskSpaceRequired} MiB required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // verify download directory drive in MiB
                if ((downloadDriveInfo.AvailableFreeSpace / 1024 / 1024) <= downloadDiskSpaceRequired)
                {
                    MessageBox.Show($"Not enough free disk space! {downloadDiskSpaceRequired} MiB for download directory drive required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // verify install directory drive in MiB
                if ((installDriveInfo.AvailableFreeSpace / 1024 / 1024) <= installDiskSpaceRequired)
                {
                    MessageBox.Show($"Not enough free disk space! {installDiskSpaceRequired} MiB for installation directory drive required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            this.Hide();

            new DownloadInstallComponents()
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location
            }.Show();
        }

        private int toolTipIndex;
        private void InstallComponentsList_ShowToolTip(object source, MouseEventArgs args)
        {
            int newIndex = InstallComponentsList.IndexFromPoint(args.Location);

            if (toolTipIndex == newIndex
                || newIndex == -1)
            {
                return;
            }

            toolTipIndex = newIndex;

            InstallerComponentToolTip.SetToolTip(InstallComponentsList, _descriptionList[toolTipIndex]);
        }

        private void SelectInstallComponents_Closing(object sender, CancelEventArgs args) => Application.Exit();
    }
}
