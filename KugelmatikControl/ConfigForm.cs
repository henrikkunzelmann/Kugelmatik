using KugelmatikLibrary;
using System;
using System.Windows.Forms;

namespace KugelmatikControl
{
    public partial class ConfigForm : Form
    {
        private MainForm mainForm;
        private Kugelmatik kugelmatik;

        public ConfigForm(MainForm mainForm, Kugelmatik kugelmatik)
        {
            this.mainForm = mainForm;
            this.kugelmatik = kugelmatik;

            InitializeComponent();

            propertyGrid.SelectedObject = kugelmatik.Config;
            clusterPropertyGrid.SelectedObject = kugelmatik.ClusterConfig;
        }

        private void SaveConfig()
        {
            Config config = (Config)propertyGrid.SelectedObject;
            ClusterConfig clusterConfig = (ClusterConfig)clusterPropertyGrid.SelectedObject;

            ConfigHelper.SaveToFile(MainForm.ConfigFile, config);
            ConfigHelper.SaveToFile(MainForm.ClusterConfigFile, clusterConfig);

            if (mainForm.CheckChoreography(true))
            {
                kugelmatik.Config = config;
                kugelmatik.ClusterConfig = clusterConfig;
            }
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
