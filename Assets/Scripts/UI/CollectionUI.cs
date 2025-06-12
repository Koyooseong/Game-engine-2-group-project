using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 도감 UI를 구성하고, 물고기 획득 여부와 수집률 및 선물 알림(red dot)을 관리하는 UI입니다.
/// </summary>
public class CollectionUI : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    private struct FishUI
    {
        public string fishKey;
        public Image fishImage;
        public TextMeshProUGUI nameText;
        public GameObject parentButton;
        public GameObject redDot;
    }

    [Header("도감 UI 리스트")]
    [Tooltip("도감에 등록된 물고기 UI 리스트")]
    [SerializeField] private List<FishUI> fishUIList;

    [Header("수집률 표시")]
    [Tooltip("Collection Rate를 표시할 텍스트")]
    [SerializeField] private TextMeshProUGUI collectionRateText;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        UpdateCollection();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 도감 UI를 인벤토리 데이터 기준으로 갱신합니다.
    /// </summary>
    private void UpdateCollection()
    {
        int caughtCount = 0;
        int totalCount = fishUIList.Count;

        foreach (var fishUI in fishUIList)
        {
            bool isCaught = InventoryManager.Instance.GetCount(fishUI.fishKey) > 0;

            if (isCaught)
            {
                caughtCount++;

                string spritePath = FishDataManager.Instance.GetMoveSprite(fishUI.fishKey);
                Sprite fishSprite = Resources.Load<Sprite>(spritePath);
                fishUI.fishImage.sprite = fishSprite;
                fishUI.fishImage.gameObject.SetActive(true);

                fishUI.nameText.text = FishDataManager.Instance.GetName(fishUI.fishKey);
                fishUI.parentButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                fishUI.fishImage.gameObject.SetActive(false);
                fishUI.nameText.text = "?";
                fishUI.parentButton.GetComponent<Button>().interactable = false;
            }

            // red dot 초기값은 모두 꺼둔다 (PresentManager가 필요한 것만 다시 켤 것)
            //fishUI.redDot.SetActive(false);
        }

        int rate = Mathf.FloorToInt((caughtCount / (float)totalCount) * 100f);
        collectionRateText.text = $"Collection Rate {rate} %";
    }

    /// <summary>
    /// 지정된 물고기에 대해 red dot를 표시합니다.
    /// </summary>
    /// <param name="fishKey">red dot를 표시할 물고기 키</param>
    public void ShowRedDot(string fishKey)
    {
        foreach (var fishUI in fishUIList)
        {
            if (fishUI.fishKey == fishKey)
            {
                fishUI.redDot.SetActive(true);
                break;
            }
        }
    }

    public void HideRedDot(string fishKey)
    {
        foreach (var fishUI in fishUIList)
        {
            if (fishUI.fishKey == fishKey)
            {
                fishUI.redDot.SetActive(false);
                break;
            }
        }
    }

    #endregion
}
