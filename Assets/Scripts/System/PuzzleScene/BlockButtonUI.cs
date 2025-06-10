using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 블록 위에 표시되는 회전 / 뒤집기 버튼 UI를 제어합니다.
/// </summary>
public class BlockButtonUI : MonoBehaviour
{
    #region Variables

    [Header("Button References")]
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button flipButton;

    private System.Action onRotate;
    private System.Action onFlip;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // 초기화 전까지 버튼 비활성화 방지용
        if (rotateButton != null) rotateButton.onClick.RemoveAllListeners();
        if (flipButton != null) flipButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 버튼 클릭 이벤트를 연결합니다.
    /// </summary>
    /// <param name="rotateCallback">회전 동작</param>
    /// <param name="flipCallback">뒤집기 동작</param>
    public void Initialize(System.Action rotateCallback, System.Action flipCallback)
    {
        onRotate = rotateCallback;
        onFlip = flipCallback;

        rotateButton.onClick.AddListener(OnRotateClicked);
        flipButton.onClick.AddListener(OnFlipClicked);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 회전 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnRotateClicked()
    {
        onRotate?.Invoke();
    }

    /// <summary>
    /// 뒤집기 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnFlipClicked()
    {
        onFlip?.Invoke();
    }

    #endregion
}
