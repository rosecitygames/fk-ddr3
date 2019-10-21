using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Traits;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;
using System.Collections.Generic;
using IndieDevTools.Utils;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that chooses a new location target within
    /// an agent's move radius.
    /// </summary>
    public class ChooseCrabLocation : AbstractCommand
    {
        ICrab crab;

        protected override void OnStart()
        {
            SetTargetLocation();
            Complete();
        }

        void SetTargetLocation()
        {
            bool isAgentInBounds = crab.Map.InBounds(crab.Location);
            if (isAgentInBounds)
            {
                crab.TargetLocation = GetNewLocationInMoveRadius();
            }
            else
            {
                crab.TargetLocation = GetNearestLocationInBounds();
            }
        }

        Vector2Int GetNewLocationInMoveRadius()
        {
            int moveRadius = TraitsUtil.GetMoveRadius(crab);

            HashSet<Vector2Int> cellsInsideRadius = GridUtil.GetCellsInsideRadius(crab.Map.Size, crab.Location, moveRadius, true);
            cellsInsideRadius.Remove(crab.Location);
            List<Vector2Int> locations = new List<Vector2Int>(cellsInsideRadius);

            Vector2Int location = Vector2Int.zero;

            bool isLandableLocation = false;

            int tryCount = 0;
            int maxTryCount = 100;

            while (isLandableLocation == false && locations.Count > 0)
            {
                if (++tryCount >= maxTryCount) break;

                int locationIndex = Random.Range(0, locations.Count);
                location = locations[locationIndex];
                
                bool isInBounds = GetIsFootprintInBounds(location);
                if (isInBounds == false)
                {
                    locations.RemoveAt(locationIndex);
                    continue;
                }
                else
                {
                    isLandableLocation = true;
                }
            }

            if (tryCount >= maxTryCount)
            {
                return crab.Location;
            }

            if (locations.Count <= 0)
            {
                return Vector2Int.zero;
            }

            return location;
        }

        /// <summary>
        /// Check each corner location of the footprint bounds to see if its in bounds.
        /// </summary>
        bool GetIsFootprintInBounds(Vector2Int location)
        {
            Vector2Int offset = location - crab.Location;
            if (crab.CornerFootprintElements.Count > 0)
            {
                foreach (ICrab cornerElement in crab.CornerFootprintElements)
                {
                    Vector2Int offsetLocation = cornerElement.Location + offset;
                    bool isInBounds = crab.Map.InBounds(offsetLocation);
                    if (isInBounds == false) return false;
                }

                return true;
            }
            else
            {
                Vector2Int offsetLocation = crab.Location + offset;
                bool isInBounds = crab.Map.InBounds(offsetLocation);
                return isInBounds;
            }
        }

        Vector2Int GetNearestLocationInBounds()
        {
            Vector2Int mapSize = crab.Map.Size;

            int maxX = (mapSize.x / 2) - 2;
            int minX = -maxX - 1;
            int maxY = (mapSize.y / 2) - 1;
            int minY = -maxY - 1;

            Vector2Int location = crab.Location;

            if (location.x < minX)
            {
                location.x = minX;
            }
            else if (location.x > maxX)
            {
                location.x = maxX;
            }

           
            if (location.y < minY)
            {
                location.y = minY;
            }
            else if (location.y > maxY)
            {
                location.y = maxY;
            }

            return location;
        }

        public static ICommand Create(ICrab crab)
        {
            ChooseCrabLocation command = new ChooseCrabLocation
            {
                crab = crab
            };

            return command;
        }
    }
}