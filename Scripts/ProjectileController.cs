using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, не столкнулся ли снаряд с собственным кораблем
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Не уничтожаем снаряд, если он столкнулся с кораблем игрока
        }

        Destroy(gameObject); // Удаляем снаряд при столкновении с любым другим объектом
    }
}