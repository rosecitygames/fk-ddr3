using System;

namespace IndieDevTools
{
    /// <summary>
    /// A generic interface for updating an object.
    /// </summary>
    /// <typeparam name="T">The type of object that is being updated</typeparam>
    public interface IUpdatable<T>
    {
        event Action<T> OnUpdated;
    }
}