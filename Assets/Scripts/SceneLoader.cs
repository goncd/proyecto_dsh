using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Singleton instance.
    public static SceneLoader Instance { get; private set; }

    // The name of the scene that needs to be loaded.
    private string targetScene;

    private string previousScene;

    // If true, a scene is being loaded right now and a scene
    // load can't be triggered again until the scene finishes loading.
    private bool isLoading = false;

    // Handle the singleton by ensuring only one instance of this class
    // is active at all times.
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        isLoading = true;
        targetScene = sceneName;
        previousScene = SceneManager.GetActiveScene().name;

        // Load loading screen scene.
        SceneManager.LoadScene("Loading");
    }

    public void LoadPreviousScene()
    {
        LoadScene(previousScene ?? SceneManager.GetSceneByBuildIndex(0).name);
    }

    // This method will not set the current scene as the previous scene when
    // it's reloaded.
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When the loading scene is active, begin async loading of target
        if (scene.name == "Loading" && !string.IsNullOrEmpty(targetScene))
        {
            GameObject loader = new("AsyncSceneLoader");
            loader.AddComponent<LoadingScreen>().Init(targetScene, () => isLoading = false);
        }
    }
}
