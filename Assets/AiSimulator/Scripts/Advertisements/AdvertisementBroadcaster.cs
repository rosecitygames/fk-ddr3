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
        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement) => Broadcast(advertisement);
        void IAdvertisementBroadcaster.Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => Broadcast(advertisement, excludeReceiver);
        protected void Broadcast(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver = null)
        {
            List<IMapElement> mapElements = advertisement.Map.GetMapElementsAtCells<IMapElement>(advertisement.BroadcastLocations);
            foreach(IMapElement mapElement in mapElements)
            {
                if (receiversByMapElement.ContainsKey(mapElement))
                {
                    IAdvertisementReceiver receiver = receiversByMapElement[mapElement];
                    if (receiver != excludeReceiver)
                    {
                        receiver.ReceiveAdvertisement(advertisement);
                    }                   
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

        Dictionary<IMapElement, IAdvertisementReceiver> receiversByMapElement = new Dictionary<IMapElement, IAdvertisementReceiver>();

        void IAdvertisementBroadcaster.AddReceiver(IAdvertisementReceiver receiver) => AddReceiver(receiver);
        protected void AddReceiver(IAdvertisementReceiver receiver)
        {
            receiversByMapElement.Add(receiver, receiver);
        }

        void IAdvertisementBroadcaster.RemoveReceiver(IAdvertisementReceiver receiver) => RemoveReceiver(receiver);
        protected void RemoveReceiver(IAdvertisementReceiver receiver)
        {
            receiversByMapElement.Remove(receiver);
        }

        void IAdvertisementBroadcaster.ClearReceivers() => ClearReceivers();
        protected void ClearReceivers()
        {
            receiversByMapElement.Clear();
        }

        public static IAdvertisementBroadcaster Create(List<IAdvertisementReceiver> receivers)
        {
            Dictionary<IMapElement, IAdvertisementReceiver> receiversByMapElement = new Dictionary<IMapElement, IAdvertisementReceiver>();
            foreach (IAdvertisementReceiver receiver in receivers)
            {
                receiversByMapElement.Add(receiver, receiver);
            }

            return new AdvertisementBroadcaster
            {
                receiversByMapElement = receiversByMapElement
            };
        }

        public static IAdvertisementBroadcaster Create()
        {
            return new AdvertisementBroadcaster();
        }
    }
}
