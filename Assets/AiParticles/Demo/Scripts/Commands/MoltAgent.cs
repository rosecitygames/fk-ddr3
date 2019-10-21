using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Traits;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class MoltAgent : AbstractCommand
    {
        IMoltable moltable = null;

        protected override void OnStart()
        {
            moltable.Molt();
            Complete();
        }

        public static ICommand Create(IMoltable moltable)
        {
            return new MoltAgent
            {
                moltable = moltable
            };
        }
    }
}
