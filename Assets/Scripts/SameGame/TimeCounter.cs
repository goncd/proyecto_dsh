using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    private float time;

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

    public bool HasCounterStarted()
    {
        return isStarted;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
            return;

        time += Time.deltaTime;

        timeText.text = GetText();
    }

    public string GetText()
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60F);

        return $"{minutes:00}:{seconds:00}";
    }
}
