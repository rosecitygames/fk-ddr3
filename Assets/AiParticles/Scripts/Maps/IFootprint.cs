using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A footprint is a collection of sub elements for a map element that is larger than one grid unit.
    /// </summary>
    /// <typeparam name="T">The type of map element the footprint is.</typeparam>
    public interface IFootprint<T>
    {
        /// <summary>
        /// A list of all the footprint elements.
        /// </summary>
        List<T> AllFootprintElements{ get; }

        /// <summary>
        /// A list of the corner footprint elements.
        /// </summary>
        List<T> CornerFootprintElements{ get; }

        /// <summary>
        /// A list of the outlining border footprint elements.
        /// </summary>
        List<T> BorderFootprintElements{ get; }

        /// <summary>
        /// The offset of the footprint from the main element.
        /// </summary>
        Vector2Int FootprintOffset { get; }

        /// <summary>
        /// The width and height of the footprint in grid units.
        /// </summary>
        Vector2Int FootprintSize { get; }

        /// <summary>
        /// The positional extents of the footprint in grid units.
        /// </summary>
        Vector2Int FootprintExtents { get; }

        /// <summary>
        /// Destroys the footprint.
        /// </summary>
        void Destroy();
    }
}
