using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.WSA;

public class PuzzleSaveSystem : MonoBehaviour
{
    #region Variables

    private const string SAVE_KEY = "PuzzleGridSave";
    [SerializeField] private GameObject toastUIPrefab;

    [SerializeField] private FishBlockBuilder fishBlockBuilder;
    [SerializeField] private GameObject buttonUIPrefab;
    [SerializeField] private Transform toastParent;

    private GameObject currentToast;

    #endregion

    #region Unity Methods

    private void Start()
    {
        LoadData();
    }

    #endregion

    #region Custom Methods

    public void SaveData()
    {
        Log.System("[PuzzleSaveSystem] 충돌 기반 저장 시작");

        PuzzleSaveData saveData = new PuzzleSaveData();

        // Step 1: Grid 정보 먼저 저장 (해금 상태만)
        foreach (var gridPair in TankGridManager.Instance.GetAllGrids())
        {
            GridSaveData data = new GridSaveData
            {
                gridPos = gridPair.Key,
                isUnlocked = gridPair.Value.IsUnlocked(),
                fishId = ""
            };

            saveData.gridDataList.Add(data);
        }

        // Step 2: FishBlockHandler 전체 탐색 → 위치 기준으로 격자 충돌 확인
        FishBlockHandler[] blocks = GameObject.FindObjectsOfType<FishBlockHandler>();
        foreach (var block in blocks)
        {
            if (block.IsOverlapping())
            {
                ShowToast("저장 불가 - 겹치거나 잠긴 칸에 물고기가 있어요");
                Log.Error("[Save] 충돌된 블록 존재 - 저장 중단", block);
                return;
            }

            Vector3 center = block.GetComponent<RectTransform>().position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, 4f);

            foreach (var hit in hits)
            {
                if (!hit.CompareTag("GridCell")) continue;

                TankGridController grid = hit.GetComponent<TankGridController>();
                if (grid == null || !grid.IsUnlocked()) continue;

                Vector2Int pos = grid.GetGridPosition();
                string fishId = block.GetFishKey();

                if (string.IsNullOrEmpty(fishId))
                {
                    Log.Warn($"[Save] fishId가 비어있음 - {pos}", block);
                    continue;
                }

                // 기존 GridSaveData에 fishId 덮어쓰기
                GridSaveData match = saveData.gridDataList.Find(g => g.gridPos == pos);
                if (match != null)
                {
                    match.fishId = fishId;
                    Log.Info($"[Save] 저장됨 - {pos}, {fishId}");
                }
            }
        }

        // Step 3: 저장
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Log.System("[PuzzleSaveSystem] 저장 완료");
    }


    public void LoadData()
    {
        Log.System("[PuzzleSaveSystem] 저장 데이터 로드 시작");

        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Log.Warn("[PuzzleSaveSystem] 저장 데이터 없음 - PlayerPrefs에 키 없음");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        PuzzleSaveData saveData = JsonUtility.FromJson<PuzzleSaveData>(json);
        Log.System($"[PuzzleSaveSystem] JSON 파싱 완료 - 셀 수: {saveData.gridDataList.Count}");

        foreach (GridSaveData data in saveData.gridDataList)
        {
            Log.Info($"[Load] 셀 위치: {data.gridPos}, 해금됨: {data.isUnlocked}, fishId: {data.fishId}");

            TankGridController grid = TankGridManager.Instance.GetGrid(data.gridPos);
            if (grid == null)
            {
                Log.Warn($"[Load] 유효하지 않은 위치 - 셀 없음: {data.gridPos}");
                continue;
            }

            if (data.isUnlocked)
            {
                grid.Unlock();
                Log.System($"[Load] 셀 해금 처리 완료: {data.gridPos}");
            }
            else
            {
                Log.System($"[Load] 셀 잠금 상태 유지: {data.gridPos}");
                continue;
            }

            if (!string.IsNullOrEmpty(data.fishId))
            {
                FishData fishData = FishDataManager.Instance.GetFishData(data.fishId);
                if (string.IsNullOrEmpty(fishData.id))
                {
                    Log.Warn($"[Load] fishId에 대응되는 FishData 없음: {data.fishId}");
                    continue;
                }

                Log.System($"[Load] FishData 로드 성공: {data.fishId}");

                GameObject block = fishBlockBuilder.Build(fishData, null, buttonUIPrefab);
                if (block == null)
                {
                    Log.Error($"[Load] FishBlockBuilder.Build 실패: {data.fishId}");
                    continue;
                }

                Log.System($"[Load] 블록 생성 성공: {data.fishId}");

                FishBlockHandler handler = block.GetComponent<FishBlockHandler>();
                if (handler != null)
                {
                    handler.SetFishIdManually(data.fishId);
                    handler.ForceSnapTo(data.gridPos);
                    Log.System($"[Load] 블록 위치 스냅 완료: {data.gridPos}");
                }
                else
                {
                    Log.Error("[Load] 생성된 블록에 FishBlockHandler 없음!");
                }
                // 🔥 인벤토리 수량 차감 시도
                PuzzleInventoryItemUI[] slots = GameObject.FindObjectsOfType<PuzzleInventoryItemUI>();
                foreach (var slot in slots)
                {
                    if (slot.GetFishKey() == data.fishId)
                    {
                        slot.TryConsumeOne(); // 수량 줄이기
                        break;
                    }
                }
            }
            else
            {
                Log.Info($"[Load] 해당 셀은 물고기 없음: {data.gridPos}");
            }
        }

        Log.System("[PuzzleSaveSystem] 저장 데이터 로드 완료");
    }

    private void ShowToast(string message)
    {
        if (toastUIPrefab == null || toastParent == null)
        {
            Log.Warn("[PuzzleSaveSystem] 토스트 생성 실패 - 프리팹 또는 부모 미지정");
            return;
        }

        // 이미 떠 있는 Toast가 있다면 새로 생성 안함
        if (currentToast != null)
        {
            Log.Info("[PuzzleSaveSystem] 이미 토스트가 떠있어서 중복 생성 안함");
            return;
        }

        currentToast = Instantiate(toastUIPrefab, toastParent);
        currentToast.GetComponent<ToastUI>()?.Show(message, Color.red);

        Destroy(currentToast, 2f);
    }

    #endregion
}

[Serializable]
public class GridSaveData
{
    public Vector2Int gridPos;
    public bool isUnlocked;
    public string fishId;
}

[Serializable]
public class PuzzleSaveData
{
    public List<GridSaveData> gridDataList = new();
}
