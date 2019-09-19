using IndieDevTools.Agents;
using IndieDevTools.Commands;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that attacks an agent's target map element if that
    /// map element can receieve attacks.
    /// </summary>
    public class AttackTargetMapElement : AbstractCommand
    {
        IAgent agent = null;
        string onTargetKilledTransition;

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
                CallTargetKilledTransition();
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

        public static ICommand Create(IAgent agent, string onTargetKilledTransition)
        {
            return new AttackTargetMapElement
            {
                agent = agent,
                onTargetKilledTransition = onTargetKilledTransition
            };
        }
    }
}
