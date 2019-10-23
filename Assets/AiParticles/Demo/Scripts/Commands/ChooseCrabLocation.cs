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
    /// an crab's move radius.
    /// </summary>
    public class ChooseCrabLocation : AbstractCommand
    {
        ICrab crab;

        /// <summary>
        /// Sets a target location and completes the command.
        /// </summary>
        protected override void OnStart()
        {
            SetTargetLocation();
            Complete();
        }

        /// <summary>
        /// Sets the crab's target location. 
        /// </summary>
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

        /// <summary>
        /// Gets a new location within the crab's move radius.
        /// </summary>
        /// <returns>The location</returns>
        Vector2Int GetNewLocationInMoveRadius()
        {
            int moveRadius = TraitsUtil.GetMoveRadius(crab);

            HashSet<Vector2Int> cellsInsideRadius = GridUtil.GetCellsInsideRadius(crab.Map.Size, crab.Location, moveRadius, true);
            cellsInsideRadius.Remove(crab.Location);
            List<Vector2Int> locations = new List<Vector2Int>(cellsInsideRadius);

            Vector2Int location = Vector2Int.zero;

            bool isLocationFound = false;

            int tryCount = 0;
            int maxTryCount = 100;

            // Find a random location inside the crab's move radius.
            while (isLocationFound == false && locations.Count > 0)
            {
                if (++tryCount >= maxTryCount) break;

                int locationIndex = Random.Range(0, locations.Count);
                location = locations[locationIndex];
                
                isLocationFound = GetIsFootprintInBounds(location);
             
                // If the selected location isn't valid, then remove it from the collection pool.
                if (isLocationFound == false)
                {
                    locations.RemoveAt(locationIndex);
                    continue;
                }
            }

            // If the max number of tries was reached, then stay in the current location.
            if (tryCount >= maxTryCount)
            {
                return crab.Location;
            }

            // If there were no locations left in the pool, then edge case occured and just
            // set the location to the center of the map since valid locations cant be found
            // in the current location.
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

        /// <summary>
        /// Gets the nearest location thats in bounds on the map.
        /// </summary>
        /// <returns>The new location</returns>
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

        /// <summary>
        /// Creates a command object.
        /// </summary>
        /// <param name="crab">The crab whos new location will be chosen</param>
        /// <returns>The created command object</returns>
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