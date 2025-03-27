using UnityEngine;

public class CaughtFish : MonoBehaviour
{
    #region Variables

    [Tooltip("위로 올라가는 속도입니다 (단위: 유닛/초)")]
    [SerializeField] private float moveSpeed = 0.5f;

    [Tooltip("오브젝트가 사라지기까지의 시간입니다 (초)")]
    [SerializeField] private float destroyTime = 3f;

    #endregion

    #region Unity Methods

    /// <summary>
    /// 프리팹이 위로 천천히 이동하도록 합니다.
    /// </summary>
    private void Update()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 지정된 시간 후 오브젝트를 파괴합니다.
    /// </summary>
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    #endregion
}
