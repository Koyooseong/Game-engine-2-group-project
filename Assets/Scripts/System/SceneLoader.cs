using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    /// <summary>
    /// 씬 이름으로 씬을 전환합니다.
    /// </summary>
    /// <param name="sceneName">이동할 씬 이름</param>
    public static void LoadScene(string sceneName)
    {
#if UNITY_EDITOR
        Log.System($"[SceneLoader] LoadScene: {sceneName}");
#endif
        SceneManager.LoadScene(sceneName);
    }
}