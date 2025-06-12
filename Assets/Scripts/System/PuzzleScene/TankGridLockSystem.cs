using UnityEngine;

/// <summary>
/// 잠긴 TankGrid 셀을 해금하고 골드를 차감하는 시스템.
/// 해금 비용은 셀 위치에 따라 다름 (5x5, 7x7, 9x9)
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
        popupUI.Initialize(this);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 셀 클릭 시 해금 시도 (골드 부족 검사 및 팝업 호출)
    /// </summary>
    public void TryUnlock(Vector2Int position)
    {
        TankGridController cell = gridManager.GetGrid(position);
        if (cell == null || cell.IsUnlocked()) return;

        int cost = GetUnlockCost(position);

        if (!GoldManager.Instance.HasEnoughGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            // TODO: 골드 부족 팝업 따로 구현 시 연결 가능
            return;
        }

        popupUI.Show(position, cost);
    }

    /// <summary>
    /// 팝업에서 [예] 버튼 눌렀을 때 호출됨
    /// </summary>
    public void ConfirmUnlock(Vector2Int position)
    {
        TankGridController cell = gridManager.GetGrid(position);
        if (cell == null || cell.IsUnlocked()) return;

        int cost = GetUnlockCost(position);
        GoldManager.Instance.RemoveGold(cost);
        cell.Unlock();

        Log.System($"[TankGridLockSystem] 해금 완료: {position}, 비용: {cost}G");

        // ✅ 해금만 저장 호출
        PuzzleSaveSystem saveSystem = FindObjectOfType<PuzzleSaveSystem>();
        if (saveSystem != null)
        {
            saveSystem.SaveUnlockOnly();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 셀 위치에 따른 해금 비용 계산 (중앙 기준 거리 기반)
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
