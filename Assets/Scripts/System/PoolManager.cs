using System.Collections.Generic;
using UnityEngine;
using Game;

public class PoolManager : MonoBehaviour
{
    #region Variables

    [Header("풀 오브젝트")]
    [SerializeField] private GameObject prefab;

    [Header("초기 풀 사이즈")]
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> poolQueue = new Queue<GameObject>();

    #endregion

    #region Unity Methods

    /*private void Awake()
    {
        if (prefab == null)
        {
            Log.Error("PoolManager의 프리팹이 지정되지 않았습니다.", this);
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }*/

    #endregion

    #region Custom Methods

    /// <summary>
    /// 풀에서 오브젝트를 꺼내 활성화합니다. 부족하면 새로 생성합니다.
    /// </summary>
    public GameObject Get()
    {
        GameObject obj;

        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, transform);
            Log.Warn("풀 부족으로 새 오브젝트 생성됨", this);
        }

        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 오브젝트를 비활성화하고 풀에 반환합니다.
    /// </summary>
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    public void Initialize(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }


    #endregion
}
