using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width = 25;
    public int height = 25;
    public float scale = 30f; //Масштаб для генерации случайных значений шума (Perlin noise), который определяет, будут ли острова на определённой позиции или вода

    public GameObject waterPrefab;
    public GameObject isLandedPrefab;
    public PlayerSpawner playerSpawner; // Ссылка на скрипт спавна

    private Dictionary<Vector2, GameObject> spawnedObjects = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        GenerateMap();
        playerSpawner.SpawnPlayers(width, height); // Вызываем метод спавна
    }

    void GenerateMap()
    {
        // Вычисляем смещение для центрирования карты
        float offsetX = width / 2f;
        float offsetY = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                // Используем смещение, чтобы центрировать карту
                Vector2 position = new Vector2(x - offsetX, y - offsetY);

                // Проверяем, занято ли уже место
                if (spawnedObjects.ContainsKey(position))
                {
                    continue;
                }

                if (sample > 0.5f)
                {
                    GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
                    spawnedObjects[position] = island;
                }
                else
                {
                    GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                    spawnedObjects[position] = water;
                }
            }
        }

        FillMapBorders();
    }

    void FillMapBorders()
    {
        // Вычисляем смещение для центрирования границ
        float offsetX = width / 2f;
        float offsetY = height / 2f;

        for (int x = 0; x < width; x++)
        {
            Vector2 bottomBorder = new Vector2(x - offsetX, -offsetY);
            Vector2 topBorder = new Vector2(x - offsetX, height - 1 - offsetY);

            // Если на границе уже есть объект, заменяем его на остров
            ReplaceWithIsland(bottomBorder);
            ReplaceWithIsland(topBorder);
        }

        for (int y = 0; y < height; y++)
        {
            Vector2 leftBorder = new Vector2(-offsetX, y - offsetY);
            Vector2 rightBorder = new Vector2(width - 1 - offsetX, y - offsetY);

            ReplaceWithIsland(leftBorder);
            ReplaceWithIsland(rightBorder);
        }
    }

    // Метод для замены объекта на остров
    void ReplaceWithIsland(Vector2 position)
    {
        // Если на этой позиции уже есть объект, удаляем его
        if (spawnedObjects.ContainsKey(position))
        {
            Destroy(spawnedObjects[position]);
            spawnedObjects.Remove(position);
        }

        // Создаем остров на этой позиции
        GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
        spawnedObjects[position] = island;
    }
}
