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

        /// <summary>
        /// Gibt an ob das StepperControl automatisch die Werte vom Stepper aktualisiert werden soll.
        /// </summary>
        public bool AutomaticUpdate { get; set; } = true;

        public StepperControl(Stepper stepper)
        {
            if (stepper == null)
                throw new ArgumentNullException("stepper");

            InitializeComponent();

            // UI updaten
            heightNumber.Minimum = 0;
            heightNumber.Maximum = stepper.Kugelmatik.Config.MaxHeight;
            heightTrackBar.Minimum = 0;
            heightTrackBar.Maximum = stepper.Kugelmatik.Config.MaxHeight;

            ShowStepper(stepper);
            SetupOnClick(this);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ResetStepper();
            base.OnHandleDestroyed(e);
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

        private void ResetStepper()
        {
            if (Stepper != null)
            {
                Stepper.OnHeightChange -= Stepper_OnHeightChange;
                Stepper = null;
            }
        }

        public void ShowStepper(Stepper stepper)
        {
            if (stepper == null)
                throw new ArgumentNullException("stepper");

            if (this.Stepper == stepper)
                return;

            ResetStepper();
            this.Stepper = stepper;
            UpdateHeight();

            stepper.OnHeightChange += Stepper_OnHeightChange;
        }

        public void UpdateHeight()
        {
            if (Stepper != null)
            {
                stepperUpdate = true;
                heightNumber.Value = Stepper.Height;
                heightTrackBar.Value = Stepper.Height;
                stepperUpdate = false;
            }
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
                UpdateHeight();
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
