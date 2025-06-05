using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 프리팹을 기준으로 생성된 오브젝트를 재사용하는 풀링 시스템입니다.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    #region Variables

    [Header("프리팹")]
    [Tooltip("이 풀에서 생성하고 재사용할 프리팹")]
    [SerializeField] private GameObject prefab;

    private readonly Queue<GameObject> pool = new();

    #endregion

    #region Custom Methods

    /// <summary>
    /// 프리팹을 런타임에 외부에서 설정합니다.
    /// </summary>
    /// <param name="prefab">설정할 프리팹</param>
    public void SetPrefab(GameObject prefab)
    {
        if (this.prefab != null)
        {
            Debug.LogWarning($"[ObjectPool] 기존 prefab이 덮어씌워집니다: {this.prefab.name} → {prefab.name}", this);
        }

        this.prefab = prefab;
    }

    /// <summary>
    /// 오브젝트를 풀에서 꺼내 활성화 후 반환합니다.
    /// </summary>
    public GameObject Get()
    {
        if (prefab == null)
        {
            Debug.LogError("[ObjectPool] 프리팹이 설정되지 않았습니다.", this);
            return null;
        }

        GameObject obj = (pool.Count > 0)
            ? pool.Dequeue()
            : Instantiate(prefab, transform);

        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 오브젝트를 비활성화하여 풀로 반환합니다.
    /// </summary>
    /// <param name="obj">반환할 오브젝트</param>
    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }

    #endregion
}
