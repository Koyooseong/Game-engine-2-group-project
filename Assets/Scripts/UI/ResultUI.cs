using UnityEngine;

/// <summary>
/// 탐사 종료 시 결과 패널을 활성화하고, 입력 시 Title 씬으로 전환합니다.
/// </summary>
public class ResultUI : MonoBehaviour
{
    #region Variables

    [Header("결과 패널 루트 오브젝트")]
    [Tooltip("결과 창을 담고 있는 부모 패널입니다.")]
    [SerializeField] private GameObject resultPanel;

    private bool isResultActive = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isResultActive) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            LoadTitle();
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            LoadTitle();
        }
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 결과 패널을 활성화하고 게임을 정지시킵니다.
    /// </summary>
    public void Show()
    {
        resultPanel.SetActive(true);
        isResultActive = true;
        if (GamePauseManager.Instance == null)
        {
            Log.Error("GamePauseManager가 초기화되지 않았습니다!", this);
            return;
        }

        GamePauseManager.Instance.PauseGame();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Title 씬으로 전환합니다.
    /// </summary>
    private void LoadTitle()
    {
        isResultActive = false;
        SceneLoader.LoadScene("Title");
    }

    #endregion
}
