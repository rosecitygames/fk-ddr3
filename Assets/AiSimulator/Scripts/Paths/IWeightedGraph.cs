using System.Collections.Generic;

namespace IndieDevTools.Paths
{
    /// <summary>
    /// Interface for a generic weighted graph.
    /// </summary>
    public interface IWeightedGraph<T>
    {
        int Cost(T a, T b);
        IEnumerable<T> Neighbors(T id);
    }
}
