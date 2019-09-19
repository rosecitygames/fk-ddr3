using System;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Paths
{
    /// <summary>
    /// A graph used for traversing across weighted objects on a map.
    /// </summary>
    public class WeightedGraph : IWeightedGraph<Vector2Int>
    {
        static readonly Vector2Int[] adjacentDirections = new[]
        {
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up
        };

        IMap map = null;
        int IWeightedGraph<Vector2Int>.Cost(Vector2Int a, Vector2Int b) => Cost(a, b);
        int Cost(Vector2Int a, Vector2Int b) => 0;

        IEnumerable<Vector2Int> IWeightedGraph<Vector2Int>.Neighbors(Vector2Int id) => Neighbors(id);
        IEnumerable<Vector2Int> Neighbors(Vector2Int id)
        {
            foreach (Vector2Int adjacentDirection in adjacentDirections)
            {
                Vector2Int next = new Vector2Int(id.x + adjacentDirection.x, id.y + adjacentDirection.y);

                bool isInBounds = map.InBounds(next);
                if (isInBounds)
                {
                    yield return next;
                }
            }
        }

        public static IWeightedGraph<Vector2Int> Create(IMap map)
        {
            return new WeightedGraph
            {
                map = map
            };
        }
    }
}
