using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KugelmatikLibrary;
using System.Drawing;
using System.Windows.Forms;

namespace KugelmatikControl.PingPong
{
    public class Game 
    {
        public Kugelmatik Kugelmatik { get; private set; }
        public World World { get; private set; }

        private Dictionary<Keys, bool> pressedKeys = new Dictionary<Keys, bool>();

        public Game(Kugelmatik kugelmatik)
        {
            if (kugelmatik == null)
                throw new ArgumentNullException("kugelmatik");

            this.Kugelmatik = kugelmatik;
            this.World = new World(this);
        }

        public void Update()
        {
            World.Update();
            World.DrawToKugelmatik();
        }

        public void OnKeyDown(Keys key)
        {
            pressedKeys[key] = true;
        }

        public void OnKeyUp(Keys key)
        {
            pressedKeys[key] = false;
        }

        public bool IsKeyDown(Keys key)
        {
            bool val;
            if (pressedKeys.TryGetValue(key, out val))
                return val;
            return false;
        }
    }
}
