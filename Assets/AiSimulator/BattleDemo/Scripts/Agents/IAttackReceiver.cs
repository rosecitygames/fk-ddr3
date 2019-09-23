using IndieDevTools.Agents;
using System;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// Interface for an object that can receive attacks.
    /// </summary>
    public interface IAttackReceiver
    {
        void ReceiveAttack(IAgent attackingAgent);
        event Action<IAgent> OnAttackReceived;
    }
}
