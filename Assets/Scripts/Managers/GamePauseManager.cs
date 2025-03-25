using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 일시정지 상태를 관리하고, 등록된 대상에게 Pause/Resume 명령을 전파합니다.
/// 싱글톤 방식으로 전역에서 접근할 수 있습니다.
/// </summary>
public class GamePauseManager : MonoBehaviour
{
    #region Singleton

    public static GamePauseManager Instance { get; private set; }

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    #region Variables

    private readonly HashSet<IGamePause> pauseTargets = new();
    private bool isPaused = false;

    /// <summary>
    /// 현재 일시정지 상태를 외부에서 확인합니다.
    /// </summary>
    public bool IsPaused => isPaused;

    #endregion

    #region Public Methods

    /// <summary>
    /// Pause 대상 등록
    /// </summary>
    public void Register(IGamePause target)
    {
        if (!pauseTargets.Contains(target))
        {
            pauseTargets.Add(target);
        }
    }

    /// <summary>
    /// Pause 대상 제거
    /// </summary>
    public void Unregister(IGamePause target)
    {
        if (pauseTargets.Contains(target))
        {
            pauseTargets.Remove(target);
        }
    }

    /// <summary>
    /// 게임을 일시정지합니다.
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;

        var targets = new List<IGamePause>(pauseTargets);

        foreach (var target in targets)
        {
            target.Pause();
        }

        isPaused = true;
    }

    /// <summary>
    /// 게임을 재개합니다.
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;

        var targets = new List<IGamePause>(pauseTargets);

        foreach (var target in targets)
        {
            target.Resume();
        }

        isPaused = false;
    }

    #endregion
}
