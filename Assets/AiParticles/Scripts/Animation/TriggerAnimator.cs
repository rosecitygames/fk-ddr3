using UnityEngine;

namespace IndieDevTools.Animation
{
    public class TriggerAnimator : MonoBehaviour
    {
        Animator animator;
        Animator Animator
        {
            get
            {
                InitAnimator();
                return animator;
            }
        }

        void InitAnimator()
        {
            if (animator != null) return;
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }
            InitSpriteRenderer();
        }

        SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer
        {
            get
            {
                InitSpriteRenderer();
                return spriteRenderer;
            }
        }

        void InitSpriteRenderer()
        {
            if (spriteRenderer != null) return;
            spriteRenderer = Animator.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = Animator.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void SetController(RuntimeAnimatorController controller)
        {
            Animator.runtimeAnimatorController = controller;         
        }

        Color color = Color.white;
        public void SetColor(Color value)
        {
            color = value;
            SpriteRenderer.color = value;
        }

        public void SetTrigger(string triggerName)
        {
            if (Animator.runtimeAnimatorController == null) return;
            ResetAllTriggers();
            Animator.SetTrigger(triggerName);
        }

        public void SetTrigger(int triggerNameHash)
        {
            if (Animator.runtimeAnimatorController == null) return;
            ResetAllTriggers();
            Animator.SetTrigger(triggerNameHash);
        }

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

