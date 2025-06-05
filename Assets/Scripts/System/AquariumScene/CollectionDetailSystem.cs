using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 선택된 물고기의 상세 정보를 표시하고, 보상 버튼 상태를 관리하는 시스템입니다.
/// </summary>
public class CollectionDetailSystem : MonoBehaviour
{
    #region Variables

    [Header("텍스트 출력")]
    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI explain1Text;
    [SerializeField] private TextMeshProUGUI explain2Text;
    [SerializeField] private TextMeshProUGUI explain3Text;

    [Header("이미지 출력")]
    [SerializeField] private Image moveSpriteImage;
    [SerializeField] private Image puzzleImage;

    [Header("보상 버튼")]
    [Tooltip("선물 1~3 버튼 (index 0~2 순)")]
    [SerializeField] private Button[] rewardButtons;

    private string currentFishKey;
    private PresentManager presentManager;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        presentManager = GetComponent<PresentManager>();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 상세 정보 갱신을 위한 키를 설정합니다.
    /// </summary>
    public void SetFishKey(string key)
    {
        currentFishKey = key;
        presentManager.SetFishKey(key);
        UpdateDetail();
        UpdateRewardButtonStates();
    }

    /// <summary>
    /// 텍스트 및 이미지 데이터를 업데이트합니다.
    /// </summary>
    private void UpdateDetail()
    {
        if (string.IsNullOrEmpty(currentFishKey)) return;

        var data = FishDataManager.Instance.GetFishData(currentFishKey);
        int count = InventoryManager.Instance.GetCount(currentFishKey);

        fishNameText.text = data.name;
        countText.text = $"x{count}";

        explain1Text.text = count >= 1 ? data.explain1 : "?";
        explain2Text.text = count >= 3 ? data.explain2 : "?";
        explain3Text.text = count >= 5 ? data.explain3 : "?";

        Sprite moveSprite = Resources.Load<Sprite>(data.fishSprite);
        Sprite puzzleSprite = Resources.Load<Sprite>(data.puzzleImg);

        moveSpriteImage.sprite = moveSprite;
        puzzleImage.sprite = puzzleSprite;
    }

    /// <summary>
    /// 보상 버튼의 인터랙션 상태 및 UI 상태를 현재 수량에 따라 초기화합니다.
    /// </summary>
    private void UpdateRewardButtonStates()
    {
        for (int i = 0; i < rewardButtons.Length; i++)
        {
            Button button = rewardButtons[i];
            bool canClaim = presentManager.CanClaim(currentFishKey, i);
            bool alreadyClaimed = presentManager.GetClaimedStatus(currentFishKey)[i];

            // 버튼 인터랙션 상태
            button.interactable = canClaim;

            // 자식 처리
            var children = button.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == button.transform) continue;

                if (child.TryGetComponent(out CanvasGroup group))
                {
                    group.alpha = (canClaim && !alreadyClaimed) ? 1f : 0.5f;
                }
                else
                {
                    child.gameObject.SetActive(canClaim && !alreadyClaimed);
                }
            }
        }
    }

    /// <summary>
    /// 보상 버튼 클릭 시 호출됩니다. 현재 버튼을 자동으로 참조합니다.
    /// </summary>
    public void OnClickReward(int index)
    {
        Button button = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        if (button == null)
        {
            Log.Warn("Reward 버튼 클릭 시 Button 참조를 찾을 수 없습니다.", this);
            return;
        }

        presentManager.ClaimReward(index, button);
        UpdateRewardButtonStates(); // 수령 후 UI 갱신
    }

    #endregion
}
