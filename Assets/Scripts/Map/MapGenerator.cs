using Shooter.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Required prefabs")]
        [SerializeField] GameObject tilePrefab;
        [SerializeField] GameObject obsticlePrefab;

        [Header("Basic Map Generation properties")]
        [SerializeField] GameObject navMeshFloor;
        [SerializeField] float tileSize;
        [Range(0, 100)]
        [SerializeField] int outlinePercent;

        [Header("Maps")]
        [SerializeField] Map[] maps;
        [SerializeField] int mapIndex;

        Coordinate mapCenter;
        List<Coordinate> allTileCoordinates;
        Queue<Coordinate> shuffledTileCoordinates;
        Queue<Coordinate> shuffledOpenTileCoordinates;
        Transform[,] tileMap;
        

        private void Start()
        {
            GenerateMap();
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        }
        void OnNewWave(int waveNumber)
        {
            mapIndex = waveNumber - 1;
            GenerateMap();
        }
        public void GenerateMap()
        {
            CreateAndFillDataStructures();
            Transform mapHolder = ClearMapHolder();
            LayTiles(mapHolder);
            BuildWalls(mapHolder);
            BuildBoundries(mapHolder);
            EditNavMeshQuad();
        }
        private void CreateAndFillDataStructures()
        {
            
            allTileCoordinates = new List<Coordinate>();
            for (int x = 0; x < maps[mapIndex].mapSize.x; x++)
            {
                for (int y = 0; y < maps[mapIndex].mapSize.y; y++)
                {
                    allTileCoordinates.Add(new Coordinate(x, y));
                }
            }
            shuffledTileCoordinates = new Queue<Coordinate>(Utility.ShuffleArray(allTileCoordinates.ToArray(), maps[mapIndex].seed));
            mapCenter = new Coordinate(maps[mapIndex].mapSize.x / 2, maps[mapIndex].mapSize.y / 2);
            tileMap = new Transform[maps[mapIndex].mapSize.x, maps[mapIndex].mapSize.y];
        }
        private Transform ClearMapHolder()
        {
            string holderName = "GeneratedMap";
            if (transform.Find(holderName))
            {
                DestroyImmediate(transform.Find(holderName).gameObject);
            }
            Transform mapHolder = new GameObject(holderName).transform;
            mapHolder.parent = transform;
            return mapHolder;
        }
        private void LayTiles(Transform mapHolder)
        {
            for (int x = 0; x < maps[mapIndex].mapSize.x; x++)
            {
                for (int y = 0; y < maps[mapIndex].mapSize.y; y++)
                {
                    Vector3 tilePosition = CoordinateToPosition(x, y);
                    //print("Making tile at: (" + tilePosition.x + ", " + tilePosition.z + ")");
                    GameObject newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                    newTile.transform.localScale = Vector3.one * (1 - ((float)outlinePercent / 100)) * tileSize;
                    newTile.transform.parent = mapHolder;
                    tileMap[x, y] = newTile.transform;
                }
            }
        }
        private void BuildWalls(Transform mapHolder)
        {
            bool[,] obsticleMap = new bool[maps[mapIndex].mapSize.x, maps[mapIndex].mapSize.y];
            List<Coordinate> allOpenCoordinates = new List<Coordinate>(allTileCoordinates);
            int obsticleCount = (int)(maps[mapIndex].mapSize.x * maps[mapIndex].mapSize.y * (float)maps[mapIndex].obsticlePercent / 100);
            int currentObsticleCount = 0;
            for (int i = 0; i < obsticleCount; i++)
            {
                Coordinate randomCoord = GetRandomCoordinate();
                obsticleMap[randomCoord.x, randomCoord.y] = true;
                currentObsticleCount++;
                if (randomCoord != mapCenter && MapIsFullyAccessable(obsticleMap, currentObsticleCount))
                {
                    Vector3 obsticlePosition = CoordinateToPosition(randomCoord.x, randomCoord.y);
                    GameObject newObsticle = Instantiate(obsticlePrefab, obsticlePosition + Vector3.up * (tileSize / 2), Quaternion.identity);
                    newObsticle.transform.parent = mapHolder;
                    newObsticle.transform.localScale = Vector3.one * (1 - ((float)outlinePercent / 100)) * tileSize;

                    allOpenCoordinates.Remove(randomCoord);
                }
                else
                {
                    obsticleMap[randomCoord.x, randomCoord.y] = false;
                    currentObsticleCount--;
                }
            }

            //fill open tile queues
            shuffledOpenTileCoordinates = new Queue<Coordinate>(Utility.ShuffleArray(allOpenCoordinates.ToArray(), maps[mapIndex].seed));
        }
        private void BuildBoundries(Transform mapHolder)
        {
            GameObject boundryLeft = Instantiate(obsticlePrefab, new Vector3(-((maps[mapIndex].mapSize.x * tileSize) / 2 ), tileSize/2, tileSize / 2), Quaternion.identity);
            boundryLeft.transform.parent = mapHolder;
            boundryLeft.transform.localScale = new Vector3(1, 1, maps[mapIndex].mapSize.y) * tileSize;

            GameObject boundryRight = Instantiate(obsticlePrefab, new Vector3(((maps[mapIndex].mapSize.x * tileSize) / 2 ), tileSize / 2, tileSize / 2), Quaternion.identity);
            boundryRight.transform.parent = mapHolder;
            boundryRight.transform.localScale = new Vector3(1, 1, maps[mapIndex].mapSize.y) * tileSize;

            GameObject boundryUp = Instantiate(obsticlePrefab, new Vector3(tileSize / 2, tileSize / 2, ((maps[mapIndex].mapSize.y * tileSize) / 2)), Quaternion.Euler(Vector3.up * 90));
            boundryUp.transform.parent = mapHolder;
            boundryUp.transform.localScale = new Vector3(1, 1, maps[mapIndex].mapSize.x) * tileSize;

            GameObject boundryDown = Instantiate(obsticlePrefab, new Vector3(tileSize / 2, tileSize / 2, -((maps[mapIndex].mapSize.y * tileSize) / 2) ), Quaternion.Euler(Vector3.up * 90));
            boundryDown.transform.parent = mapHolder;
            boundryDown.transform.localScale = new Vector3(1, 1, maps[mapIndex].mapSize.x) * tileSize;
        }

        private void EditNavMeshQuad()
        {
            navMeshFloor.transform.position = new Vector3(tileSize / 2, 0, tileSize / 2);
            navMeshFloor.transform.localScale = new Vector3(maps[mapIndex].mapSize.x, maps[mapIndex].mapSize.y) * tileSize;
        }

        bool MapIsFullyAccessable(bool[,] obsticleMap, int currentObsticleCount)
        {
            bool[,] mapFlags = new bool[obsticleMap.GetLength(0), obsticleMap.GetLength(1)];
            Queue<Coordinate> queue = new Queue<Coordinate>();
            queue.Enqueue(mapCenter);
            mapFlags[mapCenter.x, mapCenter.y] = true;
            int accessibleTileCount = 1;
            while (queue.Count > 0)
            {
                Coordinate tile = queue.Dequeue();
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int neighborX = tile.x + x;
                        int neighborY = tile.y + y;
                        if (x == 0 || y == 0)
                        {
                            if (neighborX >= 0 && neighborX < obsticleMap.GetLength(0) 
                                && neighborY >= 0 && neighborY < obsticleMap.GetLength(1))
                            {
                                if(!mapFlags[neighborX, neighborY] && !obsticleMap[neighborX, neighborY])
                                {
                                    mapFlags[neighborX, neighborY] = true;
                                    queue.Enqueue(new Coordinate(neighborX, neighborY));
                                    accessibleTileCount++;
                                }
                            }
                        }
                    }
                }
            }
            int targetTileCount = (maps[mapIndex].mapSize.x * maps[mapIndex].mapSize.y) - currentObsticleCount;
            return targetTileCount == accessibleTileCount;
        }
        private Vector3 CoordinateToPosition(int x, int y)
        {
            return new Vector3(-maps[mapIndex].mapSize.x / 2 + 0.5f + x, 0, -maps[mapIndex].mapSize.y / 2 + 0.5f + y) * tileSize;
        }
        public Transform PositionToTile(Vector3 positionToTranslate)
        {
            int x = Mathf.RoundToInt((positionToTranslate.x / tileSize) + (maps[mapIndex].mapSize.x - 1) / 2f);
            int y = Mathf.RoundToInt((positionToTranslate.z / tileSize) + (maps[mapIndex].mapSize.y - 1) / 2f);
            if(tileMap != null)
            {
                x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
                y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
                return tileMap[x, y];
            }
            return null;
        }

        public Coordinate GetRandomCoordinate()
        {
            Coordinate coordinate = shuffledTileCoordinates.Dequeue();
            shuffledTileCoordinates.Enqueue(coordinate);
            return coordinate;
        }
        public Transform GetRandomOpenTile()
        {
            Coordinate coordinate = shuffledOpenTileCoordinates.Dequeue();
            shuffledOpenTileCoordinates.Enqueue(coordinate);
            return tileMap[coordinate.x, coordinate.y];
        }
        [System.Serializable]
        public class Map
        {
            public Vector2Int mapSize;
            [Range(0,100)]
            public int obsticlePercent;
            public int seed;
        }

        public struct Coordinate
        {
            public int x;
            public int y;
            public Coordinate(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
            public override bool Equals(object obj)
            {
                return obj is Coordinate && this == (Coordinate)obj;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public static bool operator ==(Coordinate left, Coordinate right)
            {
                return ((left.x == right.x) && (left.y == right.y));
            }
            public static bool operator !=(Coordinate left, Coordinate right)
            {
                return !(left == right);
            }
        }
    }
}
