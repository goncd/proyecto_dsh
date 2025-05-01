using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private string sceneToLoad;
    private Action onDone;

    public void Init(string sceneName, Action onFinished)
    {
        sceneToLoad = sceneName;
        onDone = onFinished;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Wait one frame for UI to be ready.
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (GameObject.Find("Slider").TryGetComponent(out Slider slider))
                slider.value = progress;

            if (GameObject.Find("Percentage").TryGetComponent(out TMP_Text percentage))
                percentage.text = $"{Mathf.RoundToInt(progress * 100)}%";

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        onDone?.Invoke();
        Destroy(gameObject);
    }
}
