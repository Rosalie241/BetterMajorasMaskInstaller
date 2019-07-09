using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace BetterMajorasMaskInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (GetRamAmountInGb() >= 4)
                checkBox1.Checked = true;
           
        }
        /// <summary>
        /// Queries RAM capacity from ManagementObjectSearcher and converts it to GB
        /// </summary>
        /// <returns>RAM amount in GB, when it fails, returns -1</returns>
        public int GetRamAmountInGb()
        {
            int ramAmount = 0;
            string query = "SELECT Capacity FROM Win32_PhysicalMemory";

            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        ramAmount += (int)(Convert.ToInt64(mObject.Properties["Capacity"].Value) / 1024 / 1024 / 1024);
                    }
                }
            }
            catch(Exception)
            {
                return -1;
            }

            return ramAmount;
        }
    }
}
