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
        /// <summary>
        /// Broadcast an advertisement.
        /// </summary>
        void Broadcast(IAdvertisement advertisement);

        /// <summary>
        /// Broadcast an advertisement with an excluded receiver.
        /// </summary>
        void Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver);

        /// <summary>
        /// Add an advertisement receiver to the receiver collection.
        /// </summary>
        void AddReceiver(IAdvertisementReceiver receiver);

        /// <summary>
        /// Remove an advertisement receiver from the receiver collection.
        /// </summary>
        void RemoveReceiver(IAdvertisementReceiver receiver);

        /// <summary>
        /// Clear the receiver collection.
        /// </summary>
        void ClearReceivers();
    }
}
