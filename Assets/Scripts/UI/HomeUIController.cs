using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIButtonPanelPair
{
    [Tooltip("버튼 오브젝트")]
    public Button button;

    [Tooltip("이 버튼이 열어야 할 패널 오브젝트")]
    public GameObject panel;
}

public class HomeUIController : MonoBehaviour
{
    #region Variables

    [Header("UI 매니저")]
    [Tooltip("패널 열기/닫기를 담당하는 UIManager입니다.")]
    [SerializeField] private UIManager uiManager;

    [Header("버튼-패널 매핑 리스트")]
    [Tooltip("버튼과 해당 버튼이 열어야 할 패널을 짝지어 등록합니다.")]
    [SerializeField] private List<UIButtonPanelPair> buttonPanelPairs = new();

    #endregion

    #region Unity Methods

    private void Start()
    {
        foreach (UIButtonPanelPair pair in buttonPanelPairs)
        {
            if (pair.button == null || pair.panel == null)
            {
                Log.Warn("Button 또는 Panel이 설정되지 않은 Pair가 있습니다.", this);
                continue;
            }

            pair.button.onClick.AddListener(() => uiManager.OpenPanel(pair.panel));
        }
    }

    #endregion
}
