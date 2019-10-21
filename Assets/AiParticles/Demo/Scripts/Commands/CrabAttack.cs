using IndieDevTools.Agents;
using IndieDevTools.Commands;
using UnityEngine;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    public class CrabAttack : AbstractCommand
    {
        IAgent agent = null;
        string onTargetKilledTransition;
        string onTargetEatenTranstion;

        protected override void OnStart()
        {
            agent.TargetAdvertisement = null;
            AttackTarget();
            Complete();
        }

        void AttackTarget()
        {
            if (agent.TargetMapElement == null)
            {
                CallTargetKilledTransition();
                return;
            }

            IAttackReceiver attackReceiver = agent.TargetMapElement as IAttackReceiver;
            if (attackReceiver == null)
            {
                CallTargetKilledTransition();
                return;
            }

            int targetHealth = TraitsUtil.GetHealth(agent.TargetMapElement);
            if (targetHealth <= 0)
            {
                int targetSize = TraitsUtil.GetSize(agent.TargetMapElement);
                if (targetSize > 1)
                {
                    CallTargetKilledTransition();
                }
                else
                {
                    float hunger = 1.0f;
                    bool isEating = Random.value <= hunger;
                    if (isEating)
                    {
                        agent.Description = "Ate " + agent.TargetMapElement.DisplayName;
                        agent.TargetMapElement.Description = "Eaten by " + agent.DisplayName;
                        CallTargetEatenTransition();
                    }
                    else
                    {
                        CallTargetKilledTransition();
                    }
                }
            }
            else
            {
                attackReceiver.ReceiveAttack(agent);
            }
        }

        void CallTargetKilledTransition()
        {
            if (string.IsNullOrEmpty(onTargetKilledTransition) == false)
            {
                agent.HandleTransition(onTargetKilledTransition);
            }
        }

        void CallTargetEatenTransition()
        {
            if (string.IsNullOrEmpty(onTargetEatenTranstion) == false)
            {
                agent.HandleTransition(onTargetEatenTranstion);
            }
        }

        public static ICommand Create(IAgent agent, string onTargetKilledTransition, string onTargetEatenTranstion)
        {
            return new CrabAttack
            {
                agent = agent,
                onTargetKilledTransition = onTargetKilledTransition,
                onTargetEatenTranstion = onTargetEatenTranstion
            };
        }
    }
}
