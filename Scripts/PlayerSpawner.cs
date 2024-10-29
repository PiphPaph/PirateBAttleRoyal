using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // Префаб игрока должен быть зарегистрирован в Photon Resources
    public int maxNumberOfPlayers = 4; // Максимальное количество игроков
    public int minNumberOfPlayers = 2; // Минимальное количество игроков для начала игры
    public float spawnDistanceFromEdge = 10f; // Расстояние от края
    public float waitTimeBeforeStart = 10f; // Время ожидания перед началом игры

    private bool gameStarted = false;
    private int mapWidth;
    private int mapHeight;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Только мастер-клиент управляет спавном и стартом игры
        {
            StartCoroutine(CheckForStartGame());
        }
    }

    public void SetMapSize(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;
    }

    IEnumerator CheckForStartGame()
    {
        float timer = waitTimeBeforeStart;
        while (PhotonNetwork.CurrentRoom.PlayerCount < minNumberOfPlayers && timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount >= minNumberOfPlayers)
        {
            gameStarted = true;
            SpawnPlayers();
        }
        else
        {
            Debug.LogWarning("Недостаточно игроков для старта игры. Игра не начнётся.");
        }
    }

    public void SpawnPlayers()
    {
        if (!gameStarted) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal)
            {
                Vector2 spawnPoint;
                bool found = false;

                // Пытаемся найти позицию для спавна на воде
                for (int attempts = 0; attempts < 100; attempts++)
                {
                    // Генерация случайной позиции с учетом расстояния от края
                    float x = Random.Range(spawnDistanceFromEdge, mapWidth - spawnDistanceFromEdge);
                    float y = Random.Range(spawnDistanceFromEdge, mapHeight - spawnDistanceFromEdge);
                    spawnPoint = new Vector2(x - mapWidth / 2f, y - mapHeight / 2f); // Учёт центра карты

                    // Проверка, что позиция находится на воде
                    if (IsWater(spawnPoint))
                    {
                        // Используем PhotonNetwork.Instantiate для создания игрока
                        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
                        found = true;
                        Debug.Log($"Player spawned at {spawnPoint}");
                        break;
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
    }

    bool IsWater(Vector2 point)
    {
        // Проверяем, находится ли точка в воде
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, 0.1f); // Используем малый радиус для точной проверки
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Water"))
            {
                return true;
            }
        }
        return false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room. Current player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room. Current player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }
}
