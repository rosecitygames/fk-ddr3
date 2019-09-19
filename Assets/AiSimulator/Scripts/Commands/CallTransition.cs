using IndieDevTools.States;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A command that calls for a transition on a transition handler (usually a state machine).
    /// </summary>
    public class CallTransition : AbstractCommand
    {
        IStateTransitionHandler handler = null;
        string transition;

        protected override void OnStart()
        {
            if (handler != null && string.IsNullOrEmpty(transition) == false)
            {
                handler.HandleTransition(transition);
            }

            Complete();
        }

        public static ICommand Create(IStateTransitionHandler handler, string transition)
        {
            return new CallTransition
            {
                handler = handler,
                transition = transition
            };
        }
    }
}
