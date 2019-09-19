using System.Collections.Generic;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A themed collection used to define stat traits.
    /// </summary>
    public interface IStatsCollection
    {
        List<ITrait> Stats { get; }
        ITrait GetStat(string id);
    }
}
