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
        IAgent agent;

        protected override void OnStart()
        {
            SetTargetLocation();
            Complete();
        }

        void SetTargetLocation()
        {
            bool isAgentInBounds = agent.Map.InBounds(agent.Location);
            if (isAgentInBounds)
            {
                agent.TargetLocation = GetNewLocationInMoveRadius();
            }
            else
            {
                agent.TargetLocation = GetNearestLocationInBounds();
            }
        }

        const int maxTryCount = 100;

        Vector2Int GetNewLocationInMoveRadius()
        {
            int moveRadius = TraitsUtil.GetMoveRadius(agent);
            Vector2Int location = Vector2Int.zero;

            bool isLandableLocation = false;
            int tryCount = 0;

            while (isLandableLocation == false && tryCount++ < maxTryCount)
            {
                Vector2Int offset = Vector2Int.RoundToInt(Random.insideUnitCircle * moveRadius);
                location = agent.Location;
                location.x += offset.x;
                location.y += offset.y;

                bool isInBounds = agent.Map.InBounds(location);
                if (isInBounds == false) continue;

                ILandable landable = agent.Map.GetMapElementAtCell<ILandable>(location);
                bool isLandableAtCell = landable != null;
                if (isLandableAtCell)
                {
                    isLandableLocation = landable.GetIsLandable(agent);
                }
                else
                {
                    isLandableLocation = true;
                }
            }

            if (tryCount > maxTryCount)
            {
                return agent.Location;
            }

            return location;
        }

        Vector2Int GetNearestLocationInBounds()
        {
            Vector2Int mapSize = agent.Map.Size;

            int leftBound = -mapSize.x / 2;
            int rightBound = mapSize.x / 2;
            int topBound = -mapSize.y / 2;
            int bottomBound = mapSize.y / 2;

            Vector2Int location = agent.Location;

            if (location.x < leftBound)
            {
                location.x = leftBound;
            }
            else if (location.x > rightBound)
            {
                location.x = rightBound;
            }

            if (location.y < topBound)
            {
                location.y = topBound;
            }
            else if (location.y > bottomBound)
            {
                location.y = bottomBound;
            }

            return location;
        }

        public static ICommand Create(IAgent agent)
        {
            ChooseLandableLocation command = new ChooseLandableLocation
            {
                agent = agent
            };

            return command;
        }
    }
}