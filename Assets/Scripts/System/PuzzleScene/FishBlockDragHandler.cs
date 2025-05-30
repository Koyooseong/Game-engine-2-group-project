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
        ClearBlock();
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
            Destroy(currentBlock);
            currentBlock = null;
        }
    }

    #endregion
}
