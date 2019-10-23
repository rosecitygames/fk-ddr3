using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that will remove and explode a target object.
    /// </summary>
    public class ExplodeAgent : AbstractCommand
    {
        /// <summary>
        /// The agent whos data will be passed to explosion spawned instances and then
        /// removed from the map.
        /// </summary>
        IAgent agent = null;

        /// <summary>
        /// The explodable object that will be exploded. Usually, set to the agent
        /// if it implements the interface.
        /// </summary>
        IExplodable explodable = null;

        /// <summary>
        /// Removes the target from map and explodes it if it's size is not too small.
        /// </summary>
        protected override void OnStart()
        {
            int size = TraitsUtil.GetSize(agent);
            if (size <= 1)
            {
                agent.RemoveFromMap();
                Complete();
                return;
            }

            AddEventHandlers();
            agent.RemoveFromMap();
            explodable.Explode();
        }

        /// <summary>
        /// Add explodable event handlers.
        /// </summary>
        void AddEventHandlers()
        {
            RemoveEventHandlers();
            explodable.OnInstanceCreated += Explodable_OnInstanceCreated;
            explodable.OnCompleted += Explodable_OnCompleted;
        }

        /// <summary>
        /// Remove explodable event handlers.
        /// </summary>
        void RemoveEventHandlers()
        {
            explodable.OnInstanceCreated -= Explodable_OnInstanceCreated;
            explodable.OnCompleted -= Explodable_OnCompleted;
        }

        /// <summary>
        /// Complete the command after the explosion is completed.
        /// </summary>
        private void Explodable_OnCompleted()
        {
            RemoveEventHandlers();
            Complete();
        }

        /// <summary>
        /// Pass a copy of the agent's data when new instances are spawned durring an explosion.
        /// </summary>
        /// <param name="instance">A spawned instance</param>
        private void Explodable_OnInstanceCreated(GameObject instance)
        {
            IAgent instanceAgent = instance.GetComponentInChildren<IAgent>();
            if (instanceAgent == null) return;

            instanceAgent.Data = agent.Data.Copy();
            instanceAgent.DisplayName = agent.DisplayName;
            instanceAgent.Description = "";
            instanceAgent.GroupId = agent.GroupId;
        }

        /// <summary>
        /// Create a command object.
        /// </summary>
        /// <param name="agent">The agent whos data will be passed to explosion spawned instances and then remove from the map.</param>
        /// <param name="explodable">The explodable object that will be exploded. Usually, set to the agent if it implements the interface.</param>
        /// <returns></returns>
        public static ICommand Create(IAgent agent, IExplodable explodable)
        {
            return new ExplodeAgent
            {
                agent = agent,
                explodable = explodable
            };
        }
    }
}
