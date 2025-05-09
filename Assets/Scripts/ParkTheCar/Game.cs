using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance;
    [HideInInspector] public List<Route> readyRoutes = new();
    private int totalRoutes;
    private int successfulParks;
    public UnityAction<Route> OnCarEntersPark;
    public UnityAction OnCarCollision;
    private int points = 0;
    private int objectivePoints;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        totalRoutes = transform.GetComponentsInChildren<Route>().Length;
        successfulParks = 0;
        OnCarEntersPark += OnCarEntersParkHandler;
        OnCarCollision += OnCarCollisionHandler;

        if(GameState.Instance.Get("parkthecar_objective", out int pointsGoal))
        {
            objectivePoints = pointsGoal;
        }
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            GameState.Instance.Set("parkthecar_points", 0);
            points = 0;
        }
        else if (GameState.Instance.Get("parkthecar_points", out int savedPoints))
        {
            points = savedPoints;
        }

        gameOverRetryButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.ReloadCurrentScene(); });
        gameOverExitButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });

        pauseContinueButton.onClick.AddListener(() => { pauseCanvas.SetActive(false); Time.timeScale = 1f; });
        pauseRestartButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.ReloadCurrentScene(); });
        pauseExitButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });
        pauseButton.onClick.AddListener(() => { pauseCanvas.SetActive(true); Time.timeScale = 0f; });

        gameFinishedContinueButton.onClick.AddListener(() => { Time.timeScale = 1f; SceneLoader.Instance.LoadPreviousScene(); });
    }

    private void OnCarCollisionHandler()
    {
        DOVirtual.DelayedCall(2.5f, ()=> {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentLevel);
        });
    }

    private void OnCarEntersParkHandler(Route route)
    {
        route.car.StopDancingAnimation();
        successfulParks += 1;

        if(successfulParks == totalRoutes)
        {
            points += 100;
            GameState.Instance.Set("parkthecar_points", points);
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            DOVirtual.DelayedCall(1.3f, ()=> {
                if(nextLevel < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextLevel);
                }
                else
                {
                    gameFinishedCanvas.SetActive(true);
                    Time.timeScale = 0f;
                }
            });
        }
    }

    public void RegisterRoute(Route route)
    {
        readyRoutes.Add(route);

        if(readyRoutes.Count == totalRoutes)
        {
            MoveAllCars();
        }
    }

    private void MoveAllCars()
    {
        foreach(var route in readyRoutes)
        {
            route.car.Move(route.linePoints);
        }
    }
}
