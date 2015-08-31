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

namespace KugelmatikControl.PingPong
{
    public partial class PingPongForm : Form
    {
        public Kugelmatik Kugelmatik { get; private set; }
        public Game Game { get; private set; }

        public PingPongForm(Kugelmatik kugelmatik)
        {
            InitializeComponent();

            this.Kugelmatik = kugelmatik;
            this.Game = new Game(kugelmatik);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            Game.World.PlayerTop.IsComputer = playerTopTextBox.Text.Trim().StartsWith("Bot", StringComparison.InvariantCultureIgnoreCase);
            Game.World.PlayerBottom.IsComputer = playerBottomTextBox.Text.Trim().StartsWith("Bot", StringComparison.InvariantCultureIgnoreCase);

            Game.Update();

            scoreText.Text = string.Format("{0} - {1}",
                Game.World.PlayerTop.Score,
                Game.World.PlayerBottom.Score);
        }

        private void PingPongForm_KeyUp(object sender, KeyEventArgs e)
        {
            Game.OnKeyUp(e.KeyCode);
        }

        private void PingPongForm_KeyDown(object sender, KeyEventArgs e)
        {
            Game.OnKeyDown(e.KeyCode);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Game = new Game(Kugelmatik);
            this.Focus();
        }
    }
}
