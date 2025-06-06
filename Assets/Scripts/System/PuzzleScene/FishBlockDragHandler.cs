using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리에서 블록을 드래그하고, 마우스를 따라 움직이며, 종료 시 초기화를 처리합니다.
/// </summary>
public class FishBlockDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables

    [Header("Drag Dependencies")]
    [SerializeField] private FishBlockBuilder fishBlockBuilder;

    private RectTransform canvasRect;
    private PuzzleInventoryItemUI itemUI;
    private GameObject currentBlock;

    #endregion

    #region Unity Methods

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        itemUI = GetComponent<PuzzleInventoryItemUI>();
    }

    #endregion

    #region Drag Events

    /// <summary>
    /// 드래그 시작 시 퍼즐 블록을 생성합니다.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!itemUI.TryConsumeOne()) return;

        CreateBlock();
        PositionBlock(eventData);
    }

    /// <summary>
    /// 드래그 중일 때 블록이 마우스를 따라다닙니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (currentBlock == null) return;
        PositionBlock(eventData);
    }

    /// <summary>
    /// 드래그 종료 시 블록을 제거합니다. (추후 배치 기능으로 대체 가능)
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentBlock == null)
        {
            Log.Warn("[OnEndDrag] currentBlock이 null입니다", this);
            return;
        }

        // 1. 충돌 감지
        Vector3 center = currentBlock.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, 10f);
        Log.System($"[OnEndDrag] 충돌 감지됨: 총 {hits.Length}개", this);

        // 2. 후보 중 가장 가까운 GridCell 찾기
        float minDist = float.MaxValue;
        Transform nearestCell = null;

        foreach (Collider2D col in hits)
        {
            Log.Info($"[OnEndDrag] 감지된 Collider: {col.name}", col);

            if (!col.CompareTag("GridCell"))
            {
                Log.Info($"[OnEndDrag] → 태그 불일치: {col.tag}", col);
                continue;
            }

            float dist = Vector3.Distance(center, col.transform.position);
            Log.Info($"[OnEndDrag] → GridCell {col.name} 거리: {dist}", col);

            if (dist < minDist)
            {
                minDist = dist;
                nearestCell = col.transform;
                Log.System($"[OnEndDrag] ★ 현재 가장 가까운 셀: {col.name}, 거리: {dist}", col);
            }
        }

        // 3. 스냅 위치 적용
        if (nearestCell != null)
        {
            RectTransform cellRT = nearestCell.GetComponent<RectTransform>();
            Vector2 snapPos = cellRT.anchoredPosition + new Vector2(0f, 144f);
            currentBlock.GetComponent<RectTransform>().anchoredPosition = snapPos;

            Log.System($"[OnEndDrag] 블록 스냅 완료: anchoredPosition {snapPos}", currentBlock);
        }
        else
        {
            Log.Warn("[OnEndDrag] 스냅할 셀을 찾지 못했습니다!", this);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 블록을 생성하고 캔버스에 등록합니다.
    /// </summary>
    private void CreateBlock()
    {
        if (itemUI == null)
        {
            Debug.LogError("[FishBlockDragHandler] itemUI가 null입니다.");
            return;
        }

        FishData data = itemUI.GetFishData();
        if (data.name == null)
        {
            Debug.LogError("[FishBlockDragHandler] fishData가 초기화되지 않았습니다.");
            return;
        }

        currentBlock = fishBlockBuilder.Build(itemUI.GetFishData());
        currentBlock.transform.SetParent(canvasRect);
        currentBlock.transform.SetAsLastSibling();

        FishBlockHandler handler = currentBlock.GetComponent<FishBlockHandler>();
        if (handler == null)
        {
            Debug.LogError(" FishBlockHandler가 currentBlock에 없습니다!", currentBlock);
        }

    }

    /// <summary>
    /// 블록의 위치를 마우스 위치에 맞게 이동시킵니다.
    /// </summary>
    private void PositionBlock(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);

        currentBlock.GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    /// <summary>
    /// 블록을 제거합니다. (임시 처리, 나중에 퍼즐 배치로 대체 예정)
    /// </summary>
    private void ClearBlock()
    {
        if (currentBlock != null)
        {
            //Destroy(currentBlock);
            currentBlock = null;
        }
    }

    #endregion
}
