using UnityEngine;

/// <summary>
/// 두 개의 조이스틱 입력을 관리하고, 터치 거리 제한을 통해 입력을 강제로 해제할 수 있습니다.
/// 터치 시작 위치에 따라 fingerId가 고정되며, 중앙 DeadZone과 최대 반경 체크를 통해 안정된 UX를 제공합니다.
/// </summary>
public class VirtualJoystickSystem : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 핸들러 참조")]
    [SerializeField] private JoystickHandler leftJoystick;
    [SerializeField] private JoystickHandler rightJoystick;

    [Header("움직일 오브젝트 (예: 잠수함)")]
    [SerializeField] private SubmarineController submarine;

    private int leftTouchId = -1;
    private int rightTouchId = -1;

#if UNITY_EDITOR
    private JoystickHandler activeJoystick = null;
#endif

    private const float DEAD_ZONE_RATIO = 0.1f;
    private const float MAX_LEFT_JOYSTICK_RADIUS = 450f;

    #endregion

    #region Unity Methods

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
        submarine.SetInputDirection(leftJoystick.Direction);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 모바일 터치를 감지하고 각 조이스틱에 전달하며, 최대 반경 제한을 검사합니다.
    /// </summary>
    private void HandleTouch()
    {
        float screenWidth = Screen.width;
        float deadZoneWidth = screenWidth * DEAD_ZONE_RATIO;
        float leftLimit = (screenWidth * 0.5f) - deadZoneWidth;
        float rightLimit = (screenWidth * 0.5f) + deadZoneWidth;

        foreach (Touch touch in Input.touches)
        {
            Vector2 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                if (pos.x < leftLimit && leftTouchId == -1)
                    leftTouchId = touch.fingerId;

                else if (pos.x > rightLimit && rightTouchId == -1)
                    rightTouchId = touch.fingerId;
            }

            // 왼쪽 조이스틱 반경 검사
            if (touch.fingerId == leftTouchId)
            {
                Vector2 center = RectTransformUtility.WorldToScreenPoint(Camera.main, leftJoystick.CenterPosition);
                float distance = Vector2.Distance(pos, center);

                if (distance > MAX_LEFT_JOYSTICK_RADIUS)
                {
                    leftJoystick.ForceRelease();
                    leftTouchId = -1;
                    continue;
                }

                leftJoystick.ProcessTouch(touch);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    leftTouchId = -1;
            }

            // 오른쪽은 일반 처리
            else if (touch.fingerId == rightTouchId)
            {
                rightJoystick.ProcessTouch(touch);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    rightTouchId = -1;
            }
        }
    }

    /// <summary>
    /// 에디터에서 마우스 입력을 감지하고 고정된 조이스틱에만 전달합니다.
    /// 조이스틱 반경(450px)을 초과할 경우 입력을 강제로 해제합니다.
    /// </summary>
    private void HandleMouse()
    {
        float screenWidth = Screen.width;
        float deadZoneWidth = screenWidth * DEAD_ZONE_RATIO;
        float leftLimit = (screenWidth * 0.5f) - deadZoneWidth;
        float rightLimit = (screenWidth * 0.5f) + deadZoneWidth;

        if (Input.GetMouseButtonDown(0))
        {
            float mouseX = Input.mousePosition.x;

            if (mouseX < leftLimit)
                activeJoystick = leftJoystick;
            else if (mouseX > rightLimit)
                activeJoystick = rightJoystick;
            else
                activeJoystick = null;
        }

        if (Input.GetMouseButton(0) && activeJoystick != null)
        {
            // 반경 체크 (왼쪽 조이스틱일 경우에만)
            if (activeJoystick == leftJoystick)
            {
                Vector2 center = RectTransformUtility.WorldToScreenPoint(Camera.main, leftJoystick.CenterPosition);
                float distance = Vector2.Distance(Input.mousePosition, center);

                if (distance > MAX_LEFT_JOYSTICK_RADIUS)
                {
                    leftJoystick.ForceRelease();
                    activeJoystick = null;
                    return;
                }
            }

            activeJoystick.HandleMouseInput();
        }

        if (Input.GetMouseButtonUp(0))
        {
            activeJoystick = null;
        }
    }


    #endregion
}
