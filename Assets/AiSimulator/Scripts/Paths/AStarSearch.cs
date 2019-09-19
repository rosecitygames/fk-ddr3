using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Paths
{
    /// <summary>
    /// An implementation of the A* pathfinding algorithm that uses a weighted graph.
    /// </summary>
    public class AStarSearch
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, double> costSoFar = new Dictionary<Vector2Int, double>();

        public List<Vector2Int> Path = new List<Vector2Int>();

        static public double Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        AStarSearch(IWeightedGraph<Vector2Int> graph, Vector2Int start, Vector2Int goal)
        {
            var frontier = new PriorityQueue<Vector2Int>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in graph.Neighbors(current))
                {
                    double newCost = costSoFar[current] + graph.Cost(current, next);

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            bool wasGoalReached = cameFrom.ContainsKey(goal);

            if (wasGoalReached)
            {
                Vector2Int node = goal;
                while (node != start)
                {
                    Path.Add(node);
                    if (cameFrom.ContainsKey(node))
                    {
                        node = cameFrom[node];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Path.Reverse();
        }

        public static AStarSearch Create(IWeightedGraph<Vector2Int> graph, Vector2Int start, Vector2Int goal)
        {
            return new AStarSearch(graph, start, goal);
        }
    }
}