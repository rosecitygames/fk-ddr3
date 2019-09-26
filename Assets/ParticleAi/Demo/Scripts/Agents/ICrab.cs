using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface ICrab : IFootprint<ICrab>, IAgent, ILandable, IAttackReceiver { }
}
