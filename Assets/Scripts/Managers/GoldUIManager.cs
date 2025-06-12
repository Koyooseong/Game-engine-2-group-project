using UnityEngine;
using UnityEngine.UI;
using TMPro; // ✅ 추가
using System.Collections;

/// <summary>
/// 아쿠아리움 씬의 골드 수익 UI를 관리합니다.
/// </summary>
public class GoldUIManager : MonoBehaviour
{
    public static GoldUIManager Instance { get; private set; }

    #region Variables

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI incomePer5SecText;     // ✅ TMP로 변경
    [SerializeField] private TextMeshProUGUI currentGoldText;       // ✅ TMP로 변경
    [SerializeField] private TextMeshProUGUI accumulatedGoldText;   // ✅ TMP로 변경
    [SerializeField] private Button collectButton;

    private int totalIncomePer5Sec = 0;
    private int accumulatedGold = 0;
    private const string ACCUMULATED_GOLD_KEY = "AccumulatedGold";
    private const string INCOME_PER_5SEC_KEY = "IncomePer5Sec";
    private Coroutine incomeRoutine;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        collectButton.onClick.AddListener(OnCollectButtonClicked);
    }

    private void Start()
    {
        UpdateCurrentGoldUI(GoldManager.Instance.GetGold());
        totalIncomePer5Sec = PlayerPrefs.GetInt("IncomePer5Sec", 0);
        accumulatedGold = PlayerPrefs.GetInt("AccumulatedGold", 0);

        incomePer5SecText.text = $"5초 당 {totalIncomePer5Sec}골드";
        UpdateAccumulatedGoldUI();

        if (incomeRoutine != null)
            StopCoroutine(incomeRoutine);

        incomeRoutine = StartCoroutine(AccumulateGoldRoutine());
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 5초당 수익 골드를 외부에서 전달받습니다.
    /// </summary>
    public void SetIncomePer5Sec(int amount)
    {
        totalIncomePer5Sec = amount;
        incomePer5SecText.text = $"5초 당 {totalIncomePer5Sec}골드";

        if (incomeRoutine != null)
            StopCoroutine(incomeRoutine);

        incomeRoutine = StartCoroutine(AccumulateGoldRoutine());
    }

    /// <summary>
    /// 5초마다 골드를 누적합니다.
    /// </summary>
    private IEnumerator AccumulateGoldRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Log.System($"전{accumulatedGold}");
            accumulatedGold += totalIncomePer5Sec;
            Log.System($"후{accumulatedGold}");
            UpdateAccumulatedGoldUI();
            SaveIncomeState(); // ✅ 누적될 때 저장
            Log.System($"[GoldUIManager] 골드 누적됨: +{totalIncomePer5Sec} → 현재 {accumulatedGold}G");
        }
    }

    /// <summary>
    /// 골드 수확 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnCollectButtonClicked()
    {
        if (accumulatedGold <= 0)
        {
            Log.Info("[GoldUIManager] 누적 골드 없음 - 수확 생략", this);
            return;
        }

        GoldManager.Instance.AddGold(accumulatedGold);
        UpdateCurrentGoldUI(GoldManager.Instance.GetGold());

        accumulatedGold = 0;
        UpdateAccumulatedGoldUI();
        SaveIncomeState();
    }

    /// <summary>
    /// 현재 보유 골드 텍스트를 갱신합니다.
    /// </summary>
    public void UpdateCurrentGoldUI(int gold)
    {
        currentGoldText.text = $"현재 골드 : {gold}G";
    }

    /// <summary>
    /// 누적 골드 텍스트를 갱신합니다.
    /// </summary>
    private void UpdateAccumulatedGoldUI()
    {
        accumulatedGoldText.text = $"{accumulatedGold}G";
        bool isActive = accumulatedGold > 0;
        collectButton.interactable = isActive;

        // 버튼 이미지 크기를 상태에 따라 재조정
        if (collectButton.image != null)
        {
            collectButton.image.SetNativeSize();
        }
    }

    private void SaveIncomeState()
    {
        PlayerPrefs.SetInt(ACCUMULATED_GOLD_KEY, accumulatedGold);
        PlayerPrefs.SetInt(INCOME_PER_5SEC_KEY, totalIncomePer5Sec);
        PlayerPrefs.Save();

        Log.System($"[GoldUIManager] 저장 완료 - 누적:{accumulatedGold}G / 5초당:{totalIncomePer5Sec}G");
    }

    /// <summary>
    /// 현재 5초당 골드 수익을 반환합니다.
    /// </summary>
    public int GetIncomePer5Sec()
    {
        return totalIncomePer5Sec;
    }

    #endregion
}
