using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables

    [Header("패널 리스트 (인스펙터에서 등록)")]
    [Tooltip("홈 화면에서 사용할 패널 목록입니다.")]
    [SerializeField] private List<GameObject> panels = new();

    [Header("공통 배경 (외부 클릭 감지용)")]
    [Tooltip("패널 외부 클릭 시 닫히게 할 공통 배경입니다.")]
    [SerializeField] private Button backgroundButton;

    [Header("패널 닫기 버튼 (인스펙터에서 등록)")]
    [Tooltip("각 패널 내부의 닫기 버튼입니다.")]
    [SerializeField] private List<Button> closeButtons = new();

    private GameObject currentPanel;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // 닫기 버튼 등록
        foreach (Button btn in closeButtons)
        {
            btn.onClick.AddListener(CloseCurrentPanel);
        }

        // 공통 배경 클릭 시 닫기
        if (backgroundButton != null)
        {
            backgroundButton.onClick.AddListener(CloseCurrentPanel);
            backgroundButton.gameObject.SetActive(false);
        }
        else
        {
            Log.Warn("배경 버튼이 설정되지 않았습니다.", this);
        }

        // 모든 패널 기본 비활성화
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 지정한 패널만 열고, 나머지는 닫습니다. 공통 배경도 함께 표시됩니다.
    /// </summary>
    public void OpenPanel(GameObject target)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(panel == target);
        }

        currentPanel = target;

        if (backgroundButton != null)
        {
            backgroundButton.gameObject.SetActive(true);
            backgroundButton.transform.SetAsFirstSibling(); // 항상 가장 아래에 위치
        }
    }

    /// <summary>
    /// 현재 열린 패널과 공통 배경을 닫습니다.
    /// </summary>
    public void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
        }

        if (backgroundButton != null)
        {
            backgroundButton.gameObject.SetActive(false);
        }
    }

    #endregion
}
