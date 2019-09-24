using IndieDevTools.Advertisements;
using IndieDevTools.Maps;
using IndieDevTools.States;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Agents
{
    /// <summary>
    /// Used by agents that take up more than one cell on a map. The super object is delegated to for
    /// most implementation except for map methods.
    /// </summary>
    public abstract class AbstractSubAgent : IAgent
    {
        protected virtual IAgent SuperAgent { get; }
        protected virtual Vector2Int CellOffset { get;}

        int IGroupMember.GroupId { get => SuperAgent.GroupId; set => SuperAgent.GroupId = value; }

        IDescribable Describable => SuperAgent as IDescribable;
        string IDescribable.DisplayName { get => Describable.DisplayName; set => Describable.DisplayName = value; }
        string IDescribable.Description { get => Describable.Description; set => Describable.Description = value; }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { Describable.OnUpdated += value; } remove { Describable.OnUpdated -= value; } }

        IAgentData IAgent.Data { get => SuperAgent.Data; set => SuperAgent.Data = value; }

        List<ITrait> IStatsCollection.Stats => SuperAgent.Stats;
        ITrait IStatsCollection.GetStat(string id) => SuperAgent.GetStat(id);

        List<ITrait> IDesiresCollection.Desires => SuperAgent.Desires;
        ITrait IDesiresCollection.GetDesire(string id) => SuperAgent.GetDesire(id);

        float IAdvertisementBroadcastData.BroadcastDistance => SuperAgent.BroadcastDistance;
        float IAdvertisementBroadcastData.BroadcastInterval => SuperAgent.BroadcastInterval;

        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster() => SuperAgent.GetBroadcaster();
        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster) => SuperAgent.SetBroadcaster(broadcaster);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement) => SuperAgent.BroadcastAdvertisement(advertisement);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => SuperAgent.BroadcastAdvertisement(advertisement, excludeReceiver);
        void IAdvertisementReceiver.ReceiveAdvertisement(IAdvertisement advertisement) => SuperAgent.ReceiveAdvertisement(advertisement);
        event Action<IAdvertisement> IAgent.OnAdvertisementReceived { add { SuperAgent.OnAdvertisementReceived += value; } remove { SuperAgent.OnAdvertisementReceived -= value; } }

        IRankedAdvertisement IAgent.TargetAdvertisement { get => SuperAgent.TargetAdvertisement; set => SuperAgent.TargetAdvertisement = value; }
        IMapElement IAgent.TargetMapElement { get => SuperAgent.TargetMapElement; set => SuperAgent.TargetMapElement = value; }
        Vector2Int IAgent.TargetLocation { get => SuperAgent.TargetLocation; set => SuperAgent.TargetLocation = value; }

        void IStateTransitionHandler.HandleTransition(string transitionName) => SuperAgent.HandleTransition(transitionName);

        int IMapElement.InstanceId => SuperAgent.InstanceId;

        int IMapElement.SortingOrder => SuperAgent.SortingOrder;

        IMap IMapElement.Map { get => Map; set { } }
        IMap Map => SuperAgent.Map;

        void IMapElement.AddToMap(IMap value) => AddToMap();
        void IMapElement.AddToMap() => AddToMap();
        void AddToMap() => Map?.AddElement(this);

        void IMapElement.RemoveFromMap() => RemoveFromMap();
        protected virtual void RemoveFromMap() => Map?.RemoveElement(this);

        bool IMapElement.IsOnMap => IsOnMap;
        protected virtual bool IsOnMap
        {
            get
            {
                if (Map == null) return false;
                return Map.GetIsElementOnMap(this);
            }
        }

        float IMapElement.Distance(IMapElement otherMapElement)
        {
            return Vector2Int.Distance(otherMapElement.Location, Location);
        }

        float IMapElement.Distance(Vector2Int otherLocation)
        {
            return Vector2Int.Distance(otherLocation, Location);
        }

        Vector2Int ILocatable.Location => Location;
        protected virtual Vector2Int Location
        {
            get => location + CellOffset;
            set
            {
                if (location != value)
                {
                    location = value;
                    Map.AddElement(this);
                }
            }
        }
        private Vector2Int location = Vector2Int.zero;

        ILocatable Locatable => SuperAgent as ILocatable;
        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { Locatable.OnUpdated += value; } remove { Locatable.OnUpdated -= value; } }

        Vector3 IPositionable.Position { get => Position; set => Position = value; }
        Vector3 Position
        {
            get
            {
                if (Map == null) return Vector3.zero;
                return Map.CellToLocal(Location);
            }
            set
            {
                if (Map == null) return;
                Location = Map.LocalToCell(value);
            }
        }
    }
}
