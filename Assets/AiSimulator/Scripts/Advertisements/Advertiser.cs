using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    public class Advertiser : IAdvertiser
    {
        protected IAdvertisementBroadcaster Broadcaster { get; set; }

        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster()
        {
            return Broadcaster;
        }

        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster)
        {
            Broadcaster = broadcaster ?? NullAdvertisementBroadcaster.Create();
        }

        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement) => BroadcastAdvertisement(advertisement);
        protected void BroadcastAdvertisement(IAdvertisement advertisement)
        {
            Broadcaster.Broadcast(advertisement);
        }

        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => BroadcastAdvertisement(advertisement, excludeReceiver);
        protected void BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver)
        {
            Broadcaster.Broadcast(advertisement, excludeReceiver);
        }

        public static IAdvertiser Create(IAdvertisementBroadcaster broadcaster)
        {
            return new Advertiser
            {
                Broadcaster = broadcaster ?? NullAdvertisementBroadcaster.Create()
            };
        }

        public static IAdvertiser Create()
        {
            return new Advertiser
            {
                Broadcaster = NullAdvertisementBroadcaster.Create()
            };
        }
    }
}
