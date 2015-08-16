using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using KugelmatikLibrary;

namespace KugelmatikControl
{
    public partial class HeightViewForm : Form
    {
        public Kugelmatik Kugelmatik { get; private set; }

        public HeightViewForm(Kugelmatik kugelmatik)
        {
            if (kugelmatik == null)
                throw new ArgumentNullException("kugelmatik");

            this.Kugelmatik = kugelmatik;

            InitializeComponent();

            minHeight.Minimum = 0;
            minHeight.Maximum = Kugelmatik.Config.MaxHeight;


            maxHeight.Minimum = 0;
            maxHeight.Maximum = Kugelmatik.Config.MaxHeight;
            maxHeight.Value = maxHeight.Maximum;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            heightView.Invalidate();
        }

        private void heightView_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                int min = (int)minHeight.Value;
                int max = (int)maxHeight.Value;

                if (findRangeCheckBox.Checked)
                {
                    min = Kugelmatik.EnumerateSteppers().Select(s => s.Height).Min();
                    max = Kugelmatik.EnumerateSteppers().Select(s => s.Height).Max();

                    minHeight.Value = min;
                    maxHeight.Value = Math.Max(min + 1, max);
                }

                float minHeightValue = (float)min;
                float maxHeightValue = (float)max - minHeightValue;

                e.Graphics.Clear(SystemColors.Control);
                e.Graphics.TranslateTransform(32, 32);
                e.Graphics.ScaleTransform((float)scale.Value, (float)scale.Value);
                e.Graphics.DrawRectangle(Pens.LightGreen, new Rectangle(0, 0, Kugelmatik.StepperCountX, Kugelmatik.StepperCountY));
                for (int x = 0; x < Kugelmatik.StepperCountX; x++)
                    for (int y = 0; y < Kugelmatik.StepperCountY; y++)
                    {
                        Stepper stepper = Kugelmatik.GetStepperByPosition(x, y);

                        int color = (int)(255 * (stepper.Height - minHeightValue) / maxHeightValue);
                        if (color < 0)
                            color = 0;
                        if (color > byte.MaxValue)
                            color = byte.MaxValue;

                        if (invertCheckBox.Checked)
                            color = 255 - color;

                        Brush brush = new SolidBrush(Color.FromArgb(color, color, color));
                        e.Graphics.FillRectangle(brush, x, y, 1, 1);
                    }

                stopwatch.Stop();

                Font font = new Font("Consolas", 12.0f);
                e.Graphics.ResetTransform();
                e.Graphics.DrawString(stopwatch.ElapsedMilliseconds + "ms", font, Brushes.Black, new PointF(8, 8));
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        private void minHeight_ValueChanged(object sender, EventArgs e)
        {
            maxHeight.Minimum = Math.Max(1, minHeight.Value);
        }

        private void maxHeight_ValueChanged(object sender, EventArgs e)
        {
            minHeight.Maximum = Math.Max(Kugelmatik.Config.MaxHeight, maxHeight.Value);
        }
    }
}
