using UnityEngine;
using UnityEngine.UI;
using Game;

public class TitleUI : MonoBehaviour
{
    #region Variables

    [Header("탐사 시작 버튼")]
    [Tooltip("탐사를 시작하는 버튼입니다.")]
    [SerializeField] private Button startButton;


    [Header("탐사 시작 확인 팝업")]
    [Tooltip("탐사 시작 버튼을 눌렀을 때 나오는 팝업창입니다.")]
    [SerializeField] private GameObject gameStart_YesNo;

    [Header("탐사 시작 확인 팝업 확인 버튼")]
    [Tooltip("누르면 씬 이동합니다")]
    [SerializeField] private Button yesButton;
    
    [Header("탐사 시작 확인 팝업 취소 버튼")]
    [Tooltip("누르면 창이 꺼집니다")]
    [SerializeField] private Button noButton;


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

        // 팝업 비활성화로 시작
        if (gameStart_YesNo != null)
            gameStart_YesNo.SetActive(false);

        // 버튼 이벤트 연결
        if (yesButton != null)
            yesButton.onClick.AddListener(OnClickYes);
        Log.Info("탐험 시작 버튼 클릭");

        if (noButton != null)
            noButton.onClick.AddListener(OnClickNo);
        Log.Info("돌아가기 버튼 클릭");
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 게임 시작 버튼 클릭 시 결정 팝업창이 띄워지고 GameScene으로 전환합니다.
    /// </summary>
    private void OnClickStart()
    {
        if (gameStart_YesNo != null)
            gameStart_YesNo.SetActive(true);
    }

    private void OnClickYes()
    {
        SceneLoader.LoadScene(GameConstants.GAME_SCENE_NAME);
    }

    private void OnClickNo()
    {
        if (gameStart_YesNo != null)
            gameStart_YesNo.SetActive(false);
    }

    #endregion
}
