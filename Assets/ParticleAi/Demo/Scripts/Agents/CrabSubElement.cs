using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Maps;
using IndieDevTools.States;
using IndieDevTools.Traits;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    public class CrabSubElement : ICrab
    {
        ICrab superCrab;
        Vector2Int cellOffset;

        int IGroupMember.GroupId { get => superCrab.GroupId; set => superCrab.GroupId = value; }

        IDescribable Describable => superCrab as IDescribable;
        string IDescribable.DisplayName { get => Describable.DisplayName; set => Describable.DisplayName = value; }
        string IDescribable.Description { get => Describable.Description; set => Describable.Description = value; }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { Describable.OnUpdated += value; } remove { Describable.OnUpdated -= value; } }

        IAgentData IAgent.Data { get => superCrab.Data; set => superCrab.Data = value; }

        List<ITrait> IStatsCollection.Stats => superCrab.Stats;
        ITrait IStatsCollection.GetStat(string id) => superCrab.GetStat(id);

        List<ITrait> IDesiresCollection.Desires => superCrab.Desires;
        ITrait IDesiresCollection.GetDesire(string id) => superCrab.GetDesire(id);

        float IAdvertisementBroadcastData.BroadcastDistance => superCrab.BroadcastDistance;
        float IAdvertisementBroadcastData.BroadcastInterval => superCrab.BroadcastInterval;

        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster() => superCrab.GetBroadcaster();
        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster) => superCrab.SetBroadcaster(broadcaster);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement) => superCrab.BroadcastAdvertisement(advertisement);
        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver) => superCrab.BroadcastAdvertisement(advertisement, excludeReceiver);
        void IAdvertisementReceiver.ReceiveAdvertisement(IAdvertisement advertisement) => superCrab.ReceiveAdvertisement(advertisement);
        event Action<IAdvertisement> IAgent.OnAdvertisementReceived { add { superCrab.OnAdvertisementReceived += value; } remove { superCrab.OnAdvertisementReceived -= value; } }

        IRankedAdvertisement IAgent.TargetAdvertisement { get => superCrab.TargetAdvertisement; set => superCrab.TargetAdvertisement = value; }
        IMapElement IAgent.TargetMapElement { get => superCrab.TargetMapElement; set => superCrab.TargetMapElement = value; }
        Vector2Int IAgent.TargetLocation { get => superCrab.TargetLocation; set => superCrab.TargetLocation = value; }

        void IStateTransitionHandler.HandleTransition(string transitionName) => superCrab.HandleTransition(transitionName);

        int IMapElement.InstanceId => superCrab.InstanceId;

        int IMapElement.SortingOrder => superCrab.SortingOrder;

        void IAttackReceiver.ReceiveAttack(IAgent attackingAgent) => superCrab.ReceiveAttack(attackingAgent);
        event Action<IAgent> IAttackReceiver.OnAttackReceived { add { superCrab.OnAttackReceived += value; } remove { superCrab.OnAttackReceived -= value; } }

        IMap IMapElement.Map { get => Map; set { } }
        IMap Map => superCrab.Map;

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
            get => location + cellOffset;
            set
            {
                if (location != value)
                {
                    location = value;
                }
            }
        }
        private Vector2Int location = Vector2Int.zero;

        ILocatable Locatable => superCrab as ILocatable;
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

        public static ICrab Create(ICrab superCrab, Vector2Int cellOffset)
        {
            return new CrabSubElement
            {
                superCrab = superCrab,
                cellOffset = cellOffset
            };
        }
    }
}
