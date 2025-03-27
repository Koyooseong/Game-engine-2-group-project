using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일정 시간마다 물고기를 랜덤하게 생성하는 스폰 관리자입니다.
/// </summary>
public class FishSpawner : MonoBehaviour
{
    [Header("물고기 데이터 목록")]
    [SerializeField] private List<FishData> fishDataList;

    [Header("스폰 위치")]
    [SerializeField] private Transform spawnPoint;

    [Header("풀 매니저")]
    [SerializeField] private PoolManager fishPool;

    [Header("스폰 주기 (초)")]
    [SerializeField] private float spawnInterval = 10f;

    private void Start()
    {
        if (FishPoolManager.Instance == null)
        {
            Log.Error("FishPoolManager 인스턴스가 존재하지 않습니다.", this);
            return;
        }

        if (spawnPoint == null)
        {
            Log.Error("FishSpawner에 spawnPoint가 지정되지 않았습니다.", this);
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnFish();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFish()
    {
        if (fishDataList.Count == 0) return;

        FishData selected = fishDataList[Random.Range(0, fishDataList.Count)];
        GameObject fishGO = FishPoolManager.Instance.GetFish(selected);
        fishGO.transform.position = spawnPoint.position;

        Fish fish = fishGO.GetComponent<Fish>();
        if (fish != null)
        {
            var pool = FishPoolManager.Instance.GetPool(selected.fishType);
            fish.Initialize(selected, pool);
        }
    }
}
