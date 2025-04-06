using TMPro;
using UnityEngine;


public class PointsCounter : MonoBehaviour
{
    private TMP_Text timeText;

    private int points = 0;

    public int Points
    {
        get { return points; }
        set
        {
            points = value;
            timeText.text = $"Points: {points}";
        }
    }

    void Start()
    {
        timeText = GetComponent<TMP_Text>();
    }

}
