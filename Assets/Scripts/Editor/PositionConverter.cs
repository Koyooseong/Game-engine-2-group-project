using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PositionConverter : EditorWindow
{
    float inputX = 0f;
    float inputY = 0f;
    RectTransform target;

    [MenuItem("Tools/Position Converter")]
    public static void ShowWindow()
    {
        GetWindow<PositionConverter>("Position Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("기획 좌표 입력 (1080x1920 기준)", EditorStyles.boldLabel);
        inputX = EditorGUILayout.FloatField("X:", inputX);
        inputY = EditorGUILayout.FloatField("Y:", inputY);

        target = (RectTransform)EditorGUILayout.ObjectField("적용할 RectTransform", target, typeof(RectTransform), true);

        if (GUILayout.Button("좌표 변환해서 적용"))
        {
            if (target != null)
            {
                Vector2 converted = ConvertPosition(inputX, inputY);
                target.anchoredPosition = converted;
            }
        }
    }

    Vector2 ConvertPosition(float x, float y)
    {
        // 기준 해상도 1080x1920
        float convertedX = x - 1080f / 2f;
        float convertedY = 1920f / 2f - y;
        return new Vector2(convertedX, convertedY);
    }
}
