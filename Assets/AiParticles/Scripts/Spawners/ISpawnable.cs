using UnityEngine;

namespace IndieDevTools.Spawners
{
    /// <summary>
    /// An interface for objects that can spawn instances.
    /// </summary>
    public interface ISpawnable
    {
        /// <summary>
        /// Spawns an instance.
        /// </summary>
        /// <param name="localPosition">The local position of the spawned instance</param>
        /// <param name="localScale">The local scale of the spawned instance.</param>
        /// <param name="rotation">The local z euler rotation of the spawned instance.</param>
        /// <returns>The spawned instance</returns>
        GameObject Spawn(Vector3 localPosition, Vector3 localScale, float rotation = 0.0f);

        /// <summary>
        /// Spawns an instance.
        /// </summary>
        /// <param name="localPosition">The local position of the spawned instance</param>
        /// <param name="localScale">The local scale of the spawned instance.</param>
        /// <param name="rotation">The local z euler rotation of the spawned instance.</param>
        /// <param name="velocity">The velocity of the spawned instance.</param>
        /// <param name="angularVelocity">The angylar velocity of the spawned instance.</param>
        /// <param name="color">The sprite renderer color of the spawned instance.</param>
        /// <returns>The spawned instance</returns>
        GameObject Spawn(Vector3 localPosition, Vector3 localScale, float rotation, Vector2 velocity, float angularVelocity, Color color);
    }
}