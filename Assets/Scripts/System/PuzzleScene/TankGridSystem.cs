using UnityEngine;

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
    [SerializeField] private TankGridLockSystem lockSystem;

    private GameObject[,] gridCells;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (TankGridManager.Instance == null)
        {
            Debug.LogError("[TankGridSystem] TankGridManager.Instance가 null입니다.");
            return;
        }

        CreateGrid();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 간격을 포함하여 격자 셀을 생성하고 매니저에 등록합니다.
    /// </summary>
    private void CreateGrid()
    {
        gridCells = new GameObject[gridSize, gridSize];

        float totalCellSize = cellSize + cellGap;
        float offset = totalCellSize * (gridSize - 1) / 2f;
        Vector2Int center = new Vector2Int(gridSize / 2, gridSize / 2);

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                GameObject cell = Instantiate(gridCellPrefab, gridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();

                rt.sizeDelta = new Vector2(cellSize, cellSize);
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2((x * totalCellSize) - offset, (y * totalCellSize) - offset);

                gridCells[x, y] = cell;

                TankGridController controller = cell.GetComponent<TankGridController>();
                if (controller == null)
                {
                    Debug.LogError($"[TankGridSystem] 프리팹에 TankGridController 없음 ({position})");
                    continue;
                }

                bool isUnlocked = Mathf.Abs(x - center.x) <= 1 && Mathf.Abs(y - center.y) <= 1;

                controller.Initialize(position, isUnlocked);
                controller.SetLockSystem(lockSystem);

                TankGridManager.Instance.RegisterGrid(position, controller);
            }
        }

        Debug.Log($"[TankGridSystem] 격자 생성 완료 ({gridSize}x{gridSize})");
    }

    #endregion
}
