using KugelmatikLibrary;
using KugelmatikLibrary.Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KugelmatikControl
{
    public partial class ClusterControlDetailed : UserControl
    {
        public MainForm Form { get; private set; }

        /// <summary>
        /// Gibt das derzeitige gezeitige Cluster zurück.
        /// </summary>
        public Cluster CurrentCluster { get; private set; }

        private StepperControl[] steppers;
        private StepperControl selectedStepper = null;

        private bool updatingClusterHeight = false;

        public ClusterControlDetailed(MainForm form, Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException(nameof(cluster));

            this.Form = form;

            InitializeComponent();
            
            const int padding = 2; // Abstand zwischen zwei StepperControls
            int height = 0;

            steppers = new StepperControl[Cluster.Width * Cluster.Height];

            // im folgenden wird über X und Y Koordinaten eine Tabelle erzeugt
            for (int y = 0; y < Cluster.Height; y++)
            {
                Label labelRow = new Label();
                labelRow.Text = (Cluster.Height - y).ToString();
                labelRow.AutoSize = true;
                steppersPanel.Controls.Add(labelRow);

                for (int x = 0; x < Cluster.Width; x++)
                {
                    StepperControl stepper = new StepperControl(cluster.GetStepperByPosition(x, Cluster.Height - 1 - y));
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
            }

            ShowCluster(cluster);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ResetCurrentCluster();
            base.OnHandleDestroyed(e);
        }


        private void ResetCurrentCluster()
        {
            if (CurrentCluster != null)
            {
                CurrentCluster.OnPingChange -= UpdateClusterBox;
                CurrentCluster.OnInfoChange -= UpdateClusterBox;
                CurrentCluster = null;
            }
        }

        public void ShowCluster(Cluster cluster)
        {
            if (cluster == null)
                throw new ArgumentNullException(nameof(cluster));

            if (CurrentCluster == cluster)
                return;

            // UI zurücksetzen
            ActiveControl = null;
            ShowStepper(null);
            ResetCurrentCluster();

            CurrentCluster = cluster;

            // neue UI zeigen
            for (int x = 0; x < Cluster.Width; x++)
                for (int y = 0; y < Cluster.Height; y++)
                    steppers[y * Cluster.Width + x].ShowStepper(CurrentCluster.GetStepperByPosition(x, Cluster.Height - 1 - y));
            UpdateClusterBox(cluster, EventArgs.Empty);

            // Events setzen
            cluster.OnPingChange += UpdateClusterBox;
            cluster.OnInfoChange += UpdateClusterBox;

            UpdateClusterHeight();
        }

        private void UpdateClusterHeight()
        {
            updatingClusterHeight = true;

            clusterHeight.Minimum = 0;
            clusterHeight.Maximum = CurrentCluster.Kugelmatik.ClusterConfig.MaxSteps;

            clusterHeightTrackBar.Minimum = 0;
            clusterHeightTrackBar.Maximum = CurrentCluster.Kugelmatik.ClusterConfig.MaxSteps;

            int avgHeight = (int)CurrentCluster.EnumerateSteppers().Average(s => s.Height);
            clusterHeight.Value = avgHeight;
            clusterHeightTrackBar.Value = avgHeight;

            updatingClusterHeight = false;
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
                clusterBox.BeginInvoke(new Action<int, ClusterInfo>(UpdateClusterBoxInternal), CurrentCluster.Ping, CurrentCluster.Info);
            else
                UpdateClusterBoxInternal(CurrentCluster.Ping, CurrentCluster.Info);
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
                // komische PropertyGrid Exception ignorieren
            }

            clusterBox.Text = string.Format(Properties.Resources.ClusterInfo,
                CurrentCluster.X + 1, CurrentCluster.Y + 1,
                ping == -1 ? "offline" : (ping + "ms"),
                CurrentCluster.Address == null ? "none" : CurrentCluster.Address.ToString());

            ClusterControl.SetClusterBoxColor(clusterBox, ping, info);
        }

        private void ShowStepper(StepperControl stepper)
        {
            // Hintergrund-Farbe von Stepper rückgängig machen
            if (selectedStepper != null)
                selectedStepper.BackColor = SystemColors.Control;

            selectedStepper = stepper;


            if (selectedStepper != null)
            {
                stepperBox.Visible = true;
                stepperBox.Text = string.Format(Properties.Resources.StepperInfo, selectedStepper.Stepper.X + 1, selectedStepper.Stepper.Y + 1);

                selectedStepper.BackColor = SystemColors.Highlight;
            }
            else
                stepperBox.Visible = false;
        }

        public bool CheckOnlineStatus()
        {
            if (!CurrentCluster.IsOnline)
            {
                Form.ShowClusterOfflineError();
                return false;
            }
            return true;
        }

        private void homeStepperButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
            {
                if (CheckOnlineStatus() && Form.CheckChoreography())
                {
                    selectedStepper.Stepper.SendHome();
                    selectedStepper.Stepper.Cluster.SendInfo();
                }
            }
        }

        private void fixStepperButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
            {
                if (CheckOnlineStatus() && Form.CheckChoreography())
                {
                    selectedStepper.Stepper.SendFix();
                    selectedStepper.Stepper.Cluster.SendInfo();
                }
            }
        }

        private void setClusterButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
            {
                if (Form.CheckChoreography())
                {
                    selectedStepper.Stepper.Cluster.SetAllStepper(selectedStepper.Stepper.Height);
                    UpdateClusterHeight();
                }
            }
        }

        private void setKugelmatikButton_Click(object sender, EventArgs e)
        {
            if (selectedStepper != null)
            {
                if (Form.CheckOnlineStatus() && Form.CheckChoreography())
                {
                    selectedStepper.Stepper.Kugelmatik.SetAllClusters(selectedStepper.Stepper.Height);
                    UpdateClusterHeight();
                }
            }
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            if (CheckOnlineStatus() && Form.CheckChoreography())
            {
                CurrentCluster.SendHome();
                CurrentCluster.SendInfo();
                UpdateClusterHeight();
            }
        }

        private void getDataButton_Click(object sender, EventArgs e)
        {
            CurrentCluster.SendGetData();
            UpdateClusterHeight();
        }

        private void moveToTopButton_Click(object sender, EventArgs e)
        {
            if (Form.CheckChoreography())
            {
                CurrentCluster.SetAllStepper(0);
                CurrentCluster.SendData(false, true);
                UpdateClusterHeight();
            }
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            CurrentCluster.SendInfo();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (Form.CheckChoreography(true))
            {
                CurrentCluster.SendStop();
                CurrentCluster.SendInfo();
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            CurrentCluster.SendConfig(CurrentCluster.Kugelmatik.ClusterConfig);
        }

        private void blinkButton_Click(object sender, EventArgs e)
        {
            blinkContextMenu.Show(Cursor.Position);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentCluster.SendPacket(new PacketBlinkGreen(), false);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentCluster.SendPacket(new PacketBlinkRed(), false);
        }

        private void clusterHeight_ValueChanged(object sender, EventArgs e)
        {
            if (updatingClusterHeight)
                return;

            clusterHeightTrackBar.Value = (int)clusterHeight.Value;
            CurrentCluster.SetAllStepper((ushort)clusterHeight.Value);
        }

        private void clusterHeightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (updatingClusterHeight)
                return;

            clusterHeight.Value = clusterHeightTrackBar.Value;
            CurrentCluster.SetAllStepper((ushort)clusterHeightTrackBar.Value);
        }

        private void setDataButton_Click(object sender, EventArgs e)
        {
            if (CheckOnlineStatus())
                CurrentCluster.SendSetData();
        }
    }
}
