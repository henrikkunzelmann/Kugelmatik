using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using KugelmatikLibrary;
using KugelmatikControl.PingPong;

namespace KugelmatikControl
{
    public partial class MainForm : Form
    {
        public const string ConfigFile = "config.txt";

        public Kugelmatik Kugelmatik { get; private set; }

        private ChoreographyManager choreography;

        private ClusterControl[] clusterControls;

        private LogForm logForm;
        private ConfigForm configForm;
        private ClusterForm clusterForm;
        private PingPongForm pingPongForm;
        private HeightViewForm heightViewForm;

        public MainForm()
        {
            InitializeComponent();

            Log.AutomaticFlush = false;
            Log.WriteToConsole = false;
            Log.WriteToDebug = false;

            Config config = new Config();

            if (File.Exists(ConfigFile))
                config = Config.LoadConfigFromFile(ConfigFile);
            else
                config.SaveToFile(ConfigFile);



            Kugelmatik = new Kugelmatik(config);

            clusterControls = new ClusterControl[Kugelmatik.Config.KugelmatikWidth * Kugelmatik.Config.KugelmatikHeight];

            const int padding = 5;
            for (int y = 0; y < Kugelmatik.Config.KugelmatikHeight; y++)
                for (int x = 0; x < Kugelmatik.Config.KugelmatikWidth; x++)
                {
                    ClusterControl cluster = new ClusterControl(Kugelmatik.GetClusterByPosition(x, Kugelmatik.Config.KugelmatikHeight - 1 - y));
                    cluster.Location = new Point(x * (cluster.Width + padding), y * (cluster.Height + padding));
                    cluster.Click += Cluster_Click;
                    clustersPanel.Controls.Add(cluster);
                    clusterControls[y * Kugelmatik.Config.KugelmatikWidth + x] = cluster;
                }

            UpdateChoreographyStatus();;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (choreography != null)
                choreography.Dispose();

            if (Kugelmatik != null)
                Kugelmatik.Dispose();

            base.OnClosed(e);
        }

        private int tickCount = 0;

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // Ping senden
            Kugelmatik.SendPing();

            // Daten senden
            if (!viewOnlyToolStripMenuItem.Checked)
            {
                // alle 4 Ticks werden die Daten vollständig gesendet
                // damit werden out-of-sync Fehler behoben wenn ein Paket vom Cluster nicht verarbeitet wurde
                if (tickCount % 4 == 0)
                    Kugelmatik.SendData(false, true);
                else
                    Kugelmatik.SendData();
            }

            if (tickCount % 5 == 0)
                Kugelmatik.ResendPendingPackets();

            // alle 10 Ticks Board Informationen neu anfordern
            if (tickCount % 10 == 0)
                Kugelmatik.SendInfo();

            if (viewOnlyToolStripMenuItem.Checked && tickCount % 2 == 0)
                Kugelmatik.SendGetData();

            // wenn eine Choreograpie läuft, dann werden alle Stepper per Tick geupdatet, da AutomaticUpdate auf false ist
            if (choreography != null)
            {
                for (int i = 0; i < clusterControls.Length; i++)
                    clusterControls[i].UpdateSteppers();

                clusterForm?.ClusterControl?.UpdateSteppers();
            }

            UpdateChoreographyStatus();
            UpdateNetworkStatus();

            tickCount++;
        }

        public void ShowCluster(Cluster cluster)
        {
            if (clusterForm == null || clusterForm.IsDisposed)
                clusterForm = new ClusterForm();

            if (!clusterForm.Visible)
                clusterForm.Show(this);
            clusterForm.ShowCluster(cluster);
            clusterForm.BringToFront();

            if (choreography != null && choreography.IsRunning)
                SetAutomaticUpdate(false);
        }

        private void UpdateNetworkStatus()
        {
            // Ping berechnen
            double avgPing = Kugelmatik.EnumerateClusters().Select(c => c.Ping).Average();
            int maxPing = Kugelmatik.EnumerateClusters().Select(c => c.Ping).Max();
            int pending = Kugelmatik.EnumerateClusters().Select(c => c.PendingAcknowledgePacketsCount).Sum();

            networkStatusLabel.Text = string.Format(Properties.Resources.NetworkStatus,
                avgPing < 0 ? "n/a" : string.Format("{0:0.0}ms", avgPing),
                maxPing < 0 ? "n/a" : string.Format("{0}ms", maxPing), pending);
        }

