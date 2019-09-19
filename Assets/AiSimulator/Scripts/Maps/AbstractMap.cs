using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// An abstract map implementation that primarily bridges
    /// interface implemenations to protected methods and properties.
    /// </summary>
    public abstract class AbstractMap : MonoBehaviour, IMap
    {
        Transform IMap.Transform => Transform;
        protected virtual Transform Transform => transform;

        Vector2Int IMap.Size => Size;
        protected virtual Vector2Int Size { get; }

        Vector3 IMap.CellSize => CellSize;
        protected virtual Vector3 CellSize { get; }

        int IMap.CellCount => CellCount;
        protected virtual int CellCount => Size.x * Size.y;

        HashSet<Vector2Int> IMap.GetAllCells() => GetAllCells();
        protected virtual HashSet<Vector2Int> GetAllCells() => new HashSet<Vector2Int>();

        Vector2Int IMap.LocalToCell(Vector3 localPosition) => LocalToCell(localPosition);
        protected virtual Vector2Int LocalToCell(Vector3 localPosition) => Vector2Int.zero;

        Vector3 IMap.CellToLocal(Vector2Int cellPosition) => CellToLocal(cellPosition);
        protected virtual Vector3 CellToLocal(Vector2Int cellPosition) => Vector3.zero;

        Vector3 IMap.CellToWorld(Vector2Int cellPosition) => CellToWorld(cellPosition);
        protected virtual Vector3 CellToWorld(Vector2Int cellPosition) => Vector3.zero;

        int IMap.CellToSortingOrder(Vector2Int cellPosition) => CellToSortingOrder(cellPosition);
        protected virtual int CellToSortingOrder(Vector2Int cellPosition) => 0;

        void IMap.AddElement(IMapElement element) => AddElement(element);
        protected virtual void AddElement(IMapElement element) { }

        void IMap.RemoveElement(IMapElement element) => RemoveElement(element);
        protected virtual void RemoveElement(IMapElement element) { }

        event Action<IMapElement> IMap.OnElementAdded { add { OnElementAdded += value; } remove { OnElementAdded -= value; } }
        protected Action<IMapElement> OnElementAdded;

        event Action<IMapElement> IMap.OnElementRemoved { add { OnElementRemoved += value; } remove { OnElementRemoved -= value; } }
        protected Action<IMapElement> OnElementRemoved;

        bool IMap.InBounds(Vector2Int location) => InBounds(location);
        protected virtual bool InBounds(Vector2Int location) => false;

        bool IMap.GetIsElementOnMap(IMapElement element) => GetIsElementOnMap(element);
        protected virtual bool GetIsElementOnMap(IMapElement element) => false;

        List<T> IMap.GetAllMapElements<T>() => GetAllMapElements<T>();
        protected virtual List<T> GetAllMapElements<T>() => new List<T>();

        T IMap.GetMapElementAtCell<T>(Vector2Int cell) => GetMapElementAtCell<T>(cell);
        protected virtual T GetMapElementAtCell<T>(Vector2Int cell) => default;

        List<T> IMap.GetMapElementsAtCell<T>(Vector2Int cell) => GetMapElementsAtCell<T>(cell);
        protected virtual List<T> GetMapElementsAtCell<T>(Vector2Int cell) => new List<T>();

        List<T> IMap.GetMapElementsAtCells<T>(List<Vector2Int> cells) => GetMapElementsAtCells<T>(cells);
        protected virtual List<T> GetMapElementsAtCells<T>(List<Vector2Int> cells) => new List<T>();

        List<T> IMap.GetMapElementsInBounds<T>(int x, int y, int width, int height) => GetMapElementsInBounds<T>(x, y, width, height);
        protected virtual List<T> GetMapElementsInBounds<T>(int x, int y, int width, int height) => new List<T>();

        List<T> IMap.GetMapElementsInsideRadius<T>(Vector2Int centerCell, int radius) => GetMapElementsInsideRadius<T>(centerCell, radius);
        protected virtual List<T> GetMapElementsInsideRadius<T>(Vector2Int centerCell, int radius) => new List<T>();

        List<T> IMap.GetMapElementsOutsideRadius<T>(Vector2Int centerCell, int radius) => GetMapElementsOutsideRadius<T>(centerCell, radius);
        protected virtual List<T> GetMapElementsOutsideRadius<T>(Vector2Int centerCell, int radius) => new List<T>();

        List<T> IMap.GetMapElementsOnRadius<T>(Vector2Int centerCell, int radius) => GetMapElementsOnRadius<T>(centerCell, radius);
        protected virtual List<T> GetMapElementsOnRadius<T>(Vector2Int centerCell, int radius) => new List<T>();

        string IDescribable.DisplayName { get => DisplayName; set => DisplayName = value; }
        string displayName;
        protected virtual string DisplayName
        {
            get => displayName;
            set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnDescribableUpdated.Invoke(this);
                }
            }
        }

        string IDescribable.Description { get => Description; set => Description = value; }
        string description;
        protected virtual string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnDescribableUpdated.Invoke(this);
                }
            }
        }

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { OnDescribableUpdated += value; } remove { OnDescribableUpdated -= value; } }
        Action<IDescribable> OnDescribableUpdated;
    }
}
