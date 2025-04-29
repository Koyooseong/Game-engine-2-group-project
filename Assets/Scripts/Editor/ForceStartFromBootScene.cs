#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ForceStartFromBootScene
{
    static ForceStartFromBootScene()
    {
        EditorApplication.playModeStateChanged += ChangeToStartScene;
    }

    private static void ChangeToStartScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string startScenePath = "Assets/Scenes/AquariumScene.unity";
            if (EditorSceneManager.GetActiveScene().path != startScenePath)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(startScenePath);
            }
        }
    }
}
#endif
