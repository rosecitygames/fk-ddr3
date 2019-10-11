using System;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface IExplodable
    {
        void Explode();
        event Action<GameObject> OnInstanceCreated;
        event Action OnCompleted;
    }
}
