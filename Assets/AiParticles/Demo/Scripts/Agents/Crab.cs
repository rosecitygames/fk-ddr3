using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.AiParticles;
using IndieDevTools.Animation;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.States;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        SpriteRenderer SpriteRenderer
        {
            get
            {
                InitSpriteRenderer();
                return spriteRenderer;
            }
        }
        SpriteRenderer spriteRenderer = null;

        TriggerAnimator triggerAnimator = null;
        protected TriggerAnimator TriggerAnimator
        {
            get
            {
                InitAnimator();
                return triggerAnimator;
            }
        }

        ExplodedInstantiator explodedInstantiator;
        protected ExplodedInstantiator ExplodedInstantiator
        {
            get
            {
                InitExplodedInstantiator();

                return explodedInstantiator;
            }
        }

        float IExplodable.MinExplosiveStrength => ExplodedInstantiator.SpriteExploder.MinExplosiveStrength;
        float IExplodable.MaxExplosiveStrength => ExplodedInstantiator.SpriteExploder.MaxExplosiveStrength;

        int IExplodable.MaxInstanceCount => ExplodedInstantiator.MaxInstanceCount;
        int IExplodable.InstanceCount => ExplodedInstantiator.InstanceCount;

        event Action<GameObject> IExplodable.OnInstanceCreated
        {
            add => ExplodedInstantiator.OnInstanceCreated += value;
            remove => ExplodedInstantiator.OnInstanceCreated -= value;
        }

        event Action IExplodable.OnCompleted
        {
            add => ExplodedInstantiator.OnCompleted += value;
            remove => ExplodedInstantiator.OnCompleted -= value;
        }

        List<ICrab> IFootprint<ICrab>.AllFootprintElements => Footprint.AllFootprintElements;
        List<ICrab> IFootprint<ICrab>.CornerFootprintElements => Footprint.CornerFootprintElements;
        List<ICrab> IFootprint<ICrab>.BorderFootprintElements => Footprint.BorderFootprintElements;
        Vector2Int IFootprint<ICrab>.FootprintSize => Footprint.FootprintSize;
        Vector2Int IFootprint<ICrab>.FootprintExtents => Footprint.FootprintExtents;
        Vector2Int IFootprint<ICrab>.FootprintOffset => Footprint.FootprintOffset;
        void IFootprint<ICrab>.Destroy() => Footprint.Destroy();

        IFootprint<ICrab> Footprint
        {
            get
            {
                InitFootprint();
                return footprint;
            }
        }
        IFootprint<ICrab> footprint;

        protected override void Init()
        {
            base.Init();
            InitSpriteRenderer();
            InitSpriteExploder();
            InitExplodedInstantiator();
            InitFootprint();
            InitSizeTrait();
        }

        void InitSpriteRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = SortingOrder;
                }
            }
        }

        const int spriteExploderSubdivisionCount = 2;

        SpriteExploder.SpriteExploder SpriteExploder
        {
            get
            {
                InitSpriteExploder();
                return spriteExploder;
            }
        }
        SpriteExploder.SpriteExploder spriteExploder;

        void InitSpriteExploder()
        {
            if (spriteExploder != null) return;

            spriteExploder = GetComponentInChildren<SpriteExploder.SpriteExploder>();

            float spriteSizeX = spriteRenderer.sprite.bounds.size.x * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.x;
            float spriteSizeY = spriteRenderer.sprite.bounds.size.y * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.y;
            float minSpriteSize = Mathf.Min(spriteSizeX, spriteSizeY);
            int particlePixelSize = (int)(minSpriteSize / spriteExploderSubdivisionCount);

            spriteExploder.ParticlePixelSize = particlePixelSize;
        }
        
        void InitExplodedInstantiator()
        {
            if (explodedInstantiator != null) return;
            explodedInstantiator = GetComponentInChildren<ExplodedInstantiator>();
            explodedInstantiator.InstanceParent = transform.parent;
        }

        void InitFootprint()
        {
            if (footprint != null) return;
            footprint = Footprint<ICrab>.Create(SpriteRenderer, this, SubCrab.Create);
        }

        void InitSizeTrait()
        {
            ITrait sizeTrait = (this as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);
            if (sizeTrait == null) return;

            float spriteSizeX = spriteRenderer.sprite.bounds.size.x * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.x;
            float spriteSizeY = spriteRenderer.sprite.bounds.size.y * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.y;

            float sizeX = spriteSizeX / spriteExploder.MinParticlePixelSize;
            float sizeY = spriteSizeY / spriteExploder.MinParticlePixelSize;

            int size = Mathf.FloorToInt(sizeX * sizeY);
            sizeTrait.Quantity = size;

            (sizeTrait as IUpdatable<ITrait>).OnUpdated += OnSizeTraitUpdated;

            //TraitsUtil.SetHealth(this, Mathf.CeilToInt(size * 0.1f)); // Causes stack overflow?
        }

        private void OnSizeTraitUpdated(ITrait sizeTrait)
        {
         //   Debug.Log("Size increased to " + sizeTrait.Quantity);
        }

        [Sirenix.OdinInspector.ShowInInspector]
        int Size => SizeTrait.Quantity;

        [Sirenix.OdinInspector.ShowInInspector]
        int MaxSize => SizeTrait.Max;

        ITrait SizeTrait => (this as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);

        [Sirenix.OdinInspector.ShowInInspector]
        int Health => TraitsUtil.GetHealth(this);

        protected virtual void InitAnimator()
        {
            if (triggerAnimator != null) return;

            triggerAnimator = GetComponentInChildren<TriggerAnimator>();
            if (triggerAnimator == null)
            {
                GameObject triggerAnimatorGameObject;

                SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    triggerAnimatorGameObject = spriteRenderer.gameObject;
                }
                else
                {
                    triggerAnimatorGameObject = gameObject;
                }

                triggerAnimator = triggerAnimatorGameObject.AddComponent<TriggerAnimator>();
            }
        }

        bool ILandable.GetIsLandable(IAgent agent)
        {
            return agent == (this as IAgent);
        }

        void IExplodable.Explode()
        {
            SpriteExploder.SpriteExploder spriteExploder = GetComponentInChildren<SpriteExploder.SpriteExploder>();
            if (spriteExploder == null) return;

            spriteExploder.Explode();
        }

        [SerializeField]
        GameObject TilePrefab = null;
        
        // Command layer consts used for making the state machine setup more readable
        const int commandLayer0 = 0;
        const int commandLayer1 = 1;
        const int commandLayer2 = 2;
        const int commandLayer3 = 3;
        const int commandLayer4 = 4;

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
            wanderState.AddTransition(onEnemeyFoundTransition, attackEnemyState);
            wanderState.AddTransition(onItemFoundTransition, pickupItemState);
            wanderState.AddTransition(onAttackedTransition, attackEnemyState);
            wanderState.AddTransition(onDeathTransition, deathState);
            wanderState.AddCommand(ChooseLandableLocation.Create(this), commandLayer0);
            wanderState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Walk), commandLayer0);
            wanderState.AddCommand(MoveToTargetLocation.Create(this), commandLayer0);
            wanderState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            wanderState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer0);
            wanderState.SetLayerLoopCount(commandLayer0, -1); // Instead of just stopping, layers can be assigned a number of lopps. -1 is infinite looping.
            wanderState.AddCommand(FindTargetInFootprint<ICrab>.Create(this, Footprint), commandLayer1);
            wanderState.AddCommand(InspectTargetMapElement.Create(this, onEnemeyFoundTransition, onItemFoundTransition, onNothingFoundTransition), commandLayer1);
            wanderState.AddCommand(WaitForRandomTime.Create(this, 0.1f, 0.8f), commandLayer2);
            wanderState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint, TilePrefab), commandLayer2);
            wanderState.AddCommand(AdvertisementHandler.Create(this), commandLayer3);
            wanderState.AddCommand(CrabAttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), commandLayer4);

            // Inspect Target Location State
            inspectTargetLocationState.AddTransition(onEnemeyFoundTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onItemFoundTransition, pickupItemState);
            inspectTargetLocationState.AddTransition(onNothingFoundTransition, wanderState);
            inspectTargetLocationState.AddTransition(onAttackedTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onDeathTransition, deathState);
            inspectTargetLocationState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Walk), commandLayer0);
            inspectTargetLocationState.AddCommand(MoveToTargetLocation.Create(this), commandLayer0);
            inspectTargetLocationState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            inspectTargetLocationState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer0);
            inspectTargetLocationState.AddCommand(CallTransition.Create(this, onNothingFoundTransition), commandLayer0);
            inspectTargetLocationState.AddCommand(FindTargetInFootprint<ICrab>.Create(this, Footprint), commandLayer1);
            inspectTargetLocationState.AddCommand(InspectTargetMapElement.Create(this, onEnemeyFoundTransition, onItemFoundTransition, onNothingFoundTransition), commandLayer1);
            inspectTargetLocationState.AddCommand(WaitForRandomTime.Create(this, 0.1f, 0.8f), commandLayer2);
            inspectTargetLocationState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint, TilePrefab), commandLayer2);
            inspectTargetLocationState.AddCommand(AdvertisementHandler.Create(this), commandLayer3);
            inspectTargetLocationState.AddCommand(CrabAttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), commandLayer4);

            // Attack Enemey state
            attackEnemyState.AddTransition(onEnemyKilledTransition, wanderState);
            attackEnemyState.AddTransition(onDeathTransition, deathState);
            attackEnemyState.AddTransition(onTargetFoundTransition, inspectTargetLocationState);
            attackEnemyState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Fight), commandLayer0);
            attackEnemyState.AddCommand(AttackTargetMapElement.Create(this, onEnemyKilledTransition), commandLayer0);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 1.0f), commandLayer0);
            attackEnemyState.SetLayerLoopCount(commandLayer0, -1);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer1);
            attackEnemyState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint, TilePrefab), commandLayer1);
            attackEnemyState.AddCommand(CrabAttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), commandLayer2);

            // Death state
            deathState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Explode));
            deathState.AddCommand(Explode.Create(this, this));
            deathState.AddCommand(DestroyGameObject.Create(gameObject));

            // Pickup Item state
            pickupItemState.AddTransition(onPickupCompleted, wanderState);
            pickupItemState.AddTransition(onAttackedTransition, attackEnemyState);
            pickupItemState.AddTransition(onDeathTransition, deathState);
            pickupItemState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            pickupItemState.AddCommand(PickupItem.Create(this), commandLayer0);
            pickupItemState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 0.1f), commandLayer0);
            pickupItemState.AddCommand(CallTransition.Create(this, onPickupCompleted), commandLayer0);
            pickupItemState.AddCommand(CrabAttackHandler.Create(this, this, onAttackedTransition, onDeathTransition), commandLayer1);

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
            Footprint.Destroy();

            StopAllCoroutines();

            isDrawingRuntimeGizmos = false;
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
