using IndieDevTools.Advertisements;
using IndieDevTools.Traits;

namespace IndieDevTools.Items
{
    /// <summary>
    /// The item data interface used to describe a collection of stat traits and how to broadcast them.
    /// </summary>
    public interface IItemData : ICopyable<IItemData>, IDescribable, IStatsCollection, IAdvertisementBroadcastData { }
}