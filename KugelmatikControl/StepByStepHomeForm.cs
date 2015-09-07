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
    public partial class StepByStepHomeForm : Form
    {
        public Kugelmatik Kugelmatik { get; private set; }
        private int currentX;
        private int currentY;

        public StepByStepHomeForm(Kugelmatik kugelmatik)
        {
            InitializeComponent();

            this.Kugelmatik = kugelmatik;
            UpdateStatus();
            Kugelmatik.GetStepperByPosition(currentX, currentY).SendHome();
        }

        private void UpdateStatus()
        {
            float n = currentY * Kugelmatik.StepperCountX + currentX;
            statusText.Text = string.Format("X: {0} Y: {1} ({2}%)", currentX, currentY, Math.Floor(100 * (n / (Kugelmatik.StepperCountX * Kugelmatik.StepperCountY))));
        }

        private bool NextPosition()
        {
            currentX++;
            if (currentX >= Kugelmatik.StepperCountX)
            {
                currentX = 0;
                currentY++;
            }

            return currentY < Kugelmatik.StepperCountY;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (!NextPosition())
                return;

            Kugelmatik.SendStop();
            Kugelmatik.GetStepperByPosition(currentX, currentY).SendHome();

            UpdateStatus();
        }

        private void skipButton_Click(object sender, EventArgs e)
        {
            NextPosition();
            Kugelmatik.SendStop();
            UpdateStatus();
        }

        private void nextHome_Click(object sender, EventArgs e)
        {
            Kugelmatik.SendStop();
        }
    }
}
