using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 물고기 프리팹을 FishData 기준으로 풀링하여 관리하는 매니저입니다.
/// </summary>
public class FishPoolManager : MonoBehaviour
{
    #region Variables

    public static FishPoolManager Instance { get; private set; }

    [Header("풀 생성 대상 데이터 리스트")]
    [Tooltip("풀을 생성할 물고기 데이터 목록입니다.")]
    [SerializeField] private List<FishData> fishDataList;

    [Header("풀 루트 오브젝트")]
    [Tooltip("모든 풀 오브젝트는 이 오브젝트 하위에 생성됩니다.")]
    [SerializeField] private Transform poolRoot;

    [Header("풀 초기 생성 개수")]
    [SerializeField] private int initialPoolSize = 10;

    private Dictionary<FishType, PoolManager> fishPools = new();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (poolRoot == null)
        {
            Log.Error("FishPoolManager에 poolRoot가 지정되지 않았습니다.", this);
            return;
        }

        if (fishDataList == null || fishDataList.Count == 0)
        {
            Log.Error("FishPoolManager에 FishDataList가 비어 있습니다.", this);
            return;
        }

        InitializePools();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// FishData 기준으로 각 물고기별 풀을 생성합니다.
    /// </summary>
    private void InitializePools()
    {
        foreach (FishData data in fishDataList)
        {
            GameObject poolObj = new GameObject($"Pool_{data.fishType}");
            poolObj.transform.SetParent(poolRoot);

            PoolManager pool = poolObj.AddComponent<PoolManager>();
            pool.Initialize(data.prefab, initialPoolSize);

            fishPools.Add(data.fishType, pool);
        }
    }

    /// <summary>
    /// 지정한 물고기 타입에 해당하는 오브젝트를 반환합니다.
    /// </summary>
    public GameObject GetFish(FishData data)
    {
        if (!fishPools.TryGetValue(data.fishType, out var pool))
        {
            Log.Error($"풀에 {data.fishType} 이(가) 존재하지 않습니다!", this);
            return null;
        }

        return pool.Get();
    }

    public PoolManager GetPool(FishType type)
    {
        if (fishPools.TryGetValue(type, out var pool))
        {
            return pool;
        }

        Log.Error($"[FishPoolManager] {type} 타입 풀을 찾을 수 없습니다!", this);
        return null;
    }

    #endregion
}
