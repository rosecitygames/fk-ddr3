using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Traits;
using IndieDevTools.Commands;
using IndieDevTools.States;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that handles incoming attacks on an agent by comparing
    /// attack strength and defensive strength. Depending on the result,
    /// a relevent transition will be called. If defense is less than the
    /// attack strength agent, then health trait is reduced. 
    /// </summary>
    public class AttackHandler : AbstractCommand
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

            int health = TraitsUtil.GetHealth(agent);

            int attackStrength = TraitsUtil.GetRandomAttackStrength(attackingAgent);
            int defenseStrength = TraitsUtil.GetRandomDefenseStrength(agent);

            int healthDecrement = attackStrength - defenseStrength;
            if (healthDecrement > 0)
            {          
                health -= healthDecrement;
                TraitsUtil.SetHealth(agent, health);
            }


            if (health <= 0)
            {
                agent.Description = "Killed by " + attackingAgent.DisplayName;
                agent.HandleTransition(onDeathTransition);
            }
            else
            {
                agent.Description = "Attacked by " + attackingAgent.DisplayName;// + "\nattackStrength = "+attackStrength+", defenseStrength = "+defenseStrength+", remaining health = " + health;
                agent.HandleTransition(onAttackedTransition);
            }
        }
       
        public static ICommand Create(IAgent agent, IAttackReceiver attackReceiver, string onAttackedTransition = "", string onDeathTransition = "")
        {
            return new AttackHandler
            {
                agent = agent,
                attackReceiver = attackReceiver,
                onAttackedTransition = onAttackedTransition,
                onDeathTransition = onDeathTransition
            };
        }
    }
}
