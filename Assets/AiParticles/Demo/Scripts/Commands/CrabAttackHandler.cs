using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Traits;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class CrabAttackHandler : AbstractCommand
    {
        IAgent agent = null;
        IAttackReceiver attackReceiver = null;

        string onAttackedTransition;
        string onDeathTransition;

        protected override void OnStart()
        {
            AddEventHandler();
        }

        protected override void OnStop()
        {
            RemoveEventHandler();
        }

        protected override void OnDestroy()
        {
            RemoveEventHandler();
        }

        void AddEventHandler()
        {
            RemoveEventHandler();
            attackReceiver.OnAttackReceived += HandleAttack;
        }

        void RemoveEventHandler()
        {
            attackReceiver.OnAttackReceived -= HandleAttack;
        }

        void HandleAttack(IAgent attackingAgent)
        {
            attackingAgent.Description = "Attacking " + agent.DisplayName;

            agent.TargetMapElement = attackingAgent;
            agent.TargetLocation = attackingAgent.Location;
            
            int healthQuantity = TraitsUtil.GetHealth(agent);

            int attackStrength = TraitsUtil.GetRandomAttackStrength(attackingAgent);
            int defenseStrength = TraitsUtil.GetRandomDefenseStrength(agent);

            int healthDecrement = attackStrength - defenseStrength;
            if (healthDecrement > 0)
            {
                healthQuantity -= healthDecrement;
                TraitsUtil.SetHealth(agent, healthQuantity);
            }

            if (healthQuantity <= 0)
            {
                int agentSize = TraitsUtil.GetSize(agent);
                int attackingAgentSize = TraitsUtil.GetSize(attackingAgent);

                if (agentSize < attackingAgentSize)
                {
                    attackingAgentSize += agentSize;
                    TraitsUtil.SetSize(attackingAgent, attackingAgentSize);

                    ITrait attackingAgentHealth = attackingAgent.GetStat(TraitsUtil.healthTraitId);
                    attackingAgentHealth.Quantity += agentSize;

                    TraitsUtil.SetSize(agent, 0);
                }

                agent.Description = "Killed by " + attackingAgent.DisplayName;
                agent.HandleTransition(onDeathTransition);
            }
            else
            {
                agent.Description = "Attacked by " + attackingAgent.DisplayName;// + "\nattackStrength = "+attackStrength+", defenseStrength = "+defenseStrength+", remaining health = " + health;
                agent.HandleTransition(onAttackedTransition);
            }

            //Debug.Log("HandleAttack health = " + health+", "+ agent.Description);
        }

        public static ICommand Create(IAgent agent, IAttackReceiver attackReceiver, string onAttackedTransition = "", string onDeathTransition = "")
        {
            return new CrabAttackHandler
            {
                agent = agent,
                attackReceiver = attackReceiver,
                onAttackedTransition = onAttackedTransition,
                onDeathTransition = onDeathTransition
            };
        }
    }
}
