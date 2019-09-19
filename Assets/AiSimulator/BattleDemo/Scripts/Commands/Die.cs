using IndieDevTools.Agents;
using IndieDevTools.Commands;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that simply removes a given agent
    /// from its map.
    /// </summary>
    public class Die : AbstractCommand
    {
        IAgent agent = null;

        protected override void OnStart()
        {
            agent.RemoveFromMap();
            Complete();
        }

        public static ICommand Create(IAgent agent)
        {
            return new Die
            {
                agent = agent
            };
        }
    }
}
