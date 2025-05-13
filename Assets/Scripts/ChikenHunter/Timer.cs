using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 25f;
    private bool isRunning = false;
    private TMP_Text timeText;

    public bool TimeOver { get; private set; } = false;

    void Start()
    {
        timeText = GetComponent<TMP_Text>();
        UpdateTimeDisplay();
    }

    public void StartTimer()
    {
        isRunning = true;
        TimeOver = false;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public bool HasStarted()
    {
        return isRunning;
    }

    void Update()
    {
        if (!isRunning || TimeOver)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            TimeOver = true;
        }

        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
