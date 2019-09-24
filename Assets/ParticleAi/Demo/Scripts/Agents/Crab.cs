﻿using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Maps;
using IndieDevTools.States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Demo.BattleSimulator;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// Concrete implementation of ISoldier. In particular,
    /// the setup of the command based state machine.
    /// </summary>
    public class Crab : AbstractAgent, ICrab
    {
        [SerializeField]
        string displayName = "";
        protected override string DisplayName { get => displayName; set => displayName = value; }

        SpriteRenderer spriteRenderer = null;

        List<IMapElement> footprintElements = new List<IMapElement>();

        protected override void Init()
        {
            base.Init();
            InitSpriteRenderer();
            AddLocationEventHandlers();
            InitFootprint();
        }

        void InitSpriteRenderer()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = SortingOrder;
            }
        }

        void AddLocationEventHandlers()
        {
            RemoveLcoationEventHandlers();
            (this as ILocatable).OnUpdated += Location_OnUpdated;
        }

        void RemoveLcoationEventHandlers()
        {
            (this as ILocatable).OnUpdated -= Location_OnUpdated;
        }

        private void Location_OnUpdated(ILocatable obj)
        {
            UpdateFootprint();
        }

        void InitFootprint()
        {
            footprintElements.Clear();

            Vector2Int boundsMin = MapElement.Map.LocalToCell(spriteRenderer.bounds.min);
            Vector2Int boundsMax = MapElement.Map.LocalToCell(spriteRenderer.bounds.max);

            int rows = boundsMax.x - boundsMin.x;
            int columns = boundsMax.y - boundsMin.y;

            for(int column = 0; column < columns; column++)
            {
                for(int row = 0; row < rows; row++)
                {
                    //IMapElement footprintElement = 
                    //footprintElements.Add(new Vector2Int(column, row));
                }
            }

            UpdateFootprint();
        }

        void UpdateFootprint()
        {

        }


        // Command layer consts used for making the state machine setup more readable
        const int CommandLayer0 = 0;
        const int CommandLayer1 = 1;
        const int CommandLayer2 = 2;
        const int CommandLayer3 = 3;

        protected override void InitStateMachine()
        {
            // Commandable state objects
            CommandableState wanderState = CommandableState.Create("Wander");
            stateMachine.AddState(wanderState);

            CommandableState inspectTargetLocationState = CommandableState.Create("InspectTargetLocation");
            stateMachine.AddState(inspectTargetLocationState);

            CommandableState attackEnemyState = CommandableState.Create("AttackEnemyState");
            stateMachine.AddState(attackEnemyState);

            CommandableState deathState = CommandableState.Create("DeathState");
            stateMachine.AddState(deathState);

            CommandableState pickupItemState = CommandableState.Create("PickupItem");
            stateMachine.AddState(pickupItemState);

            // Transitions strings
            string onTargetFoundTransition = "OnTargetAdFound";
            string onAttackedTransition = "OnAttacked";
            string onEnemyKilledTransition = "OnEnemyKilled";
            string onDeathTransition = "OnDeath";
            string onEnemeyFoundTransition = "OnEnemyFound";
            string onItemFoundTransition = "OnItemFound";
            string onNothingFoundTransition = "OnNothingFound";
            string onPickupCompleted = "OnPickupCompleted";

            // Wander State                       
            wanderState.AddTransition(onTargetFoundTransition, inspectTargetLocationState);
            wanderState.AddTransition(onAttackedTransition, attackEnemyState);
            wanderState.AddTransition(onDeathTransition, deathState);
            wanderState.AddCommand(ChooseNewLocation.Create(this), CommandLayer0);
            wanderState.AddCommand(MoveToTargetLocation.Create(this), CommandLayer0);
            wanderState.AddCommand(WaitForRandomTime.Create(this, 0.25f, 0.5f), CommandLayer0);
            wanderState.SetLayerLoopCount(CommandLayer0, -1); // Instead of just stopping, layers can be assigned a number of lopps. -1 is infinite looping.
            wanderState.AddCommand(BroadcastAdvertisement.Create(this), CommandLayer1);
            wanderState.AddCommand(AdvertisementHandler.Create(this, onTargetFoundTransition), CommandLayer2);
            wanderState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), CommandLayer3);

            // Inspect Target Location State
            inspectTargetLocationState.AddTransition(onEnemeyFoundTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onItemFoundTransition, pickupItemState);
            inspectTargetLocationState.AddTransition(onNothingFoundTransition, wanderState);
            inspectTargetLocationState.AddTransition(onAttackedTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onDeathTransition, deathState);
            inspectTargetLocationState.AddCommand(MoveToTargetLocation.Create(this), CommandLayer0);
            inspectTargetLocationState.AddCommand(WaitForRandomTime.Create(this, 0.25f, 0.5f), CommandLayer0);
            inspectTargetLocationState.AddCommand(ChooseTargetMapElmentAtLocation.Create(this), CommandLayer0);
            inspectTargetLocationState.AddCommand(InspectTargetMapElement.Create(this, onEnemeyFoundTransition, onItemFoundTransition, onNothingFoundTransition), CommandLayer0);
            inspectTargetLocationState.AddCommand(BroadcastAdvertisement.Create(this), CommandLayer1);
            inspectTargetLocationState.AddCommand(AdvertisementHandler.Create(this), CommandLayer2);
            inspectTargetLocationState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), CommandLayer3);

            // Attack Enemey state
            attackEnemyState.AddTransition(onEnemyKilledTransition, wanderState);
            attackEnemyState.AddTransition(onDeathTransition, deathState);
            attackEnemyState.AddCommand(OffsetPositionFromTargetMapElement.Create(this), CommandLayer0);
            attackEnemyState.AddCommand(AttackTargetMapElement.Create(this, onEnemyKilledTransition), CommandLayer0);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 0.75f), CommandLayer0);
            attackEnemyState.SetLayerLoopCount(CommandLayer0, -1);
            attackEnemyState.AddCommand(BroadcastAdvertisement.Create(this), CommandLayer1);
            attackEnemyState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), CommandLayer2);

            // Death state
            deathState.AddCommand(Die.Create(this));

            // Pickup Item state
            pickupItemState.AddTransition(onPickupCompleted, wanderState);
            pickupItemState.AddTransition(onAttackedTransition, attackEnemyState);
            pickupItemState.AddTransition(onDeathTransition, deathState);
            pickupItemState.AddCommand(PickupItem.Create(this));
            pickupItemState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 0.1f), CommandLayer0);
            pickupItemState.AddCommand(CallTransition.Create(this, onPickupCompleted), CommandLayer0);
            pickupItemState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), CommandLayer1);

            stateMachine.SetState(wanderState);
        }

        void IAttackReceiver.ReceiveAttack(IAgent attackingAgent)
        {
            OnAttackReceived?.Invoke(attackingAgent);
        }

        event Action<IAgent> IAttackReceiver.OnAttackReceived
        {
            add
            {
                OnAttackReceived += value;
            }
            remove
            {
                OnAttackReceived -= value;
            }
        }

        Action<IAgent> OnAttackReceived;

        protected override void RemoveFromMap()
        {
            base.RemoveFromMap();
            StopAllCoroutines();

            isDrawingRuntimeGizmos = false;

            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color deadColor = spriteRenderer.color;
                deadColor.a = 0.1f;
                spriteRenderer.color = deadColor;
            }
        }

        public static IAgent Create(GameObject gameObject, IAgentData agentData, IAdvertisementBroadcaster broadcaster)
        {
            IAgent agent = gameObject.AddComponent<Soldier>();
            agent.Data = agentData;
            agent.SetBroadcaster(broadcaster);
            return agent;
        }
    }
}
