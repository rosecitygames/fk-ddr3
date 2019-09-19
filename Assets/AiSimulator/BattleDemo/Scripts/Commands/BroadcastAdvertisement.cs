using IndieDevTools.Agents;
using IndieDevTools.Advertisements;
using IndieDevTools.Commands;
using IndieDevTools.Items;
using IndieDevTools.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that repeatedly broadcasts a map element's stats to all cell locations
    /// within a given distance.
    /// </summary>
    public class BroadcastAdvertisement : AbstractCommand
    {
        IAdvertisingMapElement advertisingMapElement = null;
        IAdvertisementReceiver excludeReceiver = null;

        MonoBehaviour monoBehaviour;

        Coroutine broadcastCoroutine;

        List<Vector2Int> cachedBroadcastLocations = null;
        Vector2Int lastLocation = Vector2Int.zero;

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

        protected override void OnStop()
        {
            StopBroadcast();
        }

        protected override void OnDestroy()
        {
            StopBroadcast();
        }

        void StartBroadcast()
        {
            StopBroadcast();
            broadcastCoroutine = monoBehaviour.StartCoroutine(Broadcast());
        }

        void StopBroadcast()
        {
            if (broadcastCoroutine != null)
            {
                monoBehaviour.StopCoroutine(broadcastCoroutine);
            }
        }

        IEnumerator Broadcast()
        {
            YieldInstruction yieldInstruction = new WaitForSeconds(advertisingMapElement.BroadcastInterval);

            while (isCompleted == false)
            {
                CreateAndBroadcastAdvertisement();
                yield return yieldInstruction;
            }
        }

        void CreateAndBroadcastAdvertisement()
        {
            List<Vector2Int> broadcastLocations = GetBroadcastLocations();
            IAdvertisement advertisement = Advertisement.Create(advertisingMapElement.Stats, advertisingMapElement.Map, advertisingMapElement.Location, broadcastLocations, advertisingMapElement.GroupId);
            advertisingMapElement.BroadcastAdvertisement(advertisement, excludeReceiver);
        }

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
            Vector2Int adLocation = advertisingMapElement.Location;
            int broadcastDistance = Mathf.RoundToInt(advertisingMapElement.BroadcastDistance);

            int size = broadcastDistance * 2;
 
            int cellX, cellY;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    cellX = (adLocation.x + x) - broadcastDistance;
                    cellY = (adLocation.y + y) - broadcastDistance;

                    if ((cellX >= -adMap.Size.x && cellX < adMap.Size.x) && (cellY >= -adMap.Size.y && cellY < adMap.Size.y))
                    {
                        Vector2Int cell = new Vector2Int(cellX, cellY);
                        cachedBroadcastLocations.Add(cell);
                    }  
                }
            }

            return cachedBroadcastLocations;
        }

        public static ICommand Create(AbstractItem item)
        {
            BroadcastAdvertisement command = new BroadcastAdvertisement
            {
                advertisingMapElement = item,
                monoBehaviour = item
            };

            return command;
        }

        public static ICommand Create(AbstractAgent agent)
        {
            BroadcastAdvertisement command = new BroadcastAdvertisement
            {
                advertisingMapElement = agent,
                excludeReceiver = agent,
                monoBehaviour = agent
            };

            return command;
        }
    }
}

