using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// An interface for a 2D map that included various properties and helper methods.
    /// Note, a 2D map does not necessarily need to be a grid. It could be collection
    /// of node cells or anythings else you can imagine.
    /// </summary>
    public interface IMap : IDescribable
    {
        Transform Transform { get; }
        Vector2Int Size { get; }
        Vector3 CellSize { get; }
        int CellCount { get; }

        HashSet<Vector2Int> GetAllCells();

        Vector2Int LocalToCell(Vector3 localPosition);
        Vector3 CellToLocal(Vector2Int cellPosition);
        Vector3 CellToWorld(Vector2Int cellPosition);
        int CellToSortingOrder(Vector2Int cellPosition);

        void AddElement(IMapElement element);
        void RemoveElement(IMapElement element);

        event Action<IMapElement> OnElementAdded;
        event Action<IMapElement> OnElementRemoved;

        bool InBounds(Vector2Int location);
        bool GetIsElementOnMap(IMapElement element);

        List<T> GetAllMapElements<T>();
        T GetMapElementAtCell<T>(Vector2Int cell);
        List<T> GetMapElementsAtCell<T>(Vector2Int cell);
        List<T> GetMapElementsAtCells<T>(List<Vector2Int> cells);
        List<T> GetMapElementsInBounds<T>(int x, int y, int width, int height);
        List<T> GetMapElementsInsideRadius<T>(Vector2Int centerCell, int radius);
        List<T> GetMapElementsOutsideRadius<T>(Vector2Int centerCell, int radius);
        List<T> GetMapElementsOnRadius<T>(Vector2Int centerCell, int radius);
    }
}
