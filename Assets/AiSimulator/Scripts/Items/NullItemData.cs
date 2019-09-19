using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;

namespace IndieDevTools.Items
{
    /// <summary>
    /// A null implementation of item data.
    /// </summary>
    public class NullItemData : IItemData
    {
        string IDescribable.DisplayName { get => ""; set { } }
        string IDescribable.Description { get => ""; set { } }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { } remove { } }

        TraitCollection stats = new TraitCollection();
        IStatsCollection statsCollection => stats as IStatsCollection;
        List<ITrait> IStatsCollection.Stats => statsCollection.Stats;
        ITrait IStatsCollection.GetStat(string id) => statsCollection.GetStat(id);

        float IAdvertisementBroadcastData.BroadcastDistance => 0;
        float IAdvertisementBroadcastData.BroadcastInterval => 0;

        IItemData ICopyable<IItemData>.Copy() => new NullItemData();

        public static IItemData Create() => new NullItemData();
    }
}
