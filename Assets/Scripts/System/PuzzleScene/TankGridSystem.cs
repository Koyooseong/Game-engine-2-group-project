using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 수조 씬의 격자 셀을 간격 포함하여 생성하고,
/// 각 셀을 TankGridManager에 등록합니다.
/// </summary>
public class TankGridSystem : MonoBehaviour
{
    #region Variables

    [Header("격자 설정")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private RectTransform gridParent;
    [SerializeField] private int gridSize = 9;
    [SerializeField] private float cellSize = 86f;
    [SerializeField] private float cellGap = 16f;

    [Header("의존성")]
    [SerializeField] private TankGridManager gridManager;

    private GameObject[,] gridCells;

    private const float CELL_ALPHA_LOCKED = 0.3f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        CreateGrid();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 간격을 포함하여 격자 셀을 생성하고 매니저에 등록합니다.
    /// </summary>
    private void CreateGrid()
    {
        if (gridManager == null)
        {
            Log.Error("[TankGridSystem] TankGridManager가 연결되지 않았습니다", this);
            return;
        }

        gridCells = new GameObject[gridSize, gridSize];

        float totalCellSize = cellSize + cellGap;
        float offset = totalCellSize * (gridSize - 1) / 2f;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject cell = Instantiate(gridCellPrefab, gridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();

                rt.sizeDelta = new Vector2(cellSize, cellSize);
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2((x * totalCellSize) - offset, (y * totalCellSize) - offset);

                gridCells[x, y] = cell;

                // TankGridController 연결
                TankGridController controller = cell.GetComponent<TankGridController>();
                if (controller == null)
                {
                    Log.Error("[TankGridSystem] gridCellPrefab에 TankGridController 없음", cell);
                    continue;
                }

                Vector2Int gridPos = new Vector2Int(x, y);

                int center = gridSize / 2;
                bool isUnlocked = Mathf.Abs(x - center) <= 1 && Mathf.Abs(y - center) <= 1;


                controller.Initialize(gridPos, isUnlocked);
                gridManager.RegisterGrid(gridPos, controller);
            }
        }

        Log.System($"[TankGridSystem] 격자 생성 완료 ({gridSize}x{gridSize})", this);
    }

    #endregion
}
