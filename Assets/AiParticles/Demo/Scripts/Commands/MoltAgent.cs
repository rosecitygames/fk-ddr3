using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Traits;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that calls a moltable object's molt method.
    /// </summary>
    public class MoltAgent : AbstractCommand
    {
        /// <summary>
        /// The moltable object to be molted.
        /// </summary>
        IMoltable moltable = null;

        /// <summary>
        /// Call the moltable object molt method and complete the command.
        /// </summary>
        protected override void OnStart()
        {
            moltable.Molt();
            Complete();
        }

        /// <summary>
        /// Create a command object.
        /// </summary>
        /// <param name="moltable">The moltable object to be molted.</param>
        /// <returns>The command object</returns>
        public static ICommand Create(IMoltable moltable)
        {
            return new MoltAgent
            {
                moltable = moltable
            };
        }
    }
}
