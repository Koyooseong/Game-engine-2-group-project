using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 내 위치와 반경에 따라 방향 벡터를 계산하는 가상 조이스틱입니다.
/// 입력은 외부에서 Activate/Reset/Input 방식으로 제어됩니다.
/// </summary>
public class VirtualJoystick : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 핸들")]
    [Tooltip("조이스틱 핸들 RectTransform")]
    [SerializeField] private RectTransform handle;

    [Header("이동 반경")]
    [Tooltip("핸들이 이동할 수 있는 최대 거리")]
    [SerializeField] private float handleRange = 100f;

    private RectTransform rectTransform;
    private Vector2 inputDirection = Vector2.zero;
    private Vector2 startPosition;
    private bool isDragging = false;

    public int FingerId { get; private set; } = -1;

    [Header("입력 기준이 되는 조이스틱 프레임")]
    [SerializeField] private RectTransform inputArea;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (handle == null)
        {
            Debug.LogError("[VirtualJoystick] Handle이 설정되지 않았습니다.", this);
        }
    }

    private void Start()
    {
        if (handle != null)
            startPosition = handle.anchoredPosition;
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 조이스틱 입력을 활성화합니다.
    /// </summary>
    /// <param name="fingerId">입력 주체 식별용 ID</param>
    public void Activate(int fingerId)
    {
        FingerId = fingerId;
        isDragging = true;
    }

    /// <summary>
    /// 조이스틱 입력을 초기화합니다.
    /// </summary>
    public void ResetJoystick()
    {
        isDragging = false;
        FingerId = -1;
        inputDirection = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = startPosition;
    }

    /// <summary>
    /// 마우스 좌표 기준으로 입력 위치를 반영합니다.
    /// </summary>
    /// <param name="screenPosition">스크린 위치</param>
    public void SetInputByMouse(Vector2 screenPosition)
    {
        SetInputInternal(screenPosition);
    }

    /// <summary>
    /// 터치 좌표 기준으로 입력 위치를 반영합니다.
    /// </summary>
    /// <param name="screenPosition">스크린 위치</param>
    public void SetInputByTouch(Vector2 screenPosition)
    {
        SetInputInternal(screenPosition);
    }

    /// <summary>
    /// 현재 입력 방향을 반환합니다.
    /// </summary>
    public Vector2 GetInput()
    {
        return inputDirection;
    }

    /// <summary>
    /// 현재 조이스틱이 활성 상태인지 여부
    /// </summary>
    public bool IsActive()
    {
        return isDragging;
    }

    /// <summary>
    /// 입력 위치를 로컬 좌표로 변환해 반영하는 내부 함수
    /// </summary>
    private void SetInputInternal(Vector2 screenPosition)
    {
        if (!isDragging || handle == null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inputArea, screenPosition, uiCamera, out localPoint))
        {
            Vector2 offset = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = offset;
            inputDirection = offset / handleRange;
        }
    }

    #endregion
}
