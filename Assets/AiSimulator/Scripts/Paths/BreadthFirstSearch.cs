using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Paths
{
    /// <summary>
    /// A simple breatfh first search implementation.
    /// </summary>
    public class BreadthFirstSearch
    {
        static void Search<T>(Graph<T> graph, T start)
        {
            var frontier = new Queue<T>();
            frontier.Enqueue(start);

            var visited = new HashSet<T> { start };

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                foreach (var next in graph.Neighbors(current))
                {
                    if (!visited.Contains(next))
                    {
                        frontier.Enqueue(next);
                        visited.Add(next);
                    }
                }
            }
        }
    }
}
