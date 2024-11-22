using UnityEngine;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        // Подключаемся к серверу Photon
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        // Переход в лобби после подключения
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}