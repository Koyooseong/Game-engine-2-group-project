using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 탐사 종료 시 결과창을 띄우고, 획득한 골드 및 물고기를 UI에 표시하며,
/// 인벤토리 반영 및 새 물고기 표시 기능을 제공합니다.
/// </summary>
public class ResultSystem : MonoBehaviour
{
    #region Variables

    [Header("결과창 UI")]
    [Tooltip("결과 패널 오브젝트")]
    [SerializeField] private GameObject resultPanel;

    [Header("획득 골드 텍스트")]
    [SerializeField] private TMP_Text goldText;

    [Header("물고기 스크롤 콘텐츠 영역")]
    [SerializeField] private Transform contentParent;

    [Header("물고기 프리팹")]
    [Tooltip("잡은 물고기 표시용 프리팹 (Image + NewText 포함)")]
    [SerializeField] private GameObject fishItemPrefab;

    [Header("새 물고기 표시용 오브젝트 이름")]
    [Tooltip("프리팹 안에서 '새로운 물고기' 텍스트 오브젝트 이름")]
    [SerializeField] private string newTextObjectName = "NewText";

    [Header("획득 수심 텍스트")]
    [SerializeField] private TMP_Text depthText;

    [SerializeField] private DepthManager depthManager;

    #endregion

    #region Public Methods

    /// <summary>
    /// 탐사 종료 시 결과창을 표시합니다.
    /// </summary>
    public void ShowResult()
    {
        // 1. Pause는 이미 FuelManager에서 처리됨

        // 2. UI 활성화
        resultPanel.SetActive(true);

        // 3. 골드 텍스트 설정
        int earnedGold = ExploreSceneInventory.Instance.GetTempGold();
        goldText.text = $"{earnedGold} G";

        // 4. 기존 UI 항목 정리
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 5. 기존 인벤토리 목록 확보
        var ownedKeys = InventoryManager.Instance.InventoryRead();
        List<string> newFishKeys = ExploreSceneInventory.Instance.GetTempFishKeys();

        // 6. 결과 리스트 채우기
        foreach (string key in newFishKeys)
        {
            FishData data = FishDataManager.Instance.GetFishData(key);
            GameObject item = Instantiate(fishItemPrefab, contentParent);

            // 이미지 설정
            Image image = item.GetComponentInChildren<Image>();
            Sprite sprite = Resources.Load<Sprite>(data.fishSprite);
            if (image != null && sprite != null)
            {
                image.sprite = sprite;
                image.SetNativeSize();
            }
            // 새로운 물고기일 경우 텍스트 오브젝트 활성화
            if (!ownedKeys.ContainsKey(key))
            {
                foreach (Transform t in item.transform)
                {
                    if (t.name == newTextObjectName)
                    {
                        t.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }

        // 7. 수심 바꾸기
        int roundedDepth = Mathf.FloorToInt(depthManager.GetCurrentDepth());
        depthText.text = $"{roundedDepth} M";
    }

    /// <summary>
    /// 결과 수집을 InventoryManager에 반영하고 인벤토리를 정리합니다.
    /// </summary>
    public void CommitAndExit()
    {
        // 1. 인벤토리에 결과 반영
        ExploreSceneInventory.Instance.CommitInventory();
        Log.System("[ResultSystem] 결과 반영 완료", this);

        // 3. 씬 닫고 Canvas 활성화
        SceneLoader.Instance.UnloadSceneAndEnableCanvas("ExploreScene");
    }

    #endregion
}
