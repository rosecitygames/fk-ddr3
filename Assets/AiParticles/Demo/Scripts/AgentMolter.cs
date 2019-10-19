using IndieDevTools.Agents;
using IndieDevTools.AiParticles;
using IndieDevTools.Traits;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class AgentMolter : IMoltable
    {
        AbstractAgent abstractAgent = null;
        IAgent Agent => abstractAgent;
        Transform AgentTransform => abstractAgent.transform;
        GameObject AgentGameObject => abstractAgent.gameObject;

        ISpawnable spawnable = null;

        int initialSize = 1;

        ITrait GetSizeTrait() => (Agent as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);

        void Init()
        {
            ITrait sizeTrait = GetSizeTrait();
            if (sizeTrait == null) return;
            initialSize = sizeTrait.Quantity;
        }

        void IMoltable.Molt(int size)
        {
            if (size <= 1)
            {
                GameObject.Destroy(AgentGameObject);
                return;
            }

            initialSize = size - 1;

            Vector3 position = Agent.Position;

            float percentageIncrease = (float)size / initialSize;
            Vector3 scale = AgentTransform.localScale;
            scale *= percentageIncrease;

            Agent.RemoveFromMap();

            GameObject instance = spawnable.Spawn(position, scale);

            IAgent instanceAgent = instance.GetComponentInChildren<IAgent>();
            if (instanceAgent == null) return;
            instanceAgent.Data = Agent.Data.Copy();
            instanceAgent.DisplayName = Agent.DisplayName;
            instanceAgent.Description = "";
            instanceAgent.GroupId = Agent.GroupId;
            TraitsUtil.SetHealth(instanceAgent, 3);

            GameObject.Destroy(AgentGameObject);
        }

        public static IMoltable Create(AbstractAgent agent, ISpawnable spawnable)
        {
            AgentMolter agentMolter = new AgentMolter
            {
                abstractAgent = agent,
                spawnable = spawnable
            };

            agentMolter.Init();

            return agentMolter;
        }
    }
}