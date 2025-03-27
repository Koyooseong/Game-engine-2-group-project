using UnityEngine;
using TMPro;
using Game;

/// <summary>
/// 잠수함의 깊이를 시간 기준과 Y축 이동을 통해 계산하여 UI에 표시하는 컴포넌트입니다.
/// 일시정지 시스템과 연동하게 만들었습니다.
/// </summary>
public class DepthMarker : AutoPauseBehaviour
{

    #region Variables

    [Header("잠수함 참조")]
    [Tooltip("잠수함(플레이어)의 Transform")]
    [SerializeField] private Transform submarine;

    [Header("깊이 표시용 텍스트")]
    [Tooltip("UI에 표시될 TextMeshProUGUI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI depthText;

    [Header("기본 진행 속도 (배경 스크롤 기반)")]
    [Tooltip("스크롤 속도를 m/s로 환산하여 표시할 기본 속도입니다. 디폴트값: 배경 스크롤 값이랑 동일하게 설정해둠")]
    [SerializeField] private float baseSpeed = DEFAULT_BASE_SPEED;

    private float totalDepth = 0f;
    private float prevY = 0f;

    private bool isTracking = true;

    //게임 오버 시 최종 거리 값
    public int FinalDepth { get; private set; } = 0;

    //상수 정의
    private const string DEPTH_LABEL = "Depth";
    private const string UNIT = "m";
    private const float DEFAULT_BASE_SPEED = 3f;


    #endregion
    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        if (submarine != null)
            prevY = submarine.position.y;
    }

    private void Update()
    {
        if (!isTracking || submarine == null || depthText == null) return;

        // 기본 진행 속도
        totalDepth += baseSpeed * Time.deltaTime;

        // 잠수함이 위로 이동한 경우만 추가 계산
        float deltaY = submarine.position.y - prevY;
        if (deltaY > 0f)
        {

            Log.Info("잠수함 이동 기록중");
            totalDepth += deltaY;
        }

        prevY = submarine.position.y;

        // 소수점 이하는 표시X
        depthText.text = $"{DEPTH_LABEL}: {Mathf.FloorToInt(totalDepth)}{UNIT}";
    }

    #endregion

    #region Custom Methods

    // 연료 소진 시 이동 결과 값 확정 -> FinalDepth에 저장
    public void StopTracking()
    {
        isTracking = false;
        FinalDepth = Mathf.FloorToInt(totalDepth);

        //GameResultManager에 기록
        GameResultManager.FinalDepth = FinalDepth;
        Log.System("최종 이동값: "+FinalDepth);
    }

    // 일시정지 기능 넣으면 사용될 함수들, 일시정지와 이어하기 => 후에 연결
    public override void Pause()
    {
        isTracking = false;
    }

    public override void Resume()
    {
        isTracking = true;
    }

    #endregion
}
