using UnityEngine;

/// <summary>
/// 셀 해금 요청을 처리하고 골드를 차감하는 퍼즐 수조의 잠금 시스템.
/// </summary>
public class TankGridLockSystem : MonoBehaviour
{
    #region Variables

    [Header("설정")]
    [SerializeField] private TankGridManager gridManager;
    [SerializeField] private TankGridUnlockPopupUI popupUI;

    #endregion

    #region Unity Methods

    private void Start()
    {
        popupUI.OnConfirm += OnConfirmUnlock;
        popupUI.OnCancel += () => Log.System("해금 취소됨");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 셀 클릭 시 잠금 여부를 확인하고 해금 UI 팝업 호출
    /// </summary>
    public void TryUnlock(TankGridController cell)
    {
        if (cell == null || cell.IsUnlocked)
            return;

        int cost = GetUnlockCost(cell.GridPosition);

        if (!GoldManager.Instance.HasEnoughGold(cost))
        {
            Log.Warn("골드가 부족합니다.", this);
            return;
        }

        popupUI.Show(cell, cost);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 팝업에서 [예] 선택 시 호출되는 실제 해금 처리 로직
    /// </summary>
    private void OnConfirmUnlock(TankGridController cell)
    {
        int cost = GetUnlockCost(cell.GridPosition);

        if (!GoldManager.Instance.TryUseGold(cost))
        {
            Log.Warn("해금 도중 골드 부족", this);
            return;
        }

        cell.Unlock();
        Log.Info($"[해금 완료] {cell.GridPosition} 비용: {cost}G", this);
    }

    /// <summary>
    /// 셀 위치에 따라 해금 비용을 계산합니다 (중심에서 거리 기반)
    /// </summary>
    private int GetUnlockCost(Vector2Int position)
    {
        int center = gridManager.GridSize / 2;
        int dist = Mathf.Max(Mathf.Abs(position.x - center), Mathf.Abs(position.y - center));

        return dist switch
        {
            <= 2 => 30,
            <= 3 => 60,
            _ => 120
        };
    }

    #endregion
}
