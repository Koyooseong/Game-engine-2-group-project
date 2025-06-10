using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 TankGrid 정보를 저장하고 관리합니다.
/// 블록 배치, 제거 및 위치 정보 기록을 담당합니다.
/// </summary>
public class TankGridManager : MonoBehaviour
{
    #region Variables

    private Dictionary<Vector2Int, TankGridController> gridMap = new();
    private Dictionary<Vector2Int, FishBlockHandler> blockMap = new(); // 🆕

    public static TankGridManager Instance { get; private set; }

    [SerializeField] private int gridSize = 9;
    public int GridSize => gridSize;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Methods

    public void RegisterGrid(Vector2Int position, TankGridController controller)
    {
        if (gridMap.ContainsKey(position))
        {
            Log.Warn($"[TankGridManager] 중복 등록 시도: {position}", this);
            return;
        }

        gridMap[position] = controller;
        Log.System($"[TankGridManager] 셀 등록됨: {position}", this);
    }

    public void SetBlock(Vector2Int position, FishBlockHandler block)
    {
        if (!gridMap.TryGetValue(position, out var controller))
        {
            Log.Error($"[TankGridManager] 블록 추가 실패 - 셀 없음: {position}", this);
            return;
        }

        controller.SetBlockState(true);
        blockMap[position] = block;
    }

    public void ClearBlock(Vector2Int position)
    {
        if (!gridMap.TryGetValue(position, out var controller))
        {
            Log.Error($"[TankGridManager] 블록 제거 실패 - 셀 없음: {position}", this);
            return;
        }

        controller.SetBlockState(false);
        blockMap.Remove(position);
    }

    public TankGridController GetGrid(Vector2Int position)
    {
        return gridMap.TryGetValue(position, out var grid) ? grid : null;
    }

    public FishBlockHandler GetBlock(Vector2Int position)
    {
        return blockMap.TryGetValue(position, out var block) ? block : null;
    }

    public Dictionary<Vector2Int, TankGridController> GetAllGrids() => gridMap;

    public Dictionary<Vector2Int, FishBlockHandler> GetAllBlocks() => blockMap;

    public Vector2Int GetNearestGrid(Vector3 worldPos)
    {
        float minDistance = float.MaxValue;
        Vector2Int nearestGrid = Vector2Int.zero;

        foreach (var pair in gridMap)
        {
            Vector3 cellWorldPos = pair.Value.GetComponent<RectTransform>().position;
            float distance = Vector3.Distance(worldPos, cellWorldPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestGrid = pair.Key;
            }
        }

        return nearestGrid;
    }

    #endregion
}
