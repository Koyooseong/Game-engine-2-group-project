using UnityEngine;

/// <summary>
/// Rigidbody2D ������� �����⸦ �̵���Ű��, �� �浹 �� ������ �����մϴ�.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FishController : MonoBehaviour
{
    #region Variables

    public enum FishType { Small, Medium, Large }

    [Header("������ ũ�� Ÿ��")]
    [SerializeField] private FishType fishType = FishType.Small;

    [Header("�ʱ� ����")]
    [SerializeField] private bool isMovingLeft = true;

    private float horizontalSpeed;
    private float verticalSpeed;
    private Rigidbody2D rb;

    private int directionX;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;         // �߷� ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // ���� ȸ�� ����
    }

    private void Start()
    {
        InitializeSpeed();
        InitializeDirection();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(directionX * horizontalSpeed, -verticalSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            FlipDirection();
        }
    }

    #endregion

    #region Custom Methods

    private void InitializeSpeed()
    {
        switch (fishType)
        {
            case FishType.Small:
                horizontalSpeed = 3f;
                verticalSpeed = 1f;
                break;
            case FishType.Medium:
                horizontalSpeed = 4f;
                verticalSpeed = 2f;
                break;
            case FishType.Large:
                horizontalSpeed = 5f;
                verticalSpeed = 3f;
                break;
        }
    }

    private void InitializeDirection()
    {
        directionX = isMovingLeft ? -1 : 1;
        ApplyRotationFromDirection();
    }

    private void FlipDirection()
    {
        directionX *= -1;
        ApplyRotationFromDirection();
    }

    private void ApplyRotationFromDirection()
    {
        float z = directionX == -1 ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    #endregion
}
