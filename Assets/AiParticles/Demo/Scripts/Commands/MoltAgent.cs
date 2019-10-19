using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Traits;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class MoltAgent : AbstractCommand
    {
        IAgent agent = null;
        IMoltable moltable = null;

        protected override void OnStart()
        {
            ITrait sizeTrait = (agent as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);
            if (sizeTrait != null) moltable.Molt(sizeTrait.Quantity);

            Complete();
        }

        public static ICommand Create(IAgent agent, IMoltable moltable)
        {
            return new MoltAgent
            {
                agent = agent,
                moltable = moltable
            };
        }
    }
}
