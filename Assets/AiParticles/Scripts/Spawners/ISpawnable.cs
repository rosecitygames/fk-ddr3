using UnityEngine;

namespace IndieDevTools.Spawners
{
    public interface ISpawnable
    {
        GameObject Spawn(Vector3 position, Vector3 scale, float rotation = 0.0f);
        GameObject Spawn(Vector3 position, Vector3 scale, float rotation, Vector2 velocity, float angularVelocity, Color color);
    }
}