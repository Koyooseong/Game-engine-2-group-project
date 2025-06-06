using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 잠긴 셀 해금 팝업 UI
/// </summary>
public class TankGridUnlockPopupUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private TankGridController targetCell;

    public event Action<TankGridController> OnConfirm;
    public event Action OnCancel;

    private void Awake()
    {
        yesButton.onClick.AddListener(Confirm);
        noButton.onClick.AddListener(Cancel);
        gameObject.SetActive(false);
    }

    public void Show(TankGridController cell, int cost)
    {
        targetCell = cell;
        messageText.text = $"이 칸은 잠겨있습니다.\n{cost}G를 사용해 해금하시겠습니까?";
        gameObject.SetActive(true);
    }

    private void Confirm()
    {
        OnConfirm?.Invoke(targetCell);
        gameObject.SetActive(false);
    }

    private void Cancel()
    {
        OnCancel?.Invoke();
        gameObject.SetActive(false);
    }
}
