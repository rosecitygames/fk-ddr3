using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Maps;
using IndieDevTools.Traits;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    public class FindTargetInFootprint<T> : AbstractCommand where T : IFootprint<T>
    {
        IAgent agent = null;
        ILocatable Locatable => agent as ILocatable;

        IFootprint<T> footprint = null;

        protected override void OnStart()
        {
            SetTargetMapElement();
            if (agent.TargetMapElement == null)
            {
                AddEventHandlers();
            }
            else
            {
                RemoveEventHandlers();
                Complete();
            }
        }

        protected override void OnStop()
        {
            RemoveEventHandlers();
        }

        protected override void OnDestroy()
        {
            RemoveEventHandlers();
        }

        void AddEventHandlers()
        {
            Locatable.OnUpdated += Locatable_OnUpdated;
        }

        void RemoveEventHandlers()
        {
            Locatable.OnUpdated -= Locatable_OnUpdated;
        }

        private void Locatable_OnUpdated(ILocatable obj)
        {
            SetTargetMapElement();
            if (agent.TargetMapElement == null) return;

            RemoveEventHandlers();
            Complete();
        }

        void SetTargetMapElement()
        {
            agent.TargetMapElement = GetHighestRankedMapElement();
        }

        IMapElement GetHighestRankedMapElement()
        {
            int highestEnemyRank = 0;
            int highestItemRank = 0;
            IMapElement highestRankedMapElement = null;

            List<IMapElement> mapElements = agent.Map.GetMapElementsAtCell<IMapElement>(agent.Location);

            foreach (IMapElement footprintElement in footprint.AllFootprintElements)
            {
                List<IMapElement> footprintMapElementsAtCell = agent.Map.GetMapElementsAtCell<IMapElement>(footprintElement.Location);
                mapElements.AddRange(footprintMapElementsAtCell);
            }

            foreach (IMapElement mapElement in mapElements)
            {
                if (mapElement == agent) continue;

                bool isAttackable = GetIsAttackable(mapElement);
                if (isAttackable)
                {
                    bool isEnemy = GetIsEnemy(mapElement);
                    if (isEnemy)
                    {
                        int enemyRank = GetMapElementRank(mapElement);
                        if (enemyRank > highestEnemyRank)
                        {
                            highestEnemyRank = enemyRank;
                            highestRankedMapElement = mapElement;
                        }
                    }
                }
                else if (highestEnemyRank == 0)
                {
                    int itemRank = GetMapElementRank(mapElement);
                    if (itemRank > highestItemRank)
                    {
                        highestItemRank = itemRank;
                        highestRankedMapElement = mapElement;
                    }
                }
            }
 
            return highestRankedMapElement;
        }

        bool GetIsAttackable(IMapElement mapElement)
        {
            return mapElement is IAttackReceiver;
        }

        bool GetIsEnemy(IMapElement mapElement)
        {
            return mapElement.GroupId != agent.GroupId;
        }
        
        int GetMapElementRank(IMapElement itemElement)
        {
            List<ITrait> desires = agent.Desires;
            List<ITrait> stats = itemElement.Stats;

            int rank = 0;
            foreach (ITrait stat in stats)
            {
                foreach (ITrait desire in desires)
                {
                    if (stat.Id == desire.Id)
                    {
                        int statRank = stat.Quantity * desire.Quantity;
                        rank += statRank;
                    }
                }
            }
            return rank;
        }

        public static ICommand Create(IAgent agent, IFootprint<T> footprint)
        {
            return new FindTargetInFootprint<T>
            {
                agent = agent,
                footprint = footprint
            };
        }
    }
}
