using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface ICrab : IFootprint<ICrab>, IAgent, ILandable, IExplodable, IAttackReceiver
    {
        SpriteRenderer SpriteRenderer { get; }
    }
}
