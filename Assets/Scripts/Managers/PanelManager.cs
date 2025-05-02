using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아쿠아리움 씬에서 패널을 통합 관리하는 매니저입니다.
/// </summary>
public class PanelManager : MonoBehaviour
{
    #region Variables

    [Header("패널 매핑")]
    [SerializeField] private List<PanelMapping> panelMappings = new List<PanelMapping>();
    private Dictionary<PanelKey, GameObject> panelDictionary = new Dictionary<PanelKey, GameObject>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        InitializePanels();
    }

    #endregion

    #region Custom Methods

    private void InitializePanels()
    {
        foreach (var mapping in panelMappings)
        {
            if (!panelDictionary.ContainsKey(mapping.key))
            {
                panelDictionary.Add(mapping.key, mapping.panel);
            }
        }
    }

    /// <summary>
    /// 해당 키에 해당하는 패널을 활성화합니다.
    /// </summary>
    public void ShowPanel(PanelKey key)
    {
        if (panelDictionary.TryGetValue(key, out var panel))
        {
            CloseAllPanels();
            panel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"PanelManager: {key}에 해당하는 패널을 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 모든 패널을 비활성화합니다.
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panel in panelDictionary.Values)
        {
            panel.SetActive(false);
        }
    }
    /// <summary>
    /// Explore키를 가진 Panel을 활성화합니다
    /// </summary>
    public void ShowExplore()
    {
        ShowPanel(PanelKey.Explore);
    }

    /// <summary>
    /// Upgrade키를 가진 Panel을 활성화합니다
    /// </summary>
    public void ShowUpgrade()
    {
        ShowPanel(PanelKey.Upgrade);
    }

    /// <summary>
    /// Setting키를 가진 Panel을 활성화합니다
    /// </summary>
    public void ShowSetting()
    {
        ShowPanel(PanelKey.Setting);
    }

    /// <summary>
    /// Collection키를 가진 Panel을 활성화합니다
    /// </summary>
    public void ShowCollection()
    {
        ShowPanel(PanelKey.Collection);
    }

    /// <summary>
    /// Fish_Detail키를 가진 Panel을 활성화합니다
    /// </summary>
    public void ShowFishDetail()
    {
        ShowPanel(PanelKey.Fish_Detail);
    }

    /// <summary>
    /// Fish_Detail에서 뒤로가기 해서 다시 Collection창으로 돌아가기
    /// </summary>
    public void DetailBack()
    {
        ShowPanel(PanelKey.Collection);
    }


    #endregion
}

[System.Serializable]
public struct PanelMapping
{
    public PanelKey key;
    public GameObject panel;
}

public enum PanelKey
{
    Explore,
    Upgrade,
    Setting,
    Collection,
    Fish_Detail,
}
