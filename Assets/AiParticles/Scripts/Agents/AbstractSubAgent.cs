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
    /// Used by agents that take up more than one cell on a map. The super agent is delegated to for
    /// most implementation except for map methods.
    /// </summary>
    public abstract class AbstractSubAgent : IAgent
    {
        /// <summary>
        /// The super agent who will be delegated to.
        /// </summary>
        protected virtual IAgent SuperAgent { get; }

        /// <summary>
        /// The map cell offset relative to the super agent.
        /// </summary>
        protected virtual Vector2Int CellOffset { get;}

        // IGroupMember implementations are delegated to the super agent.
        int IGroupMember.GroupId { get => SuperAgent.GroupId; set => SuperAgent.GroupId = value; }

        // IDescribable implementations are delegated to the super agent.
        IDescribable Describable => SuperAgent as IDescribable;
        string IDescribable.DisplayName { get => Describable.DisplayName; set => Describable.DisplayName = value; }
        string IDescribable.Description { get => Describable.Description; set => Describable.Description = value; }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { Describable.OnUpdated += value; } remove { Describable.OnUpdated -= value; } }

        // IAgent implementations are delegated to the super agent.
        IAgentData IAgent.Data { get => SuperAgent.Data; set => SuperAgent.Data = value; }
        event Action<IAdvertisement> IAgent.OnAdvertisementReceived { add { SuperAgent.OnAdvertisementReceived += value; } remove { SuperAgent.OnAdvertisementReceived -= value; } }
        IRankedAdvertisement IAgent.TargetAdvertisement { get => SuperAgent.TargetAdvertisement; set => SuperAgent.TargetAdvertisement = value; }
        IMapElement IAgent.TargetMapElement { get => SuperAgent.TargetMapElement; set => SuperAgent.TargetMapElement = value; }
        Vector2Int IAgent.TargetLocation { get => SuperAgent.TargetLocation; set => SuperAgent.TargetLocation = value; }

        // IStatsCollection implementations are delegated to the super agent.
        List<ITrait> IStatsCollection.Stats => SuperAgent.Stats;
        ITrait IStatsCollection.GetStat(string id) => SuperAgent.GetStat(id);

        // IDesiresCollection implementations are delegated to the super agent.
        List<ITrait> IDesiresCollection.Desires => SuperAgent.Desires;
        ITrait IDesiresCollection.GetDesire(string id) => SuperAgent.GetDesire(id);

        // IAdvertisementBroadcastData implementations are delegated to the super agent.
        float IAdvertisementBroadcastData.BroadcastDistance => SuperAgent.BroadcastDistance;
        float IAdvertisementBroadcastData.BroadcastInterval => SuperAgent.BroadcastInterval;

        // IAdvertisementBroadcaster implementations are delegated to the super agent.
        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster() => GetBroadcaster();
        IAdvertisementBroadcaster GetBroadcaster() => SuperAgent.GetBroadcaster();

        // IAdvertiser implementations are delegated to the super agent.
        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster) => SuperAgent.SetBroadcaster(broadcaster);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement) => SuperAgent.BroadcastAdvertisement(advertisement);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => SuperAgent.BroadcastAdvertisement(advertisement, excludeReceiver);

        // IAdvertisementReceiver implementations are delegated to the super agent.
        void IAdvertisementReceiver.ReceiveAdvertisement(IAdvertisement advertisement) => SuperAgent.ReceiveAdvertisement(advertisement);

        // IStateTransitionHandler implementations are delegated to the super agent.
        void IStateTransitionHandler.HandleTransition(string transitionName) => SuperAgent.HandleTransition(transitionName);

        // IMapElement implementations

        /// <summary>
        /// SubAgents share the same instance Id as the super agent.
        /// </summary>
        int IMapElement.InstanceId => SuperAgent.InstanceId;

        /// <summary>
        /// SubAgents share the same sorting order as the super agent.
        /// </summary>
        int IMapElement.SortingOrder => SuperAgent.SortingOrder;

        /// <summary>
        /// Get the super agent map. Setter is not implemented to prevent separation from the super agent.
        /// </summary>
        IMap IMapElement.Map { get => Map; set { } }
        IMap Map => SuperAgent.Map;

        /// <summary>
        /// Add the SubAgent to the SuperAgent's map.
        /// </summary>
        void IMapElement.AddToMap(IMap value) => AddToMap();
        void IMapElement.AddToMap() => AddToMap();
        void AddToMap() => Map?.AddElement(this);

        /// <summary>
        /// Remove the SubAgent from the SuperAgent's map.
        /// </summary>
        void IMapElement.RemoveFromMap() => RemoveFromMap();
        protected virtual void RemoveFromMap() => Map?.RemoveElement(this);

        /// <summary>
        /// Whether or not this object is on the map.
        /// </summary>
        bool IMapElement.IsOnMap => IsOnMap;
        protected virtual bool IsOnMap
        {
            get
            {
                if (Map == null) return false;
                return Map.GetIsElementOnMap(this);
            }
        }

        /// <summary>
        /// The distance from this object's location to another map element.
        /// </summary>
        float IMapElement.Distance(IMapElement otherMapElement)
        {
            return Vector2Int.Distance(otherMapElement.Location, Location);
        }

        /// <summary>
        /// The distance from this object's location to another location on the map.
        /// </summary>
        float IMapElement.Distance(Vector2Int otherLocation)
        {
            return Vector2Int.Distance(otherLocation, Location);
        }

        /// <summary>
        /// The location of this object on the map.
        /// </summary>
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

        /// <summary>
        /// A reference to the super agent locatable interface.
        /// </summary>
        ILocatable Locatable => SuperAgent as ILocatable;

        /// <summary>
        /// An event action that delegates updates to the super agent.
        /// </summary>
        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { Locatable.OnUpdated += value; } remove { Locatable.OnUpdated -= value; } }

        /// <summary>
        /// The local position of this object on the map.
        /// </summary>
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

        /// <summary>
        /// Initialize this object
        /// </summary>
        protected virtual void Init()
        {
            InitBroadcaster();
        }

        /// <summary>
        /// Initialize the broadcaster if it exists.
        /// </summary>
        void InitBroadcaster()
        {
            IAdvertisementBroadcaster broadcaster = GetBroadcaster();
            if (broadcaster != null)
            {
                broadcaster.AddReceiver(this);
            }
        }
    }
}
