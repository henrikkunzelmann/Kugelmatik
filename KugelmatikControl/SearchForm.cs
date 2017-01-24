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
    public partial class SearchForm : Form
    {
        private ClusterSearch searcher;

        public SearchForm(Config config)
        {
            InitializeComponent();

            ShowClusters(new ClusterEntry[0]);
            searcher = new ClusterSearch(config);

            Search();
        }

        public void Search()
        {
            if (searcher.IsDisposed)
                return;

            Task.Run(() =>
            {
                ShowClusters(searcher.SearchClusters(TimeSpan.FromSeconds(10)));
            });
        }

        private void ShowClusters(ClusterEntry[] entries)
        {
            if (clusterList.InvokeRequired)
                clusterList.Invoke(new Action<ClusterEntry[]>(ShowClusters), new object[] { entries });
            else
            {
                clusterList.Items.Clear();

                if (entries.Length > 0)
                    clusterList.Items.AddRange(entries);
                else
                    clusterList.Items.Add("Searching...");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            searcher.Dispose();
            base.OnFormClosing(e);
        }

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            Search();
        }
    }
}
