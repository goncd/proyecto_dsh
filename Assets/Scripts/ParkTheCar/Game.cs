using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    [HideInInspector] public List<Route> readyRoutes = new();
    private int totalRoutes;
    private int successfulParks;
    public UnityAction<Route> OnCarEntersPark;
    public UnityAction OnCarCollision;

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
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            DOVirtual.DelayedCall(1.3f, ()=> {
                if(nextLevel < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextLevel);
                }
                else
                {
                    Debug.LogWarning("No next level to load");
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
