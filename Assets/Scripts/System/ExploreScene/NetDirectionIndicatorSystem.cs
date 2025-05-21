using UnityEngine;

/// <summary>
/// 오른쪽 조이스틱 방향을 실시간으로 받아 방향 표시를 업데이트하는 시스템입니다.
/// </summary>
public class NetDirectionIndicatorSystem : MonoBehaviour
{
    #region Variables

    [Header("화살표 오브젝트")]
    [Tooltip("화살표 방향을 회전시킬 트랜스폼")]
    [SerializeField] private Transform arrow;

    [Header("조이스틱 참조")]
    [Tooltip("오른쪽 VirtualJoystick 인스턴스")]
    [SerializeField] private VirtualJoystick rightJoystick;

    const float offset = -125f; // 오른쪽 → 위쪽으로 보정

    #endregion

    #region Unity Methods

    private void Update()
    {
        Vector2 dir = rightJoystick.GetInput();
        Debug.Log($"[DEBUG] 조이스틱 입력: {dir}");

        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;
            arrow.rotation = Quaternion.Euler(0, 0, angle);
            arrow.gameObject.SetActive(true);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
    }

    #endregion
}
