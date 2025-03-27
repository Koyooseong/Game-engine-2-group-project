using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;

public class ShopItemUI : MonoBehaviour
{
    #region Variables

    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button sellButton;

    [Header("물고기 데이터")]
    [SerializeField] private FishData fishData;

    private int sellCount = 0;

    #endregion

    #region Unity Methods

    private void Start()
    {
        fishNameText.text = fishData.fishName;
        goldText.text = $"{fishData.goldValue} G";
        UpdateCountText();

        plusButton.onClick.AddListener(OnClickPlus);
        minusButton.onClick.AddListener(OnClickMinus);
        sellButton.onClick.AddListener(OnClickSell);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 수량을 1 증가시킵니다.
    /// </summary>
    private void OnClickPlus()
    {
        int maxCount = GameResultManager.GetFishCount(fishData.fishType);
        if (sellCount < maxCount)
        {
            sellCount++;
            UpdateCountText();
        }
    }

    /// <summary>
    /// 수량을 1 감소시킵니다.
    /// </summary>
    private void OnClickMinus()
    {
        if (sellCount > 0)
        {
            sellCount--;
            UpdateCountText();
        }
    }

    /// <summary>
    /// 물고기를 판매하고, 골드를 추가합니다.
    /// </summary>
    private void OnClickSell()
    {
        if (sellCount <= 0) return;

        int currentCount = GameResultManager.GetFishCount(fishData.fishType);
        if (sellCount > currentCount)
        {
            Log.Warn("보유 수량보다 많은 수를 판매하려고 시도했습니다.", this);
            return;
        }

        int totalGold = fishData.goldValue * sellCount;

        GoldManager.AddGold(totalGold);
        GameResultManager.AddFish(fishData.fishType, -sellCount);  // 수량 차감
        Log.System($"{fishData.fishType} {sellCount}마리 판매 → +{totalGold}G");

        sellCount = 0;
        UpdateCountText();
    }

    /// <summary>
    /// 판매 수량 UI를 갱신합니다.
    /// </summary>
    private void UpdateCountText()
    {
        countText.text = $"x {sellCount}";
    }

    #endregion
}
