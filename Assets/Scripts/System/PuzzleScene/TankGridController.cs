using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 수조 셀의 상태를 제어합니다. 잠금 여부, 블럭 배치 여부 등을 관리하며,
/// 상태 변화 시 TankGridManager에 알립니다.
/// </summary>
public class TankGridController : MonoBehaviour
{
    #region Variables

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject lockOverlay;

    private Vector2Int gridPosition;
    private bool isUnlocked;
    private bool hasBlock;

    private TankGridManager manager;

    private static readonly Color UnlockedColor = Color.white;
    private static readonly Color LockedColor = new Color(1f, 1f, 1f, 0.3f);

    #endregion

    #region Public Methods

    /// <summary>
    /// 셀의 초기 위치 및 해금 상태를 설정합니다.
    /// </summary>
    public void Initialize(Vector2Int position, bool unlocked)
    {
        gridPosition = position;
        isUnlocked = unlocked;
        hasBlock = false;

        manager = TankGridManager.Instance;
        if (manager == null)
        {
            Log.Error("[TankGridController] TankGridManager를 찾을 수 없습니다", this);
        }

        UpdateVisual();
    }

    /// <summary>
    /// 셀을 해금 상태로 전환합니다.
    /// </summary>
    public void Unlock()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        UpdateVisual();

        Log.System($"[TankGridController] 해금됨: {gridPosition}", this);
    }

    /// <summary>
    /// 셀의 블럭 상태를 변경합니다.
    /// </summary>
    public void SetBlockState(bool value)
    {
        hasBlock = value;

        if (manager != null)
        {
            if (value)
                manager.SetBlock(gridPosition);
            else
                manager.ClearBlock(gridPosition);
        }
    }

    public bool IsUnlocked() => isUnlocked;

    public bool HasBlock() => hasBlock;

    public Vector2Int GetGridPosition() => gridPosition;

    #endregion

    #region Private Methods

    /// <summary>
    /// 해금 여부에 따른 색상 및 락 UI 업데이트
    /// </summary>
    private void UpdateVisual()
    {
        backgroundImage.color = isUnlocked ? UnlockedColor : LockedColor;

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!isUnlocked);
        }
    }



    #endregion
}
