using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Demo.CrabBattle
{
    public class Footprint<T> : IFootprint<T> where T : IMapElement
    {
        List<T> IFootprint<T>.AllFootprintElements => allFootprintElements;
        List<T> allFootprintElements = new List<T>();

        List<T> IFootprint<T>.CornerFootprintElements => cornerFootprintElements;
        List<T> cornerFootprintElements = new List<T>();

        List<T> IFootprint<T>.BorderFootprintElements => borderFootprintElements;
        List<T> borderFootprintElements = new List<T>();

        Vector2Int IFootprint<T>.FootprintOffset => footprintOffset;
        Vector2Int footprintOffset = Vector2Int.zero;

        Vector2Int IFootprint<T>.FootprintSize => footprintSize;
        Vector2Int footprintSize = Vector2Int.one;

        Vector2Int IFootprint<T>.FootprintExtents => footprintExtents;
        Vector2Int footprintExtents = Vector2Int.one;

        SpriteRenderer spriteRenderer;

        T mapElement;
        
        public delegate T SubCreate(T super, Vector2Int cellOffset);
        SubCreate subCreate;

        void Init()
        {
            Vector2Int boundsMin = mapElement.Map.LocalToCell(spriteRenderer.bounds.min);
            Vector2Int boundsMax = mapElement.Map.LocalToCell(spriteRenderer.bounds.max);

            footprintSize.x = boundsMax.x - boundsMin.x;
            footprintSize.y = boundsMax.y - boundsMin.y;

            footprintExtents.x = footprintSize.x / 2;
            footprintExtents.y = footprintSize.y / 2;

            Vector2Int mapElementLocation = mapElement.Location;
            Vector2Int centerLocation = mapElement.Map.LocalToCell(spriteRenderer.bounds.center);

            footprintOffset = centerLocation - mapElementLocation;
            footprintOffset.x -= footprintExtents.x;
            footprintOffset.y -= footprintExtents.y;

            int columns = footprintSize.x;
            int rows = footprintSize.y;


            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    Vector2Int footprintLocation = new Vector2Int(column + footprintOffset.x, row + footprintOffset.y);
                    if (footprintLocation == mapElementLocation) continue;

                    T footprintElement = subCreate(mapElement, footprintLocation);
                    allFootprintElements.Add(footprintElement);

                    if (column == 0)
                    {
                        borderFootprintElements.Add(footprintElement);
                        if(row == 0 || row == rows - 1)
                        {
                            cornerFootprintElements.Add(footprintElement);
                        }
                    }
                    else if (column == columns -1)
                    {
                        borderFootprintElements.Add(footprintElement);
                        if (row == 0 || row == rows - 1)
                        {
                            cornerFootprintElements.Add(footprintElement);
                        }
                    }
                    else if (row == 0 || row == rows - 1)
                    {
                        borderFootprintElements.Add(footprintElement);
                    }
                }
            }

            AddLocationEventHandlers();
            UpdateFootprintPositions();
        }

        void AddLocationEventHandlers()
        {
            RemoveLocationEventHandlers();
            (mapElement as ILocatable).OnUpdated += Location_OnUpdated;
        }

        void RemoveLocationEventHandlers()
        {
            (mapElement as ILocatable).OnUpdated -= Location_OnUpdated;
        }

        private void Location_OnUpdated(ILocatable obj)
        {
            UpdateFootprintPositions();
        }

        void UpdateFootprintPositions()
        {
            Vector3 position = mapElement.Position;

            foreach (T footprintElement in allFootprintElements)
            {
                footprintElement.Position = position;
            }
        }

        void IFootprint<T>.Destroy()
        {
            RemoveLocationEventHandlers();

            foreach (T footprintElement in allFootprintElements)
            {
                footprintElement.RemoveFromMap();
            }

            allFootprintElements.Clear();
            borderFootprintElements.Clear();
            cornerFootprintElements.Clear();
        }

        public static IFootprint<T> Create(SpriteRenderer spriteRenderer, T mapElement, SubCreate subCreate)
        {
            Footprint<T> footprint = new Footprint<T>
            {
                spriteRenderer = spriteRenderer,
                mapElement = mapElement,
                subCreate = subCreate
            };

            footprint.Init();

            return footprint;
        }
    }
}
