using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 퍼즐 씬 인벤토리 슬롯 1개의 UI. 시각 정보 관리 및 수량 제어를 담당합니다.
/// </summary>
public class PuzzleInventoryItemUI : MonoBehaviour
{
    #region Variables

    [Header("UI Components")]
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image panelBackground;

    private FishData fishData;
    private int currentCount;
    private int maxCount;
    private string fishKey;

    #endregion

    #region Public Methods

    /// <summary>
    /// 인벤토리 슬롯을 해당 키로 초기화합니다.
    /// </summary>
    public void Initialize(string key)
    {
        fishKey = key;
        fishData = FishDataManager.Instance.GetFishData(key);
        maxCount = currentCount = InventoryManager.Instance.GetCount(key);

        fishImage.sprite = Resources.Load<Sprite>(fishData.puzzleImg);
        fishNameText.text = fishData.name;
        goldText.text = $"{fishData.gold}G / 5s";
        UpdateCountText();

        if (currentCount <= 0)
        {
            panelBackground.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    /// <summary>
    /// 보유 수량이 있다면 1개 소비하고 true 반환, 없으면 false 반환
    /// </summary>
    public bool TryConsumeOne()
    {
        if (currentCount <= 0) return false;

        currentCount--;
        UpdateCountText();

        if (currentCount <= 0)
        {
            panelBackground.color = new Color(1f, 1f, 1f, 0.3f);
        }

        return true;
    }


    /// <summary>
    /// 블록이 삭제되었을 때 수량을 복구합니다.
    /// </summary>
    public void RestoreOne()
    {
        currentCount++;
        UpdateCountText();
        panelBackground.color = new Color(1f, 1f, 1f, 1f); // 다시 밝게

        Debug.Log($"[PuzzleInventoryItemUI] 수량 복구됨 → 현재 수량: {currentCount}");
    }


    public FishData GetFishData() => fishData;

    public string GetFishKey() => fishKey;

    #endregion

    #region Private Methods

    private void UpdateCountText()
    {
        countText.text = $"{currentCount} / {maxCount}";
    }

    #endregion
}
