using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 수심 상태에 따라 일정 간격으로 물고기를 생성하는 매니저입니다.
/// </summary>
public class FishManager : MonoBehaviour, IInitializable
{
    #region Variables

    [Header("수심 참조")]
    [Tooltip("현재 수심 상태를 제공하는 DepthManager")]
    [SerializeField] private DepthManager depthManager;

    [Header("물고기 풀")]
    [Tooltip("물고기 프리팹을 key 기반으로 관리하는 풀 매니저")]
    [SerializeField] private FishObjectPool fishObjectPool;

    [Header("스폰 설정")]
    [Tooltip("물고기 생성 Y 위치 (고정)")]
    [SerializeField] private float spawnY = 11.36f;

    [Tooltip("물고기 생성 X 범위 (-range ~ +range)")]
    [SerializeField] private float spawnRangeX = 4.5f;

    private const float minInterval = 3f;
    private const float maxInterval = 5f;

    private const int minSpawnCount = 1;
    private const int maxSpawnCount = 2;

    private readonly Dictionary<DepthState, Dictionary<string, float>> rarityChances = new();
    private readonly List<string> tempFilteredFish = new();

    #endregion

    #region Custom Methods

    /// <summary>
    /// GameStartController에 의해 호출되며, 물고기 생성 루틴을 시작합니다.
    /// </summary>
    public void Init()
    {
        SetupRarityTable();
        StartCoroutine(FishSpawnRoutine());
        Log.System("[Fish] 물고기 생성 루틴 시작됨", this);
    }

    /// <summary>
    /// 수심 상태별 등급 확률 테이블을 초기화합니다.
    /// </summary>
    private void SetupRarityTable()
    {
        rarityChances[DepthState.Shallow] = new()
        {
            { "normal", 0.9f },
            { "rare", 0.1f }
        };

        rarityChances[DepthState.Mid] = new()
        {
            { "normal", 0.6f },
            { "rare", 0.35f },
            { "unique", 0.05f }
        };

        rarityChances[DepthState.Deep] = new()
        {
            { "normal", 0.3f },
            { "rare", 0.5f },
            { "unique", 0.2f }
        };
    }

    /// <summary>
    /// 일정 시간마다 1~2마리의 물고기를 생성하는 루틴입니다.
    /// </summary>
    private IEnumerator FishSpawnRoutine()
    {
        while (true)
        {
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                float delay = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(delay);
                SpawnFish();
            }
        }
    }

    /// <summary>
    /// 물고기 1마리를 생성합니다.
    /// </summary>
    private void SpawnFish()
    {
        DepthState currentDepth = depthManager.CurrentState;
        string rarity = GetRarityByChance(currentDepth);
        string fishId = GetRandomFishIdByRarity(rarity);

        if (string.IsNullOrEmpty(fishId)) return;

        GameObject fish = fishObjectPool.Get(fishId);
        if (fish == null) return;

        fish.transform.position = new Vector2(
            Random.Range(-spawnRangeX, spawnRangeX),
            spawnY
        );

        fish.transform.rotation = Quaternion.Euler(0, 0, Random.value < 0.5f ? 90f : -90f);

        Log.Info($"[Fish] '{fishId}' ({rarity}) 생성됨", fish);
    }

    /// <summary>
    /// 현재 수심 상태에 따른 확률로 물고기 등급을 결정합니다.
    /// </summary>
    private string GetRarityByChance(DepthState state)
    {
        float rand = Random.value;
        float sum = 0f;

        foreach (var pair in rarityChances[state])
        {
            sum += pair.Value;
            if (rand <= sum)
                return pair.Key;
        }

        return "normal"; // fallback
    }

    /// <summary>
    /// 등급에 해당하는 물고기 중 무작위 하나의 ID를 반환합니다.
    /// </summary>
    private string GetRandomFishIdByRarity(string rarity)
    {
        tempFilteredFish.Clear();

        foreach (var pair in FishDataManager.Instance.GetAllFish())
        {
            if (pair.Value.rare == rarity)
                tempFilteredFish.Add(pair.Key);
        }

        if (tempFilteredFish.Count == 0) return null;

        int index = Random.Range(0, tempFilteredFish.Count);
        return tempFilteredFish[index];
    }

    #endregion
}
