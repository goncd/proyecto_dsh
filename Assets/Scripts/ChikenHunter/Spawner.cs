using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    private int numberToSpawn = 15;

    [Header("Zona de spawn")]
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    [Header("Zona de entrega")]
    public Vector3 deliveryZoneCenter;
    public Vector3 deliveryZoneSize;
    public int chickensDelivered = 0;
    private int objectivePoints;
    private int points = 0;
    public TMP_Text chickensToBeDelivered;
    public Timer timer;
    private bool gameEnded = false;

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
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 randomPos = GetRandomPositionInArea();
            GameObject chicken = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);

            ChikenWalk walk = chicken.GetComponent<ChikenWalk>();
            if (walk != null)
            {
                walk.areaCenter = spawnAreaCenter;
                walk.areaSize = spawnAreaSize;
            }
        }

        if(GameState.Instance.Get("parkthecar_objective", out int pointsGoal))
        {
            objectivePoints = pointsGoal;
        }

        timer.StartTimer();

        gameOverRetryButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.ReloadCurrentScene(); });
        gameOverExitButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });

        pauseContinueButton.onClick.AddListener(() => { pauseCanvas.SetActive(false); Time.timeScale = 1f; });
        pauseRestartButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.ReloadCurrentScene(); });
        pauseExitButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });
        pauseButton.onClick.AddListener(() => { pauseCanvas.SetActive(true); Time.timeScale = 0f; });

        gameFinishedContinueButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });
    }

    void Update()
    {
        if (gameEnded) return;

        if (timer.TimeOver)
        {
            gameEnded = true;
            gameOverCanvas.SetActive(true);
        }

        if(chickensDelivered == numberToSpawn && !timer.TimeOver)
        {
            gameFinishedCanvas.SetActive(true);
            timer.StopTimer();
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float z = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
        float y = Terrain.activeTerrain.SampleHeight(spawnAreaCenter + new Vector3(x, 0, z));

        return spawnAreaCenter + new Vector3(x, y, z);
    }

    public bool IsInsideDeliveryZone(Vector3 position)
    {
        Vector3 min = deliveryZoneCenter - deliveryZoneSize / 2f;
        Vector3 max = deliveryZoneCenter + deliveryZoneSize / 2f;

        return position.x >= min.x && position.x <= max.x &&
            position.z >= min.z && position.z <= max.z;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(deliveryZoneCenter, deliveryZoneSize);
    }

    public void AddChickenDelivered()
    {
        chickensDelivered++;
        points += 30;
        GameState.Instance.Set("chickenhunter_points", points);

        if (chickensToBeDelivered != null)
            chickensToBeDelivered.text = $"Gallinas restantes \n {chickensDelivered} / {numberToSpawn}";
    }
}