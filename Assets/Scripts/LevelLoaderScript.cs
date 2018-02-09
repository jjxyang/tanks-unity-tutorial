using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour {
    public GameObject loadingBar;
    public Slider loadingSlider;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        // operating asynchronously, i.e. in a coroutine
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingBar.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;

            // will wait until next frame to continue
            yield return null;
        }
    }
}
