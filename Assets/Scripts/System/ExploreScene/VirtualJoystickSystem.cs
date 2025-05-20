using UnityEngine;
using Game;

/// <summary>
/// 화면 좌/우를 기준으로 입력을 분기하여 각각의 VirtualJoystick에 전달합니다.
/// 에디터에서는 마우스 입력, 모바일에서는 터치를 사용하며
/// 한쪽 입력이 다른 쪽으로 넘어가지 않도록 처리합니다.
/// </summary>
public class VirtualJoystickSystem : MonoBehaviour
{
    #region Variables

    [Header("왼쪽 조이스틱")]
    [Tooltip("Submarine 조작용 왼쪽 VirtualJoystick")]
    [SerializeField] private VirtualJoystick leftJoystick;

    [Header("오른쪽 조이스틱")]
    [Tooltip("그물 발사용 오른쪽 VirtualJoystick")]
    [SerializeField] private VirtualJoystick rightJoystick;

    private float screenHalf;

    private int? leftFingerId = null;
    private int? rightFingerId = null;

    #endregion

    #region Unity Methods

    private void Start()
    {
        screenHalf = Screen.width * 0.5f;
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    #endregion

    #region Custom Methods

#if UNITY_EDITOR
    /// <summary>
    /// 에디터 환경에서 마우스 입력을 처리합니다.
    /// </summary>
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Log.Info($"[DEBUG] 마우스 현재 위치: {Input.mousePosition}");
            if (Input.mousePosition.x < screenHalf)
            {
                leftJoystick.Activate(-1);
                leftJoystick.SetInputByMouse(Input.mousePosition);
            }
            else
            {
                rightJoystick.Activate(-1);
                rightJoystick.SetInputByMouse(Input.mousePosition);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (leftJoystick.IsActive())
                leftJoystick.SetInputByMouse(Input.mousePosition);
            else if (rightJoystick.IsActive())
                rightJoystick.SetInputByMouse(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            leftJoystick.ResetJoystick();
            rightJoystick.ResetJoystick();
        }
    }
#else
    /// <summary>
    /// 모바일 환경에서 터치 입력을 처리합니다.
    /// </summary>
    private void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (touch.position.x < screenHalf && leftFingerId == null)
                    {
                        leftFingerId = touch.fingerId;
                        leftJoystick.Activate(touch.fingerId);
                        leftJoystick.SetInputByTouch(touch.position);
                    }
                    else if (touch.position.x >= screenHalf && rightFingerId == null)
                    {
                        rightFingerId = touch.fingerId;
                        rightJoystick.Activate(touch.fingerId);
                        rightJoystick.SetInputByTouch(touch.position);
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (touch.fingerId == leftFingerId)
                        leftJoystick.SetInputByTouch(touch.position);
                    else if (touch.fingerId == rightFingerId)
                        rightJoystick.SetInputByTouch(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == leftFingerId)
                    {
                        leftJoystick.ResetJoystick();
                        leftFingerId = null;
                    }
                    else if (touch.fingerId == rightFingerId)
                    {
                        rightJoystick.ResetJoystick();
                        rightFingerId = null;
                    }
                    break;
            }
        }
    }
#endif

    #endregion
}
