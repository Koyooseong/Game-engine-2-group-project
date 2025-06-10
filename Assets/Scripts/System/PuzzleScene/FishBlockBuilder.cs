using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 퍼즐 씬에서 물고기의 블록 형태를 동적으로 생성해주는 빌더입니다.
/// </summary>
public class FishBlockBuilder : MonoBehaviour
{
    #region Variables

    [Header("Puzzle Block Settings")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform canvasRect;

    private const float CELL_SIZE = 86f;
    private const float GAP = 16f;
    private const float DEFAULT_X = 300f;
    private const float DEFAULT_Y = -300f;

    #endregion

    #region Public Methods

    /// <summary>
    /// 퍼즐 블록 형태로 구성된 UI 오브젝트를 생성합니다.
    /// </summary>
    /// <param name="data">물고기 데이터</param>
    /// <returns>생성된 퍼즐 블록 오브젝트</returns>
    public GameObject Build(FishData data, PuzzleInventoryItemUI itemUI, GameObject buttonUIPrefab)
    {
        GameObject blockRoot = new GameObject($"Block_{data.name}", typeof(RectTransform));
        blockRoot.transform.SetParent(canvasRect, false);

        RectTransform blockRect = blockRoot.GetComponent<RectTransform>();
        blockRect.anchorMin = blockRect.anchorMax = new Vector2(0.5f, 0.5f);
        blockRect.pivot = new Vector2(0.5f, 0.5f);
        blockRect.anchoredPosition = new Vector2(DEFAULT_X, DEFAULT_Y);
        blockRect.sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);

        // 🔧 BoxCollider2D 추가
        BoxCollider2D collider = blockRoot.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(CELL_SIZE, CELL_SIZE); // 셀 크기와 동일하게
        collider.offset = Vector2.zero;
        collider.isTrigger = false; // 충돌 대상으로 작동해야 하니까

        // 🔧 Rigidbody2D 추가 (필수)
        Rigidbody2D rb = blockRoot.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.useFullKinematicContacts = true; // 트리거와 접촉도 감지 가능하게 설정

        List<Vector2Int> cellOffsets = ParseShape(data.puzzleShape);
        Log.System($"[FishBlockBuilder] 셀 {cellOffsets.Count}개 생성 예정 for {data.name}", this);

        // ✅ 스프라이트 시트 전체 로드
        string cleanId = data.id.Replace("_", "");
        string basePath = $"Sprites/Fish_Puzzle/{cleanId}";
        Sprite[] spriteSheet = Resources.LoadAll<Sprite>(basePath);
        Debug.Log($"[TEST] 스프라이트 시트 개수: {spriteSheet.Length} ({basePath})");
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

            // ✅ 모양은 x=col, y=row → 그대로 사용
            cellRect.anchoredPosition = new Vector2(offset.y * totalCellSize, -offset.x * totalCellSize);

            // ✅ 이미지 키는 row_col 순서로 이름 접근해야 함
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

        // FishBlockHandler 스크립트 부착 (드래그 및 회전/삭제 UI 담당)
        blockRoot.AddComponent<FishBlockHandler>();


        FishBlockHandler handler = blockRoot.GetComponent<FishBlockHandler>();
        if (handler != null)
        {
            handler.SetItemUI(itemUI);
            handler.SetButtonUIPrefab(buttonUIPrefab);
        }



        return blockRoot;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 퍼즐 셀의 위치 문자열을 파싱하여 셀 좌표 리스트를 반환합니다.
    /// </summary>
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
                list.Add(new Vector2Int(col, row)); // ✅ col → x, row → y
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
