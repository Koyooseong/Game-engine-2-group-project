using Game;
using UnityEngine;
using UnityEngine.UI;

public class FuelTimer : MonoBehaviour
{
    #region Variables

    [Header("연료 감소 시간")]
    [Tooltip("연료가 0이 되기까지 걸리는 시간입니다.")]
    [SerializeField] private float fuelDuration = GameConstants.DEFAULT_FUEL_TIME;

    [Header("결과 화면 스크립트")]
    [Tooltip("결과화면 스크립트 연결하면 됩니다.")]
    [SerializeField] private ResultUI resultUI;


    [Header("깊이 측정 스크립트")]
    [SerializeField] private DepthMarker depthMarker;


    private RectTransform rectTransform;
    private float currentTime;
    private float originalWidth = GameConstants.FUEL_BAR_WIDTH;
    private bool resultStarted = false;

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
            if (!resultStarted)
            {
                resultStarted = true;
                StartCoroutine(DelayShowResult());
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

        // depth측정을 멈춥니다.
        if (depthMarker != null)
        {
            depthMarker.StopTracking();
        }
        else
        {
            Log.Warn("FuelTimer에 DepthMarker가 연결되지 않았습니다!", this);
        }




        if (resultUI == null)
        {
            Log.Error("FuelTimer에 ResultUI가 연결되지 않았습니다.", this);
            yield break;
        }

        Log.System("연료 소진됨 → 결과 화면 표시", this);
        resultUI.Show();
    }

    #endregion
}
