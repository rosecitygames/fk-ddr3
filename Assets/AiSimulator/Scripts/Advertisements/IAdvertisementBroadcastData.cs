using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An interface for advertisement broadcast data properties.
    /// </summary>
    public interface IAdvertisementBroadcastData
    {
        /// <summary>
        /// The distance that an advertisement will be broadcasted.
        /// </summary>
        float BroadcastDistance { get; }

        /// <summary>
        /// The time interval that the advertisement will be broadcasted at.
        /// </summary>
        float BroadcastInterval { get; }
    }
}
