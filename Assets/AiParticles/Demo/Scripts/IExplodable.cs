using System;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface IExplodable
    {
        event Action<GameObject> OnInstanceCreated;
        event Action OnCompleted;

        float MinExplosiveStrength { get; }
        float MaxExplosiveStrength { get; }

        int MaxInstanceCount { get; }
        int InstanceCount { get; }

        void Explode();
    }
}
