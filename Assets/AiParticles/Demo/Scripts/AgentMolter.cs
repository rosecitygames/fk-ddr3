﻿using IndieDevTools.Agents;
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

        const float sizePercentageIncrease = 1.15f;

        void IMoltable.Molt()
        {
            ITrait sizeTrait = (Agent as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);

            int size = sizeTrait != null ? sizeTrait.Quantity : 0;
            if (size < 1)
            {
                GameObject.Destroy(AgentGameObject);
                return;
            }

            int newSize = (int)(size * sizePercentageIncrease);
            newSize = Mathf.Max(newSize, size + 1);
            sizeTrait.Quantity = newSize;
                        
            Vector3 scale = AgentTransform.localScale;
            scale *= sizePercentageIncrease;

            Vector3 position = Agent.Position;

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
            
            return agentMolter;
        }
    }
}