using IndieDevTools.Commands;
using IndieDevTools.Maps;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class Explode : AbstractCommand
    {
        IMapElement mapElement = null;
        IExplodable explodable = null;

        protected override void OnStart()
        {
            AddEventHandlers();
            mapElement.RemoveFromMap();
            explodable.Explode();
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

            ParticleSystem particleSystem = instance.GetComponentInChildren<ParticleSystem>();
            if (particleSystem == null) return;
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
