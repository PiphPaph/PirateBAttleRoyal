using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer; // Ссылка на LineRenderer
    public int segments = 90; // Количество сегментов
    public float radius = 5f; // Радиус окружности
    public float lineWidth = 0.1f; // Ширина линии

    void Start()
    {
        DrawCircle(); // Начальный вызов для рисования окружности
    }

    void DrawCircle()
    {
        // Настройка LineRenderer
        lineRenderer.positionCount = segments + 1; // Количество точек
        lineRenderer.useWorldSpace = false; // Используем локальные координаты
        lineRenderer.startWidth = lineWidth; // Устанавливаем ширину линии
        lineRenderer.endWidth = lineWidth;

        float angle = 0f;
        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius; // Рассчитываем координаты
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f)); // Устанавливаем позицию
            angle += 360f / segments; // Увеличиваем угол
        }
    }

    public void UpdateRadius(float newRadius)
    {
        radius = newRadius; // Обновляем радиус
        DrawCircle(); // Перерисовываем круг с новым радиусом
    }
}