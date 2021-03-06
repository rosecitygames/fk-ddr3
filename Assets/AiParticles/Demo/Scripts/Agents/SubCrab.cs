﻿using IndieDevTools.Agents;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A crab footprint's sub map element that allows it to occupy more than one cell on a
    /// grid map. Implementations are passed on to the main (super) crab map element.
    /// </summary>
    public class SubCrab : AbstractSubAgent, ICrab
    {
        ICrab superCrab;

        protected override IAgent SuperAgent => superCrab;

        Vector2Int cellOffset;
        protected override Vector2Int CellOffset => cellOffset;

        void IAttackReceiver.ReceiveAttack(IAgent attackingAgent) => superCrab.ReceiveAttack(attackingAgent);
        event Action<IAgent> IAttackReceiver.OnAttackReceived { add { superCrab.OnAttackReceived += value; } remove { superCrab.OnAttackReceived -= value; } }

        void IExplodable.Explode() => superCrab.Explode();

        float IExplodable.MinExplosiveStrength => superCrab.MinExplosiveStrength;
        float IExplodable.MaxExplosiveStrength => superCrab.MaxExplosiveStrength;
        int IExplodable.MaxInstanceCount => superCrab.MaxInstanceCount;
        int IExplodable.InstanceCount => superCrab.InstanceCount;

        event Action<GameObject> IExplodable.OnInstanceCreated
        {
            add => superCrab.OnInstanceCreated += value;
            remove => superCrab.OnInstanceCreated -= value;
        }

        event Action IExplodable.OnCompleted
        {
            add => superCrab.OnCompleted += value;
            remove => superCrab.OnCompleted -= value;
        }

        void IMoltable.Molt() => superCrab.Molt();

        // SubCrabs dont have a sub-footprint, so use a null implementation for IFootprint
        List<ICrab> IFootprint<ICrab>.AllFootprintElements => nullList;
        List<ICrab> IFootprint<ICrab>.CornerFootprintElements => nullList;
        List<ICrab> IFootprint<ICrab>.BorderFootprintElements => nullList;
        Vector2Int IFootprint<ICrab>.FootprintSize => Vector2Int.zero;
        Vector2Int IFootprint<ICrab>.FootprintExtents => Vector2Int.zero;
        Vector2Int IFootprint<ICrab>.FootprintOffset => Vector2Int.zero;
        void IFootprint<ICrab>.Destroy() {}

        List<ICrab> nullList = new List<ICrab>();

        SpriteRenderer ICrab.SpriteRenderer => superCrab.SpriteRenderer;

        /// <summary>
        /// Create a sub-crab.
        /// </summary>
        /// <param name="superCrab">The main crab that implementations will be passed on to.</param>
        /// <param name="cellOffset">The amoun the sub-crab is offset from the main crab map element.</param>
        /// <returns></returns>
        public static ICrab Create(ICrab superCrab, Vector2Int cellOffset)
        {
            SubCrab subCrab = new SubCrab
            {
                superCrab = superCrab,
                cellOffset = cellOffset
            };

            subCrab.Init();

            return subCrab;
        }
    }
}
