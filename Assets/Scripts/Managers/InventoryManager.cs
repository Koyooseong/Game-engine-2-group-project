using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 잡은 모든 물고기의 키를 보관하는 인벤토리 매니저입니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    #region Variables

    private const string SAVE_KEY = "InventorySave";

    private Dictionary<string, int> fishInventory = new Dictionary<string, int>();

    [System.Serializable]
    private class InventoryData
    {
        public List<string> keys = new();
        public List<int> counts = new();
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadInventory();
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
        SaveInventory();
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

            SaveInventory();
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

    /// <summary>
    /// 인벤토리를 PlayerPrefs에 저장합니다.
    /// </summary>
    private void SaveInventory()
    {
        InventoryData data = new();
        foreach (var pair in fishInventory)
        {
            data.keys.Add(pair.Key);
            data.counts.Add(pair.Value);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Log.System("[InventoryManager] 인벤토리 저장 완료");
    }

    /// <summary>
    /// PlayerPrefs로부터 인벤토리를 불러옵니다.
    /// </summary>
    private void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Log.Warn("[InventoryManager] 저장된 인벤토리 없음");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        InventoryData data = JsonUtility.FromJson<InventoryData>(json);

        fishInventory.Clear();

        for (int i = 0; i < data.keys.Count; i++)
        {
            fishInventory[data.keys[i]] = data.counts[i];
        }

        Log.System($"[InventoryManager] 인벤토리 불러오기 완료 - {fishInventory.Count}종");
    }

    #endregion
}
