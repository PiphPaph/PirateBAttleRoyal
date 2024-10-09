using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Transform firePoint; // Точка стрельбы носовой пушки
    public GameObject projectilePrefab; // Префаб снаряда
    public float projectileForce = 10f; // Сила выстрела
    public float projectileColliderRadius = 0.08f;

    private ShipController shipController;

    void Start()
    {
        shipController = GetComponentInParent<ShipController>();
    }

    void Update()
    {
        // Стрельба из носовой пушки (всегда доступна)
        if (Input.GetButtonDown("FireNose"))
        {
            FireCannon(firePoint);
        }

        // Стрельба из боковых пушек (только если добавлены)
        if (shipController.HasSideCannons())
        {
            if (Input.GetButtonDown("FireLeft"))
            {
                FireAllCannons(shipController.leftCannonPoint); // Стреляем из всех пушек на левом борту
            }
            if (Input.GetButtonDown("FireRight"))
            {
                FireAllCannons(shipController.rightCannonPoint); // Стреляем из всех пушек на правом борту
            }
        }
    }

    // Метод для стрельбы из всех пушек на борту
    void FireAllCannons(Transform cannonPointParent)
    {
        foreach (Transform cannonPoint in cannonPointParent)
        {
            FireCannon(cannonPoint); // Стреляем из каждой пушки на борту
        }
    }

    // Метод для стрельбы из одной пушки
    void FireCannon(Transform cannonPoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, cannonPoint.position, cannonPoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(cannonPoint.right * projectileForce, ForceMode2D.Impulse);

        // Изменяем размер CircleCollider2D для снаряда
        CircleCollider2D collider = projectile.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = projectileColliderRadius; // Установка радиуса коллайдера
        }
        else
        {
            Debug.LogWarning("CircleCollider2D не найден на снаряде!");
        }

        // Игнорируем коллизию между снарядом и кораблем, который его выпустил
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Collider2D shipCollider = GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(projectileCollider, shipCollider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверка столкновения с островом или другим кораблем
        if (collision.gameObject.CompareTag("Island") || collision.gameObject.CompareTag("Player"))
        {
            // Убедимся, что объект - это снаряд, а не корабль
            if (gameObject.CompareTag("Projectile"))
            {
                Destroy(gameObject); // Удаляем снаряд
            }
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            // Игнорируем столкновения с водой
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
