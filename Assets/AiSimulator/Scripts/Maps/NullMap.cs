using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A null implementation of a map.
    /// </summary>
    public class NullMap : IMap
    {
        Transform IMap.Transform => null;

        Vector2Int IMap.Size => Vector2Int.zero;

        Vector3 IMap.CellSize => Vector3.zero;

        int IMap.CellCount => 0;

        HashSet<Vector2Int> IMap.GetAllCells() => new HashSet<Vector2Int>();

        Vector2Int IMap.LocalToCell(Vector3 localPosition) => Vector2Int.zero;
        Vector3 IMap.CellToLocal(Vector2Int cellPosition) => Vector3.zero;
        Vector3 IMap.CellToWorld(Vector2Int cellPosition) => Vector3.zero;
        int IMap.CellToSortingOrder(Vector2Int cellPosition) => 0;

        void IMap.AddElement(IMapElement element) { }
        void IMap.RemoveElement(IMapElement element) { }

        event Action<IMapElement> IMap.OnElementAdded { add { } remove { } }
        event Action<IMapElement> IMap.OnElementRemoved { add {} remove { } }

        bool IMap.InBounds(Vector2Int location) => false;
        bool IMap.GetIsElementOnMap(IMapElement element) => false;

        List<T> IMap.GetAllMapElements<T>() => new List<T>();
        T IMap.GetMapElementAtCell<T>(Vector2Int cell) => default;
        List<T> IMap.GetMapElementsAtCell<T>(Vector2Int cell) => new List<T>();
        List<T> IMap.GetMapElementsAtCells<T>(List<Vector2Int> cells) => new List<T>();
        List<T> IMap.GetMapElementsInBounds<T>(int x, int y, int width, int height) => new List<T>();
        List<T> IMap.GetMapElementsInsideRadius<T>(Vector2Int centerCell, int radius) => new List<T>();
        List<T> IMap.GetMapElementsOutsideRadius<T>(Vector2Int centerCell, int radius) => new List<T>();
        List<T> IMap.GetMapElementsOnRadius<T>(Vector2Int centerCell, int radius) => new List<T>();

        string IDescribable.DisplayName { get => ""; set { } }
        string IDescribable.Description { get => ""; set { } }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { } remove { } }

        public static IMap Create()
        {
            return new NullMap();
        }
    }
}

