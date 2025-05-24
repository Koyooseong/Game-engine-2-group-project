using UnityEngine;

/// <summary>
/// 지정된 방향으로 네트를 이동시키며,
/// 일정 거리 이동 또는 물고기와의 충돌 시 오브젝트 풀로 반환합니다.
/// </summary>
public class NetController : MonoBehaviour
{
    #region Variables

    private Vector2 moveDirection;
    private float maxDistance;
    private float movedDistance;
    private Vector3 lastPosition;

    private bool isConsumed = false;

    [Header("이동 속도")]
    [Tooltip("초당 이동 속도 (px 기준)")]
    [SerializeField] private float moveSpeed = 8f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (isConsumed) return;

        float distance = moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection * distance);
        movedDistance += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (movedDistance >= maxDistance)
        {
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

    #endregion

    #region Custom Methods

    /// <summary>
    /// 이동 방향과 최대 이동 거리를 설정합니다.
    /// </summary>
    public void Init(Vector2 direction, float range)
    {
        moveDirection = direction;
        maxDistance = range;
        movedDistance = 0f;
        lastPosition = transform.position;
        isConsumed = false;
    }

    /// <summary>
    /// 오브젝트 풀로 반환하거나 없으면 파괴합니다.
    /// </summary>
    private void ReleaseOrDestroy()
    {
        ObjectPool pool = transform.parent.GetComponent<ObjectPool>();

        if (pool != null)
        {
            pool.Release(gameObject); // 내부에서 SetActive(false) 처리
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
