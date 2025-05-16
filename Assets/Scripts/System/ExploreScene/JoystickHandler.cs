using UnityEngine;

/// <summary>
/// 조이스틱의 입력을 감지하고 방향을 계산합니다.
/// 에디터에서는 마우스 입력, 앱에서는 터치를 사용합니다.
/// </summary>
public class JoystickHandler : MonoBehaviour, IJoystickHandler
{
    #region Variables

    [Header("조이스틱 UI 참조")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;

    [Header("조이스틱 종류 설정")]
    [SerializeField] private JoystickType joystickType;

    private int pointerId = -1;
    private Vector2 inputDirection = Vector2.zero;
    private Vector2 joystickCenter = Vector2.zero;

    private const float HANDLE_RANGE_MULTIPLIER = 0.5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // 조이스틱 중심 좌표를 스크린 좌표로 변환
        joystickCenter = RectTransformUtility.WorldToScreenPoint(Camera.main, joystickBackground.position);
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
        UpdateJoystickUI();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 외부에서 현재 입력 방향을 읽어옵니다.
    /// </summary>
    public Vector2 Direction => inputDirection;

    /// <summary>
    /// 조이스틱의 타입을 지정합니다.
    /// </summary>
    public void Initialize(JoystickType type)
    {
        joystickType = type;
    }

    /// <summary>
    /// 에디터 환경에서 마우스 입력을 처리합니다.
    /// </summary>
    public void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;

            if (IsMatchingJoystickArea(mousePos))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    joystickBackground,
                    mousePos,
                    null,
                    out Vector2 localPoint
                );

                float maxRange = joystickBackground.sizeDelta.x * HANDLE_RANGE_MULTIPLIER;
                Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, maxRange);

                // 핸들 이동
                joystickHandle.anchoredPosition = clampedPosition;

                // 이동 방향
                inputDirection = clampedPosition / maxRange;
            }
        }
        else
        {
            inputDirection = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// 모바일 환경에서 터치 입력을 처리합니다.
    /// </summary>
    private void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            Vector2 touchPos = touch.position;

            if (touch.phase == TouchPhase.Began && pointerId == -1)
            {
                if (IsMatchingJoystickArea(touchPos))
                {
                    pointerId = touch.fingerId;
                }
            }

            if (touch.fingerId == pointerId)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    inputDirection = (touchPos - joystickCenter).normalized;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    pointerId = -1;
                    inputDirection = Vector2.zero;
                }
            }
        }
    }

    /// <summary>
    /// 지정된 입력 영역(좌/우 조이스틱)에 해당하는지 확인합니다.
    /// </summary>
    private bool IsMatchingJoystickArea(Vector2 screenPos)
    {
        return (joystickType == JoystickType.Left && screenPos.x < Screen.width / 2f) ||
               (joystickType == JoystickType.Right && screenPos.x >= Screen.width / 2f);
    }

    /// <summary>
    /// 조이스틱 UI의 핸들을 방향에 맞게 이동시킵니다.
    /// </summary>
    private void UpdateJoystickUI()
    {
        if (joystickHandle == null)
            return;

        float handleRange = joystickBackground.sizeDelta.x * HANDLE_RANGE_MULTIPLIER;
        Vector2 clamped = Vector2.ClampMagnitude(inputDirection * handleRange, handleRange);
        joystickHandle.anchoredPosition = clamped;
    }

    /// <summary>
    /// 외부에서 터치 정보를 수동으로 전달받아 처리합니다.
    /// </summary>
    public void ProcessTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBackground,
                touch.position,
                null,
                out Vector2 localPoint
            );

            inputDirection = localPoint.normalized;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            inputDirection = Vector2.zero;
        }
    }

    /// <summary>
    /// 조이스틱 중심 위치를 반환합니다 (스크린 좌표).
    /// </summary>
    public Vector2 CenterPosition => joystickBackground.position;


    /// <summary>
    /// 조이스틱 입력을 외부에서 강제로 초기화합니다.
    /// </summary>
    public void ForceRelease()
    {
        inputDirection = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        pointerId = -1;
    }



    #endregion
}
