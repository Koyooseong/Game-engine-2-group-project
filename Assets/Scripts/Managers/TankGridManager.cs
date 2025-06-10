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

    /// <summary>
    /// 해당 위치에 그리드 셀을 등록합니다.
    /// </summary>
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

    /// <summary>
    /// 블록이 해당 위치에 배치되었음을 기록합니다.
    /// </summary>
    public void SetBlock(Vector2Int position)
    {
        if (!gridMap.TryGetValue(position, out var controller))
        {
            Log.Error($"[TankGridManager] 블록 추가 실패 - 셀 없음: {position}", this);
            return;
        }

        controller.SetBlockState(true);
    }

    /// <summary>
    /// 블록이 제거되었음을 기록합니다.
    /// </summary>
    public void ClearBlock(Vector2Int position)
    {
        if (!gridMap.TryGetValue(position, out var controller))
        {
            Log.Error($"[TankGridManager] 블록 제거 실패 - 셀 없음: {position}", this);
            return;
        }

        controller.SetBlockState(false);
    }

    /// <summary>
    /// 해당 위치의 컨트롤러를 반환합니다.
    /// </summary>
    public TankGridController GetGrid(Vector2Int position)
    {
        return gridMap.TryGetValue(position, out var grid) ? grid : null;
    }

    public Dictionary<Vector2Int, TankGridController> GetAllGrids() => gridMap;

    public Vector2Int GetNearestGrid(Vector3 worldPos)
    {
        float minDistance = float.MaxValue;
        Vector2Int nearestGrid = Vector2Int.zero;

        foreach (var pair in gridMap)
        {
            TankGridController controller = pair.Value;
            Vector3 cellWorldPos = controller.GetComponent<RectTransform>().position;
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
