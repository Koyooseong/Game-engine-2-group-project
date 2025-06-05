using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 잡은 모든 물고기의 키를 보관하는 인벤토리 매니저입니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    #region Variables

    private Dictionary<string, int> fishInventory = new Dictionary<string, int>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 필요 시 초기화 가능
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 해당 키의 물고기를 인벤토리에 추가합니다.
    /// </summary>
    public void AddFish(string key)
    {
        if (fishInventory.ContainsKey(key))
        {
            fishInventory[key]++;
        }
        else
        {
            fishInventory[key] = 1;
        }

        Log.Info($"물고기 추가됨: {key} (총 {fishInventory[key]}개)", this);
    }

    /// <summary>
    /// 해당 키의 물고기를 인벤토리에서 제거합니다. (최소 0)
    /// </summary>
    public void RemoveFish(string key)
    {
        if (fishInventory.ContainsKey(key))
        {
            fishInventory[key]--;
            if (fishInventory[key] <= 0)
            {
                fishInventory.Remove(key);
                Log.Warn($"물고기 제거됨: {key} (0개 되어 삭제됨)", this);
            }
            else
            {
                Log.Info($"물고기 제거됨: {key} (남은 수량: {fishInventory[key]})", this);
            }
        }
    }

    /// <summary>
    /// 해당 키의 물고기 수량을 반환합니다.
    /// </summary>
    public int GetCount(string key)
    {
        return fishInventory.ContainsKey(key) ? fishInventory[key] : 0;
    }

    /// <summary>
    /// 현재 인벤토리의 모든 물고기 키와 수량을 반환합니다.
    /// </summary>
    public Dictionary<string, int> InventoryRead()
    {
        return new Dictionary<string, int>(fishInventory); // 복사본 반환 (외부 수정 방지)
    }

    #endregion
}
