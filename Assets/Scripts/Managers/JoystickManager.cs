using Game;
using UnityEngine;

/// <summary>
/// 좌/우 화면 분기 기준으로 각 조이스틱의 입력을 담당하는 매니저입니다.
/// Editor에서는 마우스 위치를 기준으로 분기하며,
/// 게임이 일시정지 상태일 경우 입력을 무시합니다.
/// </summary>
public class JoystickManager : MonoBehaviour
{
    #region Variables

    [Header("왼쪽 조이스틱")]
    [Tooltip("Submarine 조작용 왼쪽 조이스틱")]
    [SerializeField] private VirtualJoystick leftJoystick;

    [Header("오른쪽 조이스틱")]
    [Tooltip("그물 발사용 오른쪽 조이스틱")]
    [SerializeField] private VirtualJoystick rightJoystick;

    private const int touchID = GameConstants.EDITOR_INPUT_ID;

    private float screenHalf;

    #endregion

    #region Unity Methods

    private void Start()
    {
        screenHalf = Screen.width * 0.5f;
    }

    private void Update()
    {
        var pauseManager = GamePauseManager.Instance;
        // 일시정지 상태에서는 입력 처리하지 않음
        if (pauseManager != null && pauseManager.IsPaused)
            return;

#if UNITY_EDITOR
        HandleEditorMouseInput();
#else
        HandleTouchInput();
#endif
    }

    #endregion

    #region Custom Methods

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 마우스 위치를 기준으로 조이스틱을 구분하여 활성화합니다.
    /// </summary>
    private void HandleEditorMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < screenHalf)
            {
                leftJoystick.Activate(touchID);
            }
            else
            {
                rightJoystick.Activate(touchID);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (leftJoystick.IsActive())
            {
                leftJoystick.SetInputByMouse(Input.mousePosition);
            }
            else if (rightJoystick.IsActive())
            {
                rightJoystick.SetInputByMouse(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            leftJoystick.ResetJoystick();
            rightJoystick.ResetJoystick();
        }
    }
#else
    /// <summary>
    /// 모바일 환경에서 터치 위치에 따라 조이스틱을 분기 처리합니다.
    /// </summary>
    private void HandleTouchInput()
{
    foreach (Touch touch in Input.touches)
    {
        // 조이스틱 활성화
        if (touch.phase == TouchPhase.Began)
        {
            if (touch.position.x < screenHalf && !leftJoystick.IsActive())
            {
                leftJoystick.Activate(touch.fingerId);
            }
            else if (touch.position.x >= screenHalf && !rightJoystick.IsActive())
            {
                rightJoystick.Activate(touch.fingerId);
            }
        }

        // 유지 입력 처리 (움직임 or 유지)
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            if (leftJoystick.IsActive() && leftJoystick.FingerId == touch.fingerId)
            {
                leftJoystick.SetInputByTouch(touch.position);
            }
            else if (rightJoystick.IsActive() && rightJoystick.FingerId == touch.fingerId)
            {
                rightJoystick.SetInputByTouch(touch.position);
            }
        }

        // 손 뗐을 때 리셋
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (leftJoystick.FingerId == touch.fingerId)
            {
                leftJoystick.ResetJoystick();
            }
            else if (rightJoystick.FingerId == touch.fingerId)
            {
                rightJoystick.ResetJoystick();
            }
        }
    }
}
#endif

    #endregion
}
