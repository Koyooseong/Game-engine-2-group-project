using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시간 경과에 따라 수심을 증가시키고, 현재 수심 상태를 계산하는 매니저입니다.
/// </summary>
public class DepthManager : MonoBehaviour, IInitializable
{
    #region Variables

    [Header("수심 텍스트")]
    [Tooltip("현재 수심을 표시할 텍스트 컴포넌트")]
    [SerializeField] private TMPro.TextMeshProUGUI depthText;

    private float currentDepth = 0f;
    private float depthSpeed = 3f;
    private DepthState currentState = DepthState.Shallow;

    public DepthState CurrentState => currentState;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (PauseSystem.Instance?.IsPaused() == true) return;

        currentDepth += depthSpeed * Time.deltaTime;

        UpdateDepthText();
        UpdateDepthState();
    }

    private void OnDestroy()
    {
        PauseSystem.Instance?.Unregister(this);
    }

    #endregion

    #region Custom Methods

    public void Init()
    {
        currentDepth = 0f;
        currentState = DepthState.Shallow;
        PauseSystem.Instance?.Register(this);
        Log.System("[Depth] 수심 측정 시작", this);
    }

    private void UpdateDepthText()
    {
        if (depthText != null)
        {
            int rounded = Mathf.FloorToInt(currentDepth);
            depthText.text = $"{rounded}m";
        }
    }

    private void UpdateDepthState()
    {
        DepthState newState = GetStateByDepth(currentDepth);

        if (newState != currentState)
        {
            currentState = newState;
            Log.Info($"[Depth] 수심 상태 변경 → {currentState}", this);
        }
    }

    private DepthState GetStateByDepth(float depth)
    {
        if (depth < 90f) return DepthState.Shallow;
        if (depth < 180f) return DepthState.Mid;
        return DepthState.Deep;
    }

    public float GetCurrentDepth()
    {
        return currentDepth;
    }

    #endregion
}
