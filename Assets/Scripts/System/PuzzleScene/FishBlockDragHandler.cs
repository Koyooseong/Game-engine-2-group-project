using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리에서 드래그 시 블록 복제본을 생성하고 마우스를 따라다니게 합니다.
/// </summary>
public class FishBlockDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject blockPrefab; // 복제할 블록 프리팹

    private GameObject currentBlock;
    private RectTransform canvasRect;

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 블록 프리팹 복제
        currentBlock = Instantiate(blockPrefab, canvasRect);
        currentBlock.transform.SetAsLastSibling(); // UI 최상단으로 올리기
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentBlock == null) return;

        // 마우스 위치 → Canvas 내 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);

        currentBlock.GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 아직 배치 검사 로직 없으므로, 드래그 끝나면 삭제
        if (currentBlock != null)
        {
            Destroy(currentBlock);
        }
    }
}
