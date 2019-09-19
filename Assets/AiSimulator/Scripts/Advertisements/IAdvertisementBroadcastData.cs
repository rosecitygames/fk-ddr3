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
        float BroadcastDistance { get; }
        float BroadcastInterval { get; }
    }
}
