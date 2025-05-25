using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 업그레이드 UI를 구성하고 버튼 클릭 시 업그레이드를 처리하는 UI 클래스입니다.
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    #region Variables

    [Header("골드 표시")]
    [SerializeField] private TextMeshProUGUI goldText;

    [System.Serializable]
    private struct UpgradeSlot
    {
        public UpgradeType type;

        [Header("레벨")]
        public TextMeshProUGUI levelText;

        [Header("비용")]
        public TextMeshProUGUI costText;

        [Header("현재 성능")]
        public TextMeshProUGUI currentValueText;

        [Header("다음 성능")]
        public TextMeshProUGUI nextValueText;

        [Header("업그레이드 버튼")]
        public Button upgradeButton;
    }

    [Header("업그레이드 슬롯들")]
    [SerializeField] private UpgradeSlot[] upgradeSlots;

    #endregion
    //애
    #region Unity Methods

    private void OnEnable()
    {
        UpdateUI();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 전체 업그레이드 UI를 현재 상태로 갱신합니다.
    /// </summary>
    private void UpdateUI()
    {
        goldText.text = $"Money: {GoldManager.Instance.GetGold()}G";

        foreach (var slot in upgradeSlots)
        {
            int level = UpgradeManager.Instance.GetLevel(slot.type);
            float currentValue = UpgradeManager.Instance.GetCurrentValue(slot.type);
            int nextCost = UpgradeManager.Instance.GetNextUpgradeCost(slot.type);
            float nextValue = GetNextValue(slot.type);

            // 레벨 표기
            slot.levelText.text = $"Lv. {level} / 5";

            // 업그레이드 비용 표기
            slot.costText.text = nextCost >= 0 ? $"{nextCost} G" : "MAX";

            // 현재 성능 표기
            slot.currentValueText.text = $"Lv. {level} {FormatValue(slot.type, currentValue)}";

            // 다음 성능 표기
            slot.nextValueText.text = nextCost >= 0
                ? $"Lv. {level + 1} {FormatValue(slot.type, nextValue)}"
                : "Lv. MAX";

            // 버튼 활성화 여부
            slot.upgradeButton.interactable = nextCost >= 0;
        }
    }

    /// <summary>
    /// 특정 타입의 다음 성능 수치를 반환합니다.
    /// </summary>
    private float GetNextValue(UpgradeType type)
    {
        int level = UpgradeManager.Instance.GetLevel(type);
        var table = UpgradeManager.Instance.GetUpgradeTable(type);

        if (level >= table.Count - 1)
            return table[table.Count - 1].value;

        return table[level + 1].value;
    }

    /// <summary>
    /// 업그레이드 타입에 따라 포맷을 다르게 적용합니다.
    /// </summary>
    private string FormatValue(UpgradeType type, float value)
    {
        return type switch
        {
            UpgradeType.ExploreTime => $"{value}s",
            UpgradeType.Cooldown => $"{value}s",
            UpgradeType.Range => $"{value}%",
            _ => value.ToString()
        };
    }

    /// <summary>
    /// 버튼 클릭 시 호출되어 업그레이드를 시도하고 UI를 갱신합니다.
    /// </summary>
    public void OnClickUpgrade(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= upgradeSlots.Length) return;

        UpgradeType type = upgradeSlots[slotIndex].type;

        if (UpgradeManager.Instance.TryUpgrade(type))
        {
            Log.System($"{type} 업그레이드 성공!", this);
            UpdateUI();
        }
        else
        {
            Log.Warn($"{type} 업그레이드 실패 (골드 부족 or 최대 레벨)", this);
        }
    }

    #endregion
}
