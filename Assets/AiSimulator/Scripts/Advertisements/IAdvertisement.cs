using IndieDevTools.Traits;
using IndieDevTools.Maps;
using UnityEngine;
using System.Collections.Generic;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// A interface used by implementing classes to broadcast that a collection of traits are at a location.
    /// </summary>
    public interface IAdvertisement : ITraitCollection, ILocatable, IGroupMember
    {
        IMap Map { get; }
        List<Vector2Int> BroadcastLocations { get; }
    }
}