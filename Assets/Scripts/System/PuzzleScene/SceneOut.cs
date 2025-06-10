using UnityEngine;

public class SceneOut : MonoBehaviour
{
    public void OnExitPuzzleScene()
    {
        SceneLoader.Instance.UnloadSceneAndEnableCanvas("PuzzleScene");
    }

}
