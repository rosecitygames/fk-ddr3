using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A serialized data class that couples a prefab
    /// with an odds percentage.
    /// </summary>
    [System.Serializable]
    public class MapGeneratorElementData
    {
        [SerializeField]
        GameObject prefab = null;
        public GameObject Prefab => prefab;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float perecentage = 0.0f;
        public float Percentage => perecentage;
    }
}
