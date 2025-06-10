using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 격자 위에 배치된 퍼즐 블록을 다시 드래그하거나, 회전/뒤집기/삭제 UI를 표시 및 처리합니다.
/// </summary>
public class FishBlockHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Variables

    [SerializeField] private GameObject buttonUIPrefab;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;

    private bool isSelected = false;
    private GameObject buttonUI;

    private static readonly string inventoryAreaTag = "PuzzleInventoryArea";
    private static readonly string blockCellTag = "FishBlockCell";

    private bool overlapPreviously = false;
    private Vector2Int? previousGridPos = null;

    private PuzzleInventoryItemUI itemUI;

    private string manualFishKey;

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
        if (isSelected) ShowButtons();
        else HideButtons();
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
        if (IsOverlapping())
        {
            Log.Warn("[FishBlockHandler] 겹침 또는 잠긴 칸 위에 놓여 블록 제거", this);
            ReturnBlockToInventory();
            Destroy(gameObject);
            return;
        }

        if (IsInInventoryArea())
        {
            if (previousGridPos.HasValue)
                TankGridManager.Instance.ClearBlock(previousGridPos.Value);

            Log.System("[FishBlockHandler] 인벤토리로 드래그됨 - 삭제 처리", this);
            ReturnBlockToInventory();
            Destroy(gameObject);
            return;
        }

        Vector2Int nearestGrid = TankGridManager.Instance.GetNearestGrid(transform.position);
        TankGridController cell = TankGridManager.Instance.GetGrid(nearestGrid);

        if (cell == null || !cell.IsUnlocked())
        {
            Log.Warn("[FishBlockHandler] 유효하지 않은 셀 (잠김 또는 없음)", this);
            ReturnBlockToInventory();
            Destroy(gameObject);
            return;
        }

        rectTransform.anchoredPosition = cell.GetComponent<RectTransform>().anchoredPosition + new Vector2(0f, 144f);

        if (previousGridPos.HasValue)
            TankGridManager.Instance.ClearBlock(previousGridPos.Value);

        TankGridManager.Instance.SetBlock(nearestGrid, this); // ⬅ 핵심 수정
        previousGridPos = nearestGrid;

        Log.System($"[FishBlockHandler] 스냅 완료 및 등록: {nearestGrid}", this);
    }

    #endregion

    #region Custom Methods

    private void ShowButtons()
    {
        if (buttonUI != null) return;

        buttonUI = Instantiate(buttonUIPrefab, transform);
        buttonUI.transform.localPosition = Vector3.zero;

        BlockButtonUI buttonScript = buttonUI.GetComponent<BlockButtonUI>();
        if (buttonScript != null)
            buttonScript.Initialize(RotateBlock, FlipBlock);
    }

    private void HideButtons()
    {
        if (buttonUI != null)
        {
            Destroy(buttonUI);
            buttonUI = null;
        }
    }

    public void SetItemUI(PuzzleInventoryItemUI ui)
    {
        itemUI = ui;
    }

    public void SetButtonUIPrefab(GameObject prefab)
    {
        buttonUIPrefab = prefab;
    }

    private bool IsInInventoryArea()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
        PointerEventData pointer = new PointerEventData(EventSystem.current) { position = screenPos };

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
        if (itemUI != null)
            itemUI.RestoreOne();
    }

    public bool IsOverlapping()
    {
        var images = GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            Vector3 worldPos = image.rectTransform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 2f);

            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;

                if (hit.CompareTag(blockCellTag))
                    return true;

                TankGridController grid = hit.GetComponent<TankGridController>();
                if (grid != null && !grid.IsUnlocked())
                    return true;
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
                image.gameObject.SetActive(visible);
        }
    }

    private void RotateBlock()
    {
        transform.Rotate(0f, 0f, -90f);
    }

    private void FlipBlock()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// 저장 시 사용할 물고기 키를 반환합니다.
    /// </summary>
    public string GetFishKey()
    {
        if (itemUI != null) return itemUI.GetFishKey();
        return manualFishKey;
    }
    /// <summary>
    /// 저장된 위치로 강제 배치할 때 호출됩니다.
    /// </summary>
    public void ForceSnapTo(Vector2Int gridPos)
    {
        TankGridController cell = TankGridManager.Instance.GetGrid(gridPos);
        if (cell == null) return;

        rectTransform.anchoredPosition = cell.GetComponent<RectTransform>().anchoredPosition + new Vector2(0f, 144f);

        TankGridManager.Instance.SetBlock(gridPos, this); // ✅ 필수 등록
        previousGridPos = gridPos;

        Log.System($"[FishBlockHandler] 저장 불러오기: 위치 스냅 및 등록 완료 ({gridPos})", this);
    }

    public void SetFishIdManually(string fishId)
    {
        manualFishKey = fishId;
    }

    #endregion
}
