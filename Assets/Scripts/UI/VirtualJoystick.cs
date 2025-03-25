using UnityEngine;
using Game;

/// <summary>
/// 하이브리드 방식의 조이스틱. 입력은 JoystickManager가 전달해야 합니다.
/// </summary>
public class VirtualJoystick : AutoPauseBehaviour
{
    #region Variables

    [Header("조이스틱 핸들")]
    [Tooltip("사용자가 움직이는 조이스틱의 핸들입니다.")]
    [SerializeField] private RectTransform handle;

    [Header("이동 반경")]
    [Tooltip("조이스틱 핸들이 이동할 수 있는 최대 거리입니다.")]
    [SerializeField] private float handleRange = 100f;

    [Header("UI 차단용 CanvasGroup")]
    [Tooltip("비활성화 시 입력을 차단하기 위한 캔버스 그룹입니다.")]
    [SerializeField] private CanvasGroup canvasGroup;

    private Vector2 inputDirection = Vector2.zero;
    private Vector2 startPosition;
    private bool isDragging = false;
    private RectTransform rectTransform;

    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        rectTransform = GetComponent<RectTransform>();

        if (handle == null)
        {
            Log.Error("VirtualJoystick의 Handle이 지정되지 않았습니다.", this);
            return;
        }

        startPosition = handle.anchoredPosition;
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 조이스틱 입력을 활성화합니다.
    /// </summary>
    public void Activate(int touchId)
    {
        isDragging = true;
    }

    /// <summary>
    /// 조이스틱을 초기화하고 입력을 제거합니다.
    /// </summary>
    public void ResetJoystick()
    {
        isDragging = false;
        inputDirection = Vector2.zero;
        handle.anchoredPosition = startPosition;
    }

    /// <summary>
    /// JoystickManager로부터 마우스 위치를 받아 조이스틱을 이동시킵니다.
    /// </summary>
    public void SetInputByMouse(Vector2 screenPosition)
    {
        if (!isDragging) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            null,
            out localPoint
        );

        Vector2 offset = Vector2.ClampMagnitude(localPoint, handleRange);
        handle.anchoredPosition = offset;
        inputDirection = offset.normalized;
    }

    /// <summary>
    /// 현재 조이스틱 방향 벡터를 반환합니다.
    /// </summary>
    public Vector2 GetInput()
    {
        return inputDirection;
    }

    /// <summary>
    /// 현재 조이스틱이 활성 상태인지 확인합니다.
    /// </summary>
    public bool IsActive()
    {
        return isDragging;
    }

    /// <summary>
    /// 일시정지 시 UI를 비활성화합니다.
    /// </summary>
    public override void Pause()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// 게임 재개 시 UI를 다시 활성화합니다.
    /// </summary>
    public override void Resume()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    #endregion
}
