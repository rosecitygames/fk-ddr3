using UnityEngine;

namespace IndieDevTools.Commands
{
    public class DestroyGameObject : AbstractCommand
    {
        GameObject gameObject = null;

        protected override void OnStart()
        {
            GameObject.Destroy(gameObject);
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
