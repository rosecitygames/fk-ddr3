using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// A footprint is a collection of sub elements for a map element that is larger than one grid unit.
    /// </summary>
    /// <typeparam name="T">The type of map element the footprint is.</typeparam>
    public class Footprint<T> : IFootprint<T> where T : IMapElement
    {
        /// <summary>
        /// A list of all the footprint elements.
        /// </summary>
        List<T> IFootprint<T>.AllFootprintElements => allFootprintElements;
        List<T> allFootprintElements = new List<T>();

        /// <summary>
        /// A list of the corner footprint elements.
        /// </summary>
        List<T> IFootprint<T>.CornerFootprintElements => cornerFootprintElements;
        List<T> cornerFootprintElements = new List<T>();

        /// <summary>
        /// A list of the outlining border footprint elements.
        /// </summary>
        List<T> IFootprint<T>.BorderFootprintElements => borderFootprintElements;
        List<T> borderFootprintElements = new List<T>();

        /// <summary>
        /// The offset of the footprint from the main element.
        /// </summary>
        Vector2Int IFootprint<T>.FootprintOffset => footprintOffset;
        Vector2Int footprintOffset = Vector2Int.zero;

        /// <summary>
        /// The width and height of the footprint in grid cell units.
        /// </summary>
        Vector2Int IFootprint<T>.FootprintSize => footprintSize;
        Vector2Int footprintSize = Vector2Int.one;

        /// <summary>
        /// The positional extents of the footprint in grid cell units.
        /// </summary>
        Vector2Int IFootprint<T>.FootprintExtents => footprintExtents;
        Vector2Int footprintExtents = Vector2Int.one;

        /// <summary>
        /// A reference to the main map element that the footprint belongs to.
        /// </summary>
        T mapElement;
        
        /// <summary>
        /// The delegate used to create the sub elements that make up a footprint.
        /// </summary>
        /// <param name="super">The map element type</param>
        /// <param name="cellOffset">The offset from the main map element</param>
        /// <returns></returns>
        public delegate T SubCreate(T super, Vector2Int cellOffset);
        SubCreate subCreate;

        /// <summary>
        /// Initializes the footprint.
        /// </summary>
        /// <param name="spriteRenderer">A sprite renderer used to determine the rectangular bounds of the footprint.</param>
        void Init(SpriteRenderer spriteRenderer)
        {
            Vector2Int boundsMin = mapElement.Map.LocalToCell(spriteRenderer.bounds.min);
            Vector2Int boundsMax = mapElement.Map.LocalToCell(spriteRenderer.bounds.max);
            Init(boundsMin, boundsMax);
        }

        /// <summary>
        /// Initializes the footprint.
        /// </summary>
        /// <param name="boundsMin">The bottom left corner of the footprint on the grid map.</param>
        /// <param name="boundsMax">The top right corner of the footprint on the grid map.</param>
        void Init(Vector2Int boundsMin, Vector2Int boundsMax)
        {
            footprintSize.x = boundsMax.x - boundsMin.x;
            footprintSize.y = boundsMax.y - boundsMin.y;

            footprintExtents.x = footprintSize.x / 2;
            footprintExtents.y = footprintSize.y / 2;

            Vector2Int mapElementLocation = mapElement.Location;

            Vector2Int centerLocation = new Vector2Int();
            centerLocation.x = boundsMin.x + Mathf.CeilToInt((boundsMax.x - boundsMin.x) / 2.0f);
            centerLocation.y = boundsMin.y + Mathf.CeilToInt((boundsMax.y - boundsMin.y) / 2.0f);

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

        /// <summary>
        /// Add location event handlers so the footprint location can be moved with the main map element.
        /// </summary>
        void AddLocationEventHandlers()
        {
            RemoveLocationEventHandlers();
            (mapElement as ILocatable).OnUpdated += Location_OnUpdated;
        }

        /// <summary>
        /// Remove location event handlers.
        /// </summary>
        void RemoveLocationEventHandlers()
        {
            (mapElement as ILocatable).OnUpdated -= Location_OnUpdated;
        }

        /// <summary>
        /// Location event handler for when the main map element changes location.
        /// </summary>
        /// <param name="obj">The main map element as a locatable</param>
        private void Location_OnUpdated(ILocatable obj)
        {
            UpdateFootprintPositions();
        }

        /// <summary>
        /// Updates the footprint sub elements on the grid map.
        /// </summary>
        void UpdateFootprintPositions()
        {
            Vector3 position = mapElement.Position;

            foreach (T footprintElement in allFootprintElements)
            {
                footprintElement.Position = position;
            }
        }

        /// <summary>
        /// Removes the footprint sub elements from the grid map and clears the footprint lists.
        /// </summary>
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

        /// <summary>
        /// Creates a footprint.
        /// </summary>
        /// <param name="spriteRenderer">The sprite renderer used to determine the footprint bounds.</param>
        /// <param name="mapElement">The main map element that the footprint belongs to.</param>
        /// <param name="subCreate">The delegate method used to create footprint sub elements.</param>
        /// <returns>A footprint.</returns>
        public static IFootprint<T> Create(SpriteRenderer spriteRenderer, T mapElement, SubCreate subCreate)
        {
            Footprint<T> footprint = new Footprint<T>
            {
                mapElement = mapElement,
                subCreate = subCreate
            };

            footprint.Init(spriteRenderer);

            return footprint;
        }

        /// <summary>
        /// Creates a footprint.
        /// </summary>
        /// <param name="boundsMin">The bottom left corner of the footprint on the grid map.</param>
        /// <param name="boundsMax">The top right corner of the footprint on the grid map.</param>
        /// <param name="mapElement">The main map element that the footprint belongs to.</param>
        /// <param name="subCreate">The delegate method used to create footprint sub elements.</param>
        /// <returns>A footprint.</returns>
        public static IFootprint<T> Create(Vector2Int boundsMin, Vector2Int boundsMax, T mapElement, SubCreate subCreate)
        {
            Footprint<T> footprint = new Footprint<T>
            {
                mapElement = mapElement,
                subCreate = subCreate
            };

            footprint.Init(boundsMin, boundsMax);

            return footprint;
        }
    }
}
