using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestType { OneCannon, TwoCannons }
    public ChestType chestType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShipController playerShip = collision.GetComponent<ShipController>();
            if (playerShip != null)
            {
                if (chestType == ChestType.OneCannon)
                {
                    // Проверяем, есть ли у игрока пушки
                    if (!playerShip.HasSideCannons())
                    {
                        playerShip.AddSideCannons(1); // Добавить по одной пушке на каждый борт
                        Destroy(gameObject); // Удалить сундук после использования
                    }
                }
                else if (chestType == ChestType.TwoCannons)
                {
                    // Проверяем, есть ли у игрока уже по две пушки на каждом борту
                    if (!playerShip.HasTwoCannons())
                    {
                        // Заменяем пушки на две на каждом борту
                        playerShip.AddSideCannons(2); // Добавить по две пушки на каждый борт
                        Destroy(gameObject); // Удалить сундук после использования
                    }
                    // Если у игрока уже по две пушки на каждом борту, сундук не уничтожается
                }
            }
        }
    }
}