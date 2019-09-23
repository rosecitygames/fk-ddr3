using IndieDevTools.Agents;
using IndieDevTools.Commands;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that chooses a new location target within
    /// an agent's move radius.
    /// </summary>
    public class ChooseNewLocation : AbstractCommand
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

        Vector2Int GetNewLocationInMoveRadius()
        {
            int moveRadius = TraitsUtil.GetMoveRadius(agent);
            Vector2Int location = agent.Location;

            bool isInBounds = false;

            while(isInBounds == false)
            {
                Vector2Int offset = Vector2Int.RoundToInt(Random.insideUnitCircle * moveRadius);
                location = agent.Location;
                location.x += offset.x;
                location.y += offset.y;

                isInBounds = agent.Map.InBounds(location);
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
            ChooseNewLocation command = new ChooseNewLocation
            {
                agent = agent
            };

            return command;
        }
    }
}