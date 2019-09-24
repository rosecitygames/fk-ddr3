using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A helper map element implementation that makes object composition easier.
    /// </summary>
    public class MapElementHelper : IMapElement
    {
        GameObject gameObject;
        IMapElement mapElement;

        string IDescribable.DisplayName
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
        string displayName = "";

        string IDescribable.Description
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
        string description = "";

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { OnDescribableUpdated += value; } remove { OnDescribableUpdated -= value; } }
        Action<IDescribable> OnDescribableUpdated;

        int IGroupMember.GroupId { get => groupId; set => groupId = value; }
        int groupId = 0;

        List<ITrait> IStatsCollection.Stats => stats.Stats;
        ITrait IStatsCollection.GetStat(string id) => stats.GetStat(id);       
        IStatsCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = TraitCollection.Create() as IStatsCollection;
                }
                return stats;
            }
        }
        IStatsCollection stats = null;

        IMap map;
        IMap IMapElement.Map { get => Map; set => Map = value; }
        IMap Map
        {
            get
            {
                if (map == null)
                {
                    InitMap();
                }
                return map;
            }
            set
            {
                map = value;
            }
        }

        void InitMap()
        {
            map = gameObject.GetComponentInParent<IMap>() ?? NullMap.Create();
            map.AddElement(mapElement);

            UpdateSortingOrder();
        }

        void IMapElement.AddToMap() => AddToMap();
        void AddToMap() => AddToMap(Map);

        void IMapElement.AddToMap(IMap map) => AddToMap(map);
        void AddToMap(IMap map)
        {
            if (this.map != null && this.map != map)
            {
                Map.RemoveElement(mapElement);
            }
            this.map = map;
            Map.AddElement(mapElement);
        }

        void IMapElement.RemoveFromMap() => RemoveFromMap();
        void RemoveFromMap()
        {
            Map.RemoveElement(mapElement);
        }

        bool IMapElement.IsOnMap => IsOnMap;
        bool IsOnMap
        {
            get
            {
                if (Map == null) return false;
                return Map.GetIsElementOnMap(mapElement);
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

        int IMapElement.InstanceId => gameObject.GetInstanceID();

        int sortingOrderOffset = 0;

        int IMapElement.SortingOrder => SortingOrder;
        int SortingOrder => Map.CellToSortingOrder(Location) + sortingOrderOffset;
        
        Vector2Int ILocatable.Location => Location;
        Vector2Int Location => Map.LocalToCell(Position);

        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { OnLocationUpdated += value; } remove { OnLocationUpdated -= value; } }
        Action<ILocatable> OnLocationUpdated;

        Vector3 IPositionable.Position { get => Position; set => Position = value; }
        Vector3 Position
        {
            get
            {
                return gameObject.transform.localPosition;
            }
            set
            {
                Vector2Int currentLocation = Location;
                Vector2Int newLocation = Map.LocalToCell(value);

                gameObject.transform.localPosition = value;
                UpdateSortingOrder();

                if (currentLocation != newLocation)
                {
                    Map.AddElement(mapElement);
                    OnLocationUpdated?.Invoke(this);
                }
            }
        }

        void UpdateSortingOrder()
        {
            if (SpriteRenderer == null) return;
            SpriteRenderer.sortingOrder = SortingOrder;
        }

        SpriteRenderer spriteRenderer;
        SpriteRenderer SpriteRenderer
        {
            get
            {
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                }
                return spriteRenderer;
            }
        }

        public static IMapElement Create(GameObject gameObject, IMapElement mapElement, int sortingOrderOffset = 0, IStatsCollection stats = null, int groupId = 0, string displayName = "", string description = "")
        {
            return new MapElementHelper
            {
                gameObject = gameObject,
                mapElement = mapElement,
                sortingOrderOffset = sortingOrderOffset,
                groupId = groupId,
                stats = stats,
                displayName = displayName,
                description = description
            };
        }
    }
}

