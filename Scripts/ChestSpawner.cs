using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    public GameObject chestTypeOnePrefab; // Префаб сундука типа 1 (одна пушка)
    public GameObject chestTypeTwoPrefab; // Префаб сундука типа 2 (две пушки)
    public int numberOfChests = 5; // Количество сундуков
    public float spawnDelay = 2f; // Задержка перед спавном сундуков

    void Start()
    {
        StartCoroutine(SpawnChestsWithDelay());
    }

    IEnumerator SpawnChestsWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay); // Ждем генерацию карты
        SpawnChests(); // Спавним сундуки
    }

    void SpawnChests()
    {
        // Находим все объекты с тегом "Water"
        GameObject[] waterPrefabs = GameObject.FindGameObjectsWithTag("Water");
        List<Vector2> waterPositions = new List<Vector2>();

        // Получаем позиции всех объектов воды
        foreach (GameObject water in waterPrefabs)
        {
            waterPositions.Add(water.transform.position);
        }

        // Спавним сундуки на случайных позициях воды
        for (int i = 0; i < numberOfChests; i++)
        {
            if (waterPositions.Count > 0)
            {
                Vector2 randomPosition = waterPositions[Random.Range(0, waterPositions.Count)];
                GameObject chestPrefab = Random.Range(0, 2) == 0 ? chestTypeOnePrefab : chestTypeTwoPrefab;
                Instantiate(chestPrefab, randomPosition, Quaternion.identity);
            }
        }
    }
}



// using System.Collections;
// using UnityEngine;
//
// public class ChestSpawner : MonoBehaviour
// {
//     public GameObject chestTypeOnePrefab; // Префаб сундука типа 1 (одна пушка)
//     public GameObject chestTypeTwoPrefab; // Префаб сундука типа 2 (две пушки)
//     public int numberOfChests = 5; // Количество сундуков
//     public Vector2 spawnAreaMin; // Минимальные координаты для спавна
//     public Vector2 spawnAreaMax; // Максимальные координаты для спавна
//     public LayerMask waterLayer; // Слой воды для проверки
//     public float spawnDelay = 2f; // Задержка перед спавном сундуков
//
//     void Start()
//     {
//         StartCoroutine(SpawnChestsWithDelay());
//     }
//
//     IEnumerator SpawnChestsWithDelay()
//     {
//         yield return new WaitForSeconds(spawnDelay); // Ждем генерацию карты
//         SpawnChests(); // Спавним сундуки
//     }
//
//     void SpawnChests()
//     {
//         int spawnedChests = 0;
//         int attempts = 0; // Счетчик попыток спавна
//         int maxAttempts = 100; // Максимальное количество попыток спавна
//
//         while (spawnedChests < numberOfChests && attempts < maxAttempts)
//         {
//             Vector2 randomPosition = new Vector2(
//                 Random.Range(spawnAreaMin.x, spawnAreaMax.x),
//                 Random.Range(spawnAreaMin.y, spawnAreaMax.y)
//             );
//
//             // Проверяем, находится ли точка на воде
//             if (IsPositionOnWater(randomPosition))
//             {
//                 GameObject chestPrefab = Random.Range(0, 2) == 0 ? chestTypeOnePrefab : chestTypeTwoPrefab;
//                 Instantiate(chestPrefab, randomPosition, Quaternion.identity);
//                 spawnedChests++; // Увеличиваем счетчик заспавненных сундуков
//             }
//             attempts++; // Увеличиваем счетчик попыток
//         }
//
//         if (attempts >= maxAttempts)
//         {
//             Debug.LogWarning("Не удалось заспавнить все сундуки, достигнуто максимальное количество попыток.");
//         }
//     }
//
//     bool IsPositionOnWater(Vector2 position)
//     {
//         // Проверяем, пересекается ли точка с коллайдером слоя воды
//         Collider2D hit = Physics2D.OverlapPoint(position, waterLayer);
//         return hit != null; // Возвращаем true, если в точке есть вода
//     }
// }
