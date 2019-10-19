using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class ExplodeAgent : AbstractCommand
    {
        IAgent agent = null;
        IExplodable explodable = null;

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

        void AddEventHandlers()
        {
            RemoveEventHandlers();
            explodable.OnInstanceCreated += Explodable_OnInstanceCreated;
            explodable.OnCompleted += Explodable_OnCompleted;
        }

        void RemoveEventHandlers()
        {
            explodable.OnInstanceCreated -= Explodable_OnInstanceCreated;
            explodable.OnCompleted -= Explodable_OnCompleted;
        }

        private void Explodable_OnCompleted()
        {
            RemoveEventHandlers();
            Complete();
        }

        private void Explodable_OnInstanceCreated(GameObject instance)
        {
            IAgent instanceAgent = instance.GetComponentInChildren<IAgent>();
            if (instanceAgent == null) return;

            instanceAgent.Data = agent.Data.Copy();
            instanceAgent.DisplayName = agent.DisplayName;
            instanceAgent.Description = "";
            instanceAgent.GroupId = agent.GroupId;
            TraitsUtil.SetHealth(instanceAgent, 3);
        }

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
