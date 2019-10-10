using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Items;
using IndieDevTools.Maps;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that looks at map elements at a given agent's
    /// location and calls a transition depending on the type
    /// of element found.
    /// </summary>
    public class InspectTargetMapElement : AbstractCommand
    {
        IAgent agent = null;

        string enemyFoundTransition = "";
        string itemFoundTransition = "";
        string nothingFoundTransition = "";
  
        protected override void OnStart()
        {
            agent.TargetAdvertisement = null;
            Inspect();
            Complete();
        }

        void Inspect()
        {
            IMapElement targetMapElement = agent.TargetMapElement;

            bool isNotNullMapElement = targetMapElement != null;
            bool isNotSelf = targetMapElement != agent;
            if (isNotNullMapElement && isNotSelf)
            {
                bool isEnemy = GetIsEnemy(targetMapElement);
                if (isEnemy)
                {
                    agent.Description = "Found enemy " + targetMapElement.DisplayName;
                    agent.HandleTransition(enemyFoundTransition);
                }
                else
                {
                    bool isItem = GetIsItem(targetMapElement);
                    if (isItem)
                    {
                        agent.Description = "Found item " + targetMapElement.DisplayName;
                        agent.HandleTransition(itemFoundTransition);
                    }
                    else
                    {
                        agent.HandleTransition(nothingFoundTransition);
                    }
                }
            }
            else
            {
                agent.HandleTransition(nothingFoundTransition);
            }
        }

        bool GetIsEnemy(IMapElement mapElement)
        {
            bool isAttackable = (mapElement as IAttackReceiver) != null;
            return (isAttackable && mapElement.GroupId != agent.GroupId);
        }

        bool GetIsItem(IMapElement mapElement)
        {
            return (mapElement as IItem) != null;
        }

        public static ICommand Create(IAgent agent, string handleEnemeyTransition = "", string handleItemTransition = "", string handleNothingTransition = "")
        {
            return new InspectTargetMapElement
            {
                agent = agent,
                enemyFoundTransition = handleEnemeyTransition,
                itemFoundTransition = handleItemTransition,
                nothingFoundTransition = handleNothingTransition
            };
        }
    }
}
