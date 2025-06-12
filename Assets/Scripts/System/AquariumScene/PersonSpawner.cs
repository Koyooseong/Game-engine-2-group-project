using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아쿠아리움 씬에서 사람을 주기적으로 생성하는 스폰 시스템입니다.
/// </summary>
public class PersonSpawner : MonoBehaviour
{
    #region Variables

    [Header("생성 위치")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [Header("사람 프리팹 (4종류)")]
    [SerializeField] private GameObject[] personPrefabs;

    private Dictionary<GameObject, ObjectPool> poolDictionary = new();
    private Coroutine spawnRoutine;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // 오브젝트 풀 초기화는 최초 1회만
        foreach (var prefab in personPrefabs)
        {
            GameObject poolObject = new GameObject($"{prefab.name}_Pool");
            poolObject.transform.parent = this.transform;

            ObjectPool pool = poolObject.AddComponent<ObjectPool>();
            pool.SetPrefab(prefab);

            poolDictionary[prefab] = pool;
        }

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnEnable()
    {
        // 코루틴이 이미 돌고 있지 않으면 시작
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    private void OnDisable()
    {
        // 꺼질 때 코루틴 정지
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 일정 시간마다 수익 기반 인원 수만큼 사람을 생성합니다.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int income = GoldUIManager.Instance != null ? GoldUIManager.Instance.GetIncomePer5Sec() : 0;
            int personCount = Mathf.FloorToInt(income / 5f) + 1;

            for (int i = 0; i < personCount; i++)
            {
                SpawnPerson();
            }

            float waitTime = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 한 명의 사람을 생성하고 이동 설정을 초기화합니다.
    /// </summary>
    private void SpawnPerson()
    {
        bool fromPoint1 = Random.value < 0.5f;
        Transform spawnPoint = fromPoint1 ? spawnPoint1 : spawnPoint2;

        GameObject prefab = personPrefabs[Random.Range(0, personPrefabs.Length)];
        GameObject person = poolDictionary[prefab].Get();

        person.transform.position = spawnPoint.position;
        person.transform.rotation = fromPoint1 ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);

        float moveSpeed = fromPoint1 ? -125f : 125f;
        person.GetComponent<PersonMover>().Initialize(moveSpeed, poolDictionary[prefab]);
    }

    #endregion
}
