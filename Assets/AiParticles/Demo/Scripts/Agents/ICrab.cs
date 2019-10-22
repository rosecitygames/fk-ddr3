using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Maps;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface ICrab : IFootprint<ICrab>, IAgent, IAttackReceiver, IExplodable, IMoltable
    {
        SpriteRenderer SpriteRenderer { get; }
    }
}
