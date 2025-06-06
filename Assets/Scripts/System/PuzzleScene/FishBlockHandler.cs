using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 드래그 가능한 퍼즐 블록의 UI 및 선택/이동 처리를 담당합니다.
/// </summary>
public class FishBlockHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    #region Variables
    public Vector2Int AnchorOffset => anchorOffset;

    private static FishBlockPlacer sharedPlacer;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;

    private bool isSelected = false;
    private Vector2Int[] shapeOffsets;

    [SerializeField] private FishBlockPlacer fishBlockPlacer; // 주입 없이도 SharedPlacer로 대체
    [SerializeField] private Vector2Int anchorOffset = Vector2Int.zero;
    #endregion

    #region Unity Methods

    private void Start()
    {
        if (fishBlockPlacer == null)
        {
            fishBlockPlacer = sharedPlacer;
            Log.System("[Handler] SharedPlacer 연결 시도", this);

            if (fishBlockPlacer == null)
            {
                Log.Error("[Handler] SharedPlacer 연결 실패! Placer가 아직 등록되지 않았음", this);
            }
            else
            {
                Log.Info("[Handler] SharedPlacer 연결 성공", this);
            }
        }
    }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (rectTransform == null)
            Log.Error("[Handler] RectTransform 없음!", this);

        if (canvas == null)
            Log.Error("[Handler] 상위에서 Canvas를 못 찾음!", this);
    }



    #endregion

    #region Interface Methods

    public void SetAnchorOffset(Vector2Int offset)
    {
        anchorOffset = offset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Log.System("[FishBlockHandler] OnBeginDrag 호출됨", this);
        originalPosition = rectTransform.anchoredPosition;
        isSelected = false;
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
        Log.System("[OnEndDrag] 시작됨", this);
        Log.System($"[OnEndDrag] fishBlockPlacer is null? {fishBlockPlacer == null}", this);
        Log.System("[Drag] OnEndDrag", this);
        Vector3 dropPos = transform.position;

        if (!fishBlockPlacer.TryPlaceBlock(this, dropPos))
        {
            rectTransform.anchoredPosition = originalPosition;
            Log.Warn("[FishBlockHandler] 배치 실패, 원위치로 복귀");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
    }

    #endregion

    #region Public API

    /// <summary>
    /// 퍼즐 모양 정보를 등록합니다.
    /// </summary>
    public void SetShape(Vector2Int[] shape)
    {
        shapeOffsets = shape;
        Log.System($"[Handler] shapeOffsets 등록됨: {shapeOffsets?.Length}", this);
    }


    /// <summary>
    /// 퍼즐 모양 정보를 반환합니다.
    /// </summary>
    public Vector2Int[] GetShape()
    {
        return shapeOffsets;
    }

    /// <summary>
    /// 그리드 위치에 블록을 스냅합니다.
    /// </summary>
    public void SnapToGrid(Vector2Int anchor)
    {
        Log.System($"[SnapToGrid] anchor = {anchor}, anchorOffset = {anchorOffset}", this);

        var cell = TankGridManager.Instance.GetGrid(anchor);

        if (cell == null)
        {
            Log.Error($"[SnapToGrid] GridManager에 등록되지 않은 anchor 위치입니다: {anchor}", this);
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        RectTransform cellRect = cell.GetComponent<RectTransform>();
        if (cellRect == null)
        {
            Log.Error($"[SnapToGrid] GridCell에 RectTransform이 없습니다: {anchor}", cell);
            return;
        }

        // 오프셋만큼 시각 위치 조정 (블록 기준 셀이 왼쪽 위인 경우 보정)
        float cellSpacing = 86f + 16f; // cellSize + gap
        Vector2 visualOffset = new Vector2(anchorOffset.x * cellSpacing, -anchorOffset.y * cellSpacing);

        rectTransform.anchoredPosition = cellRect.anchoredPosition - visualOffset + new Vector2(0f, 144f);

        Log.System($"[SnapToGrid] anchor = {anchor}, anchorOffset = {anchorOffset}", this);


    }




    /// <summary>
    /// 외부에서 Placer를 수동 주입합니다.
    /// </summary>
    public void SetPlacer(FishBlockPlacer placer)
    {
        fishBlockPlacer = placer;
    }

    /// <summary>
    /// 정적 방식으로 모든 블록에게 Placer를 공유합니다.
    /// </summary>
    public static void RegisterPlacer(FishBlockPlacer placer)
    {
        sharedPlacer = placer;
    }

    #endregion
}
