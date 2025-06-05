using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TankGridController : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private GameObject lockOverlay;

    private bool isUnlocked = false;
    private bool hasBlock = false;
    private Vector2Int gridPosition;
    private TankGridLockSystem lockSystem;

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

    public void SetBlockState(bool has)
    {
        hasBlock = has;
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    public bool HasBlock()
    {
        return hasBlock;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void UpdateVisual()
    {
        background.color = isUnlocked ? Color.white : new Color(1f, 1f, 1f, 0.3f);
        if (lockOverlay != null)
            lockOverlay.SetActive(!isUnlocked);
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        Debug.Log($"클릭됨: {gridPosition}");
        if (isUnlocked) return;

        if (lockSystem != null)
        {

            Debug.Log("해금 시도 중...");
            lockSystem.TryUnlock(gridPosition);
        }
        else
        {
            Debug.LogWarning("[TankGridController] lockSystem이 연결되지 않았습니다.");
        }
    }

    private void OnMouseDown()
    {
        if (isUnlocked) return;

        if (lockSystem != null)
        {
            lockSystem.TryUnlock(gridPosition);
        }
        else
        {
            Debug.LogWarning("TankGridLockSystem이 연결되지 않았습니다.");
        }
    }
}
