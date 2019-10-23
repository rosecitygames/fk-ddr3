using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Animation
{
    /// <summary>
    /// Types of tween
    /// </summary>
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

    /// <summary>
    /// An abstract component class for tweening.
    /// </summary>
	public abstract class Tweener : MonoBehaviour
	{
        /// <summary>
        /// Whether or not to automatically tween on the start event method.
        /// </summary>
        [SerializeField]
        public TweenerMethod TweenOnStart = TweenerMethod.None;

        /// <summary>
        /// Delay before the tween is started
        /// </summary>
        public virtual float Delay { get; set; }

        /// <summary>
        /// The amount of delay in seconds.
        /// </summary>
        public virtual float TweenSeconds { get; set; }

        /// <summary>
        /// Whether or not the component is in a state of tweening.
        /// </summary>
        public bool IsTweening { get; protected set; }

        /// <summary>
        /// Unity start event that plays the start method.
        /// </summary>
        public void Start() => PlayStartMethod();

        /// <summary>
        /// Unity destroy even that calls a virtual destroy method.
        /// </summary>
        public void OnDestroy() => Destroy();

        /// <summary>
        /// Stops all coroutines and cancels any invokes.
        /// </summary>
        public virtual void Destroy()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        /// <summary>
        /// Plays the assigned start method.
        /// </summary>
        protected virtual void PlayStartMethod()
        {
            if (TweenOnStart == TweenerMethod.TweenToStart)
            {
                TweenToStart();
            }
            else if(TweenOnStart == TweenerMethod.TweenToStartImmediately)
            {
                TweenToStartImmediately();
            }
            else if (TweenOnStart == TweenerMethod.TweenToEnd)
            {
                TweenToEnd();
            }
            else if (TweenOnStart == TweenerMethod.TweenToEndImmediately)
            {
                TweenToEndImmediately();
            }
            else if (TweenOnStart == TweenerMethod.TweenPingPong)
            {
                TweenPingPong();
            }
            else if (TweenOnStart == TweenerMethod.Tween)
            {
                Tween();
            }
        }
        
        /// <summary>
        /// Starts a ping pong tween.
        /// </summary>
        /// <param name="loops">How many times the ping pong should loop. For infinite looping, use -1</param>
        public virtual void TweenPingPong(int loops = -1) { }

        /// <summary>
        ///Tween from the start to end value.
        /// </summary>
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

        /// <summary>
        /// The tween coroutine.
        /// </summary>
        public virtual IEnumerator TweenCoroutine() { yield break; }

        /// <summary>
        /// Complete the tween immediately.
        /// </summary>
        public virtual void TweenImmediately() { }

        /// <summary>
        /// Tweens to the start value.
        /// </summary>
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

        /// <summary>
        /// The tween to start coroutine.
        /// </summary>
        public virtual IEnumerator TweenToStartCoroutine() { yield break; }

        /// <summary>
        /// Tween to the start value immediately
        /// </summary>
        public virtual void TweenToStartImmediately() { }

        /// <summary>
        /// Tweens to the end value.
        /// </summary>
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

        /// <summary>
        /// The tween to end coroutine.
        /// </summary>
        public virtual IEnumerator TweenToEndCoroutine() { yield break; }

        /// <summary>
        /// Tween to the end value immediately.
        /// </summary>
        public virtual void TweenToEndImmediately() { }

        /// <summary>
        /// Get whether or not the target object is at the tween start value.
        /// </summary>
        public virtual bool GetIsTargetAtStartValue() { return false; }

        /// <summary>
        /// Get whether or not the target object is at the tween end value.
        /// </summary>
        public virtual bool GetIsTargetAtEndValue() { return false; }

        /// <summary>
        /// Stop any active tweening.
        /// </summary>
        public virtual void StopTween()
        {
            StopAllCoroutines();
            IsTweening = false;
        }
    }
}
