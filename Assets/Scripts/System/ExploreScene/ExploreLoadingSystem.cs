using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExploreLoadingSystem : MonoBehaviour
{
    [Header("로딩 이미지들")]
    [SerializeField] private Image loadingImage;
    [SerializeField] private Sprite firstSprite;
    [SerializeField] private Sprite secondSprite;

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    /// <summary>
    /// 2초 간격 이미지 전환 후 ExploreScene을 Additive로 로드하고, 자신은 언로드합니다.
    /// </summary>
    private IEnumerator LoadingRoutine()
    {
        loadingImage.sprite = firstSprite;
        yield return new WaitForSeconds(2f);

        loadingImage.sprite = secondSprite;
        yield return new WaitForSeconds(2f);

        // 1. ExploreScene Additive로 로드
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("ExploreScene", LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // 2. 자신을 언로드
        Scene thisScene = gameObject.scene;
        SceneManager.UnloadSceneAsync(thisScene);
    }
}
