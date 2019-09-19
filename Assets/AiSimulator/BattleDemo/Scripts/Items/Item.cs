using IndieDevTools.Advertisements;
using IndieDevTools.Items;
using IndieDevTools.States;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A concrete implementation of an item that uses a command to
    /// broadcast advertisements to listening agents within its broadcast radius.
    /// </summary>
    public class Item : AbstractItem
    {
        protected override void Init()
        {
            base.Init();
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = SortingOrder;
            }
        }

        protected override void InitStateMachine()
        {
            CommandableState broadcastState = CommandableState.Create("Broadcast");
            broadcastState.AddCommand(BroadcastAdvertisement.Create(this));
            stateMachine.AddState(broadcastState);
            stateMachine.SetState(broadcastState);
        }

        protected override void RemoveFromMap()
        {
            base.RemoveFromMap();
            StopAllCoroutines();
            Destroy(gameObject);
        }

        public static IItem Create(GameObject gameObject, IItemData itemData, IAdvertisementBroadcaster broadcaster)
        {
            IItem agent = gameObject.AddComponent<Item>();
            agent.Data = itemData;
            agent.SetBroadcaster(broadcaster);
            return agent;
        }
    }
}
