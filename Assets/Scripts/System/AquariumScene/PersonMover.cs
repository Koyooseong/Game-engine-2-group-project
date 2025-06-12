using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 상에서 생성된 사람이 X축으로 이동하고 일정 시간 뒤 풀로 복귀합니다.
/// </summary>
public class PersonMover : MonoBehaviour
{
    private float moveSpeed;
    private ObjectPool returnPool;
    private float lifetime = 10f;

    private RectTransform rectTransform;
    private Image image;
    private Sprite lastSprite;

    public void Initialize(float speed, ObjectPool pool)
    {
        moveSpeed = speed;
        returnPool = pool;

        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // 초기화 시 강제로 맞춰줌
        if (image != null)
        {
            lastSprite = image.sprite;
            image.SetNativeSize();
        }

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void Update()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0f);
        }

        if (image != null && image.sprite != lastSprite)
        {
            lastSprite = image.sprite;
            image.SetNativeSize(); // ✅ sprite 바뀌면 자동 적용
        }
    }

    private void ReturnToPool()
    {
        returnPool?.Release(gameObject);
    }
}
