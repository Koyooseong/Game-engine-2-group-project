using UnityEngine;

/// <summary>
/// Rigidbody2D를 이용해 물고이를 이동시키며, 벽 및 그물과의 충돌을 처리합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class FishController : MonoBehaviour
{
    #region Variables

    public enum FishType { Small, Medium, Large }

    [Header("물고기 고유 키 (FishDataManager용)")]
    [SerializeField] private string fishKey;

    [Header("물고기 크기 타입")]
    [SerializeField] private FishType fishType = FishType.Small;

    [Header("초기 이동 방향")]
    [SerializeField] private bool isMovingLeft = true;

    private float horizontalSpeed;
    private float verticalSpeed;
    private int directionX;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isCaught = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InitializeSpeed();
        InitializeDirection();

        PauseSystem.Instance?.Register(this);
    }

    private void FixedUpdate()
    {
        if (PauseSystem.Instance?.IsPaused() == true || isCaught)
        {
            rb.linearVelocity = Vector2.zero; // ⭐ 바로 멈추게 하기 위해 속도 수동으로 0 설정
            return;
        }

        rb.linearVelocity = new Vector2(directionX * horizontalSpeed, -verticalSpeed);
    }

    private void Update()
    {
        if (PauseSystem.Instance?.IsPaused() == true) return;

        if (transform.position.y <= -10f)
        {
            ReleaseOrDestroy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            FlipDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCaught) return;

        if (collision.CompareTag("Net"))
        {
            CatchFish();
        }
    }

    private void OnDestroy()
    {
        PauseSystem.Instance?.Unregister(this);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 크기 타입에 따라 속도를 설정합니다.
    /// </summary>
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

    /// <summary>
    /// 초기 방향 설정 및 회전 적용
    /// </summary>
    private void InitializeDirection()
    {
        directionX = isMovingLeft ? -1 : 1;
        ApplyRotationFromDirection();
    }

    /// <summary>
    /// 좌우 방향을 반전하고 회전도 반영합니다.
    /// </summary>
    private void FlipDirection()
    {
        directionX *= -1;
        ApplyRotationFromDirection();
    }

    /// <summary>
    /// 방향에 따른 Z축 회전을 적용합니다.
    /// </summary>
    private void ApplyRotationFromDirection()
    {
        float z = directionX == -1 ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    /// <summary>
    /// 물고기를 잡힌 상태로 전환하고 인벤토리에 추가합니다.
    /// </summary>
    private void CatchFish()
    {
        isCaught = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (animator != null)
        {
            animator.enabled = false;
        }

        Sprite catchSprite = Resources.Load<Sprite>(FishDataManager.Instance.GetCatchSprite(fishKey));
        if (catchSprite != null)
        {
            spriteRenderer.sprite = catchSprite;
        }
        else
        {
            Log.Warn($"[FishController] CatchSprite 불러오기 실패 - {fishKey}", this);
        }

        ExploreSceneInventory.Instance.AddFish(fishKey);

        Invoke(nameof(ReleaseOrDestroy), 0.5f);
    }

    /// <summary>
    /// 오브젝트 풀로 반환하거나 파괴합니다.
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

    private void OnEnable()
    {
        rb.simulated = true;
    }

    private void OnDisable()
    {
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
    }

    #endregion
}
