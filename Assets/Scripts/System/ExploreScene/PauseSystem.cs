using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 일시정지를 관리하며, 등록된 컴포넌트를 일괄 활성/비활성화합니다.
/// </summary>
public class PauseSystem : MonoBehaviour, IInitializable
{
    public static PauseSystem Instance { get; private set; }

    #region Variables

    private readonly List<MonoBehaviour> pauseTargets = new();
    private bool isPaused = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 시스템 초기화 시 호출됩니다.
    /// </summary>
    public void Init()
    {
        isPaused = false;
        Log.System("[PauseSystem] 초기화 완료", this);
    }

    /// <summary>
    /// 일시정지 대상 스크립트를 등록합니다.
    /// </summary>
    public void Register(MonoBehaviour script)
    {
        if (!pauseTargets.Contains(script))
        {
            pauseTargets.Add(script);
        }
    }

    /// <summary>
    /// 등록된 스크립트를 일시정지합니다.
    /// </summary>
    public void Pause()
    {
        if (isPaused) return;

        foreach (var script in pauseTargets)
        {
            script.enabled = false;
        }

        isPaused = true;
        Log.System("[PauseSystem] 게임 일시정지됨", this);
    }

    /// <summary>
    /// 등록된 스크립트를 재개합니다.
    /// </summary>
    public void Resume()
    {
        if (!isPaused) return;

        foreach (var script in pauseTargets)
        {
            script.enabled = true;
        }

        isPaused = false;
        Log.System("[PauseSystem] 게임 재개됨", this);
    }

    /// <summary>
    /// 현재 일시정지 상태인지 반환합니다.
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }

    /// <summary>
    /// 일시정지 대상 스크립트 등록을 해제합니다.
    /// </summary>
    public void Unregister(MonoBehaviour script)
    {
        if (pauseTargets.Contains(script))
        {
            pauseTargets.Remove(script);
        }
    }

    #endregion
}
