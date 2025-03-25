using UnityEngine;

/// <summary>
/// 게임 정지 대상이 될 오브젝트는 이 클래스를 상속하여 Pause 처리를 자동으로 등록할 수 있습니다.
/// GamePauseManager에 자동 등록/해제되며 Pause/Resume 기능을 구현해야 합니다.
/// </summary>
public abstract class AutoPauseBehaviour : MonoBehaviour, IGamePause
{
    /// <summary>
    /// 오브젝트가 활성화될 때 GamePauseManager에 자동 등록합니다.
    /// </summary>
    protected virtual void Start()
    {
        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Register(this);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning($"[AutoPauseBehaviour] GamePauseManager가 초기화되지 않았습니다. ({gameObject.name})", this);
        }
#endif
    }

    /// <summary>
    /// 오브젝트가 비활성화되거나 파괴될 때 자동으로 등록 해제합니다.
    /// </summary>
    protected virtual void OnDisable()
    {
        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Unregister(this);
        }
    }

    /// <summary>
    /// 일시정지 시 호출됩니다. 상속받는 쪽에서 구현하세요.
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// 재개 시 호출됩니다. 상속받는 쪽에서 구현하세요.
    /// </summary>
    public abstract void Resume();
}
