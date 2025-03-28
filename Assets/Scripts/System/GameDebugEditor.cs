#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Game;

public class GameDebugEditor : EditorWindow
{
    #region Variables

    private const int GOLD_CHANGE_AMOUNT = 100;
    private const int FISH_CHANGE_AMOUNT = 1;

    #endregion

    #region Unity Editor Methods

    [MenuItem("Tools/Game Debugger")]
    private static void OpenWindow()
    {
        GameDebugEditor window = GetWindow<GameDebugEditor>("Game Debugger");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("¢º °ñµå µð¹ö±ë", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("+100 °ñµå"))
        {
            GoldManager.AddGold(GOLD_CHANGE_AMOUNT);
        }

        if (GUILayout.Button("-100 °ñµå"))
        {
            GoldManager.AddGold(-GOLD_CHANGE_AMOUNT);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        GUILayout.Label("¢º ¹°°í±â ¼ö Á¶ÀÛ", EditorStyles.boldLabel);

        foreach (FishType type in System.Enum.GetValues(typeof(FishType)))
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{type}", GUILayout.Width(100));

            if (GUILayout.Button("+1"))
            {
                GameResultManager.AddFish(type, FISH_CHANGE_AMOUNT);
            }

            if (GUILayout.Button("-1"))
            {
                GameResultManager.AddFish(type, -FISH_CHANGE_AMOUNT);
            }

            GUILayout.Label($"ÇöÀç: {GameResultManager.GetFishCount(type)}¸¶¸®");

            GUILayout.EndHorizontal();
        }
    }

    #endregion
}
#endif
