using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikLibrary.Choreographies
{
    public class ShowEntry
    {
        public TimeSpan Time { get; private set; }
        public IChoreographyFunction Choreography { get; private set; }

        public ShowEntry(TimeSpan time, IChoreographyFunction choreography)
        {
            this.Time = time;
            this.Choreography = choreography;
        }
    }
}
