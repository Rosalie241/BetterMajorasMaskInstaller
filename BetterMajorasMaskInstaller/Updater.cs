using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterMajorasMaskInstaller
{
    // proxy class for json
    public class InstallerSettingsProxy
    {
        public string InstallDirectory { get => InstallerSettings.InstallDirectory; set => InstallerSettings.InstallDirectory = value; }
        public string DownloadDirectory { get => InstallerSettings.DownloadDirectory; set => InstallerSettings.DownloadDirectory = value; }
        public string ConfigurationUrl { get => InstallerSettings.ConfigurationUrl; set => InstallerSettings.ConfigurationUrl = value; }
        public bool DeveloperMode { get => InstallerSettings.DeveloperMode; set => InstallerSettings.DeveloperMode = value; }
    }

    public class Updater
    {
        public static InstallerComponents InstallerComponents;

        private static readonly string ConfigDirectory = ".installer";
        private static readonly string ConfigFile = Path.Combine(ConfigDirectory, "config.json");
        private static readonly string SettingsFile = Path.Combine(ConfigDirectory, "settings.json");
        public static bool IsUpdateAvailable()
        {
            InstallerComponents newInstallerComponents;
            InstallerComponents oldInstallerComponents;
            InstallerSettingsProxy settings;
            try
            {
                settings = JsonConvert.DeserializeObject<InstallerSettingsProxy>(File.ReadAllText(SettingsFile));

                oldInstallerComponents = JsonConvert.DeserializeObject<InstallerComponents>(File.ReadAllText(ConfigFile));
                newInstallerComponents = JsonConvert.DeserializeObject<InstallerComponents>(
                                                            new WebClient().DownloadString(
                                                                InstallerSettings.ConfigurationUrl));
            }
            catch (Exception)
            {
                // ignore
                return false;
            }

            // make sure we use the same selection as before
            for (int i = 0; i < oldInstallerComponents.Components.Count; i++)
            {
                newInstallerComponents.Components[i].Enabled = oldInstallerComponents.Components[i].Enabled;
            }

            // copy components to memory
            InstallerComponents = newInstallerComponents;

            // lazy
            return JsonConvert.SerializeObject(oldInstallerComponents) != JsonConvert.SerializeObject(newInstallerComponents);
        }

        public static bool AskUserForUpdate()
        {
            DialogResult ret = MessageBox.Show("Update available, install update?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return ret == DialogResult.Yes;
        }
        
        public static void SaveUpdateData()
        {
            try
            {
                InstallerSettingsProxy settings = new InstallerSettingsProxy();

                string dir = Path.Combine(InstallerSettings.InstallDirectory, ConfigDirectory);
                string settingsFile = Path.Combine(InstallerSettings.InstallDirectory, SettingsFile);
                string configFile = Path.Combine(InstallerSettings.InstallDirectory, ConfigFile);

                if (!Directory.Exists(dir))
                {
                    DirectoryInfo info = Directory.CreateDirectory(dir);
                    info.Attributes |= FileAttributes.Hidden;
                }

                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings));
                File.WriteAllText(configFile, JsonConvert.SerializeObject(InstallerComponents));
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save update data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void InstallUpdate()
        {
            Application.Run(
                new Window.DownloadInstallComponents()
                {
                    InstallerComponents = InstallerComponents,
                    StartPosition = FormStartPosition.CenterScreen,
                    FromUpdater = true
               }
            );
        }

    }
}
