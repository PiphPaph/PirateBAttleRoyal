using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Подключаемся к Photon Master Server с использованием AppID
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");

        // Автоматически заходим в лобби
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");

        // Подключаемся к случайной комнате или создаём её, если не нашли подходящую
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No rooms available, creating a new one");
        
        // Создаём новую комнату, если не смогли присоединиться к случайной
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");

        // Проверяем, был ли уже создан объект для этого игрока
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("HasShip") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["HasShip"])
        {
            Debug.Log("Player already has a ship, skipping spawn.");
            return;
        }

        // Спавним префаб игрока с компонентом PhotonView
        Vector2 randomPosition = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        GameObject playerShip = PhotonNetwork.Instantiate("PlayerShipPrefab", randomPosition, Quaternion.identity);

        // Убедитесь, что начальные параметры сброшены
        Rigidbody2D rb = playerShip.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Устанавливаем свойство, что у игрока теперь есть корабль
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "HasShip", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }
}
