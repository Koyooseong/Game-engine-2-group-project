using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 화면 내 위치와 반경에 따라 방향 벡터를 계산하는 가상 조이스틱입니다.
/// 쿨다운 중에는 입력이 제한되며, 오버레이 이미지로 시각화됩니다.
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

    [Header("입력 기준이 되는 조이스틱 프레임")]
    [Tooltip("입력 좌표 기준이 되는 프레임")]
    [SerializeField] private RectTransform inputArea;

    [Header("쿨다운 오버레이")]
    [Tooltip("입력 제한 시 표시되는 오버레이 이미지")]
    [SerializeField] private GameObject cooldownOverlay;

    [Header("쿨다운 텍스트")]
    [Tooltip("쿨다운 남은 시간을 표시할 TMP 텍스트")]
    [SerializeField] private TMPro.TextMeshProUGUI cooldownText;

    private RectTransform rectTransform;
    private Vector2 inputDirection = Vector2.zero;
    private Vector2 startPosition;
    private bool isDragging = false;
    private bool isInteractable = true;

    public int FingerId { get; private set; } = -1;

    /// <summary>
    /// 조이스틱 입력 해제 시 호출되는 이벤트입니다.
    /// </summary>
    public event Action<Vector2> OnReleased;

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

        PauseSystem.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        PauseSystem.Instance?.Unregister(this);
    }

    #endregion

    #region Custom Methods

    public void Activate(int fingerId)
    {
        if (!isInteractable || PauseSystem.Instance?.IsPaused() == true) return;

        FingerId = fingerId;
        isDragging = true;
    }

    public void ResetJoystick()
    {
        isDragging = false;
        FingerId = -1;

        Vector2 releasedDirection = inputDirection;
        inputDirection = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = startPosition;

        OnReleased?.Invoke(releasedDirection);
    }

    public void SetInputByMouse(Vector2 screenPosition)
    {
        if (!isInteractable || PauseSystem.Instance?.IsPaused() == true) return;
        SetInputInternal(screenPosition);
    }

    public void SetInputByTouch(Vector2 screenPosition)
    {
        if (!isInteractable || PauseSystem.Instance?.IsPaused() == true) return;
        SetInputInternal(screenPosition);
    }

    public Vector2 GetInput() => inputDirection;

    public bool IsActive() => isDragging;

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        if (cooldownOverlay != null)
            cooldownOverlay.SetActive(!interactable);
        if (cooldownText != null && interactable)
            cooldownText.text = "";

        if (!interactable)
            ResetJoystick();
    }

    private void SetInputInternal(Vector2 screenPosition)
    {
        if (!isDragging || handle == null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inputArea, screenPosition, uiCamera, out Vector2 localPoint))
        {
            Vector2 offset = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = offset;
            inputDirection = offset / handleRange;
        }
    }

    public void UpdateCooldownText(float seconds)
    {
        if (cooldownText == null) return;

        float clamped = Mathf.Max(0f, seconds);
        cooldownText.text = clamped.ToString("F1");
    }

    #endregion
}
