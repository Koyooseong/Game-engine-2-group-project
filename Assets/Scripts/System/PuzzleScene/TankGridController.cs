using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 수조 퍼즐 셀 하나를 제어하는 컨트롤러
/// </summary>
[RequireComponent(typeof(Button))]
public class TankGridController : MonoBehaviour, IPointerClickHandler
{
    #region Variables

    [SerializeField] private Image background;
    [SerializeField] private GameObject lockOverlay;

    private bool isUnlocked = false;
    private bool hasBlock = false;
    private Vector2Int gridPosition;

    private TankGridLockSystem lockSystem;

    #endregion

    #region Public Methods

    public void Initialize(Vector2Int position, bool unlocked)
    {
        gridPosition = position;
        isUnlocked = unlocked;
        UpdateVisual();
    }

    public void SetLockSystem(TankGridLockSystem system)
    {
        lockSystem = system;
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
    }

    public bool IsUnlocked => isUnlocked;
    public bool HasBlock => hasBlock;
    public Vector2Int GridPosition => gridPosition;

    public void SetBlockState(bool has)
    {
        hasBlock = has;
    }

    #endregion

    #region UI Visuals

    public void UpdateVisual()
    {
        if (background != null)
            background.color = isUnlocked ? Color.white : new Color(1f, 1f, 1f, 0.3f);

        if (lockOverlay != null)
            lockOverlay.SetActive(!isUnlocked);
    }


    #endregion

    #region UI Events

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            // 추후 드래그 배치 로직 연결 가능
            return;
        }

        if (lockSystem == null)
        {
            Log.Error("TankGridLockSystem 연결되지 않음", this);
            return;
        }

        lockSystem.TryUnlock(this);
    }

    #endregion
}
