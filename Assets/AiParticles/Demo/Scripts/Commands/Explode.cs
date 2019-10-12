using IndieDevTools.Commands;
using IndieDevTools.Maps;
using IndieDevTools.Demo.BattleSimulator;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class Explode : AbstractCommand
    {
        IMapElement mapElement = null;
        IExplodable explodable = null;

        int instanceAttackStrength = 0;
        int instanceDefenseStrength = 0;

        float instanceMinExplosiveStrength = 0.0f;
        float instanceMaxExplosiveStrength = 0.0f;

        protected override void OnStart()
        {
            InitStats();
            InitSpriteExploder();
            AddEventHandlers();
            mapElement.RemoveFromMap();
            explodable.Explode();
        }

        void InitStats()
        {
            instanceAttackStrength = TraitsUtil.GetAttackStrength(mapElement);
            instanceDefenseStrength = TraitsUtil.GetDefenseStrength(mapElement);

            instanceAttackStrength /= 2;
            instanceDefenseStrength /= 2;
            
            instanceAttackStrength = Mathf.Max(1, instanceAttackStrength);
            instanceDefenseStrength = Mathf.Max(1, instanceDefenseStrength);
        }

        void InitSpriteExploder()
        {
            instanceMinExplosiveStrength = explodable.MinExplosiveStrength * 2;
            instanceMaxExplosiveStrength = explodable.MaxExplosiveStrength * 2;
        }

        void AddEventHandlers()
        {
            RemoveEventHandlers();
            explodable.OnInstanceCreated += Explodable_OnInstanceCreated;
            explodable.OnCompleted += Explodable_OnCompleted;
        }

        void RemoveEventHandlers()
        {
            explodable.OnInstanceCreated -= Explodable_OnInstanceCreated;
            explodable.OnCompleted -= Explodable_OnCompleted;
        }

        private void Explodable_OnCompleted()
        {
            RemoveEventHandlers();
            Complete();
        }

        private void Explodable_OnInstanceCreated(GameObject instance)
        {
            IMapElement instanceMapElement = instance.GetComponentInChildren<IMapElement>();
            if (instanceMapElement == null) return;

            instanceMapElement.GroupId = mapElement.GroupId;
            instanceMapElement.DisplayName = mapElement.DisplayName;

            TraitsUtil.SetAttackStrength(instanceMapElement, instanceAttackStrength);
            TraitsUtil.SetDefenseStrength(instanceMapElement, instanceDefenseStrength);

            SpriteExploder.SpriteExploder spriteExploder = instance.GetComponentInChildren<SpriteExploder.SpriteExploder>();
            if (spriteExploder == null) return;
            spriteExploder.MinExplosiveStrength = instanceMinExplosiveStrength;
            spriteExploder.MaxExplosiveStrength = instanceMaxExplosiveStrength;
        }

        public static ICommand Create(IMapElement mapElement, IExplodable explodable)
        {
            return new Explode
            {
                mapElement = mapElement,
                explodable = explodable
            };
        }
    }
}
