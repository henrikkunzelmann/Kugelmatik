using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KugelmatikLibrary;

namespace KugelmatikControl
{
    public partial class StepperControl : UserControl
    {
        public Stepper Stepper { get; private set; }

        public bool AutomaticUpdate { get; set; } = true;

        public StepperControl(Stepper stepper)
        {
            if (stepper == null)
                throw new ArgumentNullException("stepper");

            this.Stepper = stepper;

            InitializeComponent();

            // UI updaten
            heightNumber.Minimum = 0;
            heightNumber.Maximum = stepper.Kugelmatik.Config.MaxHeight;
            heightTrackBar.Minimum = 0;
            heightTrackBar.Maximum = stepper.Kugelmatik.Config.MaxHeight;

            heightNumber.Value = stepper.Height;
            stepper.OnHeightChange += Stepper_OnHeightChange;
            SetupOnClick(this);
        }

        private void SetupOnClick(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Click += Control_Click;
                SetupOnClick(control);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, EventArgs.Empty);
        }

        public void UpdateHeight()
        {
            heightNumber.Value = Stepper.Height;
            heightTrackBar.Value = Stepper.Height;
        }

        // verhindert das wir den Stepper updaten wenn die Änderung vom Stepper selbst stammt
        // damit wird ein Deadlock verhindert
        private bool stepperUpdate = false;

        private void Stepper_OnHeightChange(object sender, EventArgs e)
        {
            if (!AutomaticUpdate)
                return;

            if (heightNumber.InvokeRequired)
                heightNumber.BeginInvoke(new Action<object, EventArgs>(Stepper_OnHeightChange), sender, e);
            else
            {
                stepperUpdate = true;
                UpdateHeight();
                stepperUpdate = false;
            }
        }

        private void heightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (stepperUpdate)
                return;

            heightNumber.Value = heightTrackBar.Value;
            Stepper.MoveTo((ushort)heightTrackBar.Value);
        }

        private void heightNumber_ValueChanged(object sender, EventArgs e)
        {
            if (stepperUpdate)
                return;

            heightTrackBar.Value = (int)heightNumber.Value;
            Stepper.MoveTo((ushort)heightNumber.Value);
        }
    }
}
