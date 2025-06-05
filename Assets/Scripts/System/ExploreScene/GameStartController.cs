using UnityEngine;

/// <summary>
/// 씬 진입 시 명시적으로 등록된 시스템들을 순서대로 초기화합니다.
/// </summary>
public class GameStartController : MonoBehaviour
{
    #region Variables

    [Header("초기화 대상 시스템 목록")]
    [Tooltip("IInitializable을 구현한 시스템들을 순서대로 등록합니다.")]
    [SerializeField] private MonoBehaviour[] initializables;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeSceneSystems();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 등록된 시스템 중 IInitializable을 구현한 대상만 초기화합니다.
    /// </summary>
    private void InitializeSceneSystems()
    {
        foreach (var obj in initializables)
        {
            if (obj is IInitializable initSystem)
            {
                initSystem.Init();
                Log.System($"[Init] {initSystem.GetType().Name} 초기화 완료", obj);
            }
            else
            {
                Log.Warn($"[Init] {obj.name}은 IInitializable을 구현하지 않았습니다.", obj);
            }
        }
    }

    #endregion
}
