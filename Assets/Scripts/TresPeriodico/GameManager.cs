using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform digitsPanel;                   // Panel que contiene los dígitos 1-9
    public Transform minuendSlots;                  // Panel con 5 slots
    public Transform subtrahendSlots;               // Panel con 4 slots
    public TMP_Text resultText;                     // Texto que muestra el resultado.
    public TMP_Text enunciadoText;                  // Enunciado del problema.
    public GameObject gameOverPanel;
    public TMP_Text puntuacionText;
    public AudioClip esperaSound;
    public AudioClip correctoSound;
    public AudioClip incorrectoSound;
    public AudioClip musicaFondo;
    
    private AudioSource audioSource;

    private int Puntuacion = 100;

    public GameObject pauseMenu;

    void Start()
    {
        MostrarEnunciado();
    }

    void MostrarEnunciado()
    {
        enunciadoText.text = "Este es el problema llamado \"33333\", creo que ya sabes porque se llama así." +
        " Pues bueno, ¿ves los números que tienes arriba del 1 al 9? Rellena las casillas con esos números. 5 de " +
        " esos números son para el minuendo, los cuatro restantes para el sustraendo. Completa la ecuación para que dé dicho resultado.";
    }

    public void Verify()
    {
        string minuendStr = GetNumberFromSlots(minuendSlots);
        string subtrahendStr = GetNumberFromSlots(subtrahendSlots);

        if (minuendStr.Length != 5 || subtrahendStr.Length != 4)
        {
            resultText.text = "Debes usar 5 dígitos en el minuendo y 4 en el sustraendo.";
            return;
        }

        int minuend = int.Parse(minuendStr);
        int subtrahend = int.Parse(subtrahendStr);

        if (minuend - subtrahend == 33333)
        {
            resultText.text = "¡Correcto!";
        }
        else
        {
            resultText.text = "Incorrecto, inténtalo de nuevo.";
        }
    }


    private string GetNumberFromSlots(Transform parent)
    {
        string result = "";

        foreach (Transform slot in parent)
        {
            if (slot.childCount > 0)
            {
                TextMeshProUGUI digitText = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (digitText != null)
                {
                    result += digitText.text;
                }
                else
                {
                    Debug.LogWarning("No se encontró TMP_Text en: " + slot.name);
                }
                
            }
        }

        Debug.Log("Resultado: " + result);
        return result;
    }
}
