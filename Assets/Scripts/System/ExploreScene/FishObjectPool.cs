using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 물고기 ID를 기반으로 개별 오브젝트 풀을 자동으로 생성하고 관리하는 시스템입니다.
/// </summary>
public class FishObjectPool : MonoBehaviour
{
    #region Variables

    [Header("풀 부모 오브젝트")]
    [Tooltip("생성된 풀들을 자식으로 둘 Transform")]
    [SerializeField] private Transform poolParent;

    private readonly Dictionary<string, ObjectPool> poolDictionary = new();

    #endregion

    #region Custom Methods

    /// <summary>
    /// 해당 물고기 ID에 해당하는 풀에서 오브젝트를 꺼냅니다. 없으면 새로 생성합니다.
    /// </summary>
    /// <param name="fishKey">물고기 ID</param>
    public GameObject Get(string fishKey)
    {
        if (!poolDictionary.ContainsKey(fishKey))
        {
            CreateNewPool(fishKey);
        }

        return poolDictionary[fishKey].Get();
    }

    /// <summary>
    /// 해당 물고기 ID의 풀에 오브젝트를 반환합니다.
    /// </summary>
    /// <param name="fishKey">물고기 ID</param>
    /// <param name="obj">반환할 오브젝트</param>
    public void Release(string fishKey, GameObject obj)
    {
        if (poolDictionary.ContainsKey(fishKey))
        {
            poolDictionary[fishKey].Release(obj);
        }
        else
        {
            Destroy(obj); // 풀 없는 경우 예외 처리
        }
    }

    /// <summary>
    /// 새 풀을 생성하고 등록합니다.
    /// </summary>
    /// <param name="fishKey">물고기 ID</param>
    private void CreateNewPool(string fishKey)
    {
        GameObject prefab = FishDataManager.Instance.LoadFishPrefab(fishKey);

        if (prefab == null)
        {
            Log.Warn($"[FishPool] '{fishKey}'에 해당하는 프리팹을 찾을 수 없습니다.", this);
            return;
        }

        GameObject poolObject = new GameObject($"Pool_{fishKey}");
        poolObject.transform.SetParent(poolParent != null ? poolParent : transform);

        ObjectPool pool = poolObject.AddComponent<ObjectPool>();
        pool.SetPrefab(prefab);

        poolDictionary[fishKey] = pool;

        Log.System($"[FishPool] '{fishKey}' 풀 생성 완료", pool);
    }

    #endregion
}
