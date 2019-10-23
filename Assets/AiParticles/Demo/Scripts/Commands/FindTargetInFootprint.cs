using IndieDevTools.Agents;
using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Maps;
using IndieDevTools.Traits;
using System.Collections.Generic;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that finds an attackable target within a footprint.
    /// </summary>
    /// <typeparam name="T">The type of map element the footprint is.</typeparam>
    public class FindTargetInFootprint<T> : AbstractCommand where T : IFootprint<T>
    {
        /// <summary>
        /// The main map element agent used for ranking and location searching.
        /// </summary>
        IAgent agent = null;

        /// <summary>
        /// The locatable agent reference
        /// </summary>
        ILocatable Locatable => agent as ILocatable;

        /// <summary>
        /// The footprint that will be used for location researching.
        /// </summary>
        IFootprint<T> footprint = null;

        /// <summary>
        /// Sets a target map element if possible. Otherwise, search again after the agent's
        /// location updates.
        /// </summary>
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

        /// <summary>
        /// Remove any event handlers when the command is stopped.
        /// </summary>
        protected override void OnStop()
        {
            RemoveEventHandlers();
        }

        /// <summary>
        /// Remove any event handlers when the command is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            RemoveEventHandlers();
        }

        /// <summary>
        /// Add an event handler for when the agent's location is updated.
        /// </summary>
        void AddEventHandlers()
        {
            RemoveEventHandlers();
            Locatable.OnUpdated += Locatable_OnUpdated;
        }

        /// <summary>
        /// Remove an event handler for when the agent's location is updated.
        /// </summary>
        void RemoveEventHandlers()
        {
            Locatable.OnUpdated -= Locatable_OnUpdated;
        }

        /// <summary>
        /// When the agent's location is updated, search for a target element.
        /// If one is found, then complete the command.
        /// </summary>
        /// <param name="obj"></param>
        private void Locatable_OnUpdated(ILocatable obj)
        {
            SetTargetMapElement();
            if (agent.TargetMapElement == null) return;

            RemoveEventHandlers();
            Complete();
        }

        /// <summary>
        /// Set the agent's target map element to the highest ranked map element within
        /// it's footprint.
        /// </summary>
        void SetTargetMapElement()
        {
            agent.TargetMapElement = GetHighestRankedMapElement();
        }

        /// <summary>
        /// Gets the highest ranked map element within the agent location and the footprint locations.
        /// </summary>
        /// <returns>The highest ranked map element.</returns>
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

        /// <summary>
        /// Whether or not the given map element is an attack receiver.
        /// </summary>
        bool GetIsAttackable(IMapElement mapElement)
        {
            return mapElement is IAttackReceiver;
        }

        /// <summary>
        /// Whether or not the give map element belongs to another group id.
        /// </summary>
        bool GetIsEnemy(IMapElement mapElement)
        {
            return mapElement.GroupId != agent.GroupId;
        }
        
        /// <summary>
        /// Gets the given map element rank based off of the agent's desires and
        /// the map element's stats.
        /// </summary>
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

        /// <summary>
        /// Create a command object.
        /// </summary>
        /// <param name="agent">The agent used for ranking and main location searching.</param>
        /// <param name="footprint">The footprint that will be used for location researching.</param>
        /// <returns>The command object</returns>
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
