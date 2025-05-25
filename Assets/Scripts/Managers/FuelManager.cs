using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 탐사 중 연료를 관리하고, 연료 바 UI 및 종료 조건을 처리하는 매니저입니다.
/// </summary>
public class FuelManager : MonoBehaviour, IInitializable
{
    #region Variables

    [Header("UI")]
    [Tooltip("연료 상태를 표시할 바 이미지")]
    [SerializeField] private Image fuelBar;

    [Tooltip("연료 바의 전체 너비 (px 기준)")]
    [SerializeField] private float fullWidth = 1080f;

    [Header("색상")]
    [Tooltip("연료 50~100%: 초록")]
    [SerializeField] private Color colorGreen;

    [Tooltip("연료 20~50%: 노랑")]
    [SerializeField] private Color colorYellow;

    [Tooltip("연료 0~20%: 빨강")]
    [SerializeField] private Color colorRed;

    [Header("붉은 경고 오버레이")]
    [Tooltip("연료 부족 시 화면에 깜빡이는 빨간 오버레이")]
    [SerializeField] private GameObject redOverlay;

    [Header("탐사 종료 결과 시스템")]
    [SerializeField] private ResultSystem resultSystem;

    private float maxFuelTime;
    private float currentFuelTime;
    private RectTransform fuelTransform;

    private Coroutine blinkRoutine;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (PauseSystem.Instance != null && PauseSystem.Instance.IsPaused()) return;

        currentFuelTime -= Time.deltaTime;

        UpdateFuelUI();

        if (currentFuelTime <= 0f)
        {
            EndExplore();
        }
    }

    private void OnDestroy()
    {
        if (PauseSystem.Instance != null)
        {
            PauseSystem.Instance.Unregister(this);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 시스템 초기화 시 호출되며 연료 값을 세팅합니다.
    /// </summary>
    public void Init()
    {
        fuelTransform = fuelBar.rectTransform;
        maxFuelTime = UpgradeManager.Instance.GetCurrentValue(UpgradeType.ExploreTime);
        currentFuelTime = maxFuelTime;

        PauseSystem.Instance.Register(this);
        Log.System("[FuelManager] 연료 시스템 초기화 완료", this);

        if (redOverlay != null)
        {
            redOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// 연료 바 너비와 색상을 연료 비율에 따라 갱신합니다.
    /// </summary>
    private void UpdateFuelUI()
    {
        float ratio = Mathf.Clamp01(currentFuelTime / maxFuelTime);
        float width = fullWidth * ratio;
        fuelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        if (ratio >= 0.5f)
        {
            fuelBar.color = colorGreen;
            StopBlinkIfNeeded();
        }
        else if (ratio >= 0.2f)
        {
            fuelBar.color = colorYellow;
            StopBlinkIfNeeded();
        }
        else
        {
            fuelBar.color = colorRed;
            StartBlinkIfNeeded();
        }
    }

    /// <summary>
    /// 깜빡임 코루틴이 없을 경우 시작합니다.
    /// </summary>
    private void StartBlinkIfNeeded()
    {
        if (blinkRoutine == null && redOverlay != null)
        {
            blinkRoutine = StartCoroutine(RedOverlayBlink());
        }
    }

    /// <summary>
    /// 깜빡임 코루틴이 존재할 경우 중단합니다.
    /// </summary>
    private void StopBlinkIfNeeded()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (redOverlay != null)
        {
            redOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// 연료 부족 시 화면 깜빡임을 실행합니다.
    /// </summary>
    private IEnumerator RedOverlayBlink()
    {
        while (true)
        {
            redOverlay.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            redOverlay.SetActive(false);
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// 외부로부터 연료를 감소시킵니다.
    /// </summary>
    public void ReduceFuel(float amount)
    {
        currentFuelTime = Mathf.Max(0f, currentFuelTime - amount);
    }

    /// <summary>
    /// 연료가 모두 소진되었을 때 호출되어 탐사를 종료합니다.
    /// </summary>
    private void EndExplore()
    {
        PauseSystem.Instance.Pause();
        Log.System("[FuelManager] 연료가 모두 소진되어 탐사를 종료합니다.", this);

        StopBlinkIfNeeded();
        resultSystem.ShowResult();
    }

    /// <summary>
    /// 현재 남은 연료를 반환합니다.
    /// </summary>
    public float GetCurrentFuel()
    {
        return currentFuelTime;
    }

    #endregion
}
