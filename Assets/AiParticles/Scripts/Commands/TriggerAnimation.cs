using IndieDevTools.Animation;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A command that triggers an animation.
    /// </summary>
    public class TriggerAnimation : AbstractCommand
    {
        /// <summary>
        /// The trigger animator component.
        /// </summary>
        TriggerAnimator triggerAnimator = null;

        /// <summary>
        /// The trigger id
        /// </summary>
        int triggerNameHash;

        /// <summary>
        /// Override that triggers the animation and completes the command.
        /// </summary>
        protected override void OnStart()
        {
            Trigger();
            Complete();
        }

        /// <summary>
        /// Set the trigger with the name hash id.
        /// </summary>
        void Trigger()
        {
            triggerAnimator?.SetTrigger(triggerNameHash);
        }

        /// <summary>
        /// Creates a TriggerAnimation command.
        /// </summary>
        /// <param name="triggerAnimator">The trigger animator component</param>
        /// <param name="triggerNameHash">The trigger name hash id</param>
        /// <returns></returns>
        public static ICommand Create(TriggerAnimator triggerAnimator, int triggerNameHash)
        {
            return new TriggerAnimation
            {
                triggerAnimator = triggerAnimator,
                triggerNameHash = triggerNameHash
            };
        }
    }
}
