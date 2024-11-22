using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject waitingPanel;  // Панель ожидания
    public GameObject exitButton;    // Кнопка выхода
    public TMPro.TextMeshProUGUI messageText; // Текст сообщения

    public void ShowMessage(string message)
    {
        waitingPanel.SetActive(true);
        messageText.text = message;
    }

    public void HideMessage()
    {
        waitingPanel.SetActive(false);
    }

    public void ShowExitButton()
    {
        exitButton.SetActive(true);
    }

    public void HideExitButton()
    {
        exitButton.SetActive(false);
    }

    public void OnExitButtonPressed()
    {
        // Выход из комнаты и возвращение в главное меню
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}