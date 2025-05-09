using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System.Collections;
using System.Collections.Generic;
public class CuatroDigitos : MonoBehaviour
{
    public TextMeshProUGUI enunciadoText, ecuacionesText;
    public TMP_InputField inputField;
    public TextMeshProUGUI resultadoText;
    public GameObject gameOverPanel;
    public TMP_Text puntuacionText;

    public AudioClip esperaSound;
    public AudioClip correctoSound;
    public AudioClip incorrectoSound;
    public AudioClip musicaFondo;
    
    private AudioSource audioSource;

    private int Puntuacion = 100;

    // readonly, evidentemente, hace referencia a que este dato solo sea de lectura.
    private readonly string solucionCorrecta = "2368"; // A = 2, B = 3, C = 6, D = 8

    void Start()
    {
        MostrarEnunciado();
        resultadoText.text = "";
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = musicaFondo;
        puntuacionText.text = "Puntuación: " + Puntuacion;
        audioSource.Play();
    }

    void MostrarEnunciado()
    {
        enunciadoText.text = "Este es el problema de los cuatro dígitos.\nA, B, C y D son cifras" +
        " de un solo dígito. Todas ellas forman parte de las ecuaciones que estás viendo en pantalla.\n" +
        "Encuentra los valores de A, B, C y D. Indica la solución como una cifra de 4 dígitos: ABCD.\n" +
        "¡¡RECUERDA!! Tienes 100 puntos al principio, si fallas, pierdes 10 puntos. Así que, piensa antes de actuar!!!";

        ecuacionesText.text =
            "A + C = D\n" +
            "A x B = C\n" +
            "C - B = B\n" +
            "A x 4 = D\n\n";
    }

    public void ComprobarRespuesta()
    {
        string input = inputField.text.Trim();
        StartCoroutine(ProcesarRespuesta(input));
    }

    IEnumerator ProcesarRespuesta(string input)
    {
        // Parar música de fondo y reproducir música de espera
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = esperaSound;
        audioSource.Play();

        resultadoText.text = "Comprobando...";
        resultadoText.color = Color.yellow;

        yield return new WaitForSeconds(3f);

        // Paramos la música de espera antes de reproducir el resultado
        audioSource.Stop();

        if (input == solucionCorrecta)
        {
            audioSource.PlayOneShot(correctoSound);
            resultadoText.text = "¡Correcto!";
            resultadoText.color = Color.green;

            // Añadimos la puntuación por haber superado el juego a la puntuación global.
            AddPoints(Puntuacion);

            // Activamos el panel de Game Over.
            gameOverPanel.SetActive(true);
        }
        else
        {
            audioSource.PlayOneShot(incorrectoSound);
            resultadoText.text = "Incorrecto. Intenta de nuevo.";
            resultadoText.color = Color.red;
            
            if(Puntuacion > 0)
                Puntuacion -= 10;

            yield return new WaitForSeconds(1f);
            gameOverPanel.SetActive(true);
        }
    }

    public void ReintentarJuego()
    {
        gameOverPanel.SetActive(false);
        Start();
    }

    private void AddPoints(int value)
    {
        GameState.Instance.Set("cuatrodigitos_points", value);
    }

    public void SalirDelJuego()
    {
        SceneLoader.Instance.LoadPreviousScene();
    }
}
