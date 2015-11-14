﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KugelmatikLibrary;

namespace KugelmatikControl
{
    public partial class ClusterControl : UserControl
    {
        public Cluster Cluster { get; private set; }

        public bool AutomaticUpdate { get; set; } = true;

        public ClusterControl(Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");

            this.Cluster = cluster;
            
            InitializeComponent();

            foreach (Stepper stepper in Cluster.EnumerateSteppers())
                stepper.OnHeightChange += Stepper_OnHeightChange;

            cluster.OnPingChange += UpdateClusterBox;
            cluster.OnInfoChange += UpdateClusterBox;

            UpdateClusterBox(cluster, EventArgs.Empty);
            UpdateGrid();

            SetupOnClick(this);
        }

        private void SetupOnClick(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Click += Control_Click;
                SetupOnClick(control);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, EventArgs.Empty);
        }

        public void UpdateSteppers()
        {
            UpdateGrid();
        }

        private void Stepper_OnHeightChange(object sender, EventArgs e)
        {
            if (!AutomaticUpdate)
                return;

            UpdateGrid();
        }

        private void UpdateClusterBox(object sender, EventArgs e)
        {
            if (clusterBox.InvokeRequired)
                clusterBox.BeginInvoke(new EventHandler(UpdateClusterBox), sender, e);
            else
            {
                clusterBox.Text = string.Format(Properties.Resources.ClusterInfo,
                    Cluster.X + 1, Cluster.Y + 1,
                    Cluster.IsOnline ? (Cluster.Ping + "ms") : "n/a",
                    Cluster.Address);

                infoText.Text = string.Format(Properties.Resources.ClusterInfoLong,
                    Cluster.Info == null ? "n/a" : Cluster.Info.BuildVersion.ToString());

                SetClusterBoxColor(clusterBox, Cluster);
            }
        }

        private void UpdateGrid()
        {
            if (gridText.InvokeRequired)
                gridText.BeginInvoke(new Action(UpdateGrid));
            else
            {
                StringBuilder builder = new StringBuilder(Cluster.Width * Cluster.Height * 5);
            
                for (int y = Cluster.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < Cluster.Width; x++)
                        builder.Append(Cluster.GetStepperByPosition(x, y).Height.ToString().PadLeft(5));
                    builder.AppendLine();
                }
                gridText.Text = builder.ToString();
            }
        }

        /// <summary>
        /// Setzt die Farbe der GroupBox passend zum Status des Clusters.
        /// </summary>
        /// <param name="groupBox"></param>
        /// <param name="cluster"></param>
        public static void SetClusterBoxColor(GroupBox groupBox, Cluster cluster)
        {
            SetClusterBoxColor(groupBox, cluster.Ping);
        }

        /// <summary>
        /// Setzt die Farbe der GroupBox passend zum Ping.
        /// </summary>
        /// <param name="groupBox"></param>
        /// <param name="cluster"></param>
        public static void SetClusterBoxColor(GroupBox groupBox, int ping)
        {
            if (ping >= 150)
                groupBox.ForeColor = Color.DarkOrange;
            else if (ping >= 0)
                groupBox.ForeColor = Color.DarkGreen;
            else
                groupBox.ForeColor = Color.DarkRed;

            // Farbe wieder zurücksetzen
            foreach (Control c in groupBox.Controls)
                c.ForeColor = SystemColors.ControlText;
        }
    }
}
