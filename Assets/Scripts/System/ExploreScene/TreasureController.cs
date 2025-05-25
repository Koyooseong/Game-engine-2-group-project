using UnityEngine;

/// <summary>
/// 보물상자 오브젝트로, 지속적으로 아래로 하강하는 기능만 담당합니다.
/// </summary>
public class TreasureController : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;

    private void Update()
    {
        if (PauseSystem.Instance?.IsPaused() == true) return;
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    public void Deactivate()
    {
        var pool = transform.parent?.GetComponent<ObjectPool>();
        if (pool != null) pool.Release(gameObject);
        else Destroy(gameObject);
    }
}
