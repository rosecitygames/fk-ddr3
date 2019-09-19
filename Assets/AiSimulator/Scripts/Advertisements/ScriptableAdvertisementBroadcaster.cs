using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An advertisement broadcaster that can be shared across multiple objects.
    /// </summary>
    [CreateAssetMenu(fileName = "AdvertisementBroadcaster", menuName = "IndieDevTools/Advertisement Broadcaster")]
    public class ScriptableAdvertisementBroadcaster : ScriptableObject, IAdvertisementBroadcaster
    {
        [System.NonSerialized]
        IAdvertisementBroadcaster broadcaster = AdvertisementBroadcaster.Create();

        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement)
        {
            broadcaster.Broadcast(advertisement);
        }

        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver)
        {
            broadcaster.Broadcast(advertisement, excludeReceiver);
        }

        void IAdvertisementBroadcaster.AddReceiver(IAdvertisementReceiver receiver)
        {
            broadcaster.AddReceiver(receiver);
        }

        void IAdvertisementBroadcaster.RemoveReceiver(IAdvertisementReceiver receiver)
        {
            broadcaster.RemoveReceiver(receiver);
        }

        void IAdvertisementBroadcaster.ClearReceivers()
        {
            broadcaster.ClearReceivers();
        }
    }
}
