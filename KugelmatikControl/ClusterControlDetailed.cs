using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KugelmatikLibrary;
using KugelmatikLibrary.Protocol;

namespace KugelmatikControl
{
    public partial class ClusterControlDetailed : UserControl
    {
        public Cluster Cluster { get; private set; }

        private StepperControl[] steppers;
        private StepperControl selectedStepper = null;

        public ClusterControlDetailed(Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException("cluster");

            this.Cluster = cluster;
            
            InitializeComponent();
            stepperBox.Visible = false;

            const int padding = 2;
            int height = 0;

            steppers = new StepperControl[Cluster.Width * Cluster.Height];

            for (int y = 0; y < Cluster.Height; y++)
            {
                Label labelRow = new Label();
                labelRow.Text = (Cluster.Height - y).ToString();
                labelRow.AutoSize = true;
                steppersPanel.Controls.Add(labelRow);

                for (int x = 0; x < Cluster.Width; x++)
                {
                    StepperControl stepper = new StepperControl(Cluster.GetStepperByPosition(x, Cluster.Height - 1 - y));
                    steppersPanel.Controls.Add(stepper);

                    steppers[y * Cluster.Width + x] = stepper;

                    Label labelColumn = new Label();
                    labelColumn.Text = (x + 1).ToString();
                    labelColumn.AutoSize = true;
                    steppersPanel.Controls.Add(labelColumn);
                    labelColumn.Location = new Point(x * (stepper.Width + padding) + labelRow.Width + stepper.Width / 2 - labelColumn.Width / 2, 0);


                    stepper.Location = new Point(x * (stepper.Width + padding) + labelRow.Width, y * (stepper.Height + padding) + labelColumn.Height);
                    stepper.Click += (sender, e) =>
                    {
                        ShowStepper((StepperControl)sender);
                    };
                    height = stepper.Height + padding;
                }


                labelRow.Location = new Point(0, y * height + height / 2 - labelRow.Height / 2);

                cluster.OnPingChange += UpdateClusterBox;
                cluster.OnInfoChange += UpdateClusterBox;
            }

            UpdateClusterBox(cluster, EventArgs.Empty);
        }

        public IEnumerable<StepperControl> EnumerateStepperControls()
        {
            for (int i = 0; i < steppers.Length; i++)
                yield return steppers[i];
        }

        public void UpdateSteppers()
        {
            for (int i = 0; i < steppers.Length; i++)
                steppers[i].UpdateHeight();
        }

        private void UpdateClusterBox(object sender, EventArgs e)
        {
            if (clusterBox.InvokeRequired)
                clusterBox.BeginInvoke(new Action<int, ClusterInfo>(UpdateClusterBoxInternal), Cluster.Ping, Cluster.Info);
            else
                UpdateClusterBoxInternal(Cluster.Ping, Cluster.Info);
        }

        private void UpdateClusterBoxInternal(int ping, ClusterInfo info)
        {
            try
            {
                if (info == null)
                    clusterInfoGrid.SelectedObject = new object();
                else if (!info.Equals(clusterInfoGrid.SelectedObject))
                    clusterInfoGrid.SelectedObject = info;
            }
            catch(NullReferenceException)
            {
                // weird PropertyGrid Exception
            }

            clusterBox.Text = string.Format(Properties.Resources.ClusterInfo,
                Cluster.X + 1, Cluster.Y + 1,
                ping == -1 ? "offline" : (ping + "ms"),
                Cluster.Address);
        }

        public void ShowStepper(StepperControl stepper)
        {
            if (selectedStepper == stepper)
                return;
            stepperBox.Visible = true;

            if (selectedStepper != null)
                selectedStepper.BackColor = SystemColors.Control;
            stepper.BackColor = SystemColors.Highlight;

            selectedStepper = stepper;

            stepperBox.Text = string.Format(Properties.Resources.StepperInfo, selectedStepper.Stepper.X, selectedStepper.Stepper.Y);
        }

        private void homeStepperButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
                selectedStepper.Stepper.SendHome();
        }

        private void fixStepperButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
                selectedStepper.Stepper.SendFix();
        }

        private void setClusterButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
                selectedStepper.Stepper.Cluster.MoveAllStepper(selectedStepper.Stepper.Height);
        }

        private void setKugelmatikButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
                selectedStepper.Stepper.Kugelmatik.MoveAllClusters(selectedStepper.Stepper.Height);
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            Cluster.SendHome();
        }

        private void getDataButton_Click(object sender, EventArgs e)
        {
            Cluster.SendGetData();
        }

        private void moveToTopButton_Click(object sender, EventArgs e)
        {
            Cluster.MoveAllStepper(0);
            Cluster.SendData();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            Cluster.SendInfo();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Cluster.SendStop();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            Cluster.SendConfig(new ClusterConfig(Cluster.Kugelmatik.Config));
        }

        private void blinkButton_Click(object sender, EventArgs e)
        {
            blinkContextMenu.Show(Cursor.Position);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cluster.SendPacket(new PacketBlinkGreen(), false);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cluster.SendPacket(new PacketBlinkRed(), false);
        }
    }
}
