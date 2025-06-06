using UnityEngine;

/// <summary>
/// 퍼즐 수조 격자를 생성하고 TankGridManager에 등록합니다.
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

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (lockSystem == null)
        {
            Log.Error("[TankGridSystem] lockSystem 연결되지 않음", this);
            return;
        }

        CreateGrid(); // 격자 생성 호출
    }




    #endregion

    #region Custom Methods

    /// <summary>
    /// 수조 격자를 생성하고 TankGridManager에 등록합니다.
    /// </summary>
    private void CreateGrid()
    {
      

        float totalCellSize = cellSize + cellGap;
        float offset = totalCellSize * (gridSize - 1) / 2f;
        Vector2Int center = new Vector2Int(gridSize / 2, gridSize / 2);

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GameObject cellGO = Instantiate(gridCellPrefab, gridParent);
                RectTransform rt = cellGO.GetComponent<RectTransform>();

                rt.sizeDelta = new Vector2(cellSize, cellSize);
                rt.anchoredPosition = new Vector2((x * totalCellSize) - offset, (y * totalCellSize) - offset);
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

                TankGridController controller = cellGO.GetComponent<TankGridController>();
                if (controller == null)
                {
                    Log.Error($"[TankGridSystem] 프리팹에 TankGridController 없음 ({pos})", cellGO);
                    continue;
                }

                bool unlocked = Mathf.Abs(x - center.x) <= 1 && Mathf.Abs(y - center.y) <= 1;

                controller.Initialize(pos, unlocked);
                controller.SetLockSystem(lockSystem);

                TankGridManager.Instance.RegisterGrid(pos, controller);
            }
        }

        Log.System($"[TankGridSystem] 격자 생성 완료 ({gridSize}x{gridSize})", this);
    }


    #endregion
}
