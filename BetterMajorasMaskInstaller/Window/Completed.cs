﻿/*
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
using IWshRuntimeLibrary;
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
using File = System.IO.File;

namespace BetterMajorasMaskInstaller.Window
{
    public partial class Completed : Form
    {
        public Completed()
        {
            InitializeComponent();
        }

        public void Welcome_Closing(object sender, CancelEventArgs e) => QuitButton_Click(this, null);

        private void CreateShortcut(string path)
        {
            IWshShortcut shortcut = (IWshShortcut)new WshShell().CreateShortcut(path);

            shortcut.Description = InstallerSettings.InstallerConfiguration.ShortcutInfo.Description;
            shortcut.TargetPath = Path.Combine(
                InstallerSettings.InstallDirectory,
                InstallerSettings.InstallerConfiguration.ShortcutInfo.Executable);

            shortcut.IconLocation = Path.Combine(
                InstallerSettings.InstallDirectory,
                InstallerSettings.InstallerConfiguration.ShortcutInfo.IconLocation);

            shortcut.WorkingDirectory = InstallerSettings.InstallDirectory;

            shortcut.Save();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            // create desktop shortcut
            if (DesktopShortcutCheckBox.Checked)
            {
                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    InstallerSettings.InstallerConfiguration.ShortcutInfo.FileName)
                );
            }

            // create start menu shortcut
            if (StartMenuShortcutCheckBox.Checked)
            {
                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    InstallerSettings.InstallerConfiguration.ShortcutInfo.FileName)
                );
            }

            if (TemporaryFilesCheckBox.Checked)
            {
                ComponentHelper.CleanupDownloadFiles(InstallerSettings.InstallerConfiguration);
            }

            Application.Exit();
        }
    }
}
