using IndieDevTools.Agents;
using IndieDevTools.Animation;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Spawners;
using IndieDevTools.Traits;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A helper class that "molts" an agent by spawning a clone at a bigger size,
    /// passing on its traits, and then destroying the original.
    /// </summary>
    public class AgentMolter : IMoltable
    {
        /// <summary>
        /// The agent to be molted.
        /// </summary>
        AbstractAgent abstractAgent = null;

        /// <summary>
        /// The agent interface.
        /// </summary>
        IAgent Agent => abstractAgent;

        /// <summary>
        /// The agent transform.
        /// </summary>
        Transform AgentTransform => abstractAgent.transform;

        /// <summary>
        /// The agent game object.
        /// </summary>
        GameObject AgentGameObject => abstractAgent.gameObject;

        /// <summary>
        /// The spawnable object. Usually the agent itself, but doesn't need to be.
        /// </summary>
        ISpawnable spawnable = null;

        /// <summary>
        /// The default amount the agent will grow in size when molting.
        /// </summary>
        const float defaultMoltPercentage = 1.15f;

        /// <summary>
        /// The amount the agent will grow in size when molting.
        /// </summary>
        float moltPercentage = defaultMoltPercentage;

        /// <summary>
        /// Creates a clone of the agent at a bigger size and destroys the original.
        /// </summary>
        void IMoltable.Molt()
        {
            ITrait sizeTrait = (Agent as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);

            int size = sizeTrait != null ? sizeTrait.Quantity : 0;

            // If the size is less than one, then simply destroy the agent without molting.
            if (size < 1)
            {
                GameObject.Destroy(AgentGameObject);
                return;
            }

            int newSize = (int)(size * moltPercentage);
            newSize = Mathf.Max(newSize, size + 1);
            sizeTrait.Quantity = newSize;
                        
            Vector3 scale = AgentTransform.localScale;
            scale *= moltPercentage;

            Vector3 position = Agent.Position;

            int health = TraitsUtil.GetHealth(Agent) + 1;

            Agent.RemoveFromMap();

            GameObject instance = spawnable.Spawn(position, scale);

            IAgent instanceAgent = instance.GetComponentInChildren<IAgent>();
            if (instanceAgent != null)
            {
                instanceAgent.Data = Agent.Data.Copy();
                instanceAgent.DisplayName = Agent.DisplayName;
                instanceAgent.Description = "";
                instanceAgent.GroupId = Agent.GroupId;
                TraitsUtil.SetHealth(instanceAgent, health);
            }

            Tweener tweener = instance.GetComponentInChildren<Tweener>();
            if (tweener != null)
            {
                tweener.TweenOnStart = TweenerMethod.None;
            }

            GameObject.Destroy(AgentGameObject);
        }

        /// <summary>
        /// Creates a moltable object.
        /// </summary>
        /// <param name="agent">The agent whose traits will be passed</param>
        /// <param name="spawnable">The object to be spawned when molting. Usually the agent</param>
        /// <param name="moltPercefntage">The percentage that the spawned object will grow in size</param>
        /// <returns></returns>
        public static IMoltable Create(AbstractAgent agent, ISpawnable spawnable, float moltPercefntage = defaultMoltPercentage)
        {
            AgentMolter agentMolter = new AgentMolter
            {
                abstractAgent = agent,
                spawnable = spawnable,
                moltPercentage = moltPercefntage
            };
            
            return agentMolter;
        }
    }
}