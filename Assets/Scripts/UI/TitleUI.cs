using UnityEngine;
using UnityEngine.UI;
using Game;

public class TitleUI : MonoBehaviour
{
    #region Variables

    [Header("시작 버튼")]
    [Tooltip("게임을 시작하는 버튼입니다.")]
    [SerializeField] private Button startButton;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (startButton == null)
        {
            Log.Error("TitleUI에 StartButton이 지정되지 않았습니다.", this);
            return;
        }

        startButton.onClick.AddListener(OnClickStart);
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 게임 시작 버튼 클릭 시 GameScene으로 전환합니다.
    /// </summary>
    private void OnClickStart()
    {
        SceneLoader.LoadScene(GameConstants.GAME_SCENE_NAME);
    }

    #endregion
}
