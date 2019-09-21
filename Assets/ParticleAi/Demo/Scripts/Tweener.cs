using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCG.Animation
{
    public enum TweenerMethod
    {
        None,
        TweenToStart,
        TweenToStartImmediately,
        TweenToEnd,
        TweenToEndImmediately,
        TweenPingPong,
        Tween
    }

	public abstract class Tweener : MonoBehaviour
	{
        [SerializeField]
        public TweenerMethod TweenOnEnable = TweenerMethod.None;

        public virtual float Delay { get; set; }
        public virtual float TweenSeconds { get; set; }

        public bool IsTweening { get; protected set; }

        public void Start()
        {
            PlayEnableMethod();
        }

        public virtual void Destroy()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        protected virtual void PlayEnableMethod()
        {
            if (TweenOnEnable == TweenerMethod.TweenToStart)
            {
                TweenToStart();
            }
            else if(TweenOnEnable == TweenerMethod.TweenToStartImmediately)
            {
                TweenToStartImmediately();
            }
            else if (TweenOnEnable == TweenerMethod.TweenToEnd)
            {
                TweenToEnd();
            }
            else if (TweenOnEnable == TweenerMethod.TweenToEndImmediately)
            {
                TweenToEndImmediately();
            }
            else if (TweenOnEnable == TweenerMethod.TweenPingPong)
            {
                TweenPingPong();
            }
            else if (TweenOnEnable == TweenerMethod.Tween)
            {
                Tween();
            }
        }
        
        public virtual void TweenPingPong(int loops = -1) { }

        [ContextMenu("Tween")]
        public virtual void Tween()
        {
            StopAllCoroutines();
            if (isActiveAndEnabled)
            {
                StartCoroutine(TweenCoroutine());
            }
            else
            {
                TweenToEndImmediately();
            }
        }
        public virtual IEnumerator TweenCoroutine() { yield break; }

        public virtual void TweenImmediately() { }

        [ContextMenu("Tween To Start")]
        public virtual void TweenToStart()
        {
            StopAllCoroutines();
            if (isActiveAndEnabled)
            {
                StartCoroutine(TweenToStartCoroutine());
            }
            else
            {
                TweenToStartImmediately();
            }
        }

        public virtual IEnumerator TweenToStartCoroutine() { yield break; }

        public virtual void TweenToStartImmediately() { }

        [ContextMenu("Tween To End")]
        public virtual void TweenToEnd()
        {
            StopAllCoroutines();
            if (isActiveAndEnabled)
            {
                StartCoroutine(TweenToEndCoroutine());
            }
            else
            {
                TweenToEndImmediately();
            }
        }

        public virtual IEnumerator TweenToEndCoroutine() { yield break; }

        public virtual void TweenToEndImmediately() { }

        public virtual bool GetIsTargetAtStartValue() { return false; }
        public virtual bool GetIsTargetAtEndValue() { return false; }
        public virtual void StopTween()
        {
            StopAllCoroutines();
            IsTweening = false;
        }
    }
}
