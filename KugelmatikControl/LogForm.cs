using KugelmatikLibrary;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KugelmatikControl
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();

            // Log Klasse vorbereiten 
            Log.OnFlushBuffer += Log_OnFlushBuffer;
            Log.FlushBuffer();
            Log.AutomaticFlush = true;
        }

        private void Log_OnFlushBuffer(object sender, LogFlushEventArgs e)
        {
            if (logTextBox.InvokeRequired)
                logTextBox.BeginInvoke(new EventHandler<LogFlushEventArgs>(Log_OnFlushBuffer), sender, e);
            else
                logTextBox.AppendText(e.Buffer);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Log.OnFlushBuffer -= Log_OnFlushBuffer;
            Log.AutomaticFlush = false;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            logTextBox.Clear();
        }

        private void flushTimer_Tick(object sender, EventArgs e)
        {
            Log.FlushBuffer();
        }
    }
}
