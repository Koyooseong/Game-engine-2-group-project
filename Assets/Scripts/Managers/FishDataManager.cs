using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 물고기 CSV 데이터를 로드하고, ID 기반으로 정보를 제공하는 데이터 매니저입니다.
/// </summary>
public class FishDataManager : MonoBehaviour
{
    public static FishDataManager Instance { get; private set; }

    #region Variables

    [Header("CSV 파일")]
    [Tooltip("물고기 데이터를 담은 CSV 파일")]
    [SerializeField] private TextAsset csvFile;

    private readonly Dictionary<string, FishData> fishDataDictionary = new();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// CSV 파일을 읽어 FishData를 로드합니다.
    /// </summary>
    private void LoadCSV()
    {
        if (csvFile == null)
        {
            Log.Error("[FishDataManager] CSV 파일이 연결되지 않았습니다.", this);
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length < 13)
            {
                Log.Warn($"[FishDataManager] 열 개수가 부족한 행이 발견됨 (index: {i})", this);
                continue;
            }

            string id = columns[0];
            string name = columns[1];
            string explain1 = columns[2];
            string explain2 = columns[3];
            string explain3 = columns[4];
            int gold = int.TryParse(columns[5], out int g) ? g : 0;
            string type = columns[6];
            int puzzle = int.TryParse(columns[7], out int p) ? p : 0;
            string rare = columns[8];
            string fishImg = columns[9];
            string puzzleImg = columns[10];
            string fishCatch = columns[11];
            string fishSprite = columns[12];

            // ✅ puzzleShape: 13번째 이후 모든 컬럼을 다시 붙임 (쉼표 포함 가능)
            string puzzleShape = columns.Length > 13
                ? string.Join(",", columns, 13, columns.Length - 13).Trim().Trim('"')
                : "";

            Log.System($"[FishDataManager] {id}의 puzzleShape 원본: {puzzleShape}", this);

            FishData data = new FishData(id, name, explain1, explain2, explain3,
                                         gold, type, puzzle, rare,
                                         fishImg, puzzleImg, fishCatch, fishSprite, puzzleShape);

            fishDataDictionary[id] = data;
        }

        Log.System($"[FishDataManager] {fishDataDictionary.Count}개 로드 완료", this);
    }

    /// <summary>
    /// 특정 ID의 물고기 데이터를 반환합니다.
    /// </summary>
    public FishData GetFishData(string id)
    {
        return fishDataDictionary.TryGetValue(id, out var data) ? data : default;
    }

    /// <summary>
    /// 전체 물고기 데이터를 반환합니다.
    /// </summary>
    public Dictionary<string, FishData> GetAllFish() => fishDataDictionary;

    /// <summary>
    /// ID로 물고기 이름을 반환합니다.
    /// </summary>
    public string GetName(string id) => GetFishData(id).name;

    /// <summary>
    /// ID로 희귀도를 반환합니다.
    /// </summary>
    public string GetRarity(string id) => GetFishData(id).rare;

    /// <summary>
    /// 프리팹 경로를 반환합니다.
    /// </summary>
    public string GetPrefabPath(string id) => GetFishData(id).fishImg;

    /// <summary>
    /// 물고기 잡힌 스프라이트 경로를 반환합니다.
    /// </summary>
    public string GetCatchSprite(string id) => GetFishData(id).fishCatch;

    /// <summary>
    /// 물고기 이동 스프라이트 경로를 반환합니다.
    /// </summary>
    public string GetMoveSprite(string id) => GetFishData(id).fishSprite;

    /// <summary>
    /// Resources 폴더 기준으로 프리팹을 로드합니다.
    /// </summary>
    public GameObject LoadFishPrefab(string id)
    {
        string path = GetPrefabPath(id);
        return Resources.Load<GameObject>(path);
    }

    #endregion
}

/// <summary>
/// CSV에서 로드된 물고기 데이터 구조체입니다.
/// </summary>
public struct FishData
{
    public string id;
    public string name;
    public string explain1;
    public string explain2;
    public string explain3;
    public int gold;
    public string type;
    public int puzzle;
    public string rare;
    public string fishImg;
    public string puzzleImg;
    public string fishCatch;
    public string fishSprite;
    public string puzzleShape; //퍼즐 때문에 추가함.

    public FishData(string id, string name, string explain1, string explain2, string explain3,
                    int gold, string type, int puzzle, string rare,
                    string fishImg, string puzzleImg, string fishCatch, string fishSprite, string puzzleShape)
    {
        this.id = id;
        this.name = name;
        this.explain1 = explain1;
        this.explain2 = explain2;
        this.explain3 = explain3;
        this.gold = gold;
        this.type = type;
        this.puzzle = puzzle;
        this.rare = rare;
        this.fishImg = fishImg;
        this.puzzleImg = puzzleImg;
        this.fishCatch = fishCatch;
        this.fishSprite = fishSprite;
        this .puzzleShape = puzzleShape;
    }
}
