using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width = 25;
    public int height = 25;

    public GameObject waterPrefab;
    public GameObject isLandedPrefab;
    public PlayerSpawner playerSpawner;

    private Dictionary<Vector2, GameObject> spawnedObjects = new Dictionary<Vector2, GameObject>();
    private List<Vector2> waterPositions = new List<Vector2>(); // Список всех клеток с водой

    void Start()
    {
        //GenerateMap();
        playerSpawner.SpawnPlayers();
    }

    public void GenerateMap()
    {
        // Создаем карту с водой и сохраняем координаты всех клеток воды
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                Vector2 position = new Vector2(x, y);
                GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                spawnedObjects[position] = water;
                waterPositions.Add(position); // Добавляем координаты воды
            }
        }
        
        // Заполняем границы карты островами
        FillMapBorders();
        
        // Добавляем случайные острова внутри карты
        AddRandomIslands();
    }

    void FillMapBorders()
    {
        // Заполнение верхней и нижней границы карты
        for (int x = -width / 2; x < width / 2; x++)
        {
            Vector2 topPosition = new Vector2(x, height / 2 - 1); // Верхняя граница
            Vector2 bottomPosition = new Vector2(x, -height / 2); // Нижняя граница

            PlaceIslandIfWater(topPosition);
            PlaceIslandIfWater(bottomPosition);
        }

        // Заполнение левой и правой границы карты
        for (int y = -height / 2; y < height / 2; y++)
        {
            Vector2 leftPosition = new Vector2(-width / 2, y); // Левая граница
            Vector2 rightPosition = new Vector2(width / 2 - 1, y); // Правая граница

            PlaceIslandIfWater(leftPosition);
            PlaceIslandIfWater(rightPosition);
        }
    }

    void PlaceIslandIfWater(Vector2 position)
    {
        // Проверяем, что в этой позиции находится вода, и можем разместить остров
        if (spawnedObjects.TryGetValue(position, out GameObject obj) && obj.CompareTag("Water"))
        {
            // Уничтожаем воду и ставим остров
            Destroy(obj);
            GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
            spawnedObjects[position] = island;
            waterPositions.Remove(position); // Убираем эту позицию из списка воды
        }
    }

    void AddRandomIslands()
    {
    int islandCount = Random.Range(10, 25); // Количество отдельных островов

    for (int i = 0; i < islandCount; i++)
    {
        // Случайная начальная позиция для острова (не на границах)
        int startX = Random.Range(-width / 2 + 2, width / 2 - 2);
        int startY = Random.Range(-height / 2 + 2, height / 2 - 2);
        Vector2 startPosition = new Vector2(startX, startY);

        // Проверяем, что начальная позиция является водой
        if (spawnedObjects.TryGetValue(startPosition, out GameObject obj) && obj.CompareTag("Water"))
        {
            // Генерируем остров произвольной формы с размером от 4 до 12 клеток
            GenerateIsland(startPosition, Random.Range(3, 6));
        }
    }
    }

    private void GenerateIsland(Vector2 startPosition, int islandSize)
    {
    Queue<Vector2> positions = new Queue<Vector2>();
    positions.Enqueue(startPosition);

    HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // Для отслеживания занятых позиций
    occupiedPositions.Add(startPosition);
    ReplaceWithIsland(startPosition); // Размещаем первый участок острова

    while (positions.Count > 0 && occupiedPositions.Count < islandSize)
    {
        Vector2 position = positions.Dequeue();

        // Добавляем соседние позиции
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Пропускаем саму позицию

                Vector2 newPos = position + new Vector2(dx, dy);

                // Проверяем, что новая позиция находится в пределах карты и является водой
                if (IsInBounds(newPos) && spawnedObjects.TryGetValue(newPos, out GameObject obj) && obj.CompareTag("Water"))
                {
                    // Добавляем новую позицию к острову
                    if (!occupiedPositions.Contains(newPos))
                    {
                        positions.Enqueue(newPos);
                        occupiedPositions.Add(newPos);
                        ReplaceWithIsland(newPos); // Размещаем новый участок острова
                    }
                }
            }
        }
    }
}

private bool IsInBounds(Vector2 position)
{
    return position.x >= -width / 2 + 1 && position.x < width / 2 - 1 && position.y >= -height / 2 + 1 && position.y < height / 2 - 1;
}

void ReplaceWithIsland(Vector2 position)
{
    // Уничтожаем старый объект, если он есть
    if (spawnedObjects.ContainsKey(position))
    {
        Destroy(spawnedObjects[position]);
        spawnedObjects.Remove(position);
    }

    // Создаем новый объект острова
    GameObject island = Instantiate(isLandedPrefab, position, Quaternion.identity);
    spawnedObjects[position] = island;
    waterPositions.Remove(position); // Убираем эту позицию из списка воды
}


    public Vector2 GetRandomWaterPosition()
    {
        // Используем уже существующий список waterPositions
        if (waterPositions.Count > 0)
        {
            return waterPositions[Random.Range(0, waterPositions.Count)];
        }

        // Если нет позиций воды, возвращаем (0, 0)
        return Vector2.zero;
    }
}
