﻿using IndieDevTools.Agents;
using IndieDevTools.Advertisements;
using IndieDevTools.Commands;
using IndieDevTools.Items;
using IndieDevTools.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// A command that repeatedly broadcasts a map element's stats to all cell locations
    /// within a given distance.
    /// </summary>
    public class BroadcastFootprintAdvertisement<T> : AbstractCommand where T:IFootprint<T>
    {
        GameObject tilePrefab = null;
        List<GameObject> tiles = new List<GameObject>();

        IAdvertisingMapElement advertisingMapElement = null;
        IAdvertisementReceiver excludeReceiver = null;
        IFootprint<T> footprint = null;

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

            if (tilePrefab == null) return cachedBroadcastLocations;

            foreach(GameObject tile in tiles)
            {
                if (tile == null) continue;
                GameObject.Destroy(tile);
            }

            tiles.Clear();

            foreach(Vector2Int broadcastLocation in cachedBroadcastLocations)
            {
                Vector3 tilePosition = advertisingMapElement.Map.CellToLocal(broadcastLocation);
                GameObject tile = GameObject.Instantiate(tilePrefab, advertisingMapElement.Map.Transform);
                tile.transform.position = tilePosition;
                tiles.Add(tile);
            }

            return cachedBroadcastLocations;
        }

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

        public static ICommand Create(AbstractAgent agent, IFootprint<T> footprint, GameObject tilePrefab = null)
        {
            BroadcastFootprintAdvertisement<T> command = new BroadcastFootprintAdvertisement<T>
            {
                advertisingMapElement = agent,
                excludeReceiver = agent,
                monoBehaviour = agent,
                footprint = footprint,
                tilePrefab = tilePrefab
            };

            return command;
        }
    }
}

