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
            agent.TargetLocation = GetNewLocation();
        }

        Vector2Int GetNewLocation()
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