using UnityEngine;

public class CannonBallDamage : MonoBehaviour
{
    public float damage = 10f; // Урон от снаряда

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthController health = collision.gameObject.GetComponent<HealthController>();
            if (health != null)
            {
                health.TakeDamage(damage); // Наносим урон кораблю
            }
            Destroy(gameObject); // Уничтожаем снаряд после попадания
        }
    }
}