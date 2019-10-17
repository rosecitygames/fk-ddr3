using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface ICrab : IFootprint<ICrab>, IAgent, IAttackReceiver, ILandable, IExplodable, IMoltable
    {
        SpriteRenderer SpriteRenderer { get; }
    }
}
