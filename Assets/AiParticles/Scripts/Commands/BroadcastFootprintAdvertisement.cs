using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Items;
using IndieDevTools.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A command that repeatedly broadcasts a map elements stats from a footprint's bordering locations.
    /// </summary>
    /// <typeparam name="T">The type of footprint map element</typeparam>
    public class BroadcastFootprintAdvertisement<T> : AbstractCommand where T:IFootprint<T>
    {
        /// <summary>
        /// The advertising map element
        /// </summary>
        IAdvertisingMapElement advertisingMapElement = null;

        /// <summary>
        /// A map element excluded from receiving advertisements.
        /// </summary>
        IAdvertisementReceiver excludeReceiver = null;

        /// <summary>
        /// The footprint used for deriving map locations to advertise from.
        /// </summary>
        IFootprint<T> footprint = null;

        /// <summary>
        /// A monobehaviour to run coroutines on.
        /// </summary>
        MonoBehaviour monoBehaviour;

        /// <summary>
        /// The active broadcast coroutine.
        /// </summary>
        Coroutine broadcastCoroutine;

        /// <summary>
        /// A cache of map locations being advertised to. Updated when the main advertising map element changes location.
        /// </summary>
        List<Vector2Int> cachedBroadcastLocations = null;

        /// <summary>
        /// The last known location of the main advertising map element.
        /// </summary>
        Vector2Int lastLocation = Vector2Int.zero;

        /// <summary>
        /// Starts the broadcast if the advertising map element is broadcastable.
        /// </summary>
        protected override void OnStart()
        {
            bool isBroadcastable = (advertisingMapElement.BroadcastDistance > 0) && (advertisingMapElement.BroadcastInterval > 0);
            if (isBroadcastable)
            {
                StartBroadcast();
            }
            else
            {
                Complete();
            }
        }

        /// <summary>
        /// Stops the broadcast.
        /// </summary>
        protected override void OnStop()
        {
            StopBroadcast();
        }

        /// <summary>
        /// Stops the broadcast when the object is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            StopBroadcast();
        }

        /// <summary>
        /// Starts the broadcast coroutine.
        /// </summary>
        void StartBroadcast()
        {
            StopBroadcast();
            broadcastCoroutine = monoBehaviour.StartCoroutine(Broadcast());
        }

        /// <summary>
        /// Stops the active broadcast coroutine.
        /// </summary>
        void StopBroadcast()
        {
            if (broadcastCoroutine != null)
            {
                monoBehaviour.StopCoroutine(broadcastCoroutine);
            }
        }

        /// <summary>
        /// A coroutine that broadcasts advertisements on the advertising map element's broadcast interval.
        /// </summary>
        IEnumerator Broadcast()
        {
            YieldInstruction yieldInstruction = new WaitForSeconds(advertisingMapElement.BroadcastInterval);

            while (isCompleted == false)
            {
                CreateAndBroadcastAdvertisement();
                yield return yieldInstruction;
            }
        }

        /// <summary>
        /// Creates and broadcasts the advertising map element's stats to all the broadcast locations.
        /// </summary>
        void CreateAndBroadcastAdvertisement()
        {
            List<Vector2Int> broadcastLocations = GetBroadcastLocations();
            IAdvertisement advertisement = Advertisement.Create(advertisingMapElement.Stats, advertisingMapElement.Map, advertisingMapElement.Location, broadcastLocations, advertisingMapElement.GroupId);
            advertisingMapElement.BroadcastAdvertisement(advertisement, excludeReceiver);
        }

        /// <summary>
        /// Gets a list of all broadcast locations based on distance from the footprint's bordering elements.
        /// </summary>
        /// <returns>The broadcast locations</returns>
        List<Vector2Int> GetBroadcastLocations()
        {
            bool isUsingCachedLocations = cachedBroadcastLocations != null && lastLocation == advertisingMapElement.Location;
            if (isUsingCachedLocations)
            {
                return cachedBroadcastLocations;
            }

            if (cachedBroadcastLocations == null)
            {
                cachedBroadcastLocations = new List<Vector2Int>();
            }
            else
            {
                cachedBroadcastLocations.Clear();
            }

            IMap adMap = advertisingMapElement.Map;

            List<Vector2Int> adLocations = new List<Vector2Int>();
            adLocations.Add(advertisingMapElement.Location);

            foreach(IMapElement footprintElement in footprint.BorderFootprintElements)
            {
                adLocations.Add(footprintElement.Location);
            }

            int broadcastDistance = Mathf.RoundToInt(advertisingMapElement.BroadcastDistance);
            int size = broadcastDistance * 2;
            int cellX, cellY;

            foreach (Vector2Int adLocation in adLocations)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        cellX = (adLocation.x + x) - broadcastDistance;
                        cellY = (adLocation.y + y) - broadcastDistance;

                        if ((cellX >= -adMap.Size.x && cellX < adMap.Size.x) && (cellY >= -adMap.Size.y && cellY < adMap.Size.y))
                        {
                            Vector2Int cell = new Vector2Int(cellX, cellY);
                            if (cachedBroadcastLocations.Contains(cell) == false)
                            {
                                cachedBroadcastLocations.Add(cell);
                            }
                        }
                    }
                }
            }

            return cachedBroadcastLocations;
        }

        /// <summary>
        /// Creates the BroadcastFootprintAdvertisement command.
        /// </summary>
        /// <param name="item">A map item</param>
        /// <param name="footprint">The item footprint.</param>
        /// <returns>A BroadcastFootprintAdvertisement command.</returns>
        public static ICommand Create(AbstractItem item, IFootprint<T> footprint)
        {
            BroadcastFootprintAdvertisement<T> command = new BroadcastFootprintAdvertisement<T>
            {
                advertisingMapElement = item,
                monoBehaviour = item,
                footprint = footprint
            };

            return command;
        }

        /// <summary>
        /// Creates the BroadcastFootprintAdvertisement command.
        /// </summary>
        /// <param name="agent">An advertising agent</param>
        /// <param name="footprint">The agent footprint.</param>
        /// <returns>A BroadcastFootprintAdvertisement command.</returns>
        public static ICommand Create(AbstractAgent agent, IFootprint<T> footprint)
        {
            BroadcastFootprintAdvertisement<T> command = new BroadcastFootprintAdvertisement<T>
            {
                advertisingMapElement = agent,
                excludeReceiver = agent,
                monoBehaviour = agent,
                footprint = footprint
            };

            return command;
        }
    }
}

