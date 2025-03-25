using UnityEngine;

/// <summary>
/// 조이스틱 입력을 기반으로 Submarine을 상하좌우로 이동시키는 컨트롤러입니다.
/// Rigidbody2D를 사용하여 충돌 처리를 고려한 FixedUpdate 기반 이동을 수행합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SubmarineController : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 참조")]
    [Tooltip("이동 방향을 받아올 왼쪽 VirtualJoystick입니다.")]
    [SerializeField] private VirtualJoystick leftJoystick;

    [Header("이동 속도")]
    [Tooltip("잠수함의 기본 이동 속도입니다.")]
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
        // Pause 상태에서는 입력 갱신하지 않음
        if (GamePauseManager.Instance != null && GamePauseManager.Instance.IsPaused)
        {
            currentInput = Vector2.zero;
            return;
        }

        if (leftJoystick != null)
        {
            currentInput = leftJoystick.GetInput();
        }
    }

    private void FixedUpdate()
    {
        if (currentInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 move = currentInput.normalized * moveSpeed;
        rb.linearVelocity = move;
    }
    #endregion
}
