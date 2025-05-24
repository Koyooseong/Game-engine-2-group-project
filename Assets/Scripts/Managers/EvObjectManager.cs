using UnityEngine;

/// <summary>
/// ExploreScene에서 장애물 및 보물상자를 일정 시간마다 생성하는 시스템입니다.
/// </summary>
public class EvObjectManager : MonoBehaviour
{
    #region Variables

    [Header("풀")]
    [Tooltip("장애물 오브젝트 풀")]
    [SerializeField] private ObjectPool obstaclePool;

    [Tooltip("보물상자 오브젝트 풀")]
    [SerializeField] private ObjectPool treasurePool;

    [Header("위치 관련")]
    [Tooltip("Y축 고정 생성 위치")]
    [SerializeField] private float spawnY = 11.36f;

    [Tooltip("X축 생성 범위 (-x ~ +x)")]
    [SerializeField] private float xRange = 4.5f;

    private float nextSpawnTime;

    #endregion

    #region Unity Methods

    private void Start()
    {
        ScheduleNextSpawn(Random.Range(2f, 5f));
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEvObject();
            ScheduleNextSpawn(Random.Range(2f, 5f));
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 다음 생성 시간을 설정합니다.
    /// </summary>
    private void ScheduleNextSpawn(float delay)
    {
        nextSpawnTime = Time.time + delay;
    }

    /// <summary>
    /// 장애물 또는 보물상자를 생성합니다.
    /// </summary>
    private void SpawnEvObject()
    {
        float x = Random.Range(-xRange, xRange);
        Vector2 spawnPos = new Vector2(x, spawnY);

        bool spawnTreasure = Random.value < 0.05f;

        GameObject obj = spawnTreasure
            ? treasurePool.Get()
            : obstaclePool.Get();

        obj.transform.position = spawnPos;
    }

    #endregion
}
