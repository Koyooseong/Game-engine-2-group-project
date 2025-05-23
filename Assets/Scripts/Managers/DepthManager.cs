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
    private float depthSpeed = 3f; // 1초에 1m
    private DepthState currentState = DepthState.Shallow;

    /// <summary>
    /// 현재 수심 상태를 외부에서 읽을 수 있는 프로퍼티입니다.
    /// </summary>
    public DepthState CurrentState => currentState;

    #endregion

    #region Unity Methods

    private void Update()
    {
        currentDepth += depthSpeed * Time.deltaTime;

        UpdateDepthText();
        UpdateDepthState();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 게임 시작 시 호출되는 초기화 함수입니다.
    /// </summary>
    public void Init()
    {
        currentDepth = 0f;
        currentState = DepthState.Shallow;
        Log.System("[Depth] 수심 측정 시작", this);
    }

    /// <summary>
    /// 수심 숫자를 m 단위로 표시합니다.
    /// </summary>
    private void UpdateDepthText()
    {
        if (depthText != null)
        {
            int rounded = Mathf.FloorToInt(currentDepth);
            depthText.text = $"{rounded}m";
        }
    }

    /// <summary>
    /// 수심 구간에 따라 현재 수심 상태를 계산합니다.
    /// </summary>
    private void UpdateDepthState()
    {
        DepthState newState = GetStateByDepth(currentDepth);

        if (newState != currentState)
        {
            currentState = newState;
            Log.Info($"[Depth] 수심 상태 변경 → {currentState}", this);
            // 상태 변화 이벤트가 필요하면 여기서 Broadcast 가능
        }
    }

    /// <summary>
    /// 수심 값에 따라 상태를 반환합니다.
    /// </summary>
    private DepthState GetStateByDepth(float depth)
    {
        if (depth < 90f) return DepthState.Shallow;
        if (depth < 180f) return DepthState.Mid;
        return DepthState.Deep;
    }

    #endregion
}
