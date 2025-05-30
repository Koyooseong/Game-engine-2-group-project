using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 씬에서 물고기의 블록 형태를 동적으로 생성해주는 빌더입니다.
/// </summary>
public class FishBlockBuilder : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform canvasRect;

    /// <summary>
    /// 퍼즐 블록 형태로 구성된 UI 오브젝트를 생성합니다.
    /// </summary>
    public GameObject Build(FishData data)
    {
        GameObject blockRoot = new GameObject($"Block_{data.name}", typeof(RectTransform));
        blockRoot.transform.SetParent(canvasRect, false);

        RectTransform blockRect = blockRoot.GetComponent<RectTransform>();
        blockRect.anchorMin = blockRect.anchorMax = new Vector2(0.5f, 0.5f);
        blockRect.pivot = new Vector2(0.5f, 0.5f);
        blockRect.anchoredPosition = new Vector2(300, -300); // 위치 강제

        List<Vector2Int> cells = data.puzzleShape
            .Split(';')
            .Select(s => {
                var xy = s.Split(',');
                return new Vector2Int(int.Parse(xy[0]), int.Parse(xy[1]));
            }).ToList();

        Debug.Log($"셀 {cells.Count}개 생성 예정 for {data.name}");

        foreach (Vector2Int pos in cells)
        {
            GameObject cell = Instantiate(cellPrefab, blockRoot.transform);
            RectTransform cellRect = cell.GetComponent<RectTransform>();
            cellRect.anchoredPosition = new Vector2(pos.x * 86, -pos.y * 86);
            Debug.Log($"셀 위치: {cellRect.anchoredPosition}");
        }

        return blockRoot;
    }

}
