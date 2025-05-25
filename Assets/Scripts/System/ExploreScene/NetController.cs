using UnityEngine;

/// <summary>
/// 지정된 방향으로 네트를 이동시키며,
/// 화면 이탈 또는 물고기와의 충돌 시 오브젝트 풀로 반환합니다.
/// </summary>
public class NetController : MonoBehaviour
{
    #region Variables

    private Vector2 moveDirection;
    private Vector3 lastPosition;
    private bool isConsumed = false;

    [Header("이동 속도")]
    [Tooltip("초당 이동 속도 (world unit 기준)")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("화면 이탈 한계")]
    [Tooltip("양 옆(x) 최대 거리")]
    [SerializeField] private float maxXDistance = 30f;

    [Tooltip("위아래(y) 최대 거리")]
    [SerializeField] private float maxYDistance = 30f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (PauseSystem.Instance?.IsPaused() == true || isConsumed) return;

        float distance = moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection * distance);
        lastPosition = transform.position;

        if (IsOutOfBounds())
        {
            isConsumed = true;
            ReleaseOrDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConsumed) return;

        if (other.CompareTag("Fish"))
        {
            isConsumed = true;
            ReleaseOrDestroy();
        }
    }

    private void OnDestroy()
    {
        PauseSystem.Instance?.Unregister(this);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 이동 방향과 스케일을 설정합니다.
    /// </summary>
    public void Init(Vector2 direction, float scale)
    {
        moveDirection = direction;
        isConsumed = false;

        transform.localScale = Vector3.one * scale;
        lastPosition = transform.position;

        PauseSystem.Instance?.Register(this);
    }

    /// <summary>
    /// 현재 위치가 화면을 벗어났는지 확인합니다.
    /// </summary>
    private bool IsOutOfBounds()
    {
        Vector3 pos = transform.position;
        return Mathf.Abs(pos.x) > maxXDistance || Mathf.Abs(pos.y) > maxYDistance;
    }

    /// <summary>
    /// 오브젝트 풀로 반환하거나 없으면 파괴합니다.
    /// </summary>
    private void ReleaseOrDestroy()
    {
        ObjectPool pool = transform.parent?.GetComponent<ObjectPool>();

        if (pool != null)
        {
            pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
