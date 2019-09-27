using IndieDevTools.Agents;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface ILandable
    {
        bool GetIsLandable(IAgent agent);
    }
}
