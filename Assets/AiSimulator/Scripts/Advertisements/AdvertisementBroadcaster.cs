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
        /// <summary>
        /// The instance Ids of advertisement receivers that have receieved an advertisement.
        /// </summary>
        List<int> calledInstanceIds = new List<int>();

        /// <summary>
        /// Broadcast an advertisement.
        /// </summary>
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

        /// <summary>
        /// Broadcast an advertisement to a list of advertisement receievers.
        /// </summary>
        void BroadcastToReceivers(IAdvertisement advertisement, List<IAdvertisementReceiver> receivers)
        {
            foreach (IAdvertisementReceiver receiver in receivers)
            {
                BroadcastToReceiver(advertisement, receiver);
            }
        }

        /// <summary>
        /// Broadcast an advertisement to a single advertisement receiver.
        /// </summary>
        void BroadcastToReceiver(IAdvertisement advertisement, IAdvertisementReceiver receiver)
        {
            receiver.ReceiveAdvertisement(advertisement);
        }

        /// <summary>
        /// A dictionary of advertisement receivers keyed by their instance id.
        /// </summary>
        Dictionary<int, IAdvertisementReceiver> receiversByInstanceId = new Dictionary<int, IAdvertisementReceiver>();

        /// <summary>
        /// Add an advertisement receiver to the collection of receivers that will receive advertisements.
        /// </summary>
        void IAdvertisementBroadcaster.AddReceiver(IAdvertisementReceiver receiver) => AddReceiver(receiver);
        protected void AddReceiver(IAdvertisementReceiver receiver)
        {
            if (receiversByInstanceId.ContainsKey(receiver.InstanceId)) return;
            receiversByInstanceId.Add(receiver.InstanceId, receiver);
        }

        /// <summary>
        /// Remove an advertisement receiver from the receiver collection.
        /// </summary>
        void IAdvertisementBroadcaster.RemoveReceiver(IAdvertisementReceiver receiver) => RemoveReceiver(receiver);
        protected void RemoveReceiver(IAdvertisementReceiver receiver)
        {
            if (receiversByInstanceId.ContainsKey(receiver.InstanceId) == false) return;
            receiversByInstanceId.Remove(receiver.InstanceId);
        }

        /// <summary>
        /// Clear the collection of receievers.
        /// </summary>
        void IAdvertisementBroadcaster.ClearReceivers() => ClearReceivers();
        protected void ClearReceivers()
        {
            receiversByInstanceId.Clear();
        }

        /// <summary>
        /// Create an advertisement receiver
        /// </summary>
        /// <param name="receivers">A list of receivers that will receive broadcasted advertisements</param>
        /// <returns>An advertisement receiver</returns>
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

        /// <summary>
        /// Create an advertisement receiver
        /// </summary>
        /// <returns>An advertisement receiver</returns>
        public static IAdvertisementBroadcaster Create()
        {
            return new AdvertisementBroadcaster();
        }
    }
}
