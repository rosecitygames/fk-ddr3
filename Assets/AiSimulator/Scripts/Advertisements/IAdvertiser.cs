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
        /// <summary>
        /// Gets the advertiser's broadcaster.
        /// </summary>
        IAdvertisementBroadcaster GetBroadcaster();

        /// <summary>
        /// Set the advertiser's broadcaster.
        /// </summary>
        void SetBroadcaster(IAdvertisementBroadcaster broadcaster);

        /// <summary>
        /// Broadcast a given advertisement.
        /// </summary>
        void BroadcastAdvertisement(IAdvertisement advertisement);

        /// <summary>
        /// Broadcast a given advertisement with an excluded receiver.
        /// </summary>
        void BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver);
    }
}

