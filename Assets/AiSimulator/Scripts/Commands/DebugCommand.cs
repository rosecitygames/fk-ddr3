using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A command that sends a debug log message.
    /// </summary>
    public class DebugCommand : AbstractCommand
    {
        string message;

        protected override void OnStart()
        {
            Debug.Log(message);
            Complete();
        }

        public static ICommand Create(string message)
        {
            return new DebugCommand
            {
                message = message
            };
        }
    }
}
