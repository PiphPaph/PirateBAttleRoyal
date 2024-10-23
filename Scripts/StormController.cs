using UnityEngine;

public class StormController : MonoBehaviour
{
    public float startRadius = 50f; // Начальный радиус бури
    public float finalRadius = 0f; // Радиус в конце, когда буря полностью сузится
    public float shrinkDuration = 120f; // Время, за которое буря полностью сузится
    public Vector3 centerPoint = Vector3.zero; // Центр карты (координаты 0,0)
    private float currentRadius; // Текущий радиус бури
    private float timeElapsed = 0f; // Время, прошедшее с начала сужения

    // Для визуализации границы бури
    public GameObject stormVisualPrefab; // Префаб для визуализации
    private GameObject stormVisual; // Инстанцированный объект бури

    void Start()
    {
        // Устанавливаем начальный радиус бури
        currentRadius = startRadius;

        // Инстанцируем префаб, если он не существует на сцене
        stormVisual = Instantiate(stormVisualPrefab, centerPoint, Quaternion.identity);
        
        // Устанавливаем начальный размер для визуализации
        CircleRenderer circleRenderer = stormVisual.GetComponent<CircleRenderer>();
        if (circleRenderer != null)
        {
            circleRenderer.UpdateRadius(currentRadius); // Устанавливаем радиус
        }
    }

    void Update()
    {
        // Постепенное сужение бури
        if (timeElapsed < shrinkDuration)
        {
            timeElapsed += Time.deltaTime;

            // Рассчитываем текущий радиус в зависимости от времени
            currentRadius = Mathf.Lerp(startRadius, finalRadius, timeElapsed / shrinkDuration);

            // Обновляем радиус визуализации через CircleRenderer
            CircleRenderer circleRenderer = stormVisual.GetComponent<CircleRenderer>();
            if (circleRenderer != null)
            {
                circleRenderer.UpdateRadius(currentRadius);
            }
        }

        // Проверяем состояние всех игроков
        CheckPlayersOutsideStorm();
    }

    private void CheckPlayersOutsideStorm()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            float distanceFromCenter = Vector3.Distance(player.transform.position, stormVisual.transform.position); // Проверяем расстояние до позиции stormVisual

            HealthController healthController = player.GetComponent<HealthController>();

            if (healthController != null)
            {
                if (distanceFromCenter > currentRadius)
                {
                    healthController.StartTakingDamage(); // Игрок получает урон
                }
                else
                {
                    healthController.StopTakingDamage(); // Игрок не получает урон
                }
            }
        }
    }
}
