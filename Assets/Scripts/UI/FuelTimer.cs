using Game;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 연료 게이지(CurrentHP)의 Width를 시간에 따라 줄이며, 남은 연료를 시각화합니다.
/// </summary>
public class FuelTimer : MonoBehaviour
{
    #region Variables

    [Header("연료 감소 시간")]
    [Tooltip("연료가 0이 되기까지 걸리는 시간입니다.")]
    [SerializeField] private float fuelDuration = GameConstants.DEFAULT_FUEL_TIME;

    [Header("결과 화면 스크립트")]
    [Tooltip("결과화면 스크립트 연결하면 됩니다.")]
    [SerializeField] private ResultUI resultUI;

    private RectTransform rectTransform;
    private float currentTime;
    private float originalWidth = GameConstants.FUEL_BAR_WIDTH;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        ResetFuel();
    }

    private void Update()
    {
        if (currentTime <= 0f)
        {
            if (!resultUI.gameObject.activeSelf)
            {
                StartCoroutine(DelayShowResult()); // 한 프레임 딜레이로 Result 호출
            }

            return;
        }

        currentTime -= Time.deltaTime;
        float percent = Mathf.Clamp01(currentTime / fuelDuration);
        rectTransform.sizeDelta = new Vector2(originalWidth * percent, rectTransform.sizeDelta.y);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 연료를 초기화하고 다시 시작합니다.
    /// 외부에서 시간 조정 가능하도록 float 매개변수도 포함합니다.
    /// </summary>
    public void ResetFuel(float customDuration = -1f)
    {
        fuelDuration = customDuration > 0 ? customDuration : GameConstants.DEFAULT_FUEL_TIME;
        currentTime = fuelDuration;
        rectTransform.sizeDelta = new Vector2(originalWidth, rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// 현재 남은 시간 비율을 반환합니다 (0~1).
    /// </summary>
    public float GetFuelPercent()
    {
        return Mathf.Clamp01(currentTime / fuelDuration);
    }

    /// <summary>
    /// 1프레임 딜레이 후 결과 패널을 표시합니다.
    /// GamePauseManager가 확실히 초기화된 이후 실행되도록 보장합니다.
    /// </summary>
    private System.Collections.IEnumerator DelayShowResult()
    {
        yield return null;
        resultUI.Show();
    }

    #endregion
}
