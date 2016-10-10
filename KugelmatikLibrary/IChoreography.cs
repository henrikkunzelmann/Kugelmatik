using System;

namespace KugelmatikLibrary
{
    public interface IChoreography
    {
        ushort GetHeight(Cluster cluster, TimeSpan time, int x, int y);
    }
}
