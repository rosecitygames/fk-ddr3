using UnityEngine;

namespace IndieDevTools.Maps
{
    /// <summary>
    /// Interface for objects with a Unity Vector3 position
    /// </summary>
    public interface IPositionable
    {
        Vector3 Position { get; set; }
    }
}
