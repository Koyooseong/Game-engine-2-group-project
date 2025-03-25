using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 재사용 가능한 오브젝트 풀을 관리하는 클래스입니다.
/// </summary>
public class PoolManager : MonoBehaviour
{
    #region Variables

    [Header("풀 오브젝트")]
    [SerializeField] private GameObject prefab;

    [Header("풀 초기 크기")]
    [SerializeField] private int initialSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject instance = pool.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// 오브젝트를 풀로 되돌립니다.
    /// </summary>
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    #endregion
}
