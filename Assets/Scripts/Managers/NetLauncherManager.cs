using UnityEngine;
using System.Collections;

/// <summary>
/// 네트의 발사 쿨다운과 풀링, 발사 방향을 제어하는 매니저입니다.
/// </summary>
public class NetLauncherManager : MonoBehaviour
{
    #region Variables

    [Header("발사 위치")]
    [Tooltip("잠수함에서 네트가 생성될 위치")]
    [SerializeField] private Transform launchPoint;

    [Header("오브젝트 풀")]
    [Tooltip("네트를 생성/관리할 오브젝트 풀")]
    [SerializeField] private ObjectPool netPool;

    [Header("조이스틱 참조")]
    [Tooltip("오른쪽 VirtualJoystick 인스턴스")]
    [SerializeField] private VirtualJoystick rightJoystick;


    private float cooldownTime;
    private float range;
    private float nextFireTime;

    #endregion

    #region Unity Methods

    private void Start()
    {
        cooldownTime = UpgradeManager.Instance.GetCurrentValue(UpgradeType.Cooldown);
        range = UpgradeManager.Instance.GetCurrentValue(UpgradeType.Range);

        if (rightJoystick != null)
            rightJoystick.OnReleased += HandleJoystickReleased;
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// 조이스틱 입력 해제 시 발사를 시도합니다.
    /// </summary>
    private void HandleJoystickReleased(Vector2 direction)
    {
        TryLaunch(direction);
    }

    /// <summary>
    /// 조이스틱 입력 방향으로 네트를 발사합니다.
    /// </summary>
    public void TryLaunch(Vector2 direction)
    {
        if (Time.time < nextFireTime || direction.sqrMagnitude < 0.01f)
            return;

        nextFireTime = Time.time + cooldownTime;

        GameObject net = netPool.Get();
        net.transform.position = launchPoint.position;

        NetController controller = net.GetComponent<NetController>();
        controller.Init(direction.normalized, range);

        StopAllCoroutines();
        StartCoroutine(ShowCooldownUI());
    }

    /// <summary>
    /// 쿨다운 동안 조이스틱을 잠그고 입력을 막습니다.
    /// </summary>
    private IEnumerator ShowCooldownUI()
    {
        rightJoystick.SetInteractable(false);

        while (Time.time < nextFireTime)
        {
            float remaining = nextFireTime - Time.time;
            rightJoystick.UpdateCooldownText(remaining);
            yield return null;
        }

        rightJoystick.SetInteractable(true);
    }



    #endregion
}
