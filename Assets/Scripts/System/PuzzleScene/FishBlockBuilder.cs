using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 씬에서 물고기의 블록 형태를 동적으로 생성해주는 빌더입니다.
/// </summary>
public class FishBlockBuilder : MonoBehaviour
{
    #region Variables

    [Header("Puzzle Block Settings")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private FishBlockPlacer fishBlockPlacer;

    private const float CELL_SIZE = 86f;
    private const float GAP = 16f;
    private const float DEFAULT_X = 300f;
    private const float DEFAULT_Y = -300f;

    #endregion

    #region Public Methods

    public GameObject Build(FishData data)
    {
        GameObject blockRoot = new GameObject($"Block_{data.name}", typeof(RectTransform));
        blockRoot.transform.SetParent(canvasRect, false);

        RectTransform blockRect = blockRoot.GetComponent<RectTransform>();
        blockRect.anchorMin = blockRect.anchorMax = new Vector2(0.5f, 0.5f);
        blockRect.pivot = new Vector2(0.5f, 0.5f);
        blockRect.anchoredPosition = new Vector2(DEFAULT_X, DEFAULT_Y);
        blockRect.sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);

        BoxCollider2D collider = blockRoot.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(CELL_SIZE, CELL_SIZE);
        collider.offset = Vector2.zero;
        collider.isTrigger = false;

        Rigidbody2D rb = blockRoot.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.useFullKinematicContacts = true;

        List<Vector2Int> cellOffsets = ParseShape(data.puzzleShape);
        Log.System($"[FishBlockBuilder] 셀 {cellOffsets.Count}개 생성 예정 for {data.name}", this);

        string cleanId = data.id.Replace("_", "");
        string basePath = $"Sprites/Fish_Puzzle/{cleanId}";
        Sprite[] spriteSheet = Resources.LoadAll<Sprite>(basePath);

        for (int i = 0; i < spriteSheet.Length; i++)
        {
            Sprite s = spriteSheet[i];
            Debug.Log($"[DEBUG] [{i}] 조각 이름: {s.name}");
        }

        foreach (Vector2Int offset in cellOffsets)
        {
            GameObject cell = Instantiate(cellPrefab, blockRoot.transform);
            RectTransform cellRect = cell.GetComponent<RectTransform>();
            float totalCellSize = CELL_SIZE + GAP;
            cellRect.anchoredPosition = new Vector2(offset.y * totalCellSize, -offset.x * totalCellSize);

            int row = offset.y;
            int col = offset.x;
            string spriteKey = $"{cleanId}_{row}_{col}";

            Sprite sprite = spriteSheet.FirstOrDefault(s => s.name == spriteKey);
            if (sprite == null)
            {
                Log.Warn($"[FishBlockBuilder] 스프라이트 없음: {spriteKey}, 기본값으로 대체", cell);
                sprite = spriteSheet.FirstOrDefault(s => s.name == $"{cleanId}_0_0");
            }

            Image image = cell.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
            }

            Log.Info($"[FishBlockBuilder] 셀 위치: {cellRect.anchoredPosition}, 스프라이트: {spriteKey}", cell);
        }

        // ✅ 기준 anchorOffset 계산: 좌상단 기준
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        foreach (var offset in cellOffsets)
        {
            if (offset.x < min.x) min.x = offset.x;
            if (offset.y < min.y) min.y = offset.y;
        }
        Vector2Int anchorOffset = min;

        // 핸들러 연결
        FishBlockHandler handler = blockRoot.AddComponent<FishBlockHandler>();
        handler.SetShape(cellOffsets.ToArray());
        handler.SetPlacer(fishBlockPlacer);
        handler.SetAnchorOffset(anchorOffset); // 위치 기준 정보 전달

        return blockRoot;
    }

    #endregion

    #region Private Methods

    private List<Vector2Int> ParseShape(string shapeString)
    {
        var list = new List<Vector2Int>();

        if (string.IsNullOrWhiteSpace(shapeString))
        {
            Debug.LogWarning("[FishBlockBuilder] puzzleShape가 비어있습니다.");
            return list;
        }

        string[] tokens = shapeString
            .Trim()
            .Trim('"')
            .Replace("\r", "")
            .Replace("\n", "")
            .Split(';');

        foreach (string token in tokens)
        {
            if (string.IsNullOrWhiteSpace(token)) continue;

            string[] parts = token.Split(',');
            if (parts.Length != 2)
            {
                Debug.LogWarning($"[FishBlockBuilder] 잘못된 셀 좌표 형식: '{token}'");
                continue;
            }

            if (int.TryParse(parts[0].Trim().Trim('"'), out int row) &&
                int.TryParse(parts[1].Trim().Trim('"'), out int col))
            {
                list.Add(new Vector2Int(col, row));
            }
            else
            {
                Debug.LogWarning($"[FishBlockBuilder] 숫자 파싱 실패: '{parts[0]}', '{parts[1]}'");
            }
        }

        return list;
    }

    #endregion
}
