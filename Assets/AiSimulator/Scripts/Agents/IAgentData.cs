using IndieDevTools.Advertisements;
using IndieDevTools.Traits;

namespace IndieDevTools.Agents
{
    /// <summary>
    /// An interface for agent data. Includes trait collections, and data for broadcasting advertisements.
    /// </summary>
    public interface IAgentData : ICopyable<IAgentData>, IDescribable, IStatsCollection, IDesiresCollection, IAdvertisementBroadcastData { }
}
