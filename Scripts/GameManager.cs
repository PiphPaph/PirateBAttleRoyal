using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.LogError("Игрок не подключён к комнате!");
            PhotonNetwork.LoadLevel("MainMenuScene");
        }
    }

    void SpawnPlayer()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        PhotonNetwork.Instantiate("PlayerShipPrefab", spawnPosition, Quaternion.identity);
    }
}