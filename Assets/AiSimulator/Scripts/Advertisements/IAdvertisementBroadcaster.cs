using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An interface for how implementing classes broadcast advertisements.
    /// </summary>
    public interface IAdvertisementBroadcaster
    {
        void Broadcast(IAdvertisement advertisement);
        void Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver);

        void AddReceiver(IAdvertisementReceiver receiver);
        void RemoveReceiver(IAdvertisementReceiver receiver);
        void ClearReceivers();
    }
}
