using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Utils
{
    /// <summary>
    /// A utility class with various grid related methods.
    /// </summary>
    public static class GridUtil
    {
        /// <summary>
        /// Gets a set of cells given a grid size. 
        /// </summary>
        /// /// <param name="gridSize">The size of the grid.</param>
        public static HashSet<Vector2Int> GetAllCells(Vector2Int gridSize)
        {
            Vector2Int centerOffset = Vector2Int.zero;
            centerOffset.x -= gridSize.x / 2;
            centerOffset.y -= gridSize.y / 2;

            HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
            for(int x = 0; x < gridSize.x; x++)
            {
                for(int y = 0; y < gridSize.y; y++)
                {
                    cells.Add(CreateCell(x + centerOffset.x, y + centerOffset.y));
                }
            }
            return cells;
        }

        /// <summary>
        /// Gets a set cells outside of a rasterized diamond. Similar to punching a hole in a grid.
        /// </summary>
        /// <param name="gridSize">The size of the grid.</param>
        /// <param name="centerCell">The center location of the diamond.</param>
        /// <param name="radius">The radius of the diamond.</param>
        /// <param name="isFilled">Whether or not the returned set includes all cells outside the diamond or just an outline.</param>
        /// <returns>A set of cells outside of a diamond.</returns>
        public static HashSet<Vector2Int> GetCellsOutsideRadius(Vector2Int gridSize, Vector2Int centerCell, int radius, bool isFilled)
        {
            HashSet<Vector2Int> gridCells = GetAllCells(gridSize);
            HashSet<Vector2Int> circleCells = GetCellsInsideRadius(gridSize, centerCell, radius, isFilled);
            gridCells.ExceptWith(circleCells);
            return gridCells;
        }

        /// <summary>
        /// Gets a set cells inside of a rasterized diamond.
        /// </summary>
        /// <param name="gridSize">The size of the grid.</param>
        /// <param name="centerCell">The center location of the diamond.</param>
        /// <param name="radius">The radius of the diamond.</param>
        /// <param name="isFilled">Whether or not the returned set includes all cells inside the diamond or just an outline.</param>
        /// <returns>A set of cells inside of a diamond.</returns>
        public static HashSet<Vector2Int> GetCellsInsideRadius(Vector2Int gridSize, Vector2Int centerCell, int radius, bool isFilled)
        {
            Vector2Int offsetCenter = centerCell;

            offsetCenter.x += gridSize.x / 2;
            offsetCenter.y += gridSize.y / 2;

            HashSet<Vector2Int> cells = new HashSet<Vector2Int>();

            radius = Mathf.Max(0, radius);

            for(int y = 0; y < radius; y++)
            {
                int x = radius - y;

                AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, y);
                AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, -x, y);
                AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, -y);
                AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, -x, -y);

                if (isFilled == false || y <= 0) continue;

                for(int fy = y -1; fy >= 0; fy--)
                {
                    AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, fy);
                    AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, -x, fy);
                    AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, -fy);
                    AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, -x, -fy);
                }
            }

            if (isFilled)
            {
                AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, 0, 0);
            }

            return cells;
        }

        /// <summary>
        /// Gets a set cells outside of a rasterized circle. Similar to punching a hole in a grid.
        /// </summary>
        /// <param name="gridSize">The size of the grid.</param>
        /// <param name="centerCell">The center location of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="isFilled">Whether or not the returned set includes all cells outside the circle or just an outline.</param>
        /// <returns>A set of cells outside of a circle.</returns>
        public static HashSet<Vector2Int> GetCircleCellsOutsideRadius(Vector2Int gridSize, Vector2Int centerCell, int radius, bool isFilled)
        {
            HashSet<Vector2Int> gridCells = GetAllCells(gridSize);
            HashSet<Vector2Int> circleCells = GetCircleCellsInsideRadius(gridSize, centerCell, radius, isFilled);
            gridCells.ExceptWith(circleCells);
            return gridCells;
        }

        /// <summary>
        /// Gets a set cells inside of a rasterized circle.
        /// </summary>
        /// <param name="gridSize">The size of the grid.</param>
        /// <param name="centerCell">The center location of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="isFilled">Whether or not the returned set includes all cells inside the circle or just an outline.</param>
        /// <returns>A set of cells inside of a circle.</returns>
        public static HashSet<Vector2Int> GetCircleCellsInsideRadius(Vector2Int gridSize, Vector2Int centerCell, int radius, bool isFilled)
        {       
            Vector2Int offsetCenter = centerCell;

            offsetCenter.x += gridSize.x / 2;
            offsetCenter.y += gridSize.y / 2;

            HashSet<Vector2Int> cells = new HashSet<Vector2Int>();

            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                if (isFilled)
                {
                    for (int ty = y; ty > 0; ty--)
                    {
                        AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, ty);
                    }
                }
                else
                {
                    AddCellInBoundsToHashSet(cells, gridSize, centerCell, offsetCenter, x, y);
                }

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);

            if (isFilled)
            {
                if (offsetCenter.x >= 0 && offsetCenter.x <= gridSize.x - 1 && offsetCenter.y >= 0 && offsetCenter.y <= gridSize.y - 1) cells.Add(CreateCell(centerCell.x, centerCell.y));
            }

            return cells;
        }

        /// <summary>
        /// Helper method for adding a cell to a set if its within bounds.
        /// </summary>
        static void AddCellInBoundsToHashSet(HashSet<Vector2Int> cells, Vector2Int mapSize, Vector2Int center, Vector2Int offsetCenter, int x, int y)
        {
            if (offsetCenter.x + x >= 0 && offsetCenter.x + x <= mapSize.x - 1 && offsetCenter.y + y >= 0 && offsetCenter.y + y <= mapSize.y - 1) cells.Add(CreateCell(center.x + x, center.y + y));
            if (offsetCenter.x + x >= 0 && offsetCenter.x + x <= mapSize.x - 1 && offsetCenter.y - y >= 0 && offsetCenter.y - y <= mapSize.y - 1) cells.Add(CreateCell(center.x + x, center.y - y));
            if (offsetCenter.x - x >= 0 && offsetCenter.x - x <= mapSize.x - 1 && offsetCenter.y + y >= 0 && offsetCenter.y + y <= mapSize.y - 1) cells.Add(CreateCell(center.x - x, center.y + y));
            if (offsetCenter.x - x >= 0 && offsetCenter.x - x <= mapSize.x - 1 && offsetCenter.y - y >= 0 && offsetCenter.y - y <= mapSize.y - 1) cells.Add(CreateCell(center.x - x, center.y - y));
            if (offsetCenter.x + y >= 0 && offsetCenter.x + y <= mapSize.x - 1 && offsetCenter.y + x >= 0 && offsetCenter.y + x <= mapSize.y - 1) cells.Add(CreateCell(center.x + y, center.y + x));
            if (offsetCenter.x + y >= 0 && offsetCenter.x + y <= mapSize.x - 1 && offsetCenter.y - x >= 0 && offsetCenter.y - x <= mapSize.y - 1) cells.Add(CreateCell(center.x + y, center.y - x));
            if (offsetCenter.x - y >= 0 && offsetCenter.x - y <= mapSize.x - 1 && offsetCenter.y + x >= 0 && offsetCenter.y + x <= mapSize.y - 1) cells.Add(CreateCell(center.x - y, center.y + x));
            if (offsetCenter.x - y >= 0 && offsetCenter.x - y <= mapSize.x - 1 && offsetCenter.y - x >= 0 && offsetCenter.y - x <= mapSize.y - 1) cells.Add(CreateCell(center.x - y, center.y - x));
        }

        /// <summary>
        /// Helper method for creating a cell.
        /// </summary>
        static Vector2Int CreateCell(int x, int y)
        {
            return new Vector2Int(x, y);
        }
    }
}
