using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Traits;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that chooses a new location target within
    /// an agent's move radius.
    /// </summary>
    public class ChooseLandableLocation : AbstractCommand
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

        const int maxTryCount = 100;

        Vector2Int GetNewLocationInMoveRadius()
        {
            int moveRadius = TraitsUtil.GetMoveRadius(crab);
            Vector2Int location = Vector2Int.zero;

            bool isLandableLocation = false;
            int tryCount = 0;

            int columns = crab.FootprintSize.x;
            int rows = crab.FootprintSize.y;

            Vector2Int extents = new Vector2Int();
            extents.x = columns / 2;
            extents.y = rows / 2;

            while (isLandableLocation == false && tryCount++ < maxTryCount)
            {
                Vector2Int offset =  Vector2Int.RoundToInt(Random.insideUnitCircle * moveRadius);
                location = crab.Location;
                location.x += offset.x;
                location.y += offset.y;
                
                bool isInBounds = GetIsLocationOffsetInBounds(offset);
                if (isInBounds == false) continue;

                ILandable landable = crab.Map.GetMapElementAtCell<ILandable>(location);
                bool isLandableAtCell = landable != null;
                if (isLandableAtCell)
                {
                    isLandableLocation = landable.GetIsLandable(crab);
                }
                else
                {
                    isLandableLocation = true;
                }
            }

            if (tryCount > maxTryCount)
            {
                Debug.Log("tryCount = " + tryCount);
                return crab.Location;
            }

            return location;
        }

        /// <summary>
        /// Check each corner location of the footprint bounds to see if its in bounds.
        /// </summary>
        /// <param name="offset">The location offset</param>
        /// <returns></returns>
        bool GetIsLocationOffsetInBounds(Vector2Int offset)
        {
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

            int leftBound = -mapSize.x / 2;
            int rightBound = mapSize.x / 2;
            int topBound = -mapSize.y / 2;
            int bottomBound = mapSize.y / 2;

            Vector2Int location = crab.Location;

            if (location.x <= leftBound)
            {
                location.x = leftBound + 1;
            }
            else if (location.x >= rightBound)
            {
                location.x = rightBound - 1;
            }

            if (location.y <= topBound)
            {
                location.y = topBound + 1;
            }
            else if (location.y >= bottomBound)
            {
                location.y = bottomBound - 1;
            }

            return location;
        }

        public static ICommand Create(ICrab crab)
        {
            ChooseLandableLocation command = new ChooseLandableLocation
            {
                crab = crab
            };

            return command;
        }
    }
}