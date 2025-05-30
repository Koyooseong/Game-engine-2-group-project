using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 잠금 상태의 TankGrid를 해금 처리하는 시스템입니다.
/// </summary>
public class TankGridLockSystem : MonoBehaviour
{
    #region Variables

    [Header("설정")]
    [SerializeField] private int unlockCost = 30;

    [Header("의존성")]
    [SerializeField] private TankGridManager gridManager;

    #endregion

    #region Public Methods

    /// <summary>
    /// 특정 좌표의 셀을 해금 시도합니다.
    /// </summary>
    public void TryUnlock(Vector2Int position)
    {
        TankGridController target = gridManager.GetGrid(position);

        if (target == null)
        {
            Log.Warn($"[TankGridLockSystem] 대상 셀 없음: {position}", this);
            return;
        }

        if (target.IsUnlocked())
        {
            Log.Info($"[TankGridLockSystem] 이미 해금된 셀: {position}", this);
            return;
        }

        if (!HasEnoughGold())
        {
            Log.Warn("[TankGridLockSystem] 골드 부족: 해금 불가", this);
            return;
        }

        target.Unlock();
        ConsumeGold();

        Log.System($"[TankGridLockSystem] 해금 성공: {position} (-{unlockCost}G)", this);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 현재 골드가 충분한지 확인합니다.
    /// </summary>
    private bool HasEnoughGold()
    {
        return GoldManager.Instance.GetGold() >= unlockCost;
    }

    /// <summary>
    /// 해금 비용만큼 골드를 차감합니다.
    /// </summary>
    private void ConsumeGold()
    {
        GoldManager.Instance.RemoveGold(unlockCost);
    }

    #endregion
}
