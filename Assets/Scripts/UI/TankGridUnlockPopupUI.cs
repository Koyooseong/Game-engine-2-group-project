using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 셀 해금 팝업 UI 처리
/// </summary>
public class TankGridUnlockPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Vector2Int currentTarget;
    private TankGridLockSystem lockSystem;

    public void Initialize(TankGridLockSystem system)
    {
        lockSystem = system;
        gameObject.SetActive(false);

        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);
    }

    public void Show(Vector2Int targetPos, int cost)
    {
        currentTarget = targetPos;
        messageText.text = $"해당 셀은 잠겨있습니다.\n{cost}골드를 내고 해금하시겠습니까?";
        gameObject.SetActive(true);
    }

    public void OnYes()
    {
        lockSystem.ConfirmUnlock(currentTarget);
        gameObject.SetActive(false);
    }

    public void OnNo()
    {
        gameObject.SetActive(false);
    }
}
