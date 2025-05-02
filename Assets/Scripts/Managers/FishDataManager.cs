using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 물고기 데이터를 관리하는 매니저입니다.
/// </summary>
public class FishDataManager : MonoBehaviour
{
    public static FishDataManager Instance { get; private set; }

    #region Variables

    [Header("CSV 파일")]
    [SerializeField] private TextAsset csvFile;

    private Dictionary<string, FishData> fishDataDictionary = new Dictionary<string, FishData>();

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
    /// CSV 파일을 읽어 데이터를 로드합니다.
    /// </summary>
    private void LoadCSV()
    {
        if (csvFile == null)
        {
            Log.Error("CSV 파일이 연결되지 않았습니다.", this);
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] columns = lines[i].Trim().Split(',');

            string id = columns[0];
            string name = columns[1];
            string explain1 = columns[2];
            string explain2 = columns[3];
            string explain3 = columns[4];
            int gold = int.Parse(columns[5]);
            string type = columns[6];
            int puzzle = int.Parse(columns[7]);
            string rare = columns[8];
            string fishImg = columns[9];
            string puzzleImg = columns[10];

            FishData data = new FishData(id, name, explain1, explain2, explain3, gold, type, puzzle, rare, fishImg, puzzleImg);
            fishDataDictionary[id] = data;
        }

        Log.System($"FishData {fishDataDictionary.Count}개 로드 완료", this);
    }

    public FishData GetFishData(string id)
    {
        return fishDataDictionary.TryGetValue(id, out var data) ? data : default;
    }

    public string GetExplain1(string id) => GetFishData(id).explain1;
    public string GetExplain2(string id) => GetFishData(id).explain2;
    public string GetExplain3(string id) => GetFishData(id).explain3;
    public string GetName(string id) => GetFishData(id).name;
    public int GetGold(string id) => GetFishData(id).gold;
    public string GetType(string id) => GetFishData(id).type;
    public int GetPuzzleCount(string id) => GetFishData(id).puzzle;
    public string GetRarity(string id) => GetFishData(id).rare;
    public string GetPrefabPath(string id) => GetFishData(id).fishImg;
    public string GetPuzzleSpritePath(string id) => GetFishData(id).puzzleImg;

    #endregion
}

/// <summary>
/// 물고기 데이터를 저장하는 구조체입니다.
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

    public FishData(string id, string name, string explain1, string explain2, string explain3,
                    int gold, string type, int puzzle, string rare, string fishImg, string puzzleImg)
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
    }
}
