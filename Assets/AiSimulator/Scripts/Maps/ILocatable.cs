using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// An interface for objects with a cell location.
    /// </summary>
    public interface ILocatable : IUpdatable<ILocatable>
    {
        Vector2Int Location { get; }
    }
}
