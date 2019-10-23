using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Animation;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Exploders;
using IndieDevTools.Maps;
using IndieDevTools.Spawners;
using IndieDevTools.States;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// Concrete implementation of ICrab.
    /// </summary>
    public class Crab : AbstractAgent, ICrab
    {
        /// <summary>
        /// The name that will be displayed.
        /// </summary>
        protected override string DisplayName { get => displayName; set => displayName = value; }
        [SerializeField]
        string displayName = "";

        /// <summary>
        /// The minimum explosive strength when the crab explodes.
        /// </summary>
        float IExplodable.MinExplosiveStrength => Spawner.SpriteExploder.MinExplosiveStrength;

        /// <summary>
        /// The maximum explosive strength when the crab explodes.
        /// </summary>
        float IExplodable.MaxExplosiveStrength => Spawner.SpriteExploder.MaxExplosiveStrength;

        /// <summary>
        /// The total number of mini crabs that will be created when the crab explodes.
        /// </summary>
        int IExplodable.MaxInstanceCount => Spawner.MaxInstanceCount;

        /// <summary>
        /// The current number of active mini crabs that have been created durring the explosion.
        /// </summary>
        int IExplodable.InstanceCount => Spawner.InstanceCount;

        /// <summary>
        /// An event that gets invoked whenever a new mini crab has been created durring the explosion.
        /// </summary>
        event Action<GameObject> IExplodable.OnInstanceCreated
        {
            add => Spawner.OnInstanceCreated += value;
            remove => Spawner.OnInstanceCreated -= value;
        }

        /// <summary>
        /// An event that gets invoked when all the mini crabs have been created after an explosion.
        /// </summary>
        event Action IExplodable.OnCompleted
        {
            add => Spawner.OnCompleted += value;
            remove => Spawner.OnCompleted -= value;
        }

        /// <summary>
        /// Explodes the crab into mini crabs.
        /// </summary>
        void IExplodable.Explode() => SpriteExploder.Explode();

        // Footprint implementations that get passed to the Footprint object.
        List<ICrab> IFootprint<ICrab>.AllFootprintElements => Footprint.AllFootprintElements;
        List<ICrab> IFootprint<ICrab>.CornerFootprintElements => Footprint.CornerFootprintElements;
        List<ICrab> IFootprint<ICrab>.BorderFootprintElements => Footprint.BorderFootprintElements;
        Vector2Int IFootprint<ICrab>.FootprintSize => Footprint.FootprintSize;
        Vector2Int IFootprint<ICrab>.FootprintExtents => Footprint.FootprintExtents;
        Vector2Int IFootprint<ICrab>.FootprintOffset => Footprint.FootprintOffset;
        void IFootprint<ICrab>.Destroy() => Footprint.Destroy();

        /// <summary>
        /// Initializes the crab. Note, there are a few order dependencies. For example,
        /// trait initialization needs to happen after data, etc.
        /// </summary>
        protected override void Init()
        {
            InitData();
            InitAdvertiser();
            InitMapElement();
            InitSpriteRenderer();
            InitSpriteExploder();
            InitSpawner();
            InitFootprint();
            InitTraits();
            InitMolter();
            InitStateMachine();
        }

        /// <summary>
        /// Initialize the sprite renderer if it hasn't already.
        /// </summary>
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

        /// <summary>
        /// The crab's sprite renderer.
        /// </summary>
        SpriteRenderer ICrab.SpriteRenderer => SpriteRenderer;
        SpriteRenderer SpriteRenderer
        {
            get
            {
                InitSpriteRenderer();
                return spriteRenderer;
            }
        }
        SpriteRenderer spriteRenderer = null;

        /// <summary>
        /// The crab's sprite exploder.
        /// </summary>
        SpriteExploder SpriteExploder
        {
            get
            {
                InitSpriteExploder();
                return spriteExploder;
            }
        }
        SpriteExploder spriteExploder;

        const int spriteExploderSubdivisionCount = 2;

        /// <summary>
        /// Initialize the sprite exploder if hasn't been done already.
        /// </summary>
        void InitSpriteExploder()
        {
            if (spriteExploder != null) return;

            spriteExploder = GetComponentInChildren<SpriteExploder>();

            float spriteSizeX = spriteRenderer.sprite.bounds.size.x * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.x;
            float spriteSizeY = spriteRenderer.sprite.bounds.size.y * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.y;
            float minSpriteSize = Mathf.Min(spriteSizeX, spriteSizeY);
            int particlePixelSize = (int)(minSpriteSize / spriteExploderSubdivisionCount);

            spriteExploder.ParticlePixelSize = particlePixelSize;
        }

        /// <summary>
        /// The crab's spawner. Used for exploding and molting.
        /// </summary>
        protected ExplosionSpawner Spawner
        {
            get
            {
                InitSpawner();
                return spawner;
            }
        }
        ExplosionSpawner spawner;

        /// <summary>
        /// Initialize the crab's spawner object.
        /// </summary>
        void InitSpawner()
        {
            if (spawner != null) return;
            spawner = GetComponentInChildren<ExplosionSpawner>();
            spawner.InstanceParent = transform.parent;
        }

        /// <summary>
        /// Molt the crab to a bigger size.
        /// </summary>
        void IMoltable.Molt() => Molter.Molt();

        /// <summary>
        /// The crab's molter object.
        /// </summary>
        IMoltable Molter
        {
            get
            {
                InitMolter();
                return molter;
            }
        }

        IMoltable molter = null;

        void InitMolter()
        {
            if (molter != null) return;
            molter = AgentMolter.Create(this, Spawner);
        }

        /// <summary>
        /// The crab's footprint. This lets bigger crabs occupy more than one tile.
        /// </summary>
        IFootprint<ICrab> Footprint
        {
            get
            {
                InitFootprint();
                return footprint;
            }
        }
        IFootprint<ICrab> footprint;

        /// <summary>
        /// Initialize the crab's footprint if it hasn't been done already.
        /// </summary>
        void InitFootprint()
        {
            if (footprint != null) return;
            footprint = Footprint<ICrab>.Create(SpriteRenderer, this, SubCrab.Create);
        }

        /// <summary>
        /// Initialize the crab's traits.
        /// </summary>
        void InitTraits()
        {
            name = DisplayName;

            float spriteSizeX = spriteRenderer.sprite.bounds.size.x * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.x;
            float spriteSizeY = spriteRenderer.sprite.bounds.size.y * spriteRenderer.sprite.pixelsPerUnit * spriteRenderer.transform.lossyScale.y;

            float sizeX = spriteSizeX / spriteExploder.MinParticlePixelSize;
            float sizeY = spriteSizeY / spriteExploder.MinParticlePixelSize;

            int size = Mathf.FloorToInt(sizeX * sizeY);

            ITrait sizeTrait = (this as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);
            sizeTrait.Quantity = size;

            float explosionMultiplier =  2.0f - (float)sizeTrait.Quantity / (sizeTrait.Max - 1.0f);
            spriteExploder.MinExplosiveStrength *= explosionMultiplier;
            spriteExploder.MaxExplosiveStrength *= explosionMultiplier;

            ITrait healthTrait = (this as IStatsCollection).GetStat(TraitsUtil.healthTraitId);
            int health;

            if (healthTrait.Quantity <= 0)
            {
                health = Mathf.CeilToInt(sizeTrait.Quantity * 0.5f);        
            }
            else
            {
                health = healthTrait.Quantity;
            }

            health = Mathf.Max(3, health);
            healthTrait.Quantity = health;

            Description = "(HP: " + healthTrait.Quantity + ")";
        }

        /// <summary>
        /// The crab's trigger animator.
        /// </summary>
        protected TriggerAnimator TriggerAnimator
        {
            get
            {
                InitTriggerAnimator();
                return triggerAnimator;
            }
        }
        TriggerAnimator triggerAnimator = null;

        /// <summary>
        /// Initialize the crab's trigger animator if hasn't been done already.
        /// </summary>
        protected virtual void InitTriggerAnimator()
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

        /// <summary>
        /// Initialize the crab's state machine.
        /// </summary>
        protected override void InitStateMachine()
        {
            // Commandable state objects
            CommandableState wanderState = CommandableState.Create("Wander");
            stateMachine.AddState(wanderState);

            CommandableState inspectTargetLocationState = CommandableState.Create("InspectTargetLocation");
            stateMachine.AddState(inspectTargetLocationState);

            CommandableState attackEnemyState = CommandableState.Create("AttackEnemyState");
            stateMachine.AddState(attackEnemyState);

            CommandableState pickupItemState = CommandableState.Create("PickupItem");
            stateMachine.AddState(pickupItemState);

            CommandableState explodeState = CommandableState.Create("ExplodeState");
            stateMachine.AddState(explodeState);

            CommandableState moltState = CommandableState.Create("MoltState");
            stateMachine.AddState(moltState);

            // Transitions strings
            const string onTargetFoundTransition = "OnTargetAdFound";
            const string onAttackedTransition = "OnAttacked";
            const string onEnemyKilledTransition = "OnEnemyKilled";
            const string onExplodeTransition = "OnExplode";
            const string onEnemyEatenTransition = "OnMolt";
            const string onEnemeyFoundTransition = "OnEnemyFound";
            const string onItemFoundTransition = "OnItemFound";
            const string onNothingFoundTransition = "OnNothingFound";
            const string onPickupCompleted = "OnPickupCompleted";

            // Command layer consts used for making the state machine setup more readable
            const int commandLayer0 = 0;
            const int commandLayer1 = 1;
            const int commandLayer2 = 2;
            const int commandLayer3 = 3;
            const int commandLayer4 = 4;

            // Wander State                       
            wanderState.AddTransition(onTargetFoundTransition, inspectTargetLocationState);
            wanderState.AddTransition(onEnemeyFoundTransition, attackEnemyState);
            wanderState.AddTransition(onItemFoundTransition, pickupItemState);
            wanderState.AddTransition(onAttackedTransition, attackEnemyState);
            wanderState.AddTransition(onExplodeTransition, explodeState);
            wanderState.AddCommand(ChooseCrabLocation.Create(this), commandLayer0);
            wanderState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Walk), commandLayer0);
            wanderState.AddCommand(MoveToTargetLocation.Create(this), commandLayer0);
            wanderState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            wanderState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer0);
            wanderState.SetLayerLoopCount(commandLayer0, -1); // Instead of just stopping, layers can be assigned a number of lopps. -1 is infinite looping.
            wanderState.AddCommand(FindTargetInFootprint<ICrab>.Create(this, Footprint), commandLayer1);
            wanderState.AddCommand(InspectTargetMapElement.Create(this, onEnemeyFoundTransition, onItemFoundTransition, onNothingFoundTransition), commandLayer1);
            wanderState.AddCommand(WaitForRandomTime.Create(this, 0.1f, 0.8f), commandLayer2);
            wanderState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint), commandLayer2);
            wanderState.AddCommand(AdvertisementHandler.Create(this), commandLayer3);
            wanderState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onExplodeTransition), commandLayer4);

            // Inspect Target Location State
            inspectTargetLocationState.AddTransition(onEnemeyFoundTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onItemFoundTransition, pickupItemState);
            inspectTargetLocationState.AddTransition(onNothingFoundTransition, wanderState);
            inspectTargetLocationState.AddTransition(onAttackedTransition, attackEnemyState);
            inspectTargetLocationState.AddTransition(onExplodeTransition, explodeState);
            inspectTargetLocationState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Walk), commandLayer0);
            inspectTargetLocationState.AddCommand(MoveToTargetLocation.Create(this), commandLayer0);
            inspectTargetLocationState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            inspectTargetLocationState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer0);
            inspectTargetLocationState.AddCommand(CallTransition.Create(this, onNothingFoundTransition), commandLayer0);
            inspectTargetLocationState.AddCommand(FindTargetInFootprint<ICrab>.Create(this, Footprint), commandLayer1);
            inspectTargetLocationState.AddCommand(InspectTargetMapElement.Create(this, onEnemeyFoundTransition, onItemFoundTransition, onNothingFoundTransition), commandLayer1);
            inspectTargetLocationState.AddCommand(WaitForRandomTime.Create(this, 0.1f, 0.8f), commandLayer2);
            inspectTargetLocationState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint), commandLayer2);
            inspectTargetLocationState.AddCommand(AdvertisementHandler.Create(this), commandLayer3);
            inspectTargetLocationState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onExplodeTransition), commandLayer4);

            // Attack Enemey state
            attackEnemyState.AddTransition(onEnemyKilledTransition, wanderState);
            attackEnemyState.AddTransition(onTargetFoundTransition, inspectTargetLocationState);
            attackEnemyState.AddTransition(onExplodeTransition, explodeState);
            attackEnemyState.AddTransition(onEnemyEatenTransition, moltState);
            attackEnemyState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Fight), commandLayer3);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.1f, 0.2f), commandLayer3);
            attackEnemyState.AddCommand(CrabAttack.Create(this, onEnemyKilledTransition, onEnemyEatenTransition), commandLayer3);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 1.0f), commandLayer3);
            attackEnemyState.SetLayerLoopCount(commandLayer3, -1);
            attackEnemyState.AddCommand(WaitForRandomTime.Create(this, 0.2f, 0.8f), commandLayer1);
            attackEnemyState.AddCommand(BroadcastFootprintAdvertisement<ICrab>.Create(this, Footprint), commandLayer1);
            attackEnemyState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onExplodeTransition), commandLayer2);

            // Pickup Item state
            pickupItemState.AddTransition(onPickupCompleted, wanderState);
            pickupItemState.AddTransition(onAttackedTransition, attackEnemyState);
            pickupItemState.AddTransition(onExplodeTransition, explodeState);
            pickupItemState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Idle), commandLayer0);
            pickupItemState.AddCommand(PickupItem.Create(this), commandLayer0);
            pickupItemState.AddCommand(WaitForRandomTime.Create(this, 0.5f, 0.1f), commandLayer0);
            pickupItemState.AddCommand(CallTransition.Create(this, onPickupCompleted), commandLayer0);
            pickupItemState.AddCommand(AttackHandler.Create(this, this, onAttackedTransition, onExplodeTransition), commandLayer1);

            // Explode state
            explodeState.AddCommand(TriggerAnimation.Create(TriggerAnimator, CrabAnimationTrigger.Explode));
            explodeState.AddCommand(ExplodeAgent.Create(this, this));
            explodeState.AddCommand(DestroyGameObject.Create(gameObject));

            // Molt state
            moltState.AddCommand(MoltAgent.Create(this));

            stateMachine.SetState(wanderState);
        }

        /// <summary>
        /// Invokes the attack received event.
        /// </summary>
        /// <param name="attackingAgent">The attacking agent</param>
        void IAttackReceiver.ReceiveAttack(IAgent attackingAgent)
        {
            OnAttackReceived?.Invoke(attackingAgent);
        }

        /// <summary>
        /// Implementation for adding and remove the OnAttackReceived event.
        /// </summary>
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

        /// <summary>
        /// The action for when the crab is attacked by another agent.
        /// </summary>
        Action<IAgent> OnAttackReceived;

        /// <summary>
        /// Remove the crab from its map. Will also destroy its footprint.
        /// </summary>
        protected override void RemoveFromMap()
        {
            base.RemoveFromMap();

            Footprint.Destroy();

            StopAllCoroutines();
            isDrawingRuntimeGizmos = false;
        }
    }
}
