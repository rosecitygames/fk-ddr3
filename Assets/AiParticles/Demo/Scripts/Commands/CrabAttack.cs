using IndieDevTools.Agents;
using IndieDevTools.Commands;
using UnityEngine;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that makes an agent attack its target attack receiver.
    /// </summary>
    public class CrabAttack : AbstractCommand
    {
        /// <summary>
        /// The agent that will be doing the attacking
        /// </summary>
        IAgent agent = null;

        /// <summary>
        /// The transition that will be called when the target is killed after the attack.
        /// </summary>
        string onTargetKilledTransition;

        /// <summary>
        /// The transition that will be called when the target is eaten after the attack.
        /// </summary>
        string onTargetEatenTranstion;

        /// <summary>
        /// Clears the target advertisement and then attacks the target.
        /// </summary>
        protected override void OnStart()
        {
            agent.TargetAdvertisement = null;
            AttackTarget();
            Complete();
        }

        /// <summary>
        /// Attack the target attack receiever and then call the relevant transition.
        /// </summary>
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

        /// <summary>
        /// Create a command object.
        /// </summary>
        /// <param name="agent">The agent that will be doing the attack</param>
        /// <param name="onTargetKilledTransition">A transition that will be called if the target was killed</param>
        /// <param name="onTargetEatenTranstion">A transition that will be called if the target is eaten.</param>
        /// <returns></returns>
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
