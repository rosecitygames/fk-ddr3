using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieDevTools.Maps;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A component that instantiates map element prefabs onto a map.
    /// Instantiation counts are determine by the generator's fill
    /// percentage multiplied by the element's odds percentage.
    /// </summary>
    [RequireComponent(typeof(IMap))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 1.0f)]
        float fillPercentage = 1.0f;

        [SerializeField]
        List<MapGeneratorElementData> datas = new List<MapGeneratorElementData>();

        IMap map;

        void Start()
        {
            Init();
            Generate();
        }

        void Init()
        {
            map = GetComponent<IMap>();
        }

        void Generate()
        {
            int mapCellCount = map.CellCount;
            int fillCellCount = Mathf.FloorToInt(mapCellCount * fillPercentage);
            Debug.Log("mapCellCount = " + mapCellCount + ", fillCellCount = " + fillCellCount);

            Vector2Int mapSize = map.Size;
            int offsetX = -mapSize.x / 2;
            int offsetY = -mapSize.y / 2;

            List<Vector2Int> occupiedCells = new List<Vector2Int>();

            foreach(MapGeneratorElementData data in datas )
            {
                int elementCount = Mathf.FloorToInt(data.Percentage * fillCellCount);
                Debug.Log("Generating " + elementCount + " " + data.Prefab.name);
                
                for(int i = 0; i < elementCount; i++)
                {
                    bool isMapFilled = occupiedCells.Count >= fillCellCount;
                    if (isMapFilled) break;

                    GameObject mapElementGameObject = Instantiate(data.Prefab);
                    mapElementGameObject.transform.parent = transform;
                    IMapElement mapElement = mapElementGameObject.GetComponent<IMapElement>();
                    mapElement.DisplayName = mapElement.DisplayName + " " + i;
                    Vector2Int cell = new Vector2Int();

                    bool isLookingForCell = true;
                    while (isLookingForCell)
                    {
                        cell.x = Random.Range(0, mapSize.x) + offsetX;
                        cell.y = Random.Range(0, mapSize.y) + offsetY;
                        isLookingForCell = occupiedCells.Contains(cell);
                    }

                    occupiedCells.Add(cell);

                    Vector3 position = map.CellToLocal(cell);
                    mapElement.Position = position;
                }             
            }           
        }
    }
}
