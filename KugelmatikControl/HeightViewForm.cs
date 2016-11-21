using KugelmatikLibrary;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

            // UI initialisieren
            minHeight.Minimum = 0;
            minHeight.Maximum = Kugelmatik.ClusterConfig.MaxSteps;


            maxHeight.Minimum = 0;
            maxHeight.Maximum = Kugelmatik.ClusterConfig.MaxSteps;
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
                // Zeit messen die ein Frame brauch zum Zeichnen
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Minimum und Maximum Werte finden
                int min = (int)minHeight.Value;
                int max = (int)maxHeight.Value;

                if (findRangeCheckBox.Checked)
                {
                    min = Kugelmatik.EnumerateSteppers().Select(s => s.Height).Min();
                    max = Kugelmatik.EnumerateSteppers().Select(s => s.Height).Max();

                    minHeight.Value = min;

                    if (min == maxHeight.Maximum)
                        maxHeight.Value = min;
                    else
                        maxHeight.Value = Math.Max(min + 1, max);
                }

                float minHeightValue = (float)min;
                float maxHeightValue = (float)max - minHeightValue;

                // Graphics vorbereiten
                e.Graphics.Clear(SystemColors.Control);
                e.Graphics.TranslateTransform(32, 32);
                e.Graphics.ScaleTransform((float)scale.Value, (float)scale.Value);

                // Outline zeichnen
                e.Graphics.DrawRectangle(Pens.LightGreen, new Rectangle(0, 0, Kugelmatik.StepperCountX, Kugelmatik.StepperCountY));


                for (int x = 0; x < Kugelmatik.StepperCountX; x++)
                    for (int y = 0; y < Kugelmatik.StepperCountY; y++)
                    {
                        Stepper stepper = Kugelmatik.GetStepperByPosition(x, y);

                        int color = (int)Math.Round(255 * (stepper.Height - minHeightValue) / maxHeightValue);
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

                // Frame-Zeit anzeigen
                Font font = new Font("Consolas", 12.0f);
                e.Graphics.ResetTransform();
                e.Graphics.DrawString(stopwatch.Elapsed.TotalMilliseconds.ToString("0.00ms"), font, Brushes.Black, new PointF(8, 8));
            }
            catch(Exception ex)
            {
                Log.Error(ex);
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
            minHeight.Maximum = Math.Max(Kugelmatik.ClusterConfig.MaxSteps, maxHeight.Value);
        }
    }
}
