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
    private GameObject buttonUI; // 추후 회전/뒤집기 버튼 연결 예정

    private static readonly string inventoryAreaTag = "PuzzleInventoryArea"; // 삭제용 영역 태그
    private static readonly string blockCellTag = "FishBlockCell"; // 블록 셀 태그

    private bool overlapPreviously = false;

    private Vector2Int? previousGridPos = null;

    private PuzzleInventoryItemUI itemUI; // 드래그 시 넘겨받은 인벤토리 항목 참조


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
        // 겹침 or 잠김 셀 감지되면 블록 제거
        if (IsOverlapping())
        {
            Debug.Log("[FishBlockHandler] 겹침 또는 잠긴 칸 위에 놓여 블록 제거");

            ReturnBlockToInventory();
            Destroy(gameObject);
            return;
        }

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
            Debug.Log("[FishBlockHandler] 유효하지 않은 셀 (잠김 또는 없음)");

            ReturnBlockToInventory(); // 인벤토리로 다시 보내기
            Destroy(gameObject);      // 블록 삭제
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
        if (buttonUI != null) return;

        buttonUI = Instantiate(buttonUIPrefab, transform);
        buttonUI.transform.localPosition = Vector3.zero;

        BlockButtonUI buttonScript = buttonUI.GetComponent<BlockButtonUI>();
        if (buttonScript != null)
        {
            buttonScript.Initialize(RotateBlock, FlipBlock);
            Debug.Log("[FishBlockHandler] 버튼 UI 생성 및 이벤트 연결 완료");
        }
        else
        {
            Debug.LogWarning("[FishBlockHandler] BlockButtonUI 스크립트가 프리팹에 없습니다!");
        }
    }


    private void HideButtons()
    {
       
        if (buttonUI != null)
        {
            Destroy(buttonUI);
            buttonUI = null;
            Debug.Log("[FishBlockHandler] 버튼 UI 제거 완료");
        }
    }

    public void SetItemUI(PuzzleInventoryItemUI ui)
    {
        itemUI = ui;
    }

    /// <summary>
    /// 버튼 UI 프리팹을 설정합니다.
    /// </summary>
    public void SetButtonUIPrefab(GameObject prefab)
    {
        buttonUIPrefab = prefab;
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
        if (itemUI != null)
        {
            itemUI.RestoreOne(); // 인벤토리 수량 복구
            Debug.Log("[FishBlockHandler] 인벤토리 수량 1개 복구 완료");
        }
        else
        {
            Debug.LogWarning("[FishBlockHandler] itemUI 참조 없음 - 수량 복구 실패");
        }
    }


    private bool IsOverlapping()
    {
        var images = GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            Vector3 worldPos = image.rectTransform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, 2f);

            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;


                // 블록 겹침 감지 (기존 로직)
                if (hit.CompareTag(blockCellTag))
                {
                    Debug.Log($"[겹침 감지] {hit.name}");
                    return true;
                }

                TankGridController grid = hit.GetComponent<TankGridController>();
                

                if (grid != null)
                {
                    Vector2Int gridPos = grid.GetGridPosition();
                    bool unlocked = grid.IsUnlocked();

                    //Debug.Log($"[디버그] 셀 위치 {gridPos} / 해금 여부: {unlocked}");

                    if (!grid.IsUnlocked())
                    {
                        Debug.LogWarning($"[잠김 격자 감지] → {gridPos}");
                        return true;
                    }
                }


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

    /// <summary>
    /// 블록을 시계 방향으로 90도 회전시킵니다.
    /// </summary>
    private void RotateBlock()
    {
        transform.Rotate(0f, 0f, -90f);
        Debug.Log("[FishBlockHandler] 블록 회전");
    }

    /// <summary>
    /// 블록을 좌우로 반전시킵니다.
    /// </summary>
    private void FlipBlock()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        Debug.Log("[FishBlockHandler] 블록 좌우 반전");
    }


    #endregion
}
