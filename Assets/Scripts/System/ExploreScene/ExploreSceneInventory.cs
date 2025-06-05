using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ExploreScene에서 획득한 골드와 물고기 키를 임시로 저장하고,
/// 결과 출력 및 InventoryManager, GoldManager에 분배하는 시스템입니다.
/// </summary>
public class ExploreSceneInventory : MonoBehaviour
{
    #region Variables

    public static ExploreSceneInventory Instance { get; private set; }

    [Header("임시 인벤토리")]
    [Tooltip("임시 저장된 골드 수치")]
    [SerializeField] private int tempGold;

    [Tooltip("획득한 물고기 키 목록")]
    [SerializeField] private List<string> tempFishKeys = new();

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
            return;
        }

        tempGold = 0;
        tempFishKeys.Clear();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 골드를 임시 인벤토리에 추가합니다.
    /// </summary>
    public void AddGold(int amount)
    {
        tempGold += amount;
        Log.Info($"[ExploreSceneInventory] 골드 +{amount} (총: {tempGold})", this);
    }

    /// <summary>
    /// 물고기 키를 임시 인벤토리에 추가합니다.
    /// </summary>
    public void AddFish(string fishId)
    {
        tempFishKeys.Add(fishId);
        Log.Info($"[ExploreSceneInventory] 물고기 획득: {fishId}", this);
    }

    /// <summary>
    /// 임시 인벤토리의 골드를 반환합니다.
    /// </summary>
    public int GetTempGold()
    {
        return tempGold;
    }

    /// <summary>
    /// 임시 인벤토리의 물고기 키 리스트를 반환합니다.
    /// </summary>
    public List<string> GetTempFishKeys()
    {
        return new List<string>(tempFishKeys);
    }

    /// <summary>
    /// 모든 재화를 InventoryManager와 GoldManager에 전달하고 초기화합니다.
    /// </summary>
    public void CommitInventory()
    {
        GoldManager.Instance.AddGold(tempGold);

        foreach (string fishId in tempFishKeys)
        {
            InventoryManager.Instance.AddFish(fishId);
        }

        Log.System($"[ExploreSceneInventory] 골드 {tempGold}, 물고기 {tempFishKeys.Count}개 전달 완료", this);

        tempGold = 0;
        tempFishKeys.Clear();
    }

    #endregion
}
