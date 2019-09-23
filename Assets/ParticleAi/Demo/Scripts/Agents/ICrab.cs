﻿using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using System;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// Interface for an agent that can receive attacks and invoke an event that the attack was received.
    /// </summary>
    public interface ICrab : IAgent, IAttackReceiver { }
}
