using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 물고기별 선물 수령 여부를 관리하고, 보상 수령 및 알림 갱신을 처리하는 매니저입니다.
/// </summary>
public class PresentManager : MonoBehaviour
{
    #region Variables

    private Dictionary<string, bool[]> presentClaimed = new();

    private readonly int[] requiredCounts = { 1, 3, 5 };
    private readonly int[] rewardAmounts = { 50, 60, 70 };

    [Header("연동 UI")]
    [SerializeField] private CollectionUI collectionUI;

    private string currentFishKey;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        CheckAllRewards();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 모든 인벤토리 키를 검사하여 수령 가능한 보상이 있는 경우 CollectionUI에 알림 요청을 보냅니다.
    /// </summary>
    private void CheckAllRewards()
    {
        Dictionary<string, int> inventory = InventoryManager.Instance.InventoryRead();

        foreach (var entry in inventory)
        {
            string fishKey = entry.Key;

            if (HasClaimableReward(fishKey))
            {
                collectionUI.ShowRedDot(fishKey);
            }
            else
            {
                // 꺼주는 함수도 추가 필요
                collectionUI.HideRedDot(fishKey);
            }
        }
    }

    /// <summary>
    /// 상세창이 표시될 때 해당 물고기 키를 저장합니다.
    /// </summary>
    public void SetFishKey(string key)
    {
        currentFishKey = key;
    }

    /// <summary>
    /// 현재 물고기 키에 대해 보상을 지급하고 UI를 처리합니다.
    /// </summary>
    public void ClaimReward(int index, Button button)
    {
        if (string.IsNullOrEmpty(currentFishKey))
        {
            Log.Warn("[PresentManager] currentFishKey가 설정되지 않았습니다.", this);
            return;
        }

        Log.Info($"[PresentManager] 수령 처리 시작 - key: {currentFishKey}, index: {index}", this);

        if (!CanClaim(currentFishKey, index)) return;

        int amount = rewardAmounts[index];
        GoldManager.Instance.AddGold(amount);
        GetClaimedStatus(currentFishKey)[index] = true;

        // 버튼 비활성화 및 시각 처리
        button.interactable = false;

        var children = button.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child == button.transform) continue;

            if (child.TryGetComponent(out CanvasGroup group))
            {
                group.alpha = 0.5f;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        Log.System($"[PresentManager] {currentFishKey} - {index + 1}번째 선물 수령 완료 (+{amount}G)", this);
    }

    /// <summary>
    /// 해당 키의 수령 여부 배열을 반환합니다.
    /// </summary>
    public bool[] GetClaimedStatus(string fishKey)
    {
        if (!presentClaimed.ContainsKey(fishKey))
        {
            presentClaimed[fishKey] = new bool[3];
        }
        return presentClaimed[fishKey];
    }

    /// <summary>
    /// 특정 선물이 수령 가능한지 여부를 반환합니다.
    /// </summary>
    public bool CanClaim(string fishKey, int index)
    {
        if (index < 0 || index >= 3) return false;

        int count = InventoryManager.Instance.GetCount(fishKey);
        bool alreadyClaimed = GetClaimedStatus(fishKey)[index];

        return count >= requiredCounts[index] && !alreadyClaimed;
    }

    /// <summary>
    /// 해당 물고기에 대해 수령 가능한 보상이 하나라도 있는지 확인합니다.
    /// </summary>
    public bool HasClaimableReward(string fishKey)
    {
        for (int i = 0; i < 3; i++)
        {
            if (CanClaim(fishKey, i)) return true;
        }
        return false;
    }

    #endregion
}
