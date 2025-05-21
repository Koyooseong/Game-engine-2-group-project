using UnityEngine;

/// <summary>
/// 지정된 방향으로 네트를 이동시키고, 거리 초과 시 풀로 반환합니다.
/// </summary>
public class NetController : MonoBehaviour
{
    #region Variables

    private Vector2 moveDirection;
    private float maxDistance;
    private float movedDistance;
    private Vector3 lastPosition;

    [Header("이동 속도")]
    [Tooltip("초당 이동 속도 (px 기준)")]
    [SerializeField] private float moveSpeed = 8f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        float distance = moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection * distance);
        movedDistance += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (movedDistance >= maxDistance)
        {
            // 하이라키에 있는 ObjectPool 사용
            ObjectPool pool = transform.parent.GetComponent<ObjectPool>();
            if (pool != null)
            {
                pool.Release(gameObject);
            }
            else
            {
                Destroy(gameObject); // 예외 처리
            }
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
    }

    #endregion
}
