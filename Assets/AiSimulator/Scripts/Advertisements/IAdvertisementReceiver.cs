using IndieDevTools.Maps;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An interface used by implementing classes to receive broadcasted advertisements.
    /// </summary>
    public interface IAdvertisementReceiver: IMapElement
    {
        /// <summary>
        /// Receive a given receiver.
        /// </summary>
        void ReceiveAdvertisement(IAdvertisement advertisement);
    }
}

