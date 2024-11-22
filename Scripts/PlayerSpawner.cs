using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // Префаб игрока, зарегистрированный в Photon Resources
    public int maxNumberOfPlayers = 4; // Максимальное количество игроков
    public int minNumberOfPlayers = 2; // Минимальное количество игроков для начала игры
    public float spawnDistanceFromEdge = 10f; // Расстояние от края карты

    private int mapWidth;
    private int mapHeight;
    private List<Vector2> waterPositions; // Список позиций воды

    public void Initialize(int width, int height, List<Vector2> waterPositions)
    {
        this.mapWidth = width;
        this.mapHeight = height;
        this.waterPositions = waterPositions;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsLocal)
        {
            StartCoroutine(WaitForPlayersAndSpawn());
        }
    }

    IEnumerator WaitForPlayersAndSpawn()
    {
        // Ждём минимального количества игроков
        while (PhotonNetwork.CurrentRoom.PlayerCount < minNumberOfPlayers)
        {
            Debug.Log($"Ожидание других игроков... Текущие игроки: {PhotonNetwork.CurrentRoom.PlayerCount}/{minNumberOfPlayers}");
            yield return new WaitForSeconds(1f);
        }

        // Убедимся, что каждый игрок спавнит только своего корабля
        if (PhotonNetwork.LocalPlayer.IsMasterClient || !AlreadySpawned())
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Vector2 spawnPoint;
        bool found = false;

        for (int attempts = 0; attempts < 100; attempts++)
        {
            float x = Random.Range(spawnDistanceFromEdge, mapWidth - spawnDistanceFromEdge);
            float y = Random.Range(spawnDistanceFromEdge, mapHeight - spawnDistanceFromEdge);
            spawnPoint = new Vector2(x - mapWidth / 2f, y - mapHeight / 2f); // Учёт центра карты

            if (IsWater(spawnPoint))
            {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
                found = true;
                Debug.Log($"Player spawned at {spawnPoint}");
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Не удалось найти место для спавна игрока.");
        }
    }

    bool IsWater(Vector2 point)
    {
        if (waterPositions == null || waterPositions.Count == 0)
        {
            Debug.LogWarning("Список позиций воды пуст.");
            return false;
        }

        foreach (Vector2 waterPosition in waterPositions)
        {
            if (Vector2.Distance(point, waterPosition) < 0.5f) // Допускаем небольшую погрешность
            {
                return true;
            }
        }

        return false;
    }

    private bool AlreadySpawned()
    {
        // Проверяем, есть ли объект игрока, принадлежащий текущему клиенту
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player"); // Убедитесь, что префабы имеют тег "Player"
        foreach (var player in allPlayers)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
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
