using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Maps;
using IndieDevTools.States;
using IndieDevTools.Traits;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    public class SubCrab : AbstractSubAgent, ICrab
    {
        ICrab superCrab;
        protected override IAgent SuperAgent => superCrab;

        Vector2Int cellOffset;
        protected override Vector2Int CellOffset => cellOffset;

        void IAttackReceiver.ReceiveAttack(IAgent attackingAgent) => superCrab.ReceiveAttack(attackingAgent);
        event Action<IAgent> IAttackReceiver.OnAttackReceived { add { superCrab.OnAttackReceived += value; } remove { superCrab.OnAttackReceived -= value; } }

        bool ILandable.GetIsLandable(IAgent agent) => superCrab.GetIsLandable(agent);

        public static ICrab Create(ICrab superCrab, Vector2Int cellOffset)
        {
            return new SubCrab
            {
                superCrab = superCrab,
                cellOffset = cellOffset
            };
        }
    }
}
