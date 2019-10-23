using System;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// An explodable object.
    /// </summary>
    public interface IExplodable
    {
        /// <summary>
        /// An event for when an instance is created durring an explosion.
        /// </summary>
        event Action<GameObject> OnInstanceCreated;

        /// <summary>
        /// An event for when instance creation is completed after an explosion.
        /// </summary>
        event Action OnCompleted;

        /// <summary>
        /// The minimum explosive strength durring an explosion.
        /// </summary>
        float MinExplosiveStrength { get; }

        /// <summary>
        /// The maximum explosive strength durring an explosion.
        /// </summary>
        float MaxExplosiveStrength { get; }

        /// <summary>
        /// The maximum amount of instance that will be created durring an explosion.
        /// </summary>
        int MaxInstanceCount { get; }

        /// <summary>
        /// The current amount of instance created durring an explosion.
        /// </summary>
        int InstanceCount { get; }

        /// <summary>
        /// Explode the object.
        /// </summary>
        void Explode();
    }
}
