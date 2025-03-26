using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;

public class SceneAutoRegister : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        // 씬이 추가된 경우만 체크
        bool hasNewScene = importedAssets.Any(path => path.EndsWith(".unity"));

        if (!hasNewScene) return;

        // 현재 Build Settings에 등록된 씬 경로 목록
        var currentScenePaths = EditorBuildSettings.scenes.Select(s => s.path).ToList();

        // 새로 추가된 씬 중 등록 안된 씬 찾기
        var newScenesToAdd = importedAssets
            .Where(path => path.EndsWith(".unity") && !currentScenePaths.Contains(path))
            .Select(path => new EditorBuildSettingsScene(path, true));

        // 기존 씬 + 새 씬 병합
        var updatedScenes = currentScenePaths
            .Select(p => new EditorBuildSettingsScene(p, true))
            .Concat(newScenesToAdd)
            .ToArray();

        EditorBuildSettings.scenes = updatedScenes;

        if (newScenesToAdd.Any())
        {
            UnityEngine.Debug.Log($" 새로운 씬이 Build Settings에 자동 추가되었습니다.");
        }
    }
}
