using Shooter.Core;
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
        [SerializeField] Vector2Int mapSize;
        [SerializeField] float tileSize;
        [Range(0, 1)]
        [SerializeField] float outlinePercent;
        [Range(0, 1)]
        [SerializeField] float obsticlePercent;
        [SerializeField] int seed = 0;

        Coordinate mapCenter;

        List<Coordinate> allTileCoordinates;
        Queue<Coordinate> shuffledTileCoordinates;
        

        private void Start()
        {
            GenerateMap();
        }
        public void GenerateMap()
        {
            FillListAndQueue();
            Transform mapHolder = ClearMapHolder();
            LayTiles(mapHolder);
            BuildWalls(mapHolder);
            EditNavMeshQuad();
        }

        private void EditNavMeshQuad()
        {
            navMeshFloor.transform.position = new Vector3(tileSize / 2, 0, tileSize / 2);
            navMeshFloor.transform.localScale = new Vector3(mapSize.x, mapSize.y) * tileSize;
        }

        private void FillListAndQueue()
        {
            allTileCoordinates = new List<Coordinate>();
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    allTileCoordinates.Add(new Coordinate(x, y));
                }
            }
            shuffledTileCoordinates = new Queue<Coordinate>(Utility.ShuffleArray(allTileCoordinates.ToArray(), seed));
            mapCenter = new Coordinate(mapSize.x / 2, mapSize.y / 2);
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
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    Vector3 tilePosition = CoordinateToPosition(x, y);
                    //print("Making tile at: (" + tilePosition.x + ", " + tilePosition.z + ")");
                    GameObject newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                    newTile.transform.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                    newTile.transform.parent = mapHolder;
                }
            }
        }

        private void BuildWalls(Transform mapHolder)
        {
            bool[,] obsticleMap = new bool[mapSize.x, mapSize.y];
            int obsticleCount = (int)(mapSize.x * mapSize.y * obsticlePercent);
            int currentObsticleCount = 0;
            for (int i = 0; i < obsticleCount; i++)
            {
                Coordinate randomCoord = GetCoordinate();
                obsticleMap[randomCoord.x, randomCoord.y] = true;
                currentObsticleCount++;
                if (randomCoord != mapCenter && MapIsFullyAccessable(obsticleMap, currentObsticleCount))
                {
                    Vector3 obsticlePosition = CoordinateToPosition(randomCoord.x, randomCoord.y);
                    GameObject newObsticle = Instantiate(obsticlePrefab, obsticlePosition + Vector3.up * (tileSize/2), Quaternion.identity);
                    newObsticle.transform.parent = mapHolder;
                    newObsticle.transform.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                }
                else
                {
                    obsticleMap[randomCoord.x, randomCoord.y] = false;
                    currentObsticleCount--;
                }
            }
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
            int targetTileCount = (mapSize.x * mapSize.y) - currentObsticleCount;
            return targetTileCount == accessibleTileCount;
        }
        private Vector3 CoordinateToPosition(int x, int y)
        {
            return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * tileSize;
        }

        public Coordinate GetCoordinate()
        {
            Coordinate coordinate = shuffledTileCoordinates.Dequeue();
            shuffledTileCoordinates.Enqueue(coordinate);
            return coordinate;
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
