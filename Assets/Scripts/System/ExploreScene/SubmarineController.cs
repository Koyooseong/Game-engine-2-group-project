using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class SubmarineController : MonoBehaviour
{
    #region Variables

    [Header("조이스틱 참조")]
    [SerializeField] private VirtualJoystick leftJoystick;

    [Header("속도 설정")]
    [Range(0f, 10f)]
    [SerializeField] private float moveSpeed = 6f;

    [Header("연결된 시스템")]
    [SerializeField] private DepthManager depthManager;
    [SerializeField] private FuelManager fuelManager;

    [Header("이벤트 텍스트")]
    [SerializeField] private TextMeshPro eventText;

    [Header("깜빡임 대상")]
    [SerializeField] private SpriteRenderer submarineRenderer;

    private Rigidbody2D rb;
    private Vector2 currentInput = Vector2.zero;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Treasure"))
        {
            HandleTreasure(other.GetComponent<TreasureController>());
        }
        else if (other.CompareTag("Obstacle"))
        {
            HandleObstacle(other.GetComponent<ObstacleController>());
        }
    }

    #endregion

    #region Custom Methods

    private void UpdateInput()
    {
        currentInput = (leftJoystick != null) ? leftJoystick.GetInput() : Vector2.zero;
    }

    private void Move()
    {
        rb.linearVelocity = currentInput * moveSpeed;
    }

    private void HandleTreasure(TreasureController treasure)
    {
        int bonusGold = 50 + Mathf.FloorToInt(depthManager.GetCurrentDepth());
        ExploreSceneInventory.Instance.AddGold(bonusGold);

        if (eventText != null)
        {
            eventText.text = $"+ {bonusGold} G";
            eventText.gameObject.SetActive(true);
            StartCoroutine(HideTextAfterDelay());
        }

        treasure?.Deactivate();
    }

    private void HandleObstacle(ObstacleController obstacle)
    {
        float damage = fuelManager.GetCurrentFuel() * 0.1f;
        fuelManager.ReduceFuel(damage);

        if (submarineRenderer != null)
        {
            StartCoroutine(BlinkSubmarine());
        }

        obstacle?.Deactivate();
    }
    private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (eventText != null)
        {
            eventText.gameObject.SetActive(false);
        }
    }

    private IEnumerator BlinkSubmarine()
    {
        for (int i = 0; i < 2; i++)
        {
            submarineRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.5f);
            submarineRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion
}
