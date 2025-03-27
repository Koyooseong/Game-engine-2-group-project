using Game;
using UnityEngine;

/// <summary>
/// 물고기의 좌우 이동, 벽 충돌 반전, 방향 회전 적용을 담당합니다.
/// </summary>
public class Fish : MonoBehaviour
{
    #region Variables

    private FishData data;
    private Vector2 direction;
    private Camera mainCamera;
    private PoolManager pool;
    private bool isCaught = false;

    private const float VIEWPORT_REMOVE_Y = -0.1f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isCaught) return;

        Move();

        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewPos.y < VIEWPORT_REMOVE_Y)
        {
            pool.Return(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 물고기를 초기화하고, 오른쪽으로 시작하게 설정합니다.
    /// </summary>
    public void Initialize(FishData fishData, PoolManager poolManager)
    {
        data = fishData;
        pool = poolManager;

        direction = Vector2.right; //  항상 오른쪽으로 시작

        transform.localScale = data.size;

        SetRotationByDirection();
    }

    /// <summary>
    /// 물고기를 이동시킵니다.
    /// </summary>
    private void Move()
    {
        //  수직 낙하 (월드 기준 아래 방향)
        transform.Translate(Vector2.down * data.fallSpeed * Time.deltaTime, Space.World);

        //  좌우 이동 (로컬 기준에서 "오른쪽"은 transform.up 방향)
        transform.Translate(Vector3.up * data.moveSpeed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    /// 벽 충돌 시 좌우 방향을 반전하고, 회전도 함께 적용합니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCaught) return;

        if (other.CompareTag("Net"))
        {
            Log.Info("Net과 충돌", this);
            CatchFish();
        }

        if (other.CompareTag("Wall"))
        {
            direction.x *= -1f;         //  좌우 방향만 반전
            SetRotationByDirection();  //  회전도 반영
        }
    }

    /// <summary>
    /// 이동 방향에 따라 프리팹의 Z 회전을 조정합니다.
    /// </summary>
    private void SetRotationByDirection()
    {
        float zRotation = direction.x > 0 ? -90f : 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    /// <summary>
    /// 그물에 걸렸을 때 동작을 처리합니다.
    /// </summary>
    private void CatchFish()
    {
        isCaught = true;

        // 잡힌 물고기 수 카운트 증가
        GameResultManager.AddFish(data.fishType, 1);

        // 잡힌 프리팹이 존재할 경우 교체
        if (data.caughtPrefab != null)
        {
            GameObject caught = Instantiate(data.caughtPrefab, transform.position, Quaternion.identity);
            Destroy(caught, 3f); // 3초 후 사라짐 (천천히 위로 이동은 caughtPrefab 내에서 처리)
        }
        else
        {
            Log.Warn("caughtPrefab이 지정되지 않았습니다.", this);
        }

        // 기존 물고기 반환
        pool.Return(gameObject);
    }
    #endregion
}
