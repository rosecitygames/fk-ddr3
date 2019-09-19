using IndieDevTools.Agents;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// Interface for an object that can receive attacks.
    /// </summary>
    public interface IAttackReceiver
    {
        void ReceiveAttack(IAgent attackingAgent);
    }
}
