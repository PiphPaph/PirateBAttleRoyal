using UnityEngine;

public class ShipController : MonoBehaviour
{
    public GameObject sideCannonPrefab; // Префаб пушки
    public Transform leftCannonPoint; // Точка для левой пушки
    public Transform rightCannonPoint; // Точка для правой пушки
    private int leftCannonsCount = 0; // Количество левых пушек
    private int rightCannonsCount = 0; // Количество правых пушек
    public Vector3 cannonScale = new Vector3(0.5f, 0.5f, 0.5f);
    
    // Смещения для пушек на носу и корме относительно точки их установки
    public Vector3 canonPositionNose = new Vector3(-0.5f, -0.2f, 0); // Смещение вдоль оси X (к носу)
    public Vector3 canonPositionStern = new Vector3(-0.5f, -0.5f, 0); // Смещение вдоль оси X (к корме)

    // Метод для добавления пушек на борта
    public void AddSideCannons(int cannonsPerSide)
    {
        // Удаляем старые пушки, если они есть
        foreach (Transform child in leftCannonPoint) Destroy(child.gameObject);
        foreach (Transform child in rightCannonPoint) Destroy(child.gameObject);

        // Добавляем пушки на левый борт
        for (int i = 0; i < cannonsPerSide; i++)
        {
            Vector3 leftCannonPosition = i == 0 ? canonPositionNose : canonPositionStern;
            GameObject leftCannon = Instantiate(sideCannonPrefab, leftCannonPoint);
            
            // Позиция и масштаб
            leftCannon.transform.localPosition = leftCannonPosition;
            leftCannon.transform.localScale = cannonScale;
            
            // Поворот на 180 градусов для левой пушки
            leftCannon.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }

        // Добавляем пушки на правый борт
        for (int i = 0; i < cannonsPerSide; i++)
        {
            Vector3 rightCannonPosition = i == 0 ? canonPositionNose : canonPositionStern;
            GameObject rightCannon = Instantiate(sideCannonPrefab, rightCannonPoint);
            
            // Позиция и масштаб
            rightCannon.transform.localPosition = rightCannonPosition;
            rightCannon.transform.localScale = cannonScale;
            
            // Оставляем поворот без изменений для правых пушек
            rightCannon.transform.localRotation = Quaternion.identity;
        }

        // Обновляем счетчики пушек
        leftCannonsCount = cannonsPerSide;
        rightCannonsCount = cannonsPerSide;
    }

    public bool HasSideCannons()
    {
        return leftCannonsCount > 0 || rightCannonsCount > 0; // Проверка на наличие боковых пушек
    }

    public bool HasTwoCannons()
    {
        return leftCannonsCount == 2 && rightCannonsCount == 2; // Проверка на наличие по две пушки на каждом борту
    }
}
