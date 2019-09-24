using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A simple map element implementation without a game object dependency.
    /// </summary>
    public class SimpleMapElement : IMapElement
    {
        int IGroupMember.GroupId { get => GroupId; set => GroupId = value; }
        protected virtual int GroupId { get; set; }

        List<ITrait> IStatsCollection.Stats => Stats.Traits;
        protected ITraitCollection Stats = TraitCollection.Create();

        ITrait IStatsCollection.GetStat(string id) => GetStat(id);
        protected virtual ITrait GetStat(string id) => Stats.GetTrait(id);

        string IDescribable.DisplayName { get => DisplayName; set => DisplayName = value; }
        protected virtual string DisplayName
        {
            get => displayName;
            set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnDescribableUpdated?.Invoke(this);
                }
            }
        }
        private string displayName;

        string IDescribable.Description { get => Description; set => Description = value; }
        protected virtual string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnDescribableUpdated?.Invoke(this);
                }
            }
        }
        private string description;

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { OnDescribableUpdated += value; } remove { OnDescribableUpdated -= value; } }
        protected event Action<IDescribable> OnDescribableUpdated;

        IMap IMapElement.Map { get => Map; set => Map = value; }
        protected virtual IMap Map
        {
            get => map;
            set
            {
                map = value;
                AddToMap();
            }
        }
        private IMap map;

        void IMapElement.AddToMap() => AddToMap();
        protected virtual void AddToMap() => Map?.AddElement(this);

        void IMapElement.AddToMap(IMap value) => AddToMap(value);
        protected virtual void AddToMap(IMap value) => Map = value;

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

        int IMapElement.InstanceId => InstanceId;
        protected virtual int InstanceId => 0;

        

        int IMapElement.SortingOrder => SortingOrder;
        protected virtual int SortingOrder
        {
            get
            {
                if (Map == null) return 0;
                return Map.CellToSortingOrder(Location) + sortingOrderOffset;
            }
        }  
        int sortingOrderOffset = 0;

        Vector2Int ILocatable.Location => Location;
        protected virtual Vector2Int Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    OnLocationUpdated?.Invoke(this);
                }
            }
        }
        private Vector2Int location = Vector2Int.zero;

        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { OnLocationUpdated += value; } remove { OnLocationUpdated -= value; } }
        protected event Action<ILocatable> OnLocationUpdated;

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