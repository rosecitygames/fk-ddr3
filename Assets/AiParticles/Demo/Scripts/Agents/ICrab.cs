using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Maps;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// An agent with a footprint that can attack, explode, and molt.
    /// </summary>
    public interface ICrab : IAgent, IFootprint<ICrab>, IAttackReceiver, IExplodable, IMoltable
    {
        /// <summary>
        /// The crab's sprite renderer.
        /// </summary>
        SpriteRenderer SpriteRenderer { get; }
    }
}
