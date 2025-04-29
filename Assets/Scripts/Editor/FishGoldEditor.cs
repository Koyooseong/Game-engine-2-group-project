#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 에디터에서 물고기 및 골드 조작을 위한 개발자 도구입니다.
/// </summary>
public class FishGoldEditor : EditorWindow
{
    private string fishKey = "FISH_001";
    private int goldAmount = 100;

    [MenuItem("Tools/Dev Tools/Fish & Gold Editor")]
    public static void ShowWindow()
    {
        GetWindow<FishGoldEditor>("Fish & Gold Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Fish Control", EditorStyles.boldLabel);

        fishKey = EditorGUILayout.TextField("Fish Key", fishKey);
        if (GUILayout.Button("Add Fish"))
        {
            if (Application.isPlaying)
            {
                InventoryManager.Instance.AddFish(fishKey);
                Debug.Log($"[EDITOR] {fishKey} 추가됨");
            }
            else
            {
                Debug.LogWarning("Play 모드에서만 작동합니다.");
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Gold Control", EditorStyles.boldLabel);

        goldAmount = EditorGUILayout.IntField("Gold Amount", goldAmount);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Gold"))
        {
            if (Application.isPlaying)
                GoldManager.Instance.AddGold(goldAmount);
            else
                Debug.LogWarning("Play 모드에서만 작동합니다.");
        }

        if (GUILayout.Button("Remove Gold"))
        {
            if (Application.isPlaying)
                GoldManager.Instance.RemoveGold(goldAmount);
            else
                Debug.LogWarning("Play 모드에서만 작동합니다.");
        }
        GUILayout.EndHorizontal();
    }
}
#endif
