using KugelmatikControl.PingPong;
using KugelmatikLibrary;
using KugelmatikLibrary.Script;
using System;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KugelmatikLibrary.Choreographies;

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
        private SearchForm searchForm;

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
            CloseWindow(searchForm);
        }

        private void CloseWindow(Form form)
        {
            if (form != null && form.Visible)
                form.Close();
        }

        private void LoadKugelmatik()
        {
            try
            {
                loadError.Visible = false;
                reloadKugelmatik.Visible = false;

                UnloadKugelmatik();

                Log.Info("Loading kugelmatik...");

                CheckOtherInstances();

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
            }
            catch(Exception e)
            {
                Log.Error(e);
                MessageBox.Show(e.ToString(), "Could not load kugelmatik", MessageBoxButtons.OK, MessageBoxIcon.Error);

                loadError.Visible = true;
                reloadKugelmatik.Visible = true;

                clusterControls = new ClusterControl[0];
            }

            UpdateNetworkStatus();
            UpdateChoreographyStatus();
        }

        

        private void CheckOtherInstances()
        {
            DialogResult result;
            do
            {
                Process self = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(self.ProcessName);
                if (processes.Length <= 1)
                    return;

                string text = "Following instances found:";
                text += Environment.NewLine;
                text += string.Join(Environment.NewLine,
                    processes.Where(p => p.Id != self.Id).Select(p => string.Format("{0} (PID {1})", p.ProcessName, p.Id)));

                result = MessageBox.Show(text, "Multiple Kugelmatik instances running",
                    MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (result == DialogResult.Ignore)
                    return;

                if (result == DialogResult.Abort)
                    throw new InvalidOperationException("A Kugelmatik instance is already running");
            }
            while (result == DialogResult.Retry);
        }

        private static T LoadOrDefault<T>(string file, T defaultValue)
        {
            if (File.Exists(file))
                return ConfigHelper.LoadConfigFromFile(file, defaultValue);

            ConfigHelper.SaveToFile(file, defaultValue);
            return defaultValue;
        }

        private void UnloadKugelmatik()
        {
            CloseAllWindows();
            StopChoreographyInternal();

            if (Kugelmatik != null)
                Kugelmatik.Dispose();
        }

        protected override void OnClosed(EventArgs e)
        {
            UnloadKugelmatik();
            base.OnClosed(e);
        }

        private int tickCount = 0;

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Kugelmatik == null)
                    return;

                // Ping senden
                Kugelmatik.SendPing();

                // Daten senden
                if (!viewOnlyToolStripMenuItem.Checked)
                {
                    // alle 8 Ticks werden die Daten vollständig gesendet
                    // damit werden out-of-sync Fehler behoben wenn ein Paket vom Cluster nicht verarbeitet wurde
                    if (tickCount % 8 == 0)
                        SendDataOnlyToTouchedCluster();
                    else
                        Kugelmatik.SendData();
                }

                if (tickCount % 5 == 0)
                    Kugelmatik.ResendPendingPackets();

                if (tickCount % 20 == 0)
                    Kugelmatik.SendInfo();

                if (viewOnlyToolStripMenuItem.Checked && tickCount % 2 == 0)
                    Kugelmatik.SendGetData();

                // wenn eine Choreograpie läuft, dann werden alle Stepper per Tick geupdatet, da AutomaticUpdate auf false ist
                if (choreography != null && choreography.IsRunning)
                {
                    for (int i = 0; i < clusterControls.Length; i++)
                        clusterControls[i]?.UpdateSteppers();

                    clusterForm?.ClusterControl?.UpdateSteppers();

                    CheckChoreographyAutoStop();
                }

                UpdateChoreographyStatus();
                UpdateNetworkStatus();

                tickCount++;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void SendDataOnlyToTouchedCluster()
        {
            DateTime now = DateTime.Now;
            foreach (Cluster cluster in Kugelmatik.EnumerateClusters())
                if (now - cluster.LastSet < TimeSpan.FromSeconds(60))
                    cluster.SendData(false, true);
        }

        public void ShowCluster(Cluster cluster)
        {
            if (clusterForm == null || clusterForm.IsDisposed)
                clusterForm = new ClusterForm(this);

            if (!clusterForm.Visible)
                clusterForm.Show(this);
            clusterForm.ShowCluster(cluster);
            clusterForm.BringToFront();

            if (choreography != null && choreography.IsRunning)
                SetAutomaticUpdate(false);
        }

        private void UpdateNetworkStatus()
        {
            if (Kugelmatik == null)
            {
                networkStatusLabel.Text = "Error while loading kugelmatik";
                return;
            }
            
            if (!Kugelmatik.AnyClusterOnline)
            {
                networkStatusLabel.Text = "All clusters are offline";
                return;
            }

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
            {
                string name;
                ChoreographyDirect direct = choreography.Choreography as ChoreographyDirect;
                if (direct != null)
                    name = direct.Function.GetType().Name;
                else
                    name = choreography.Choreography.GetType().Name;

                choreographyStatusLabel.Text = string.Format(Properties.Resources.ChoreographyStatus, name, choreography.FPS, choreography.TargetFPS);
            }
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

        private void StartChoreography(IChoreographyFunction c)
        {
            StartChoreography(new ChoreographyDirect(c));
        }

        private void StartChoreography(Choreography c)
        {
            if (Kugelmatik == null)
                return;

            if (!ShowOfflineError())
                return;
          
            // wenn schon eine Choreography läuft, dann stoppen
            StopChoreographyInternal();

            choreography = new ChoreographyManager(Kugelmatik, 60, c);
            choreography.Start();

            UpdateChoreographyStatus();
            SetAutomaticUpdate(false);
        }

        private bool ShowOfflineError()
        {
            DialogResult result;
            do
            {
                if (Kugelmatik.AnyClusterOnline)
                    return true;

                if (autoStopToolStripMenuItem.Checked)
                {
                    MessageBox.Show("Can not start choreography because all clusters are offline and auto stop is checked.", "Choreography not started", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }  

                result = MessageBox.Show("Can not start choreography because all clusters are offline. Auto stop is not checked. Ignore will start the choreography anyway.", "Choreography not started", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

                if (result == DialogResult.Ignore)
                    return true;
            }
            while (result == DialogResult.Retry);
            return false;
        }

        private void StopChoreography()
        {
            StopChoreographyInternal();
            UpdateChoreographyStatus();
            SetAutomaticUpdate(true);
        }

        private void StopChoreographyInternal()
        {
            if (choreography != null)
            {
                if (choreography.IsRunning)
                    choreography.Stop();

                choreography.Dispose();
                choreography = null;
            }
        }

        public bool CheckOnlineStatus()
        {
            if (!Kugelmatik.AnyClusterOnline)
            {
                ShowClusterOfflineError(true);
                return false;
            }
            return true;
        }

        public void ShowClusterOfflineError(bool multiple)
        {
            if (multiple)
                MessageBox.Show("Can not run command because all clusters are offline.", "Command did not run", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Can not run command because cluster is offline.", "Command did not run", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool CheckChoreography()
        {
            return CheckChoreography(false);
        }

        public bool CheckChoreography(bool forceStop)
        {
            if (choreography == null || !choreography.IsRunning)
                return true;

            // Choreography soll direkt gestoppt werden
            if (forceStop)
            {
                StopChoreography();
                return true;
            }

            // Benutzer erst fragen
            DialogResult result = MessageBox.Show("Command not ran, because a choreography is running. Do you want to stop the choreography? No will run the command anyway.",
                "Warning: Stop the choreography?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.Yes)
                StopChoreography();

            return true;
        }

        public void CheckChoreographyAutoStop()
        {
            if (!autoStopToolStripMenuItem.Checked)
                return;

            if (!Kugelmatik.AnyClusterOnline)
            {
                StopChoreography();
                MessageBox.Show("Choreography stopped because all clusters are offline.", "Choreography stopped", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            if (Kugelmatik == null)
                return;

            if (CheckChoreography())
                Kugelmatik.SendHome();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                ShowForm(configForm, () => configForm = new ConfigForm(this, Kugelmatik));
        }

        private void getDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                Kugelmatik.SendGetData();
        }

        private void moveAllTo0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null && CheckChoreography())
            {
                Kugelmatik.SetAllClusters(0);
                Kugelmatik.SendData(false, true);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null && CheckChoreography(true))
                Kugelmatik.SendStop();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                Kugelmatik.SendInfo();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(logForm, () => logForm = new LogForm());
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                Kugelmatik.SendConfig(Kugelmatik.ClusterConfig);
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                Kugelmatik.SendData();
        }

        private void pingPongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null && CheckChoreography())
                ShowForm(pingPongForm, () => pingPongForm = new PingPongForm(Kugelmatik));
        }

        private void heightViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                ShowForm(heightViewForm, () => heightViewForm = new HeightViewForm(Kugelmatik));
        }

        private void searchClustersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null)
                ShowForm(searchForm, () => searchForm = new SearchForm(Kugelmatik.Config));
        }

        private void resetRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Kugelmatik == null)
                return;

            foreach (Cluster cluster in Kugelmatik.EnumerateClusters())
                cluster.SendPacket(new KugelmatikLibrary.Protocol.PacketResetRevision(), false);
        }

        private void stopChoreographyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopChoreography();
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
                    Log.Error(ex);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Internal error: \r\n" + ex.ToString(), "Internal error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(ex);
                }
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (Kugelmatik != null && CheckChoreography(true))
                Kugelmatik.SendStop();
        }

        private void autoStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckChoreographyAutoStop();
        }

        private void sineWaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new SineWave(Direction.Y, 0.01f));
        }

        private void distanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new DistanceChoreography(TimeSpan.FromMinutes(2)));
        }

        private void rippleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new Ripple());
        }

        private void linearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new LinearMove(TimeSpan.FromMinutes(1)));
        }

        private void planeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new Plane(TimeSpan.FromMinutes(2), 0.9f));
        }

        private void splitPlaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartChoreography(new SplitPlane(TimeSpan.FromMinutes(2), 0.9f));
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEntry[] entries = new ShowEntry[]
            {
                new ShowEntry(TimeSpan.FromMinutes(3), new SineWave(Direction.X, 0.01f)),
                new ShowEntry(TimeSpan.FromMinutes(3), new SineWave(Direction.Y, 0.01f)),

                new ShowEntry(TimeSpan.FromMinutes(4), new Plane(TimeSpan.FromMinutes(2), 0.9f)),
                new ShowEntry(TimeSpan.FromMinutes(4), new SplitPlane(TimeSpan.FromMinutes(2), 0.9f)),
                new ShowEntry(TimeSpan.FromMinutes(4), new Plane(TimeSpan.FromMinutes(2), 0.9f)),
            };

            Show show = new Show(entries, TimeSpan.FromMinutes(1));
            StartChoreography(show);
        }
    }
}
