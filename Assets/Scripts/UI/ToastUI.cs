using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 일정 시간 동안 메시지를 표시한 후 사라지는 토스트 UI입니다.
/// </summary>
public class ToastUI : MonoBehaviour
{
    #region Variables

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image backgroundImage;

    [Header("설정")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float fadeTime = 0.5f;

    private float timer = 0f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
        else if (timer >= duration - fadeTime)
        {
            float alpha = 1f - ((timer - (duration - fadeTime)) / fadeTime);
            SetAlpha(alpha);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 토스트 메시지를 표시합니다.
    /// </summary>
    /// <param name="message">표시할 텍스트</param>
    /// <param name="color">텍스트 및 배경 색상</param>
    public void Show(string message, Color color)
    {
        if (messageText != null)
            messageText.text = message;

        timer = 0f;
    }

    #endregion

    #region Private Methods

    private void SetAlpha(float alpha)
    {
        if (messageText != null)
        {
            Color textColor = messageText.color;
            textColor.a = alpha;
            messageText.color = textColor;
        }

        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            bgColor.a = alpha * 0.25f;
            backgroundImage.color = bgColor;
        }
    }

    #endregion
}
