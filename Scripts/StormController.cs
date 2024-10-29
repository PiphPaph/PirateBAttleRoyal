using UnityEngine;

public class StormController : MonoBehaviour
{
    [Header("Storm Configuration")]
    public float startRadius = 50f; // Начальный радиус бури
    public float finalRadius = 0.06f; // Радиус в конце, когда буря полностью сузится
    public float shrinkDuration = 120f; // Время, за которое буря полностью сузится

    private Vector3 centerPoint; // Центр бури
    private float currentRadius; // Текущий радиус бури
    private float timeElapsed; // Время, прошедшее с начала сужения

    public GameObject stormVisualPrefab; // Префаб для визуализации
    private GameObject stormVisual; // Инстанцированный объект бури

    private MapGenerator mapGenerator;

    void Start()
    {
        // Получаем ссылку на MapGenerator
        mapGenerator = FindObjectOfType<MapGenerator>();
        mapGenerator.GenerateMap();
        SetStormCenter();

        // Устанавливаем начальный радиус бури
        currentRadius = startRadius;

        // Инстанцируем префаб, если он не существует на сцене
        stormVisual = Instantiate(stormVisualPrefab, centerPoint, Quaternion.identity);

        // Устанавливаем начальный размер для визуализации
        UpdateStormVisual();
    }

    void Update()
    {
        ShrinkStorm(); // Обновляем состояние бури
        CheckPlayersOutsideStorm(); // Проверяем состояние всех игроков
    }

    public void SetStormCenter()
    {
        if (mapGenerator != null)
        {
            Vector2 randomWaterPosition = mapGenerator.GetRandomWaterPosition();
            Debug.Log($"Полученная позиция воды: {randomWaterPosition}");

            // Проверяем, если позиция воды валидна
            if (randomWaterPosition != Vector2.zero) // Если позиция не (0, 0)
            {
                centerPoint = new Vector3(randomWaterPosition.x, randomWaterPosition.y, 0f);
            }
            else
            {
                Debug.LogWarning("Нет доступной позиции воды для установки центра бури. Используется позиция (0, 0, 0).");
                centerPoint = Vector3.zero; // Возвращаем к (0, 0, 0) по умолчанию
            }
        }
        else
        {
            Debug.LogWarning("MapGenerator не найден. Центр бури установлен в (0, 0, 0).");
            centerPoint = Vector3.zero;
        }
    }


    private void ShrinkStorm()
    {
        if (timeElapsed < shrinkDuration)
        {
            timeElapsed += Time.deltaTime;
            currentRadius = Mathf.Lerp(startRadius, finalRadius, timeElapsed / shrinkDuration);
            UpdateStormVisual(); // Обновляем радиус визуализации
        }
    }

    private void UpdateStormVisual()
    {
        if (stormVisual != null)
        {
            CircleRenderer circleRenderer = stormVisual.GetComponent<CircleRenderer>();
            if (circleRenderer != null)
            {
                circleRenderer.UpdateRadius(currentRadius); // Устанавливаем радиус
            }
            else
            {
                Debug.LogWarning("CircleRenderer не найден в префабе бури.");
            }
        }
    }

    private void CheckPlayersOutsideStorm()
    {
        // Проверяем всех игроков на наличие за пределами бури
        foreach (var player in FindObjectsOfType<PlayerShip>()) // Замените на ваш класс управления игроком
        {
            if (Vector3.Distance(player.transform.position, centerPoint) > currentRadius)
            {
                // Получаем компонент HealthController и наносим урон
                HealthController healthController = player.GetComponent<HealthController>();
                if (healthController != null)
                {
                    healthController.TakeDamage(1 * Time.deltaTime); // Метод для получения урона
                }
                else
                {
                    Debug.LogWarning("HealthController не найден на объекте игрока.");
                }
            }
        }
    }
}
