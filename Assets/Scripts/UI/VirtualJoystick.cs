using UnityEngine;

/// <summary>
/// 하이브리드 방식의 조이스틱. 입력은 JoystickManager가 전달해야 합니다.
/// </summary>
public class VirtualJoystick : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 핸들")]
    [SerializeField] private RectTransform handle;

    [Header("이동 반경")]
    [SerializeField] private float handleRange = 100f;

    private Vector2 inputDirection = Vector2.zero;
    private Vector2 startPosition;
    private bool isDragging = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        startPosition = handle.anchoredPosition;
    }

    #endregion

    #region Custom Methods

    public void Activate(int touchId)
    {
        isDragging = true;
    }

    public void ResetJoystick()
    {
        isDragging = false;
        inputDirection = Vector2.zero;
        handle.anchoredPosition = startPosition;
    }

    /// <summary>
    /// JoystickManager로부터 마우스 위치를 받아 직접 처리합니다.
    /// </summary>
    public void SetInputByMouse(Vector2 screenPosition)
    {
        if (!isDragging) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            screenPosition,
            null,
            out pos
        );

        Vector2 offset = Vector2.ClampMagnitude(pos, handleRange);
        handle.anchoredPosition = offset;
        inputDirection = offset.normalized;
    }

    public Vector2 GetInput()
    {
        return inputDirection;
    }

    public bool IsActive()
    {
        return isDragging;
    }

    #endregion
}
