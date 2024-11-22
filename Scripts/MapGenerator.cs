using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapGenerator : MonoBehaviourPunCallbacks
{
    public int width = 25;
    public int height = 25;

    public GameObject waterPrefab;
    public GameObject isLandedPrefab;
    public PlayerSpawner playerSpawner;

    private Dictionary<Vector2, GameObject> spawnedObjects = new Dictionary<Vector2, GameObject>();
    private List<Vector2> waterPositions = new List<Vector2>();

    private int mapSeed;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            mapSeed = Random.Range(0, 10000);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "MapSeed", mapSeed } });
        }
        else
        {
            mapSeed = (int)PhotonNetwork.CurrentRoom.CustomProperties["MapSeed"];
        }

        Random.InitState(mapSeed);
        GenerateMap();

        // Передаём данные о карте в PlayerSpawner
        playerSpawner.Initialize(width, height, new List<Vector2>(waterPositions));

        // Только мастер спавнит игроков
        if (PhotonNetwork.IsMasterClient)
        {
            playerSpawner.SpawnPlayer();
        }
    }

    public void GenerateMap()
    {
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                Vector2 position = new Vector2(x, y);
                GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                spawnedObjects[position] = water;
                waterPositions.Add(position);
            }
        }

        FillMapBorders();
        AddRandomIslands();
    }

    void FillMapBorders()
    {
        for (int x = -width / 2; x < width / 2; x++)
        {
            Vector2 topPosition = new Vector2(x, height / 2 - 1);
            Vector2 bottomPosition = new Vector2(x, -height / 2);
            PlaceIslandIfWater(topPosition);
            PlaceIslandIfWater(bottomPosition);
        }

        for (int y = -height / 2; y < height / 2; y++)
        {
            Vector2 leftPosition = new Vector2(-width / 2, y);
            Vector2 rightPosition = new Vector2(width / 2 - 1, y);
            PlaceIslandIfWater(leftPosition);
            PlaceIslandIfWater(rightPosition);
        }
    }

    void PlaceIslandIfWater(Vector2 position)
    {
        if (spawnedObjects.TryGetValue(position, out GameObject obj) && obj.CompareTag("Water"))
        {
            Destroy(obj);
            GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
            spawnedObjects[position] = island;
            waterPositions.Remove(position);
        }
    }

    void AddRandomIslands()
    {
        int islandCount = Random.Range(10, 20);

        for (int i = 0; i < islandCount; i++)
        {
            int startX = Random.Range(-width / 2 + 2, width / 2 - 2);
            int startY = Random.Range(-height / 2 + 2, height / 2 - 2);
            Vector2 startPosition = new Vector2(startX, startY);

            if (spawnedObjects.TryGetValue(startPosition, out GameObject obj) && obj.CompareTag("Water"))
            {
                GenerateSolidIsland(startPosition, Random.Range(4, 10));
            }
        }
    }

    private void GenerateSolidIsland(Vector2 startPosition, int islandSize)
    {
        Queue<Vector2> positions = new Queue<Vector2>();
        positions.Enqueue(startPosition);

        HashSet<Vector2> occupiedPositions = new HashSet<Vector2> { startPosition };
        ReplaceWithIsland(startPosition);

        int layers = Mathf.CeilToInt(Mathf.Sqrt(islandSize));

        for (int layer = 0; layer < layers; layer++)
        {
            int layerCount = positions.Count;

            for (int i = 0; i < layerCount && occupiedPositions.Count < islandSize; i++)
            {
                Vector2 position = positions.Dequeue();

                foreach (Vector2 offset in GetNeighborOffsets())
                {
                    Vector2 newPos = position + offset;

                    if (IsInBounds(newPos) && spawnedObjects.TryGetValue(newPos, out GameObject obj) && obj.CompareTag("Water") && !occupiedPositions.Contains(newPos))
                    {
                        positions.Enqueue(newPos);
                        occupiedPositions.Add(newPos);
                        ReplaceWithIsland(newPos);
                    }
                }
            }
        }
    }

    private bool IsInBounds(Vector2 position)
    {
        return position.x >= -width / 2 + 1 && position.x < width / 2 - 1 && position.y >= -height / 2 + 1 && position.y < height / 2 - 1;
    }

    private List<Vector2> GetNeighborOffsets()
    {
        return new List<Vector2>
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1),
        };
    }

    void ReplaceWithIsland(Vector2 position)
    {
        if (spawnedObjects.ContainsKey(position))
        {
            Destroy(spawnedObjects[position]);
            spawnedObjects.Remove(position);
        }

        GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
        spawnedObjects[position] = island;
        waterPositions.Remove(position);
    }

    /// <summary>
    /// Возвращает случайную позицию воды.
    /// </summary>
    public Vector2 GetRandomWaterPosition()
    {
        if (waterPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, waterPositions.Count);
            return waterPositions[randomIndex];
        }

        Debug.LogWarning("Нет доступных позиций воды.");
        return Vector2.zero;
    }
}
