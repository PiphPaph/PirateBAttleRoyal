using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public UIManager uiManager;    // Ссылка на UIManager
    public float waitTime = 15f;  // Время ожидания в секундах
    private float timer = 0f;      // Таймер для отсчета времени
    private bool gameStarted = false;  // Флаг для отслеживания старта игры
    
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // Показываем сообщение об ожидании при старте
        uiManager.ShowMessage("Ожидание других игроков...");
        uiManager.HideExitButton();  // Прячем кнопку выхода
    }

    void Update()
    {
        if (gameStarted) return;

        timer += Time.deltaTime;

        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            // Если игроков недостаточно, показываем сообщение и кнопку выхода
            uiManager.ShowMessage("Недостаточно игроков для начала игры");
            uiManager.ShowExitButton();
        }
        else
        {
            // Скрываем сообщение и кнопку, если условия выполнены
            uiManager.HideMessage();
            uiManager.HideExitButton();
        }

        if (timer >= waitTime && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            // Если время ожидания истекло и игроков недостаточно
            uiManager.ShowMessage("Время ожидания истекло");
            uiManager.ShowExitButton();

            // Выходим из комнаты и возвращаемся в главное меню
            PhotonNetwork.LeaveRoom();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Игрок {newPlayer.NickName} вошёл в комнату. Всего игроков: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && !gameStarted)
        {
            // Условие выполнено, запускаем игру
            StartGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Игрок {otherPlayer.NickName} покинул комнату. Всего игроков: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    void StartGame()
    {
        gameStarted = true;  // Помечаем, что игра началась
        if (PhotonNetwork.IsMasterClient)
        {
            // Загружаем игровую сцену
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}
