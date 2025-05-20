using UnityEngine;

/// <summary>
/// 조이스틱 입력의 방향 + 강도를 그대로 반영해 즉시 이동합니다.
/// 손을 떼면 즉시 멈추고, 조이스틱을 약하게 밀면 느리게 움직입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SubmarineController : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 참조")]
    [Tooltip("잠수함 이동을 위한 왼쪽 VirtualJoystick")]
    [SerializeField] private VirtualJoystick leftJoystick;

    [Header("이동 속도")]
    [Tooltip("조이스틱 최대 입력 시 이동 속도")]
    [Range(0f, 10f)]
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody2D rb;
    private Vector2 currentInput = Vector2.zero;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 조이스틱 입력값을 가져옵니다.
    /// </summary>
    private void UpdateInput()
    {
        currentInput = (leftJoystick != null) ? leftJoystick.GetInput() : Vector2.zero;
    }

    /// <summary>
    /// 조이스틱의 세기와 방향을 그대로 반영하여 속도를 지정합니다.
    /// </summary>
    private void Move()
    {
        rb.linearVelocity = currentInput * moveSpeed;
    }

    #endregion
}
