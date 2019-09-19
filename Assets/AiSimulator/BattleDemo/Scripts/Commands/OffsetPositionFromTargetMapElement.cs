using IndieDevTools.Agents;
using IndieDevTools.Traits;
using IndieDevTools.Commands;
using System.Collections;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that moves a given agent to an offsetted
    /// position of its target map element location. For example,
    /// this is used when agents are in a battle so that they stand
    /// on each side of a map cell to prevent them from overlapping
    /// in the same position.
    /// </summary>
    public class OffsetPositionFromTargetMapElement : AbstractCommand
    {
        IAgent agent;
        MonoBehaviour monoBehaviour;

        Coroutine coroutine;

        SpriteRenderer spriteRenderer;

        protected override void OnStart()
        {
            spriteRenderer = monoBehaviour.GetComponentInChildren<SpriteRenderer>();

            if (agent.TargetMapElement != null && agent.Location == agent.TargetMapElement.Location)
            {
                StartMove();
            }
            else
            {
                Complete();
            }
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
            Vector3 targetPosition = agent.Map.CellToLocal(agent.TargetLocation);

            float offsetX = agent.Map.CellSize.x * 0.25f;
            if (agent.InstanceId < agent.TargetMapElement.InstanceId)
            {
                offsetX *= -1.0f;
            }

            targetPosition.x += offsetX;

            float moveSpeed = TraitsUtil.GetMoveSpeed(agent);

            YieldInstruction yieldInstruction = new WaitForEndOfFrame();

            bool isLocationReached = false;
            while (isLocationReached == false)
            {
                yield return yieldInstruction;
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
            OffsetPositionFromTargetMapElement command = new OffsetPositionFromTargetMapElement
            {
                agent = agent,
                monoBehaviour = agent
            };

            return command;
        }
    }
}

