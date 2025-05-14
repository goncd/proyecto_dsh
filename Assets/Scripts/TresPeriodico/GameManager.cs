using UnityEngine;
using System.Collections;
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

    private Transform lastParent;

    void Start()
    {
        MostrarEnunciado();
        resultText.text = "";
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = musicaFondo;
        puntuacionText.text = "Puntuación: " + Puntuacion;
        audioSource.Play();
    }

    void MostrarEnunciado()
    {
        enunciadoText.text = "Este es el problema llamado \"33333\", creo que ya sabes porque se llama así." +
        " Pues bueno, ¿ves los números que tienes arriba del 1 al 9? Rellena las casillas con esos números. 5 de " +
        "esos números son para el minuendo, los cuatro restantes para el sustraendo. Completa la ecuación para que dé dicho resultado.\n" +
        "Piensa bien el resultado, si fallas, los puntos se restarán.";
    }

    public void Verify()
    {
        string minuendStr = GetNumberFromSlots(minuendSlots);
        string subtrahendStr = GetNumberFromSlots(subtrahendSlots);

        StartCoroutine(ComprobarSolucion(minuendStr, subtrahendStr));
    }

    IEnumerator ComprobarSolucion(string minuendo, string sustraendo)
    {
        // Parar música de fondo y reproducir música de espera
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = esperaSound;
        audioSource.Play();

        resultText.text = "Comprobando...";
        resultText.color = Color.yellow;

        yield return new WaitForSeconds(3f);
        
        // Paramos la música de espera antes de reproducir el resultado
        audioSource.Stop();

        if (minuendo.Length != 5 || sustraendo.Length != 4)
        {
            resultText.text = "Debes usar 5 dígitos en el minuendo y 4 en el sustraendo.";
        }
        else
        {
            int minuend = int.Parse(minuendo);
            int subtrahend = int.Parse(sustraendo);

            if (minuend - subtrahend == 33333)
            {
                audioSource.PlayOneShot(correctoSound);
                resultText.text = "¡Correcto!";
                resultText.color = Color.green;

                // Añadimos la puntuación por haber superado el juego a la puntuación global.
                AddPoints(Puntuacion);

                // Activamos el panel de Game Over.
                gameOverPanel.SetActive(true);
            }
            else
            {
                audioSource.PlayOneShot(incorrectoSound);
                resultText.text = "Incorrecto. Intenta de nuevo.";
                resultText.color = Color.red;
                
                if(Puntuacion > 0)
                    Puntuacion -= 10;

                yield return new WaitForSeconds(1f);
                gameOverPanel.SetActive(true);
            }
        }
    }

    private string GetNumberFromSlots(Transform parent)
    {
        string result = "";

        foreach (Transform slot in parent)
        {
            if (slot.childCount > 0)
            {
                TextMeshProUGUI digitText = slot.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

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

    private void ResetAllDigits()
    {
        InitialPosition[] digits = digitsPanel.GetComponentsInChildren<InitialPosition>(true); // incluye ocultos

        foreach (InitialPosition digit in digits)
        {
            digit.ResetPosition();
        }

        // También revisamos los slots, por si algún número quedó como hijo de ellos
        foreach (Transform slot in minuendSlots)
        {
            if (slot.childCount > 0)
            {
                InitialPosition digit = slot.GetChild(0).GetComponent<InitialPosition>();
                if (digit != null) digit.ResetPosition();
            }
        }

        foreach (Transform slot in subtrahendSlots)
        {
            if (slot.childCount > 0)
            {
                InitialPosition digit = slot.GetChild(0).GetComponent<InitialPosition>();
                if (digit != null) digit.ResetPosition();
            }
        }
    }


    public void ReintentarJuego()
    {
        ResetAllDigits();
        gameOverPanel.SetActive(false);
        Start();
    }

    public void PausarJuego()
    {
        audioSource.Pause();
        pauseMenu.SetActive(true);
    }

    public void ContinuarJuego()
    {
        audioSource.Play();
        pauseMenu.SetActive(false);
    }

    private void AddPoints(int value)
    {
        GameState.Instance.Set("tresperiodico_points", value);
    }

    public void SalirDelJuego()
    {
        SceneLoader.Instance.LoadPreviousScene();
    }
}
