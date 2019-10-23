using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An advertiser that broadcasts advertisements with a given broadcaster
    /// </summary>
    public class Advertiser : IAdvertiser
    {
        /// <summary>
        /// The broadcaster used to broadcast advertisements
        /// </summary>
        protected IAdvertisementBroadcaster Broadcaster { get; set; }

        /// <summary>
        /// Get the advertiser's broadcaster object.
        /// </summary>
        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster()
        {
            return Broadcaster;
        }

        /// <summary>
        /// Set the advertiser's broadcaster object.
        /// </summary>
        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster)
        {
            Broadcaster = broadcaster ?? NullAdvertisementBroadcaster.Create();
        }

        /// <summary>
        /// Broadcast a given advertisement.
        /// </summary>
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement) => BroadcastAdvertisement(advertisement);
        protected void BroadcastAdvertisement(IAdvertisement advertisement)
        {
            Broadcaster.Broadcast(advertisement);
        }

        /// <summary>
        /// Broadcast a given advertisement with an excluded receiver.
        /// </summary>
        /// <param name="advertisement">The advertisement to be broadcasted</param>
        /// <param name="excludeReceiver">A receiver that won't receive that broadcasted advertisement</param>
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => BroadcastAdvertisement(advertisement, excludeReceiver);
        protected void BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver)
        {
            Broadcaster.Broadcast(advertisement, excludeReceiver);
        }

        /// <summary>
        /// Create an advertiser
        /// </summary>
        /// <param name="broadcaster">The broadcaster that the advertiser will use to broadcast advertisements</param>
        /// <returns>An advertiser</returns>
        public static IAdvertiser Create(IAdvertisementBroadcaster broadcaster)
        {
            return new Advertiser
            {
                Broadcaster = broadcaster ?? NullAdvertisementBroadcaster.Create()
            };
        }

        /// <summary>
        /// Creates an advertiser without a broadcaster.
        /// </summary>
        /// <returns>An advertiser</returns>
        public static IAdvertiser Create()
        {
            return new Advertiser
            {
                Broadcaster = NullAdvertisementBroadcaster.Create()
            };
        }
    }
}
