using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using IndieDevTools.States;

namespace IndieDevTools.Items
{
    /// <summary>
    /// An interface for item map elements.
    /// </summary>
    public interface IItem : IAdvertisingMapElement, IStateTransitionHandler
    {
        IItemData Data { get; set; }
    }
}