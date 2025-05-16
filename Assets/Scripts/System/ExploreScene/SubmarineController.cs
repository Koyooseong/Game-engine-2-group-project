using UnityEngine;

/// <summary>
/// 조이스틱 입력 방향을 바탕으로 잠수함을 이동시킵니다.
/// 회전 없이 XY 평면 상에서만 이동합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SubmarineController : MonoBehaviour
{
    #region Variables

    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 inputDirection = Vector2.zero;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Rigidbody2D 컴포넌트 참조 및 중력 제거
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 외부에서 입력 방향을 전달받습니다.
    /// </summary>
    public void SetInputDirection(Vector2 direction)
    {
        inputDirection = direction;
    }

    /// <summary>
    /// Rigidbody를 이용해 잠수함을 이동시킵니다.
    /// </summary>
    private void Move()
    {
        if (inputDirection == Vector2.zero)
            return;

        Vector2 newPos = rb.position + inputDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    #endregion
}
