using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자식 오브젝트로 프리팹을 생성하고 재사용하는 풀링 시스템입니다.
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
    /// 오브젝트를 풀에서 꺼내 활성화하고 반환합니다.
    /// </summary>
    public GameObject Get()
    {
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