        private void UpdateChoreographyStatus()
        {
            if (choreography == null || !choreography.IsRunning || choreography.IsDisposed)
                choreographyStatusLabel.Text = Properties.Resources.ChoreographyStatusNone;
            else
                choreographyStatusLabel.Text = string.Format(Properties.Resources.ChoreographyStatus, choreography.Choreography.GetType().Name, choreography.FPS, choreography.TargetFPS);
        }

        /// <summary>
        /// Setzt alle ClusterControl und StepperControl AutomaticUpdate Werte auf value.
        /// Wenn AutomaticUpdate auf false ist, dann reagieren die Controls nicht mehr auf das HeightChanged-Event von Stepper.
        /// </summary>
        /// <param name="value"></param>
        private void SetAutomaticUpdate(bool value)
        {
            for (int i = 0; i < clusterControls.Length; i++)
                clusterControls[i].AutomaticUpdate = value;

            if (clusterForm != null && clusterForm.ClusterControl != null)
                foreach (StepperControl stepper in clusterForm.ClusterControl.EnumerateStepperControls())
                    stepper.AutomaticUpdate = value;
        }

        /// <summary>
        /// Öffnet ein Fenster.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="onClosed">Wird aufgerufen wenn form geschlossen ist.</param>
        /// <returns></returns>
        public Form ShowForm(Form form, Func<Form> onClosed)
        {
            if (onClosed == null)
                throw new ArgumentNullException("onClosed");

            if (form == null || form.IsDisposed)
                form = onClosed();

            if (!form.Visible)
                form.Show(this);
            form.BringToFront();
            return form;
        }

        private void StartChoreography(IChoreography c)
        {
            // wenn schon eine Choreography läuft, dann stoppen
            if (choreography != null)
            {
                if (choreography.IsRunning)
                    choreography.Stop();
                choreography.Dispose();
            }

            choreography = new ChoreographyManager(Kugelmatik, 60, c);
            choreography.Start();

            UpdateChoreographyStatus();
            SetAutomaticUpdate(false);
        }

        private void Cluster_Click(object sender, EventArgs e)
        {
            ShowCluster(((ClusterControl)sender).Cluster);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendHome();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(configForm, () => configForm = new ConfigForm(Kugelmatik.Config));
        }

        private void getDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendGetData();
        }

        private void moveAllTo0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.MoveAllClusters(0);
            Kugelmatik.SendData();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (choreography != null && choreography.IsRunning)
                choreography.Stop();

            UpdateChoreographyStatus();
            SetAutomaticUpdate(true);
        }

        private void sineWaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new SineWave(SineWave.Direction.Y, 0.001f, 0.20f));
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendInfo();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(logForm, () => logForm = new LogForm());
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendStop();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendConfig(new ClusterConfig(Kugelmatik.Config));
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendData();
        }

        private void pingPongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(pingPongForm, () => pingPongForm = new PingPongForm(Kugelmatik));
        }

        private void heightViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(heightViewForm, () => heightViewForm = new HeightViewForm(Kugelmatik));
        }

        private void resetRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Cluster cluster in Kugelmatik.EnumerateClusters())
                cluster.SendPacket(new KugelmatikLibrary.Protocol.PacketResetRevision(), false);
        }

        private void distanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new DistanceChoreography());
        }

        private void rippleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new Ripple());
        }

        private void viewOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewOnlyToolStripMenuItem.Checked = !viewOnlyToolStripMenuItem.Checked;
        }

        private void upDownUpDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new FunctionChoreography((config, time, x, y) =>
            {
                float d = (float)((y + time.TotalSeconds * 0.5f) % 10);
                if (d >= 5)
                    d = 10 - d;
                return (ushort)(d / 5 * config.MaxHeight);
            }));
        }
    }
}
