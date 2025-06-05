using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Additive 방식으로 씬을 추가 로드하고, 지정한 Canvas를 비활성화하는 시스템입니다.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    #region Variables

    [Header("비활성화할 캔버스 목록")]
    [SerializeField] private Canvas[] canvasesToDisable;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 필요하면 DontDestroyOnLoad(gameObject); 추가 가능
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 지정한 씬을 Additive로 로드하고, 연결된 Canvas를 끕니다.
    /// </summary>
    public void LoadSceneAdditiveAndDisableCanvas(string sceneName)
    {
        DisableAssignedCanvas();

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Log.System($"씬 추가 로드 + 연결된 Canvas 비활성화: {sceneName}", this);
    }

    /// <summary>
    /// 연결된 Canvas들을 비활성화합니다.
    /// </summary>
    private void DisableAssignedCanvas()
    {
        if (canvasesToDisable == null || canvasesToDisable.Length == 0)
        {
            Log.Warn("비활성화할 Canvas가 설정되지 않았습니다.", this);
            return;
        }

        foreach (Canvas canvas in canvasesToDisable)
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
                Log.Info($"Canvas 비활성화 완료: {canvas.gameObject.name}", this);
            }
        }
    }

    /// <summary>
    /// 지정된 씬을 언로드하고, 비활성화된 Canvas들을 다시 활성화합니다.
    /// </summary>
    public void UnloadSceneAndEnableCanvas(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);

        if (canvasesToDisable == null || canvasesToDisable.Length == 0)
        {
            Log.Warn("다시 활성화할 Canvas가 설정되지 않았습니다.", this);
            return;
        }

        foreach (Canvas canvas in canvasesToDisable)
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
                Log.Info($"Canvas 다시 활성화 완료: {canvas.gameObject.name}", this);
            }
        }

        Log.System($"씬 언로드 및 Canvas 활성화 완료: {sceneName}", this);
    }


    #endregion
}
