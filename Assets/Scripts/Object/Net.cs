using UnityEngine;

/// <summary>
/// 발사된 그물이 이동하고, 화면 밖으로 나가면 자동 반환되는 클래스입니다.
/// </summary>
public class Net : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed = 8f;

    private Vector2 direction;
    private Camera mainCamera;
    private PoolManager pool;

    private const float SCREEN_PADDING = 0.1f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (!IsVisible())
        {
            if (pool != null)
            {
                pool.Return(gameObject);
            }
            else
            {
                Log.Error("풀이 설정되지 않아 Net 반환 실패", this);
                gameObject.SetActive(false); // fallback
            }
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 이동 방향과 반환용 Pool을 설정합니다.
    /// </summary>
    public void Initialize(Vector2 dir, PoolManager poolManager)
    {
        direction = dir.normalized;
        pool = poolManager;
    }

    /// <summary>
    /// 화면 안에 있는지 확인합니다.
    /// </summary>
    private bool IsVisible()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        return viewPos.x >= -SCREEN_PADDING && viewPos.x <= 1 + SCREEN_PADDING &&
               viewPos.y >= -SCREEN_PADDING && viewPos.y <= 1 + SCREEN_PADDING;
    }

    #endregion
}
