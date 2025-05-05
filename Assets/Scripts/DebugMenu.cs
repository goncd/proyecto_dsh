using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    public Button arkanoidLoad;
    public TMP_Text arkanoidPoints;

    public Button sameGameLoad;
    public TMP_Text sameGamePoints;

    public Button wantedLoad;
    public TMP_Text wantedPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arkanoidLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Arkanoid"));
        sameGameLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("SameGame"));
        wantedLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Wanted"));

        GameState.Instance.Set("arkanoid_objective", 400);
        GameState.Instance.Set("samegame_objective", 500);

        if (GameState.Instance.Get("arkanoid_points", out int arkanoid_points))
            arkanoidPoints.text = $"Puntos: {arkanoid_points}";

        if (GameState.Instance.Get("samegame_points", out int samegame_points))
            sameGamePoints.text = $"Puntos: {samegame_points}";

        if (GameState.Instance.Get("wanted_points", out int wanted_points))
            wantedPoints.text = $"Puntos: {wanted_points}";
    }
}
