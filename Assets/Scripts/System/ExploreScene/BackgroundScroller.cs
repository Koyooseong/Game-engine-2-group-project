using UnityEngine;

/// <summary>
/// 반복적으로 배경을 아래로 스크롤시키는 컨트롤러입니다.
/// 배경이 화면 아래로 내려가면 위로 재배치됩니다.
/// 잠수함은 스크롤의 영향을 받지 않습니다.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{

    // 깃허브 커밋 테스트
    #region Variables

    [Header("배경 스프라이트들")]
    [Tooltip("반복적으로 스크롤될 배경 오브젝트들")]
    [SerializeField] private Transform[] backgrounds;

    [Header("스크롤 속도")]
    [Tooltip("배경이 아래로 이동하는 속도입니다.")]
    [SerializeField] private float scrollSpeed = 3f;

    [Header("배경 재배치 기준 Y")]
    [Tooltip("배경이 이 위치보다 아래로 내려가면 위로 재배치됩니다.")]
    [SerializeField] private float resetY = -19f;

    [Header("배경 재배치 Y 위치")]
    [Tooltip("배경이 재등장할 위치입니다.")]
    [SerializeField] private float startY = 19f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        foreach (Transform bg in backgrounds)
        {
            if (bg == null) continue; // 실수로 삭제되었을 경우 안전하게 스킵

            bg.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

            if (bg.position.y <= resetY)
            {
                bg.position = new Vector3(bg.position.x, startY, bg.position.z);
            }
        }
    }

    #endregion
}