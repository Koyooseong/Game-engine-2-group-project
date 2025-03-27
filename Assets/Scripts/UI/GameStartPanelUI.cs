using UnityEngine;
using UnityEngine.UI;
using Game;

public class GameStartPanelUI : MonoBehaviour
{
    #region Variables

    [Header("게임 시작 버튼")]
    [Tooltip("게임 시작 시 씬을 전환하는 버튼입니다.")]
    [SerializeField] private Button startGameButton;

    #endregion

    #region Unity Methods

    private void Start()
    {
        startGameButton.onClick.AddListener(OnClickStartGame);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 게임 씬으로 이동합니다.
    /// </summary>
    private void OnClickStartGame()
    {
        SceneLoader.LoadScene(GameConstants.GAME_SCENE_NAME);
    }

    #endregion
}
