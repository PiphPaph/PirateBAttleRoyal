using UnityEngine;
using Photon.Pun;

public class PlayerShip : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f; // Сила движения вперёд
    public float turnSpeed = 20f; // Скорость поворота
    public float inertia = 0.05f; // Сила инерции
    public float turnInertia = 0.05f; // Сила инерции поворота
    private Rigidbody2D rb;
    private bool isTouchingIsland = false; // Флаг, который указывает, касаемся ли мы суши

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

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, moveSpeed * Time.deltaTime);
            rb.rotation = Mathf.LerpAngle(rb.rotation, networkRotation, Time.deltaTime * turnSpeed / 100f);
            return;
        }

        if (!isTouchingIsland) // Проверка, чтобы корабль мог двигаться, если он не касался острова
        {
            MoveShip();
        }
    }

    void MoveShip()
    {
        if (!photonView.IsMine) return;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical != 0)
        {
            rb.AddForce(transform.up * moveSpeed * vertical);
        }

        if (horizontal != 0)
        {
            rb.AddTorque(-horizontal * turnSpeed * Time.deltaTime);
        }

        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        rb.velocity *= (1 - inertia);
        rb.angularVelocity *= (1 - turnInertia);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            isTouchingIsland = true;

            // Вычисляем направление от точки столкновения к центру корабля
            Vector2 directionToCenter = (rb.position - collision.contacts[0].point).normalized;
            // Откатываем корабль от точки столкновения, чтобы удерживать его у границы суши
            rb.AddForce(directionToCenter * moveSpeed * 2f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Island"))
        {
            isTouchingIsland = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            networkPosition = (Vector2)stream.ReceiveNext();
            networkRotation = (float)stream.ReceiveNext();
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        }
    }
}
