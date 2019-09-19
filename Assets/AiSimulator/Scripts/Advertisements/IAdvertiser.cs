using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An interface for implementing classes to broadcast advertisements.
    /// </summary>
    public interface IAdvertiser
    {
        IAdvertisementBroadcaster GetBroadcaster();
        void SetBroadcaster(IAdvertisementBroadcaster broadcaster);
        void BroadcastAdvertisement(IAdvertisement advertisement);
        void BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver);
    }
}

