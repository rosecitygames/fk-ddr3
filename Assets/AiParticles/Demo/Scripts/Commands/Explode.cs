using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Maps;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class Explode : AbstractCommand
    {
        IAgent agent = null;
        IExplodable explodable = null;

        float instanceMinExplosiveStrength = 0.0f;
        float instanceMaxExplosiveStrength = 0.0f;

        protected override void OnStart()
        {

            int size = TraitsUtil.GetSize(agent);
            if (size <= 1)
            {
                agent.RemoveFromMap();
                Complete();
                return;
            }

            InitSpriteExploder();
            AddEventHandlers();
            agent.RemoveFromMap();
            explodable.Explode();
        }

        void InitSpriteExploder()
        {
            instanceMinExplosiveStrength = explodable.MinExplosiveStrength * 2;
            instanceMaxExplosiveStrength = explodable.MaxExplosiveStrength * 2;
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

            SpriteExploder.SpriteExploder spriteExploder = instance.GetComponentInChildren<SpriteExploder.SpriteExploder>();
            if (spriteExploder == null) return;
            spriteExploder.MinExplosiveStrength = instanceMinExplosiveStrength;
            spriteExploder.MaxExplosiveStrength = instanceMaxExplosiveStrength;
        }

        public static ICommand Create(IAgent agent, IExplodable explodable)
        {
            return new Explode
            {
                agent = agent,
                explodable = explodable
            };
        }
    }
}
