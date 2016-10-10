﻿using KugelmatikControl.PingPong;
using KugelmatikLibrary;
using KugelmatikLibrary.Script;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KugelmatikControl
{
    public partial class MainForm : Form
    {
        public const string ConfigFile = "config.txt";
        public const string ClusterConfigFile = "cluster.txt";

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

            LoadKugelmatik();
        }

        private void CloseAllWindows()
        {
            CloseWindow(logForm);
            CloseWindow(configForm);
            CloseWindow(clusterForm);
            CloseWindow(pingPongForm);
            CloseWindow(heightViewForm);
        }

        private void CloseWindow(Form form)
        {
            if (form != null && form.Visible)
                form.Close();
        }

        private void LoadKugelmatik()
        {
            CloseAllWindows();

            if (choreography != null)
            {
                if (choreography.IsRunning)
                    choreography.Stop();
                choreography.Dispose();
                choreography = null;
            }
            if (Kugelmatik != null)
                Kugelmatik.Dispose();

            Log.Info("Loading kugelmatik...");

            // Config kopieren oder laden
            Config config;
            ClusterConfig clusterConfig;

            if (Kugelmatik != null)
            {
                config = Kugelmatik.Config;
                clusterConfig = Kugelmatik.ClusterConfig;
            }
            else
            {
                config = LoadOrDefault(ConfigFile, Config.GetDefault());
                clusterConfig = LoadOrDefault(ClusterConfigFile, ClusterConfig.GetDefault());
            }

            Kugelmatik = new Kugelmatik(config, clusterConfig);

            // UI erstellen
            clustersPanel.Controls.Clear();
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

            UpdateChoreographyStatus();
        }

        private static T LoadOrDefault<T>(string file, T defaultValue)
        {
            if (File.Exists(file))
                return ConfigHelper.LoadConfigFromFile(file, defaultValue);

            ConfigHelper.SaveToFile(file, defaultValue);
            return defaultValue;
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
                // alle 8 Ticks werden die Daten vollständig gesendet
                // damit werden out-of-sync Fehler behoben wenn ein Paket vom Cluster nicht verarbeitet wurde
                if (tickCount % 8 == 0)
                    Kugelmatik.SendData(false, true);
                else
                    Kugelmatik.SendData();
            }

            if (tickCount % 5 == 0)
                Kugelmatik.ResendPendingPackets();

            if (tickCount % 3 == 0)
                Kugelmatik.SendInfo();

            if (viewOnlyToolStripMenuItem.Checked && tickCount % 2 == 0)
                Kugelmatik.SendGetData();

            // wenn eine Choreograpie läuft, dann werden alle Stepper per Tick geupdatet, da AutomaticUpdate auf false ist
            if (choreography != null)
            {
                for (int i = 0; i < clusterControls.Length; i++)
                    clusterControls[i]?.UpdateSteppers();

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
            var clusters = Kugelmatik.EnumerateClusters();
            var pings = clusters.Select(c => c.Ping);

            double avgPing = pings.Average();
            int maxPing = pings.Max();
            int pending = clusters.Select(c => c.PendingAcknowledgePacketsCount).Sum();

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
                if (clusterControls[i] != null)
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
            StartChoreography(new ChoreographyDirect(c));
        }

        private void StartChoreography(Choreography c)
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
            ShowForm(configForm, () => configForm = new ConfigForm(Kugelmatik));
        }

        private void getDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendGetData();
        }

        private void moveAllTo0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kugelmatik.SetAllClusters(0);
            Kugelmatik.SendData(false, true);
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
            Kugelmatik.SendConfig(Kugelmatik.ClusterConfig);
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

        private void reloadClustersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadKugelmatik();
        }

        private void scriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scriptFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StartChoreography(KugelmatikScript.LoadScript(scriptFileDialog.FileName));
                }
                catch(CompileException ex)
                {
                    MessageBox.Show(string.Format("Compile error in line {0}: {1}", ex.Line, ex.Message), "Compile error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(ex.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Internal error: \r\n" + ex.ToString(), "Internal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
