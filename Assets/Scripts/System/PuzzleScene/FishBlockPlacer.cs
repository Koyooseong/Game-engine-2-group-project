using UnityEngine;

/// <summary>
/// 퍼즐 블록의 배치 유효성 검사를 수행하고 실제 배치를 확정합니다.
/// </summary>
public class FishBlockPlacer : MonoBehaviour
{
    [SerializeField] private TankGridManager gridManager;


    public bool TryPlaceBlock(FishBlockHandler block, Vector3 worldPos)
    {
        Log.System("[Placer] TryPlaceBlock 호출됨", block);
        Log.System($"[Placer] worldPos = {worldPos}, anchorOffset = {block.AnchorOffset}", block);



        Vector2Int[] shape = block.GetShape();

        // ✅ anchorOffset 보정값 적용 (시각적 중심 → 기준 셀 위치로 변환)
        Vector2 offsetCorrection = new Vector2(
            block.AnchorOffset.x * (86f + 16f),
            -block.AnchorOffset.y * (86f + 16f)
        );

        Vector3 correctedPos = worldPos - (Vector3)offsetCorrection;

        Log.System($"[Placer] correctedPos = {correctedPos}", block);

        // ✅ 기준 셀 위치로 정확하게 anchor 계산
        Vector2Int anchor = gridManager.GetNearestGrid(correctedPos);

        if (!gridManager.CanPlaceBlock(anchor, shape))
        {
            Log.Warn($"[FishBlockPlacer] 배치 실패 - anchor: {anchor}, offset: {block.AnchorOffset}", block);
            return false;
        }

        // ✅ 블록 상태 기록
        foreach (var offset in shape)
        {
            Vector2Int pos = anchor + offset;
            gridManager.SetBlock(pos);
        }

        // ✅ 정확한 위치로 스냅
        block.SnapToGrid(anchor);
        return true;


        Log.System($"[Placer] shape count = {(shape == null ? -1 : shape.Length)}", block);

    }


    private void Awake()
    {
        FishBlockHandler.RegisterPlacer(this);
    }

}
