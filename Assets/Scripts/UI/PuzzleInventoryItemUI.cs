using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 퍼즐 씬 인벤토리 슬롯 1개의 UI. 드래그 및 시각 상태를 제어합니다.
/// </summary>
public class PuzzleInventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image panelBackground;

    [Header("Drag Dependencies")]
    [SerializeField] private FishBlockBuilder fishBlockBuilder;

    private FishData fishData;
    private int totalCount;
    private GameObject currentBlock;
    private RectTransform canvasRect;

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void Initialize(FishData data, int ownedCount)
    {
        fishData = data;
        totalCount = ownedCount;

        fishImage.sprite = Resources.Load<Sprite>(data.puzzleImg);
        fishNameText.text = data.name;
        goldText.text = $"{data.gold}G / 5s";
        countText.text = $"{ownedCount} / {ownedCount}";

        if (ownedCount <= 0)
        {
            panelBackground.color = new Color(1f, 1f, 1f, 0.3f); // 투명도 낮춤
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log($"어떤 물고기 드래그 했는지: {fishData.name}");//디버그 테스트
        if (totalCount <= 0) return;

        currentBlock = fishBlockBuilder.Build(fishData); 
        if (currentBlock != null)
            currentBlock.transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentBlock == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);
        currentBlock.GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentBlock != null)
        {
            Destroy(currentBlock);
        }
    }
}
