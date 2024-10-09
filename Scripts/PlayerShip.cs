using UnityEngine;
using Photon.Pun;

public class PlayerShip : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f; // Сила движения вперёд
    public float turnSpeed = 20f; // Скорость поворота
    public float inertia = 0.05f; // Сила инерции
    public float turnInertia = 0.05f; // Сила инерции поворота
    private bool canMove = true; // Переменная, управляющая возможностью движения
    private Rigidbody2D rb;

    private Vector2 networkPosition;
    private float networkRotation;
    private float lag;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (photonView.IsMine)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.GetComponent<CameraFollow>().target = transform;
            }
            else
            {
                Debug.LogError("Главная камера не найдена в сцене!");
            }
        }

        // Убедитесь, что начальная скорость и импульс сброшены
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void Update()
    {
        if (!photonView.IsMine) // Проверка принадлежности объекта
        {
            // Интерполяция движения и поворота для плавного перемещения объектов других игроков
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, moveSpeed * Time.deltaTime);
            rb.rotation = Mathf.LerpAngle(rb.rotation, networkRotation, Time.deltaTime * turnSpeed / 100f);
            return;
        }

        if (canMove) // Проверка возможности двигаться
        {
            MoveShip();
        }
    }

    void MoveShip()
    {
        if (!photonView.IsMine) return;

        // Управление движением вперёд/назад
        float vertical = Input.GetAxis("Vertical");

        // Управление поворотом
        float horizontal = Input.GetAxis("Horizontal");

        // Движение вперёд/назад, используя -transform.up для корректного направления
        if (vertical != 0)
        {
            rb.AddForce(transform.up * moveSpeed * vertical);
        }

        // Поворот
        if (horizontal != 0)
        {
            rb.AddTorque(-horizontal * turnSpeed * Time.deltaTime);
        }

        // Ограничение максимальной скорости
        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // Применение инерции
        rb.velocity *= (1 - inertia);
        
        // Применение инерции поворота
        rb.angularVelocity *= (1 - turnInertia);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            canMove = false; // Запретить движение при касании земли
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            canMove = true; // Разрешить движение, когда корабль покинет землю
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Если это наш объект, отправляем его позицию и поворот другим игрокам
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // Если это объект другого игрока, получаем его позицию и поворот
            networkPosition = (Vector2)stream.ReceiveNext();
            networkRotation = (float)stream.ReceiveNext();

            // Вычисляем лаг для корректной интерполяции
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        }
    }
}

// using UnityEngine;
// using Photon.Pun;
//
// public class PlayerShip : MonoBehaviourPun, IPunObservable
// {
//     public float moveSpeed = 5f; // Сила движения вперёд
//     public float turnSpeed = 20f; // Скорость поворота
//     private bool canMove = true; // Переменная, управляющая возможностью движения
//     private Rigidbody2D rb; // Компонент Rigidbody2D
//
//     private Vector2 networkPosition;
//     private float networkRotation;
//     private float lag;
//
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D
//
//         if (photonView.IsMine)
//         {
//             Camera mainCamera = Camera.main;
//             if (mainCamera != null)
//             {
//                 mainCamera.GetComponent<CameraFollow>().target = transform;
//             }
//             else
//             {
//                 Debug.LogError("Главная камера не найдена в сцене!");
//             }
//         }
//
//         // Убедитесь, что начальная скорость и импульс сброшены
//         rb.velocity = Vector2.zero;
//         rb.angularVelocity = 0f;
//     }
//
//     void Update()
//     {
//         if (!photonView.IsMine) // Проверка принадлежности объекта
//         {
//             // Интерполяция движения и поворота для плавного перемещения объектов других игроков
//             rb.position = Vector2.MoveTowards(rb.position, networkPosition, moveSpeed * Time.deltaTime);
//             rb.rotation = Mathf.LerpAngle(rb.rotation, networkRotation, Time.deltaTime * turnSpeed / 100f);
//             return;
//         }
//
//         if (canMove) // Проверка возможности двигаться
//         {
//             MoveShip();
//         }
//     }
//
//     void MoveShip()
//     {
//         if (!photonView.IsMine) return;
//
//         // Управление движением вперёд/назад
//         float vertical = Input.GetAxis("Vertical");
//
//         // Управление поворотом
//         float horizontal = Input.GetAxis("Horizontal");
//
//         // Движение вперёд/назад, используя -transform.up для корректного направления
//         if (vertical != 0)
//         {
//             rb.AddForce(transform.up * moveSpeed * vertical);
//         }
//
//         // Поворот
//         rb.AddTorque(-horizontal * turnSpeed * Time.deltaTime);
//
//         // Ограничение максимальной скорости
//         if (rb.velocity.magnitude > moveSpeed)
//         {
//             rb.velocity = rb.velocity.normalized * moveSpeed;
//         }
//     }
//
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (!photonView.IsMine) return;
//
//         if (collision.gameObject.CompareTag("Island"))
//         {
//             canMove = false; // Запретить движение при касании земли
//         }
//     }
//
//     private void OnCollisionExit2D(Collision2D collision)
//     {
//         if (!photonView.IsMine) return;
//
//         if (collision.gameObject.CompareTag("Island"))
//         {
//             canMove = true; // Разрешить движение, когда корабль покинет землю
//         }
//     }
//
//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         if (stream.IsWriting)
//         {
//             // Если это наш объект, отправляем его позицию и поворот другим игрокам
//             stream.SendNext(rb.position);
//             stream.SendNext(rb.rotation);
//         }
//         else
//         {
//             // Если это объект другого игрока, получаем его позицию и поворот
//             networkPosition = (Vector2)stream.ReceiveNext();
//             networkRotation = (float)stream.ReceiveNext();
//
//             // Вычисляем лаг для корректной интерполяции
//             lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
//         }
//     }
// }
