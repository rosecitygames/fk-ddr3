using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public interface IFootprint<T>
    {
        List<T> AllFootprintElements{ get; }
        List<T> CornerFootprintElements{ get; }
        List<T> BorderFootprintElements{ get; }

        Vector2Int FootprintOffset { get; }
        Vector2Int FootprintSize { get; }
        Vector2Int FootprintExtents { get; }

        void Destroy();
    }
}
