using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;

namespace IndieDevTools.Agents
{
    /// <summary>
    /// A null implementation of agent data.
    /// </summary>
    public class NullAgentData : IAgentData
    {
        string IDescribable.DisplayName { get => ""; set { } }
        string IDescribable.Description { get => ""; set { } }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { } remove { } }

        ITraitCollection stats = new TraitCollection();
        List<ITrait> IStatsCollection.Stats { get => stats.Traits; }
        ITrait IStatsCollection.GetStat(string id) { return stats.GetTrait(id); }

        ITraitCollection desires = new TraitCollection();
        List<ITrait> IDesiresCollection.Desires { get => desires.Traits; }
        ITrait IDesiresCollection.GetDesire(string id) { return desires.GetTrait(id); }

        float IAdvertisementBroadcastData.BroadcastDistance { get => 0; }
        float IAdvertisementBroadcastData.BroadcastInterval { get => 0; }

        IAgentData ICopyable<IAgentData>.Copy() { return new NullAgentData(); }

        public static IAgentData Create() => new NullAgentData();
    }
}
