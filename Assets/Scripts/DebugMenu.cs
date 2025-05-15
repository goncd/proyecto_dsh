using TMPro;
using UnityEditor.SearchService;
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

    public Button parkTheCarLoad;
    public TMP_Text parkTheCarPoints;

    public Button cuatroDigitosLoad;
    public TMP_Text cuatroDigitosPoints;

    public Button chickenHunterLoad;
    public TMP_Text chickenHunterPoints;

    public Button tresPeriodicoLoad;
    public TMP_Text tresPeriodicoPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arkanoidLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Arkanoid"));
        sameGameLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("SameGame"));
        wantedLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Wanted"));
        cuatroDigitosLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("CuatroDigitos"));
        parkTheCarLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("Scenes/ParkTheCar/Level 1"));
        chickenHunterLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("ChickenHunter"));
        tresPeriodicoLoad.onClick.AddListener(() => SceneLoader.Instance.LoadScene("TresPeriodico"));

        GameState.Instance.Set("arkanoid_objective", 400);
        GameState.Instance.Set("samegame_objective", 500);
        GameState.Instance.Set("parkthecar_objective", 400);
        GameState.Instance.Set("chickenhunter_objective", 450);

        if (GameState.Instance.Get("arkanoid_points", out int arkanoid_points))
            arkanoidPoints.text = $"Puntos: {arkanoid_points}";

        if (GameState.Instance.Get("samegame_points", out int samegame_points))
            sameGamePoints.text = $"Puntos: {samegame_points}";

        if (GameState.Instance.Get("wanted_points", out int wanted_points))
            wantedPoints.text = $"Puntos: {wanted_points}";

        if(GameState.Instance.Get("cuatrodigitos_points", out int cuatrodigitos_points))
            cuatroDigitosPoints.text = $"Puntos: {cuatrodigitos_points}";

        if(GameState.Instance.Get("parkthecar_points", out int parkthecar_points))
            parkTheCarPoints.text = $"Puntos: {parkthecar_points}";
        
        if(GameState.Instance.Get("chickenhunter_points", out int chickenhunter_points))
            chickenHunterPoints.text = $"Puntos: {chickenhunter_points}";

        if(GameState.Instance.Get("tresperiodico_points", out int tresperiodico_points))
            tresPeriodicoPoints.text = $"Puntos: {tresperiodico_points}";
    }
}
