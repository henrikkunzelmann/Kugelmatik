using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KugelmatikProxy
{
    public static class RevisionHelper
    {
        public static bool CheckRevision(int lastRevision, int revision)
        {
            if (revision < 0 && lastRevision >= 0)
                return true;

            return revision > lastRevision;
        }
    }
}
