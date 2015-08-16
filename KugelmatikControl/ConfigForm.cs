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
        private Config config;

        public ConfigForm(Config config)
        {
            this.config = config;

            InitializeComponent();

            propertyGrid.SelectedObject = config;
        }

        protected override void OnClosed(EventArgs e)
        {
            config.SaveToFile(MainForm.ConfigFile);
            base.OnClosed(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            config.SaveToFile(MainForm.ConfigFile);
        }
    }
}
