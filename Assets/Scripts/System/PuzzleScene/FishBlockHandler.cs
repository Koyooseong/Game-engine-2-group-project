using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 격자 위에 배치된 퍼즐 블록을 다시 드래그하거나, 회전/뒤집기/삭제 UI를 표시 및 처리합니다.
/// </summary>
public class FishBlockHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Variables

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;

    private bool isSelected = false;
    private GameObject buttonUI; // 추후 회전/뒤집기 버튼 연결 예정

    private static readonly string inventoryAreaTag = "PuzzleInventoryArea"; // 삭제용 영역 태그
    private static readonly string blockCellTag = "FishBlockCell"; // 블록 셀 태그

    private bool overlapPreviously = false;

    private Vector2Int? previousGridPos = null;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        bool isOverlapping = IsOverlapping();

        if (isOverlapping != overlapPreviously)
        {
            overlapPreviously = isOverlapping;
            SetHighlightVisible(isOverlapping);
        }
    }

    #endregion

    #region Interface Methods

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;

        if (isSelected)
        {
            ShowButtons();
        }
        else
        {
            HideButtons();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        isSelected = false;
        HideButtons();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position, eventData.pressEventCamera,
            out Vector2 localPoint
        );
        rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsInInventoryArea())
        {
            if (previousGridPos.HasValue)
            {
                TankGridManager.Instance.ClearBlock(previousGridPos.Value);
            }

            Debug.Log("[FishBlockHandler] 인벤토리로 드래그됨 - 삭제 처리");
            ReturnBlockToInventory();
            Destroy(gameObject);
            return;
        }

        Vector2Int nearestGrid = TankGridManager.Instance.GetNearestGrid(transform.position);
        TankGridController cell = TankGridManager.Instance.GetGrid(nearestGrid);

        if (cell == null || !cell.IsUnlocked())
        {
            Debug.Log("[FishBlockHandler] 유효하지 않은 셀");
            return;
        }

        rectTransform.anchoredPosition = cell.GetComponent<RectTransform>().anchoredPosition + new Vector2(0f, 144f);

        if (previousGridPos.HasValue)
        {
            TankGridManager.Instance.ClearBlock(previousGridPos.Value);
        }

        TankGridManager.Instance.SetBlock(nearestGrid);
        previousGridPos = nearestGrid;

        Debug.Log($"[FishBlockHandler] 스냅 완료 및 등록: {nearestGrid}");
    }

    #endregion

    #region Custom Methods

    private void ShowButtons()
    {
        Debug.Log("[FishBlockHandler] 버튼 표시");
    }

    private void HideButtons()
    {
        Debug.Log("[FishBlockHandler] 버튼 숨김");
    }

    private bool IsInInventoryArea()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag(inventoryAreaTag)) return true;
        }
        return false;
    }

    private void ReturnBlockToInventory()
    {
        Debug.Log("[FishBlockHandler] 인벤토리 수량 복구 필요");
    }

    private bool IsOverlapping()
    {
        var images = GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            Vector3 worldPos = image.rectTransform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 5f);

            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;
                if (hit.CompareTag(blockCellTag))
                {
                    Debug.Log($"[겹침 감지] {hit.name}");
                    return true;
                }
            }
        }

        return false;
    }

    private void SetHighlightVisible(bool visible)
    {
        var highlights = GetComponentsInChildren<Image>(true);
        foreach (var image in highlights)
        {
            if (image.gameObject.name.Contains("Highlight"))
            {
                image.gameObject.SetActive(visible);
                Debug.Log($"[Highlight] {image.name} → {visible}");
            }
        }
    }

    #endregion
}
