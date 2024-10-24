using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width = 25;
    public int height = 25;
    public float scale = 30f;

    public GameObject waterPrefab;
    public GameObject isLandedPrefab;
    public PlayerSpawner playerSpawner;

    private Dictionary<Vector2, GameObject> spawnedObjects = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        GenerateMap();
        playerSpawner.SpawnPlayers(width, height);
    }

    void GenerateMap()
    {
        // Сначала создаем всю карту с водой
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x - width / 2f, y - height / 2f);
                GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                spawnedObjects[position] = water;
            }
        }

        // Заполняем границы карты островами
        FillMapBorders();

        // Добавляем случайные острова внутри карты
        AddRandomIslands();
    }

    void FillMapBorders()
    {
        for (int x = 0; x < width; x++)
        {
            // Нижняя граница
            ReplaceWithIsland(new Vector2(x - width / 2f, -height / 2f));
            // Верхняя граница
            ReplaceWithIsland(new Vector2(x - width / 2f, height / 2f - 1));
        }

        for (int y = 0; y < height; y++)
        {
            // Левый край
            ReplaceWithIsland(new Vector2(-width / 2f, y - height / 2f));
            // Правый край
            ReplaceWithIsland(new Vector2(width / 2f - 1, y - height / 2f));
        }
    }

    void AddRandomIslands()
    {
        int islandCount = Random.Range(50, 250); // Увеличиваем количество островов

        for (int i = 0; i < islandCount; i++)
        {
            int x = Random.Range(1 - width / 2, width / 2 - 1);
            int y = Random.Range(1 - height / 2, height / 2 - 1);
            Vector2 position = new Vector2(x, y);

            // Проверяем, что позиция свободна и можем ставить остров
            if (!spawnedObjects.ContainsKey(position))
            {
                GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
                spawnedObjects[position] = island;
            }
        }
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
    }
}
