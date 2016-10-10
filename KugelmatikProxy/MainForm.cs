using KugelmatikLibrary;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;

namespace KugelmatikProxy
{
    public partial class MainForm : Form
    {
        private ClusterProxy cluster;

        public MainForm()
        {
            InitializeComponent();

            cluster = new ClusterProxy();

            Log.Info("Running");

            Log.OnFlushBuffer += Log_OnFlushBuffer;
            Log.AutomaticFlush = true;
            Log.FlushBuffer();
            Log.BufferCapacity = 1;
        }

        private void Log_OnFlushBuffer(string obj)
        {
            if (logTextBox.InvokeRequired)
                logTextBox.BeginInvoke(new Action<string>(Log_OnFlushBuffer), obj);
            else
                logTextBox.AppendText(obj);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Log.OnFlushBuffer -= Log_OnFlushBuffer;
        }

        private void connectProxyButton_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ipTextBox.Text.Trim(), out ip))
            {
                MessageBox.Show("Invalid ip address!");
                return;
            }
            cluster.ConnectProxy(ip);
        }
    }
}
