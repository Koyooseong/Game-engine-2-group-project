using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 업그레이드 상태를 관리하는 싱글톤 매니저입니다.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    #region Variables

    // 현재 각 업그레이드 타입의 레벨
    private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    // 업그레이드 데이터 테이블 (고정)
    private Dictionary<UpgradeType, List<UpgradeData>> upgradeTable = new Dictionary<UpgradeType, List<UpgradeData>>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 업그레이드 테이블과 초기 상태를 설정합니다.
    /// </summary>
    private void InitializeUpgrades()
    {
        // 각 타입 초기 레벨 설정
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            upgradeLevels[type] = 0;
        }

        // 업그레이드 테이블 설정
        upgradeTable[UpgradeType.ExploreTime] = new List<UpgradeData>
        {
            new(20, 0), new(30, 55), new(40, 110), new(50, 220), new(60, 440), new(70, 880)
        };

        upgradeTable[UpgradeType.Cooldown] = new List<UpgradeData>
        {
            new(7.0f, 0), new(6.5f, 55), new(6.0f, 110), new(5.5f, 220), new(5.0f, 440), new(4.5f, 880)
        };

        upgradeTable[UpgradeType.Range] = new List<UpgradeData>
        {
            new(40, 0), new(50, 55), new(60, 110), new(70, 220), new(80, 440), new(90, 880)
        };
    }

    /// <summary>
    /// 해당 업그레이드의 현재 레벨을 반환합니다.
    /// </summary>
    public int GetLevel(UpgradeType type)
    {
        return upgradeLevels.TryGetValue(type, out int level) ? level : 0;
    }

    /// <summary>
    /// 현재 효과 수치를 반환합니다.
    /// </summary>
    public float GetCurrentValue(UpgradeType type)
    {
        int level = GetLevel(type);
        return upgradeTable[type][level].value;
    }

    /// <summary>
    /// 다음 레벨의 필요 골드를 반환합니다.
    /// </summary>
    public int GetNextUpgradeCost(UpgradeType type)
    {
        int level = GetLevel(type);
        if (level >= upgradeTable[type].Count - 1)
            return -1; // 최댓값 도달
        return upgradeTable[type][level + 1].cost;
    }

    /// <summary>
    /// 업그레이드를 시도하고 성공 여부를 반환합니다.
    /// </summary>
    public bool TryUpgrade(UpgradeType type)
    {
        int currentLevel = GetLevel(type);
        if (currentLevel >= upgradeTable[type].Count - 1)
            return false; // 이미 최고 레벨

        int cost = upgradeTable[type][currentLevel + 1].cost;
        if (GoldManager.Instance.GetGold() < cost)
            return false;

        GoldManager.Instance.RemoveGold(cost);
        upgradeLevels[type]++;
        Log.System($"{type} 업그레이드 성공 → Lv.{upgradeLevels[type]}", this);
        return true;
    }

    #endregion
}

/// <summary>
/// 업그레이드 타입 열거형입니다.
/// </summary>
public enum UpgradeType
{
    ExploreTime,
    Cooldown,
    Range
}

/// <summary>
/// 각 업그레이드 단계의 수치와 비용을 저장합니다.
/// </summary>
public struct UpgradeData
{
    public float value; // 시간, 쿨타임, 범위 (px)
    public int cost;

    public UpgradeData(float value, int cost)
    {
        this.value = value;
        this.cost = cost;
    }
}
