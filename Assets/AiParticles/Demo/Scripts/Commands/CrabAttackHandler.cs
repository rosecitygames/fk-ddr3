using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class CrabAttackHandler : AbstractCommand
    {
        ICrab crab = null;

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
            crab.OnAttackReceived += HandleAttack;
        }

        void RemoveEventHandler()
        {
            crab.OnAttackReceived -= HandleAttack;
        }

        void HandleAttack(IAgent attackingAgent)
        {
            attackingAgent.Description = "Attacking " + crab.DisplayName;

            crab.TargetMapElement = attackingAgent;
            crab.TargetLocation = attackingAgent.Location;
            
            int healthQuantity = TraitsUtil.GetHealth(crab);

            int attackStrength = TraitsUtil.GetRandomAttackStrength(attackingAgent);
            int defenseStrength = TraitsUtil.GetRandomDefenseStrength(crab);

            int healthDecrement = attackStrength - defenseStrength;
            if (healthDecrement > 0)
            {
                healthQuantity -= healthDecrement;
                TraitsUtil.SetHealth(crab, healthQuantity);
            }

            if (healthQuantity <= 0)
            {
                crab.Description = "Killed by " + attackingAgent.DisplayName;
                crab.HandleTransition(onDeathTransition);
            }
            else
            {
                crab.Description = "Attacked by " + attackingAgent.DisplayName;
                crab.HandleTransition(onAttackedTransition);
            }
        }

        public static ICommand Create(ICrab crab, string onAttackedTransition = "", string onDeathTransition = "")
        {
            return new CrabAttackHandler
            {
                crab = crab,
                onAttackedTransition = onAttackedTransition,
                onDeathTransition = onDeathTransition
            };
        }
    }
}
