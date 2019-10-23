using UnityEngine;

namespace IndieDevTools.Animation
{
    /// <summary>
    /// A component that sets trigger paramater values on an Animator.
    /// </summary>
    public class TriggerAnimator : MonoBehaviour
    {
        /// <summary>
        /// The animator component that will have its trigger paramater values set.
        /// </summary>
        Animator Animator
        {
            get
            {
                InitAnimator();
                return animator;
            }
        }
        Animator animator;

        /// <summary>
        /// Initializes the animator component if its not already.
        /// </summary>
        void InitAnimator()
        {
            if (animator != null) return;
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }
        }
        
        /// <summary>
        /// Helper method to swap out the runtime animator controller on the animator component.
        /// </summary>
        /// <param name="controller">The runtime animator controller</param>
        public void SetController(RuntimeAnimatorController controller)
        {
            Animator.runtimeAnimatorController = controller;         
        }

        /// <summary>
        /// Sets the given trigger paramater value.
        /// </summary>
        /// <param name="triggerName">The trigger name</param>
        public void SetTrigger(string triggerName)
        {
            if (Animator.runtimeAnimatorController == null) return;
            ResetAllTriggers();
            Animator.SetTrigger(triggerName);
        }

        /// <summary>
        /// Sets the given trigger parameter value.
        /// </summary>
        /// <param name="triggerNameHash">The parameterized hash id of a trigger</param>
        public void SetTrigger(int triggerNameHash)
        {
            if (Animator.runtimeAnimatorController == null) return;
            ResetAllTriggers();
            Animator.SetTrigger(triggerNameHash);
        }

        /// <summary>
        /// Setting a trigger doesn't always unset previously set triggers. So, this
        /// method keeps things clean by resetting all trigger parameters.
        /// </summary>
        void ResetAllTriggers()
        {
            if (Animator.runtimeAnimatorController == null) return;

            foreach (AnimatorControllerParameter parameter in Animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    Animator.ResetTrigger(parameter.nameHash);
                }                 
            }
        }
    }
}

