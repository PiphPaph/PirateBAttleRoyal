using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Цель, за которой следует камера (корабль)
    public float smoothSpeed = 0.125f; // Скорость сглаживания движения
    public Vector3 offset; // Смещение камеры относительно цели
    public float fixedZ = -10f; // Фиксированное значение координаты Z камеры

    private void LateUpdate()
    {
        if (target != null)
        {
            // Целевая позиция камеры с учетом смещения
            Vector3 desiredPosition = target.position + offset;
            
            // Фиксируем координату Z, чтобы камера всегда оставалась на одном уровне по глубине
            desiredPosition.z = fixedZ;

            // Плавное движение камеры к целевой позиции
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            
            // Обновление позиции камеры
            transform.position = smoothPosition;

            // Сброс вращения камеры, чтобы она не следовала за поворотом корабля
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}