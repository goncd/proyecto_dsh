using TMPro;
using UnityEngine;


public class HealthPointsCounter : MonoBehaviour
{
    private TMP_Text healthText;

    private int healthPoints = 0;

    public int HealthPoints
    {
        get { return healthPoints; }
        set
        {
            healthPoints = value;
            healthText.text = $"Health points: {healthPoints}";
        }
    }

    void Start()
    {
        healthText = GetComponent<TMP_Text>();
    }

}
