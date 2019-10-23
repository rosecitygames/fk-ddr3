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
        /// <summary>
        /// The map that the advertisement will be broadcasted on.
        /// </summary>
        IMap Map { get; }

        /// <summary>
        /// The locations where the advertisement will be broadcasted at.
        /// </summary>
        List<Vector2Int> BroadcastLocations { get; }
    }
}