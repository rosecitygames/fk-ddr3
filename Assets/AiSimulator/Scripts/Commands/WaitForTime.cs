using System.Collections;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A generic command that waits for x time until completed.
    /// </summary>
    public class WaitForTime : AbstractCommand
    {
        MonoBehaviour monoBehaviour;
        YieldInstruction yieldInstruction;

        Coroutine coroutine;

        protected override void OnStart()
        {
            StartWait();
        }

        protected override void OnStop()
        {
            StopWait();
        }

        protected override void OnDestroy()
        {
            StopWait();
        }

        void StartWait()
        {
            StopWait();
            coroutine = monoBehaviour.StartCoroutine(Wait());
        }

        void StopWait()
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }
        }

        IEnumerator Wait()
        {
            yield return yieldInstruction;
            Complete();
        }

        public static ICommand Create(MonoBehaviour monoBehaviour, float seconds)
        {
            return new WaitForTime
            {
                monoBehaviour = monoBehaviour,
                yieldInstruction = new WaitForSeconds(seconds)
            };
        }
    }
}
