using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 퍼즐 씬 인벤토리 전체 UI를 구성합니다. 모든 물고기를 표시하며, 보유 수량에 따라 표시 상태를 조절합니다.
/// </summary>
public class PuzzleInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform inventoryContentParent;

    private void Start()
    {
        BuildInventoryUI();
    }

    private void BuildInventoryUI()
    {
        foreach (Transform child in inventoryContentParent)
        {
            Destroy(child.gameObject);
        }

        List<FishData> allFish = FishDataManager.Instance.GetAllFish().Values.ToList();

        foreach (FishData fishData in allFish)
        {
            int ownedCount = InventoryManager.Instance.GetCount(fishData.id);

            GameObject itemObj = Instantiate(inventoryItemPrefab, inventoryContentParent);
            PuzzleInventoryItemUI itemUI = itemObj.GetComponent<PuzzleInventoryItemUI>();
            itemUI.Initialize(fishData, ownedCount);
        }
    }
}
