using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Animation
{
    /// <summary>
    /// A tweener component that scales between two local scale values.
    /// </summary>
    class ScaleTweener : Tweener
    {
        /// <summary>
        /// The target object that will be scaled.
        /// </summary>
        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }
        [SerializeField] Transform target;

        /// <summary>
        /// Delay before the tween is started
        /// </summary>
        public override float Delay
        {
            get { return delay; }
            set { delay = value; }
        }
        [SerializeField] float delay = 0.0f;
       
        /// <summary>
        /// The amount of delay in seconds.
        /// </summary>
        public override float TweenSeconds
        {
            get { return tweenSeconds; }
            set { tweenSeconds = value; }
        }
        [SerializeField] float tweenSeconds = 0.5f;

        /// <summary>
        /// The animation curved used on the tween.
        /// </summary>
        [SerializeField] AnimationCurve tweenCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        /// <summary>
        /// The start scale.
        /// </summary>
        public Vector3 StartScale
        {
            get { return startScale; }
            set { startScale = value; }
        }
        [SerializeField] Vector3 startScale;

        /// <summary>
        /// The end scale.
        /// </summary>
        public Vector3 EndScale
        {
            get { return endScale; }
            set { endScale = value; }
        }
        [SerializeField] Vector3 endScale = Vector3.one;

        /// <summary>
        /// Whether or not the tween will be relative to the initial scale.
        /// This will multiply the scale values. Useful if you want to do 
        /// something like double the initial scale size.
        /// </summary>
        [SerializeField] bool isUsingRelativeInitialScale = false;

        /// <summary>
        /// The initial scale value of the target transform
        /// </summary>
        Vector3 initialScale;

        /// <summary>
        /// Whether or not the tween seconds use unscaled time.
        /// </summary>
        [SerializeField] bool isUsingUnscaledTime = false;

        /// <summary>
        /// Ses the initial scale value and then plays the start method.
        /// </summary>
        protected override void PlayStartMethod()
        {
            initialScale = target.localScale;
            base.PlayStartMethod();
        }

        /// <summary>
        /// Starts a ping pong tween.
        /// </summary>
        /// <param name="loops">How many times the ping pong should loop. For infinite looping, use -1</param>
        public override void TweenPingPong(int loops = -1)
        {
            StopTween();
            StartCoroutine(TweenPingPongCoroutine(startScale, endScale));
        }

        /// <summary>
        /// The ping pong tween coroutine that tweens back and forth between the start and end values.
        /// </summary>
        /// <param name="startValue">The start scale</param>
        /// <param name="endValue">The end scale</param>
        IEnumerator TweenPingPongCoroutine(Vector3 startValue, Vector3 endValue)
        {
            IsTweening = true;

            if (Delay > 0)
            {
                yield return new WaitForSeconds(Delay);
            }

            if (TweenSeconds > 0)
            {
                float elapsedTime = 0;

                while (isActiveAndEnabled)
                {
                    elapsedTime += Time.deltaTime;
                    float timePercentage = elapsedTime / TweenSeconds;
                    SetTargetValueFromCurve(startValue, endValue, timePercentage);

                    if (elapsedTime >= TweenSeconds)
                    {
                        elapsedTime = 0;
                        Vector3 newStartValue = endValue;
                        Vector3 newEndValue = startValue;
                        startValue = newStartValue;
                        endValue = newEndValue;
                    }

                    yield return null;
                }
            }
        }

        /// <summary>
        /// The tween coroutine that tweens from the start to end scale value.
        /// </summary>
        public override IEnumerator TweenCoroutine()
        {
            yield return StartCoroutine(Tween(StartScale, EndScale));
        }

        /// <summary>
        /// The parameterized coroutine that tweens from the start to end value.
        /// </summary>
        /// <param name="startValue">The start scale</param>
        /// <param name="endValue">The end scale</param>
        /// <returns></returns>
        public IEnumerator Tween(Vector3 startValue, Vector3 endValue)
        {
            IsTweening = true;

            if (isActiveAndEnabled)
            {
                Target.localScale = GetParsedScale(startValue);
            }
            else
            {
                Target.localScale = GetParsedScale(endValue);
            }

            bool isAtEndValue = GetIsAtTargetValue(endValue);
            if (isAtEndValue)
            {
                IsTweening = false;
                yield break;
            }

            if (Delay > 0)
            {
                if (isUsingUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }

            if (TweenSeconds > 0)
            {
                float elapsedTime = 0;
                while (elapsedTime < TweenSeconds)
                {
                    if (isUsingUnscaledTime)
                    {
                        elapsedTime += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        elapsedTime += Time.deltaTime;
                    }

                    float timePercentage = elapsedTime / tweenSeconds;
                    SetTargetValueFromCurve(startValue, endValue, timePercentage);

                    if (isUsingUnscaledTime)
                    {
                        yield return new WaitForSecondsRealtime(1 / 30);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }

            TweenImmediately(endValue);
        }

        /// <summary>
        /// Coroutine that tweens from the current scale value to the start scale value.
        /// </summary>
        public override IEnumerator TweenToStartCoroutine()
        {
            yield return Tween(Target.localScale, StartScale);
        }

        /// <summary>
        /// Sets the target transform to the start scale value immediately.
        /// </summary>
        public override void TweenToStartImmediately()
        {
            TweenImmediately(StartScale);
        }

        /// <summary>
        /// Tweens to the end scale value.
        /// </summary>
        public override IEnumerator TweenToEndCoroutine()
        {
            yield return Tween(Target.localScale, EndScale);
        }

        /// <summary>
        /// Sets the target transform to the end scale value immediately.
        /// </summary>
        public override void TweenToEndImmediately()
        {
            TweenImmediately(EndScale);
        }

        /// <summary>
        /// Sets the target transform to the end scale value immediately.
        /// </summary>
        public override void TweenImmediately() => TweenToEndImmediately();

        /// <summary>
        /// Stops any active tweening and sets the target transform to the given scale value.
        /// </summary>
        public void TweenImmediately(Vector3 value)
        {
            IsTweening = false;
            StopAllCoroutines();
            target.localScale = GetParsedScale(value);
        }

        /// <summary>
        /// Whether or not the target transform is at the start value.
        /// </summary>
        public override bool GetIsTargetAtStartValue()
        {
            return GetIsAtTargetValue(startScale);
        }

        /// <summary>
        /// Whether or not the target transform is at the end value.
        /// </summary>
        public override bool GetIsTargetAtEndValue()
        {
            return GetIsAtTargetValue(endScale);
        }

        /// <summary>
        /// Whether or not the target transform is at the given scale value.
        /// </summary>
        /// <param name="scale">The approximate scale value to check</param>
        public bool GetIsAtTargetValue(Vector3 scale)
        {
            scale = GetParsedScale(scale);
            return Mathf.Approximately(target.localScale.x, scale.x) && Mathf.Approximately(target.localScale.y, scale.y) && Mathf.Approximately(target.localScale.z, scale.z);
        }

        /// <summary>
        /// Sets the target transform scale between a start and value using a tween curve percentage.
        /// </summary>
        /// <param name="startValue">A start scale value</param>
        /// <param name="endValue">An end scale value</param>
        /// <param name="curvePercentage">The time percentage along the tween curve</param>
        void SetTargetValueFromCurve(Vector3 startValue, Vector3 endValue, float curvePercentage)
        {
            float lerpPercentage = tweenCurve.Evaluate(curvePercentage);
            Vector3 newScale = Vector3.LerpUnclamped(startValue, endValue, lerpPercentage);
            if (newScale.x < 0) newScale.x = 0;
            if (newScale.y < 0) newScale.y = 0;
            if (newScale.z < 0) newScale.z = 0;
            target.localScale = GetParsedScale(newScale);
        }

        /// <summary>
        /// Gets a parsed scale value that can change depending on whether relative scaling is being used.
        /// </summary>
        /// <param name="scale">The scale in value</param>
        /// <returns>The parsed scale value</returns>
        Vector3 GetParsedScale(Vector3 scale)
        {
            if (isUsingRelativeInitialScale)
            {
                scale.x *= initialScale.x;
                scale.y *= initialScale.y;
                scale.z *= initialScale.z;
            }

            return scale;
        }

    }
}
