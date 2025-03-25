using Game;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 오른쪽 조이스틱 입력을 받아 그물을 발사하고,
/// 쿨타임 동안 조이스틱을 비활성화하는 기능을 담당합니다.
/// </summary>
public class NetLauncher : MonoBehaviour
{
    #region Variables

    private enum NetState { Ready, Cooldown }
    private NetState currentState = NetState.Ready;

    [Header("그물 오브젝트 풀")]
    [Tooltip("Net 프리팹을 관리하는 PoolManager입니다.")]
    [SerializeField] private PoolManager netPool;

    [Header("그물 생성 위치")]
    [Tooltip("그물이 생성될 시작 위치입니다.")]
    [SerializeField] private Transform spawnPoint;

    [Header("조이스틱 입력")]
    [Tooltip("오른쪽 VirtualJoystick입니다.")]
    [SerializeField] private VirtualJoystick rightJoystick;

    [Header("조이스틱 UI 비주얼")]
    [Tooltip("조이스틱의 CanvasGroup (투명도 및 입력 제어용)")]
    [SerializeField] private CanvasGroup joystickCanvasGroup;

    [Header("쿨타임 설정")]
    [Tooltip("그물 발사 후 조이스틱 쿨타임 시간입니다.")]
    [SerializeField] private float cooldownTime = 5f;

    private bool isTouching = false;
    private Vector2 lastInput = Vector2.zero;

    private const int touchID = GameConstants.EDITOR_INPUT_ID;

    #endregion

    #region Unity Methods

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleEditorInput();
#else
        HandleTouchInput();
#endif
    }

    #endregion

    #region Input Handlers

    /// <summary>
    /// 에디터 환경에서 마우스 입력을 처리합니다.
    /// </summary>
    private void HandleEditorInput()
    {
        if (Input.GetMouseButtonDown(0) && currentState == NetState.Ready)
        {
            if (Input.mousePosition.x >= Screen.width * 0.5f)
            {
                isTouching = true;
            }
        }

        if (Input.GetMouseButton(0) && isTouching)
        {
            lastInput = rightJoystick.GetInput();
        }

        if (Input.GetMouseButtonUp(0) && isTouching)
        {
            isTouching = false;
            Fire();
        }
    }

    /// <summary>
    /// 모바일 환경에서 터치 입력을 처리합니다.
    /// </summary>
    private void HandleTouchInput()
    {
        if (currentState == NetState.Cooldown) return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touch.position.x >= Screen.width * 0.5f)
            {
                isTouching = true;
            }

            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouching)
            {
                lastInput = rightJoystick.GetInput();
            }

            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isTouching)
            {
                isTouching = false;
                Fire();
            }
        }
    }

    #endregion

    #region Fire & Cooldown

    /// <summary>
    /// 입력 방향으로 그물을 발사하고 쿨타임을 시작합니다.
    /// </summary>
    private void Fire()
    {
        if (lastInput == Vector2.zero || netPool == null || spawnPoint == null)
        {
            Log.Warn("Fire 실패: 입력 값이 0이거나 필수 참조가 누락됨", this);
            return;
        }

        GameObject net = SpawnNet();
        if (net == null)
        {
            Log.Error("Net 생성 실패: 풀에서 오브젝트를 가져오지 못함", this);
            return;
        }

        InitNet(net);
        StartCoroutine(Cooldown());
    }

    /// <summary>
    /// 풀에서 그물 오브젝트를 가져옵니다.
    /// </summary>
    private GameObject SpawnNet()
    {
        return netPool?.Get();
    }

    /// <summary>
    /// 그물 오브젝트를 초기화합니다.
    /// </summary>
    private void InitNet(GameObject net)
    {
        net.transform.position = spawnPoint.position;
        net.transform.rotation = Quaternion.identity;

        Net netScript = net.GetComponent<Net>();
        if (netScript == null)
        {
            Log.Error("Net 스크립트가 존재하지 않습니다", net);
            return;
        }

        netScript.Initialize(lastInput, netPool);
    }

    /// <summary>
    /// 일정 시간 동안 조이스틱을 비활성화합니다.
    /// </summary>
    private IEnumerator Cooldown()
    {
        currentState = NetState.Cooldown;

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.alpha = 0.3f;
            joystickCanvasGroup.blocksRaycasts = false;
        }

        yield return new WaitForSeconds(cooldownTime);

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.alpha = 1f;
            joystickCanvasGroup.blocksRaycasts = true;
        }

        currentState = NetState.Ready;
    }

    #endregion
}
