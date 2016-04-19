using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KugelmatikLibrary;

namespace KugelmatikControl
{
    public partial class ConfigForm : Form
    {
        private Kugelmatik kugelmatik;

        public ConfigForm(Kugelmatik kugelmatik)
        {
            this.kugelmatik = kugelmatik;

            InitializeComponent();

            propertyGrid.SelectedObject = kugelmatik.Config;
            clusterPropertyGrid.SelectedObject = kugelmatik.ClusterConfig;
        }

        private void SaveConfig()
        {
            Config config = (Config)propertyGrid.SelectedObject;
            ClusterConfig clusterConfig = (ClusterConfig)clusterPropertyGrid.SelectedObject;

            kugelmatik.Config = config;
            kugelmatik.ClusterConfig = clusterConfig;

            ConfigHelper.SaveToFile(MainForm.ConfigFile, config);
            ConfigHelper.SaveToFile(MainForm.ClusterConfigFile, clusterConfig);
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveConfig();
            base.OnClosed(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }
    }
}
