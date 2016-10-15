using KugelmatikLibrary;
using System;
using System.Windows.Forms;

namespace KugelmatikControl
{
    public partial class ClusterForm : Form
    {
        public MainForm Form { get; private set; }
        public Cluster Cluster { get; private set; }
        public ClusterControlDetailed ClusterControl { get; private set; }

        public ClusterForm(MainForm form)
        {
            this.Form = form;
            InitializeComponent();
        }

        public void UpdateSteppers()
        {
            if (ClusterControl != null)
                ClusterControl.UpdateSteppers();
        }

        public void ShowCluster(Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");

            if (this.Cluster == cluster)
                return;

            this.Cluster = cluster;

            Text = string.Format("Cluster {0}, {1}", cluster.X + 1, cluster.Y + 1);

            if (ClusterControl == null)
            {
                ClusterControl = new ClusterControlDetailed(Form, cluster);
                ClusterControl.Dock = DockStyle.Fill;
                Controls.Add(ClusterControl);
            }
            else
                ClusterControl.ShowCluster(Cluster);
        }
    }
}
