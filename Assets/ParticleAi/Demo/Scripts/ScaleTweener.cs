using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCG.Animation
{
    class ScaleTweener : Tweener
    {
        [SerializeField] Transform target;
        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }

        [SerializeField] float delay = 0.0f;
        public override float Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        [SerializeField] float tweenSeconds = 0.5f;
        public override float TweenSeconds
        {
            get { return tweenSeconds; }
            set { tweenSeconds = value; }
        }

        [SerializeField] AnimationCurve tweenCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [SerializeField] Vector3 startScale;
        public Vector3 StartScale
        {
            get { return startScale; }
            set { startScale = value; }
        }

        [SerializeField] Vector3 endScale = Vector3.one;
        public Vector3 EndScale
        {
            get { return endScale; }
            set { endScale = value; }
        }

        [SerializeField] bool isUsingRelativeInitialScale = false;

        Vector3 initialScale;

        [SerializeField] bool isUsingUnscaledTime = false;

        protected override void PlayEnableMethod()
        {
            initialScale = target.localScale;
            base.PlayEnableMethod();
        }

        public override void TweenPingPong(int loops = -1)
        {
            StopTween();
            StartCoroutine(TweenPingPongCoroutine(startScale, endScale));
        }

        IEnumerator TweenPingPongCoroutine(Vector3 startValue, Vector3 endValue)
        {
            IsTweening = true;

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            if (tweenSeconds > 0)
            {
                float elapsedTime = 0;

                while (isActiveAndEnabled)
                {
                    elapsedTime += Time.deltaTime;
                    float timePercentage = elapsedTime / tweenSeconds;
                    SetTargetValueFromCurve(startValue, endValue, timePercentage);

                    if (elapsedTime >= tweenSeconds)
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

        public override IEnumerator TweenCoroutine()
        {
            yield return StartCoroutine(Tween(startScale, endScale));
        }

        public IEnumerator Tween(Vector3 startValue, Vector3 endValue)
        {
            IsTweening = true;

            if (isActiveAndEnabled)
            {
                target.localScale = GetParsedScale(startValue);
            }
            else
            {
                target.localScale = GetParsedScale(endValue);
            }

            bool isAtEndValue = GetIsAtTargetValue(endValue);
            if (isAtEndValue)
            {
                IsTweening = false;
                yield break;
            }

            if (delay > 0)
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

            if (tweenSeconds > 0)
            {
                float elapsedTime = 0;
                while (elapsedTime < tweenSeconds)
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

        public override IEnumerator TweenToStartCoroutine()
        {
            yield return Tween(target.localScale, startScale);
        }

        public override void TweenToStartImmediately()
        {
            TweenImmediately(startScale);
        }

        public override IEnumerator TweenToEndCoroutine()
        {
            yield return Tween(target.localScale, endScale);
        }

        public override void TweenToEndImmediately()
        {
            TweenImmediately(endScale);
        }

        public override void TweenImmediately()
        {
            TweenToEndImmediately();
        }

        public void TweenImmediately(Vector3 value)
        {
            IsTweening = false;
            StopAllCoroutines();
            target.localScale = GetParsedScale(value);
        }

        public override bool GetIsTargetAtStartValue()
        {
            return GetIsAtTargetValue(startScale);
        }

        public override bool GetIsTargetAtEndValue()
        {
            return GetIsAtTargetValue(endScale);
        }

        public bool GetIsAtTargetValue(Vector3 scale)
        {
            scale = GetParsedScale(scale);
            return Mathf.Approximately(target.localScale.x, scale.x) && Mathf.Approximately(target.localScale.y, scale.y) && Mathf.Approximately(target.localScale.z, scale.z);
        }

        void SetTargetValueFromCurve(Vector3 startValue, Vector3 endValue, float curvePercentage)
        {
            float lerpPercentage = tweenCurve.Evaluate(curvePercentage);
            Vector3 newScale = Vector3.LerpUnclamped(startValue, endValue, lerpPercentage);
            if (newScale.x < 0) newScale.x = 0;
            if (newScale.y < 0) newScale.y = 0;
            if (newScale.z < 0) newScale.z = 0;
            target.localScale = GetParsedScale(newScale);
        }

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
