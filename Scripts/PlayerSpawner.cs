using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Префаб игрока должен быть зарегистрирован в Photon Resources
    public int numberOfPlayers = 4; // Количество игроков
    public float spawnDistanceFromEdge = 10f; // Расстояние от края

    public void SpawnPlayers(int width, int height)
    {
        
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector2 spawnPoint;
            bool found = false;

            // Пытаемся найти позицию для спавна
            for (int attempts = 0; attempts < 100; attempts++)
            {
                float x = Random.Range(spawnDistanceFromEdge, width - spawnDistanceFromEdge);
                float y = Random.Range(spawnDistanceFromEdge, height - spawnDistanceFromEdge);
                spawnPoint = new Vector2(x, y);

                if (IsWater(spawnPoint))
                {
                    // Используем PhotonNetwork.Instantiate вместо обычного Instantiate
                    PhotonNetwork.Instantiate("PlayerShipPrefab", spawnPoint, Quaternion.identity);
                    found = true;
                    Debug.Log($"Player spawned at {spawnPoint}"); // Логирование успешного спавна
                    break; // Выход из цикла попыток
                }
                else
                {
                    Debug.Log($"Attempt {attempts}: No water at {spawnPoint}");
                }
            }

            if (!found)
            {
                Debug.LogWarning("Не удалось найти место для спавна игрока.");
            }
        }
    }

    bool IsWater(Vector2 point)
    {
        // Проверяем, находится ли точка в воде
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, 0.5f); // Увеличил радиус для проверки
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Water"))
            {
                return true;
            }
        }
        return false;
    }
}
