using System.Drawing;
using System.Windows.Forms;

namespace KugelmatikControl
{
    /// <summary>
    /// Stellt ein Panel dar, welches nicht automatisch scrollt. 
    /// </summary>
    public class NoScrollingPanel : Panel
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            // nicht zum aktiven Control wechseln
            return DisplayRectangle.Location;
        }
    }
}
