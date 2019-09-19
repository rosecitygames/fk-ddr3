using IndieDevTools.Maps;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An interface used by implementing classes to receive broadcasted advertisements.
    /// </summary>
    public interface IAdvertisementReceiver: IMapElement
    {
        void ReceiveAdvertisement(IAdvertisement advertisement);
    }
}

