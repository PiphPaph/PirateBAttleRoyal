using UnityEngine;
using UnityEngine.UI; // Для работы с UI

public class HealthController : MonoBehaviour
{
    public float maxHealth = 100f; // Максимальное здоровье
    public float currentHealth;

    // Ссылка на UI-элемент (полоса здоровья)
    public Image healthBar;

    // Урон за секунду, если игрок находится за пределами бури
    public float stormDamagePerSecond = 1f;

    private bool takingDamage = false; // Флаг, указывающий, получает ли игрок урон

    void Start()
    {
        currentHealth = maxHealth;

        // Инициализация UI-полосы здоровья
        UpdateHealthBar();
    }

    void Update()
    {
        if (takingDamage)
        {
            TakeDamage(stormDamagePerSecond * Time.deltaTime); // Уменьшаем здоровье, пока игрок в зоне шторма
        }
    }

    public void StartTakingDamage()
    {
        takingDamage = true; // Устанавливаем флаг для получения урона
    }

    public void StopTakingDamage()
    {
        takingDamage = false; // Сбрасываем флаг для остановки урона
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Ограничиваем здоровье
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Обновляем значение здоровья в UI
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Корабль уничтожен!");
        Destroy(gameObject); // Удаляем корабль
    }
}