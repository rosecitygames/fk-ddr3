using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A command that destroys a given game object.
    /// </summary>
    public class DestroyGameObject : AbstractCommand
    {
        GameObject gameObject = null;

        protected override void OnStart()
        {
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
            }
            
            Complete();
        }

        public static ICommand Create(GameObject gameObject)
        {
            return new DestroyGameObject
            {
                gameObject = gameObject
            };
        }
    }
}
