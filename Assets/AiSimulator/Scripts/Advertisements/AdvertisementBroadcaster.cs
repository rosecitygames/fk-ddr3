using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// A class used to broadcast advertisements.
    /// </summary>
    public class AdvertisementBroadcaster : IAdvertisementBroadcaster
    {
        List<int> calledInstanceIds = new List<int>();

        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement) => Broadcast(advertisement);
        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => Broadcast(advertisement, excludeReceiver);
        protected void Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver = null)
        {
            calledInstanceIds.Clear();
            List<IAdvertisementReceiver> mapReceivers = advertisement.Map.GetMapElementsAtCells<IAdvertisementReceiver>(advertisement.BroadcastLocations);
            foreach (IAdvertisementReceiver mapReceiver in mapReceivers)
            {
                if (receiversByInstanceId.ContainsKey(mapReceiver.InstanceId) && calledInstanceIds.Contains(mapReceiver.InstanceId) == false)
                {
                    if (mapReceiver != excludeReceiver)
                    {
                        mapReceiver.ReceiveAdvertisement(advertisement);
                    }
                    calledInstanceIds.Add(mapReceiver.InstanceId);
                }
            }
        }

        void BroadcastToReceivers(IAdvertisement advertisement, List<IAdvertisementReceiver> receivers)
        {
            foreach (IAdvertisementReceiver receiver in receivers)
            {
                BroadcastToReceiver(advertisement, receiver);
            }
        }

        void BroadcastToReceiver(IAdvertisement advertisement, IAdvertisementReceiver receiver)
        {
            receiver.ReceiveAdvertisement(advertisement);
        }

        Dictionary<int, IAdvertisementReceiver> receiversByInstanceId = new Dictionary<int, IAdvertisementReceiver>();

        void IAdvertisementBroadcaster.AddReceiver(IAdvertisementReceiver receiver) => AddReceiver(receiver);
        protected void AddReceiver(IAdvertisementReceiver receiver)
        {
            if (receiversByInstanceId.ContainsKey(receiver.InstanceId)) return;
            receiversByInstanceId.Add(receiver.InstanceId, receiver);
        }

        void IAdvertisementBroadcaster.RemoveReceiver(IAdvertisementReceiver receiver) => RemoveReceiver(receiver);
        protected void RemoveReceiver(IAdvertisementReceiver receiver)
        {
            if (receiversByInstanceId.ContainsKey(receiver.InstanceId) == false) return;
            receiversByInstanceId.Remove(receiver.InstanceId);
        }

        void IAdvertisementBroadcaster.ClearReceivers() => ClearReceivers();
        protected void ClearReceivers()
        {
            receiversByInstanceId.Clear();
        }

        public static IAdvertisementBroadcaster Create(List<IAdvertisementReceiver> receivers)
        {
            Dictionary<int, IAdvertisementReceiver> receiversByInstanceId = new Dictionary<int, IAdvertisementReceiver>();
            foreach (IAdvertisementReceiver receiver in receivers)
            {
                if (receiversByInstanceId.ContainsKey(receiver.InstanceId)) continue;
                receiversByInstanceId.Add(receiver.InstanceId, receiver);
            }

            return new AdvertisementBroadcaster
            {
                receiversByInstanceId = receiversByInstanceId
            };
        }

        public static IAdvertisementBroadcaster Create()
        {
            return new AdvertisementBroadcaster();
        }
    }
}
