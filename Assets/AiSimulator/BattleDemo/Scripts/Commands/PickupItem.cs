using IndieDevTools.Agents;
using IndieDevTools.Commands;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that removes a given agent's target map element
    /// from the map. If the demo was expanded to include agent
    /// inventories, then this would als be a good place to add
    /// the item it the agent's inventory.
    /// </summary>
    public class PickupItem : AbstractCommand
    {
        IAgent agent = null;

        protected override void OnStart()
        {
            if (agent.TargetMapElement != null)
            {
                agent.TargetMapElement.Description = "Picked up by " + agent.DisplayName;
                agent.Description = "Picked up " + agent.TargetMapElement.DisplayName;
                agent.TargetMapElement.RemoveFromMap();
                agent.TargetMapElement = null;
            }

            Complete();
        }

        public static ICommand Create(IAgent agent)
        {
            return new PickupItem
            {
                agent = agent
            };
        }
    }
}
