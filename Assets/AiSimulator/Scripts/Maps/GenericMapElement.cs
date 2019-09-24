using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A generic map element component implementation.
    /// </summary>
    public class GenericMapElement : MonoBehaviour, IMapElement
    {
        IMapElement mapElement = null;
        IMapElement MapElement
        {
            get
            {
                InitMapElement();
                return mapElement;
            }
        }

        void InitMapElement()
        {
            if (mapElement == null)
            {
                mapElement = MapElementHelper.Create(gameObject, this);
                mapElement.AddToMap();
            }
        }

        int IGroupMember.GroupId { get => MapElement.GroupId; set => MapElement.GroupId = value; }

        List<ITrait> IStatsCollection.Stats => MapElement.Stats;
        ITrait IStatsCollection.GetStat(string id) => MapElement.GetStat(id);

        string IDescribable.DisplayName { get => MapElement.DisplayName; set => MapElement.DisplayName = value; }
        string IDescribable.Description { get => MapElement.Description; set => MapElement.Description = value; }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { (MapElement as IDescribable).OnUpdated += value; } remove { (MapElement as IDescribable).OnUpdated -= value; } }

        IMap IMapElement.Map { get => MapElement.Map; set => MapElement.Map = value; }
        void IMapElement.AddToMap() => MapElement.AddToMap();
        void IMapElement.AddToMap(IMap map) => MapElement.AddToMap(map);
        void IMapElement.RemoveFromMap() => MapElement.RemoveFromMap();
        bool IMapElement.IsOnMap => MapElement.IsOnMap;
        float IMapElement.Distance(IMapElement otherMapElement) => MapElement.Distance(otherMapElement);
        float IMapElement.Distance(Vector2Int otherLocation) => MapElement.Distance(otherLocation);
        int IMapElement.InstanceId => MapElement.InstanceId;
        int IMapElement.SortingOrder =>  MapElement.SortingOrder;

        Vector2Int ILocatable.Location => MapElement.Location;
        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { (MapElement as ILocatable).OnUpdated += value; } remove { (MapElement as ILocatable).OnUpdated -= value; } }

        Vector3 IPositionable.Position { get => MapElement.Position; set => MapElement.Position = value; }
    }
}