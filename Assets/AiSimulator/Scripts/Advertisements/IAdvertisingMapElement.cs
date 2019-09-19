using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using IndieDevTools.Maps;
using IndieDevTools.States;
using System;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An extended interface for advertisers that reside on the map.
    /// </summary>
    public interface IAdvertisingMapElement : IMapElement, IAdvertiser, IAdvertisementBroadcastData
    {

    }
}
