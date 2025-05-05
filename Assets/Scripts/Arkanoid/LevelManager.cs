using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public int columns = 8;
    public int rows = 5;
    public float spacing = 1.5f;
    public Vector3 startPosition = new Vector3(-5.25f, 12f, 0.15f);

    public TMP_Text pointsText;
    public TMP_Text healthText;
    private int points = 0;
    private int objectivePoints;
    private int currentHealth = 3;

    public GameObject gameOverCanvas;
    public Button gameOverRetryButton;
    public Button gameOverExitButton;

    public GameObject pauseCanvas;
    public Button pauseContinueButton;
    public Button pauseRestartButton;
    public Button pauseExitButton;
    public Button pauseButton;

    public GameObject gameFinishedCanvas;
    public Button gameFinishedContinueButton;

    void Start()
    {
        Color[] rowColors = new Color[]
        {
            Color.green,
            new Color(1f, 0.4f, 0.7f),
            Color.blue,
            Color.yellow,
            Color.red
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = startPosition + new Vector3(col * 1.6f, row * 1.1f, -0.4f);
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);

                Renderer rend = block.GetComponent<Renderer>();
                if (rend != null && row < rowColors.Length)
                {
                    rend.material.color = rowColors[row];
                }
            }
        }

        if(GameState.Instance.Get("arkanoid_objective", out int pointsGoal))
        {
            objectivePoints = pointsGoal;
        }

        gameOverRetryButton.onClick.AddListener(() => SceneLoader.Instance.ReloadCurrentScene());
        gameOverExitButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());

        pauseContinueButton.onClick.AddListener(() => { pauseCanvas.SetActive(false); Time.timeScale = 1f; });
        pauseRestartButton.onClick.AddListener(() => SceneLoader.Instance.ReloadCurrentScene());
        pauseExitButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());
        pauseButton.onClick.AddListener(() => { pauseCanvas.SetActive(true); Time.timeScale = 0f; });

        gameFinishedContinueButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());
    }

    public void AddPoints(int value)
    {
        points += value;
        GameState.Instance.Set("arkanoid_points", points);

        pointsText.text = $"Puntos: {points}";

        if(points == objectivePoints)
        {
            gameFinishedCanvas.SetActive(true);
        }
    }

    public void ReduceHealth()
    {
        currentHealth -= 1;
        healthText.text = $"Vidas: {currentHealth}";

        if(currentHealth <= 0)
        {
            gameOverCanvas.SetActive(true);
        }
    }
}