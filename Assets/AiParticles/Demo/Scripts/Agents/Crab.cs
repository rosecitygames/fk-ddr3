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

        TriggerAnimator triggerAnimator = null;
        protected TriggerAnimator TriggerAnimator
        {
            get
            {
                InitAnimator();
                return triggerAnimator;
            }
        }

        ExplosionSpawner spawner;
        protected ExplosionSpawner Spawner
        {
            get
            {
                InitSpawner();
                return spawner;
            }
        }

        float IExplodable.MinExplosiveStrength => Spawner.SpriteExploder.MinExplosiveStrength;
        float IExplodable.MaxExplosiveStrength => Spawner.SpriteExploder.MaxExplosiveStrength;

        int IExplodable.MaxInstanceCount => Spawner.MaxInstanceCount;
        int IExplodable.InstanceCount => Spawner.InstanceCount;

        event Action<GameObject> IExplodable.OnInstanceCreated
        {
            add => Spawner.OnInstanceCreated += value;
            remove => Spawner.OnInstanceCreated -= value;
        }

        event Action IExplodable.OnCompleted
        {
            add => Spawner.OnCompleted += value;
            remove => Spawner.OnCompleted -= value;
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

        void InitSpawner()
        {
            if (spawner != null) return;
            spawner = GetComponentInChildren<ExplosionSpawner>();
            spawner.InstanceParent = transform.parent;
            //spawner.IsSpawningParticleColor = false;
        }

        void IMoltable.Molt() => Molter.Molt();
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

        void InitFootprint()
        {
            if (footprint != null) return;
            footprint = Footprint<ICrab>.Create(SpriteRenderer, this, SubCrab.Create);
        }

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
        }

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

        void IExplodable.Explode()
        {
            SpriteExploder.SpriteExploder spriteExploder = GetComponentInChildren<SpriteExploder.SpriteExploder>();
            if (spriteExploder == null) return;

            spriteExploder.Explode();
        }
                        
        // Command layer consts used for making the state machine setup more readable
        const int commandLayer0 = 0;
        const int commandLayer1 = 1;
        const int commandLayer2 = 2;
        const int commandLayer3 = 3;
        const int commandLayer4 = 4;
        const int commandLayer5 = 5;

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
            string onTargetFoundTransition = "OnTargetAdFound";
            string onAttackedTransition = "OnAttacked";
            string onEnemyKilledTransition = "OnEnemyKilled";
            string onExplodeTransition = "OnExplode";
            string onEnemyEatenTransition = "OnMolt";
            string onEnemeyFoundTransition = "OnEnemyFound";
            string onItemFoundTransition = "OnItemFound";
            string onNothingFoundTransition = "OnNothingFound";
            string onPickupCompleted = "OnPickupCompleted";

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
            wanderState.AddCommand(CrabAttackHandler.Create(this, onAttackedTransition, onExplodeTransition), commandLayer4);

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
