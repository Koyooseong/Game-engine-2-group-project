using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 수조 씬의 격자 셀을 간격 포함하여 생성합니다.
/// -> 왜 스크립트로 생성했냐면 나중에 10x10로 확장 가능 및 물고리 블록 인식 가능 원활
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

    private GameObject[,] gridCells;

    #endregion

    #region Unity Methods

    private void Start()
    {
        CreateGrid();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 간격을 포함하여 9x9 격자 셀을 생성합니다.
    /// </summary>
    private void CreateGrid()
    {
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
            }
        }
    }

    #endregion
}
