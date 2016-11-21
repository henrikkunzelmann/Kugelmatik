using System;

namespace KugelmatikLibrary
{
    public interface IChoreographyFunction
    {
        ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y);
    }
}
