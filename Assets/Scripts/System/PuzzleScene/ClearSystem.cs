using System.Collections.Generic;
using UnityEngine;

public class ClearSystem : MonoBehaviour
{
    public void ClearAllBlocks()
    {
        // 1. 씬에 존재하는 모든 블록 탐색
        FishBlockHandler[] allBlocks = GameObject.FindObjectsOfType<FishBlockHandler>();

        foreach (var block in allBlocks)
        {
            // 2. 인벤토리 수량 복원
            string fishId = block.GetFishKey();
            PuzzleInventoryItemUI[] slots = GameObject.FindObjectsOfType<PuzzleInventoryItemUI>();
            foreach (var slot in slots)
            {
                if (slot.GetFishKey() == fishId)
                {
                    slot.RestoreOne(); // 수량 +1
                    break;
                }
            }

            // 3. GridManager에 등록된 블록이면 해제 처리
            Vector2Int gridPos = TankGridManager.Instance.GetNearestGrid(block.transform.position);
            if (TankGridManager.Instance.GetBlock(gridPos) == block)
            {
                TankGridManager.Instance.ClearBlock(gridPos);
            }

            // 4. 블록 GameObject 제거
            Destroy(block.gameObject);
        }

        Debug.Log("[ClearSystem] 저장 여부와 무관하게 모든 블록 초기화 완료");
    }
}
