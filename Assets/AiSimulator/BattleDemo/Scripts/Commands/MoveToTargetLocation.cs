using IndieDevTools.Agents;
using IndieDevTools.Traits;
using IndieDevTools.Commands;
using System.Collections;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that moves a given agent to its target location
    /// over time at a speed determined by its speed trait.
    /// </summary>
    public class MoveToTargetLocation : AbstractCommand
    {
        IAgent agent;
        MonoBehaviour monoBehaviour;

        Coroutine coroutine;

        SpriteRenderer spriteRenderer;

        protected override void OnStart()
        {
            spriteRenderer = monoBehaviour.GetComponentInChildren<SpriteRenderer>();
            StartMove();
        }

        protected override void OnStop()
        {
            StopMove();
        }

        protected override void OnDestroy()
        {
            StopMove();
        }

        void StartMove()
        {
            StopMove();
            coroutine = monoBehaviour.StartCoroutine(Move());
        }

        void StopMove()
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }
        }

        IEnumerator Move()
        {
            float moveSpeed = TraitsUtil.GetMoveSpeed(agent);

            YieldInstruction yieldInstruction = new WaitForEndOfFrame();

            bool isLocationReached = false;
            while (isLocationReached == false)
            {
                yield return yieldInstruction;

                Vector3 targetPosition = agent.Map.CellToLocal(agent.TargetLocation);
                float targetDistance = Vector2.Distance(agent.Position, targetPosition);

                isLocationReached = targetDistance < 0.001f;
                if (isLocationReached == false)
                {
                    agent.Position = Vector2.MoveTowards(agent.Position, targetPosition, moveSpeed);
                }

                UpdateSortingOrder();
            }

            Complete();
        }

        void UpdateSortingOrder()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = agent.SortingOrder;
        }

        public static ICommand Create(AbstractAgent agent)
        {
            MoveToTargetLocation command = new MoveToTargetLocation
            {
                agent = agent,
                monoBehaviour = agent
            };

            return command;
        }
    }
}

