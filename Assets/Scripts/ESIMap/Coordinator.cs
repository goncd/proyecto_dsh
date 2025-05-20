using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Coordinator : MonoBehaviour
{
    public GameObject[] disableWhenPaused;

    public GameObject[] disableOnTitleScreen;

    public GameObject introCanvas;
    public GameObject creditsCanvas;

    public GameObject minigamesCanvas;

    public GameObject pauseCanvas;

    public Button startButton;

    public Button introCreditsButton;

    public Button creditsBackButton;

    public Button pauseContinueButton;

    public Button pauseRestartButton;

    public Transform playerCamera;

    public Transform playerRealPosition;

    public GameObject player;

    public Button introMinigamesButton;

    public Button arkanoidLoad;
    public TMP_Text arkanoidPoints;

    public Button sameGameLoad;
    public TMP_Text sameGamePoints;

    public Button wantedLoad;
    public TMP_Text wantedPoints;

    public Button parkTheCarLoad;
    public TMP_Text parkTheCarPoints;

    public Button cuatroDigitosLoad;
    public TMP_Text cuatroDigitosPoints;

    public Button chickenHunterLoad;
    public TMP_Text chickenHunterPoints;

    public Button tresPeriodicoLoad;
    public TMP_Text tresPeriodicoPoints;

    public Button minigamesBackButton;

    private bool isUIActive = true;

    private bool wasUIAlreadyDisabled = false;

    private bool isBeingAnimated = false;

    private bool hasFinishedLoading = false;

    public void SetBeingAnimated(bool beingAnimated)
    {
        isBeingAnimated = beingAnimated;
    }

    public void ToggleUI(bool isActive)
    {
        if (!hasFinishedLoading)
            return;

        if (!isUIActive && !isActive)
            wasUIAlreadyDisabled = true;

        isUIActive = isActive;

        foreach (GameObject gameObject in disableWhenPaused)
            gameObject.SetActive(isActive);

        if (isActive)
        {
            StartCoroutine(ReapplyCursorLockNextFrame());

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void SetPause(bool isActive)
    {
        pauseCanvas.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(OnStartButton);
        introCreditsButton.onClick.AddListener(OnIntroCreditsButton);
        introMinigamesButton.onClick.AddListener(OnIntroMinigamesButton);

        creditsBackButton.onClick.AddListener(OnCreditsBackButton);
        minigamesBackButton.onClick.AddListener(OnMinigamesBackButton);


        pauseContinueButton.onClick.AddListener(OnPauseContinueButton);
        pauseRestartButton.onClick.AddListener(OnPauseRestartButton);

        if (GameState.Instance.Get("player_transform", out Tuple<Vector3, Quaternion> oldPlayerTransform))
        {
            introCanvas.SetActive(false);

            hasFinishedLoading = true;

            player.transform.SetPositionAndRotation(oldPlayerTransform.Item1, oldPlayerTransform.Item2);

            ToggleUI(true);
        }
        else
        {
            StartCoroutine(FadeInCanvas(introCanvas, 1f));

            foreach (GameObject gameObject in disableOnTitleScreen)
                gameObject.SetActive(false);

            hasFinishedLoading = true;
            ToggleUI(false);
        }
    }

    private void OnStartButton()
    {
        if (introCanvas.GetComponent<CanvasGroup>().alpha != 1f)
            return;

        GameState.Instance.Set("arkanoid_points", 0);
        GameState.Instance.Set("samegame_points", 0);
        GameState.Instance.Set("wanted_points", 0);
        GameState.Instance.Set("cuatrodigitos_points", 0);
        GameState.Instance.Set("parkthecar_points", 0);
        GameState.Instance.Set("chickenhunter_points", 0);
        GameState.Instance.Set("tresperiodico_points", 0);

        StartCoroutine(TransitionToGameMode(playerCamera, playerRealPosition, 3f));
        StartCoroutine(FadeOutCanvas(introCanvas, 1.5f));
    }

    private void OnIntroCreditsButton()
    {
        introCanvas.SetActive(false);

        creditsCanvas.SetActive(true);
    }

    private void OnIntroMinigamesButton()
    {
        arkanoidLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Arkanoid"));
        sameGameLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("SameGame"));
        wantedLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Wanted"));
        cuatroDigitosLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("CuatroDigitos"));
        parkTheCarLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Scenes/ParkTheCar/Level 1"));
        chickenHunterLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("ChickenHunter"));
        tresPeriodicoLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("TresPeriodico"));

        if (GameState.Instance.Get("arkanoid_points", out int arkanoid_points))
            arkanoidPoints.text = $"Puntos: {arkanoid_points}";

        if (GameState.Instance.Get("samegame_points", out int samegame_points))
            sameGamePoints.text = $"Puntos: {samegame_points}";

        if (GameState.Instance.Get("wanted_points", out int wanted_points))
            wantedPoints.text = $"Puntos: {wanted_points}";

        if (GameState.Instance.Get("cuatrodigitos_points", out int cuatrodigitos_points))
            cuatroDigitosPoints.text = $"Puntos: {cuatrodigitos_points}";

        if (GameState.Instance.Get("parkthecar_points", out int parkthecar_points))
            parkTheCarPoints.text = $"Puntos: {parkthecar_points}";

        if (GameState.Instance.Get("chickenhunter_points", out int chickenhunter_points))
            chickenHunterPoints.text = $"Puntos: {chickenhunter_points}";

        if (GameState.Instance.Get("tresperiodico_points", out int tresperiodico_points))
            tresPeriodicoPoints.text = $"Puntos: {tresperiodico_points}";

        GameState.Instance.Set("arkanoid_objective", 400);
        GameState.Instance.Set("samegame_objective", 500);
        GameState.Instance.Set("parkthecar_objective", 400);
        GameState.Instance.Set("chickenhunter_objective", 450);

        introCanvas.SetActive(false);
        minigamesCanvas.SetActive(true);
    }

    private void OnCreditsBackButton()
    {
        introCanvas.SetActive(true);

        creditsCanvas.SetActive(false);
    }

    private void OnMinigamesBackButton()
    {
        introCanvas.SetActive(true);

        minigamesCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBeingAnimated && Input.GetKeyDown(KeyCode.Escape) && !introCanvas.activeInHierarchy && !creditsCanvas.activeInHierarchy)
        {
            bool isPaused = pauseCanvas.activeInHierarchy;
            SetPause(!isPaused);

            if (wasUIAlreadyDisabled)
                wasUIAlreadyDisabled = false;
            else
                ToggleUI(isPaused);
        }
    }

    private IEnumerator FadeOutCanvas(GameObject canvas, float t)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha -= Time.deltaTime / t;
            yield return null;
        }

        canvas.SetActive(false);
    }

    private IEnumerator FadeInCanvas(GameObject canvas, float t)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        canvas.SetActive(true);

        while (canvasGroup.alpha < 1.0f)
        {
            canvasGroup.alpha += Time.deltaTime / t;
            yield return null;
        }
    }

    private IEnumerator TransitionToGameMode(Transform transform, Transform endTransform, float time)
    {
        SetBeingAnimated(true);
        transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;

            transform.SetPositionAndRotation(Vector3.Lerp(startPos, endTransform.position, t),
                                             Quaternion.Slerp(startRot, endTransform.rotation, t));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position and rotation are exactly as they should be.
        transform.SetPositionAndRotation(endTransform.position, endTransform.rotation);

        foreach (GameObject gameObject in disableOnTitleScreen)
            gameObject.SetActive(true);

        SetBeingAnimated(false);
        ToggleUI(true);
    }

    private void OnPauseContinueButton()
    {
        SetPause(false);

        if (wasUIAlreadyDisabled)
            wasUIAlreadyDisabled = false;
        else
            ToggleUI(true);
    }

    private void OnPauseRestartButton()
    {
        Time.timeScale = 1f;

        Destroy(GameState.Instance);

        SceneLoader.Instance.ReloadCurrentScene();
    }

    private IEnumerator ReapplyCursorLockNextFrame()
    {
        // Wait a single frame.
        yield return null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && isUIActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SendToMinigame(string name)
    {
        GameState.Instance.Set("player_transform", new Tuple<Vector3, Quaternion>(player.transform.position, player.transform.rotation));
        GameState.Instance.Set("is_reset", true);
        GameState.Instance.Set("minigame", name);

        SceneLoader.Instance.LoadScene(name);
    }

    void CheckResultBack()
    {

    }
}
