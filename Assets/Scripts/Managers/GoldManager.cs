using UnityEngine;

/// <summary>
/// 골드 재화를 전역에서 관리하는 매니저입니다.
/// </summary>
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    #region Variables

    private int gold = 0;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 원할 경우 추가 가능
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 골드를 추가합니다.
    /// </summary>
    public void AddGold(int amount)
    {
        gold += amount;
        Log.Info($"골드 증가: +{amount} → 총 {gold}G", this);
    }

    /// <summary>
    /// 골드를 감소시킵니다. (0보다 작아지지 않음)
    /// </summary>
    public void RemoveGold(int amount)
    {
        gold -= amount;
        if (gold < 0) gold = 0;
        Log.Info($"골드 감소: -{amount} → 총 {gold}G", this);
    }

    /// <summary>
    /// 현재 보유 골드를 반환합니다.
    /// </summary>
    public int GetGold()
    {
        return gold;
    }

    /// <summary>
    /// 골드가 충분한지 검사합니다.
    /// </summary>
    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }


    #endregion
}
