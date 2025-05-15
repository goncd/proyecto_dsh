using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Coordinator : MonoBehaviour
{
    public GameObject[] disableWhenPaused;

    public GameObject introCanvas;
    public GameObject creditsCanvas;

    public GameObject pauseCanvas;

    public Button startButton;

    public Button introCreditsButton;


    public Button creditsBackButton;

    public Button pauseContinueButton;

    public Button pauseRestartButton;

    public Transform playerCamera;

    public Transform playerRealPosition;

    private bool isUIActive = true;

    private bool wasUIAlreadyDisabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(OnStartButton);
        introCreditsButton.onClick.AddListener(OnIntroCreditsButton);

        creditsBackButton.onClick.AddListener(OnCreditsBackButton);

        pauseContinueButton.onClick.AddListener(OnPauseContinueButton);
        pauseRestartButton.onClick.AddListener(OnPauseRestartButton);

        StartCoroutine(FadeInCanvas(introCanvas, 1f));


        if (GameState.Instance.Get("player_transform", out Tuple<Vector3, Quaternion> oldPlayerTransform))
        {
            playerCamera.SetPositionAndRotation(oldPlayerTransform.Item1, oldPlayerTransform.Item2);
            introCanvas.SetActive(false);
        }
        else
            ToggleUI(false);
    }

    private void OnStartButton()
    {
        if (introCanvas.GetComponent<CanvasGroup>().alpha != 1f)
            return;

        StartCoroutine(TransitionToGameMode(playerCamera, playerRealPosition, 3f));
        StartCoroutine(FadeOutCanvas(introCanvas, 1.5f));
    }

    private void OnIntroCreditsButton()
    {
        introCanvas.SetActive(false);

        creditsCanvas.SetActive(true);
    }

    private void OnCreditsBackButton()
    {
        introCanvas.SetActive(true);

        creditsCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !introCanvas.activeInHierarchy && !creditsCanvas.activeInHierarchy)
        {
            pauseCanvas.SetActive(true);
            ToggleUI(false);
        }
    }

    public void ToggleUI(bool isActive)
    {
        if (!isUIActive && !isActive)
            wasUIAlreadyDisabled = true;

        isUIActive = isActive;

        foreach (GameObject gameObject in disableWhenPaused)
            gameObject.SetActive(isActive);

        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator FadeOutCanvas(GameObject canvas, float t)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha -= Time.deltaTime / t;
            yield return null;
        }

        canvas.SetActive(false);
    }

    IEnumerator FadeInCanvas(GameObject canvas, float t)
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

    IEnumerator TransitionToGameMode(Transform transform, Transform endTransform, float time)
    {
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

        // Ensure the final position and rotation are exactly as they should be
        transform.SetPositionAndRotation(endTransform.position, endTransform.rotation);
        ToggleUI(true);
    }

    private void OnPauseContinueButton()
    {
        pauseCanvas.SetActive(false);

        if (wasUIAlreadyDisabled)
            wasUIAlreadyDisabled = false;
        else
            ToggleUI(true);
    }

    private void OnPauseRestartButton()
    {
        Destroy(GameState.Instance);

        SceneLoader.Instance.ReloadCurrentScene();
    }
}
