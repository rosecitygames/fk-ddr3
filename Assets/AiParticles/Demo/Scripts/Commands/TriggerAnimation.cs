using IndieDevTools.Animation;

namespace IndieDevTools.Commands
{
    public class TriggerAnimation : AbstractCommand
    {
        TriggerAnimator triggerAnimator = null;
        int triggerNameHash;

        protected override void OnStart()
        {
            Trigger();
            Complete();
        }

        void Trigger()
        {
            if (triggerAnimator != null)
            {
                triggerAnimator.SetTrigger(triggerNameHash);
            }
        }

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
