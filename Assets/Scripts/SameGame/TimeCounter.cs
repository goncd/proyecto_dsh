using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    public float CountedTime { get; private set; }

    private bool isStarted = false;

    TMP_Text timeText;

    void Start()
    {
        timeText = GetComponent<TMP_Text>();
    }

    public void StartCounter()
    {
        isStarted = true;
    }

    public void StopCounter()
    {
        isStarted = false;
    }

    public bool HasCounterStarted()
    {
        return isStarted;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
            return;

        CountedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(CountedTime / 60F);
        int seconds = Mathf.FloorToInt(CountedTime - minutes * 60F);

        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
