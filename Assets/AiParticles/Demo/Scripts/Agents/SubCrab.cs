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

        void IExplodable.Explode() => superCrab.Explode();

        // SubCrabs dont have a sub-footprint, so use a null implementation for IFootprint
        List<ICrab> IFootprint<ICrab>.AllFootprintElements => nullList;
        List<ICrab> IFootprint<ICrab>.CornerFootprintElements => nullList;
        List<ICrab> IFootprint<ICrab>.BorderFootprintElements => nullList;
        Vector2Int IFootprint<ICrab>.FootprintSize => Vector2Int.zero;
        Vector2Int IFootprint<ICrab>.FootprintExtents => Vector2Int.zero;
        Vector2Int IFootprint<ICrab>.FootprintOffset => Vector2Int.zero;
        void IFootprint<ICrab>.Destroy() {}

        List<ICrab> nullList = new List<ICrab>();

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
