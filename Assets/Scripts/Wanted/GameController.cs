using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public RectTransform gridParent;                    // El panel donde aparecen los personajes
    public CharacterButton characterPrefab;             // Prefab del botón de personaje
    public List<Sprite> characterSprites;               // Sprites disponibles
    private int targetCharacterId;                      // ID del personaje objetivo
    public TextMeshProUGUI feedbackTxt;                 // Texto para indicar si hemos acertado, fallado, perdido o ganado.
    public Image imagenDelObjetivo;                     // Imagen que se pondrá arriba en el medio para indicarle al jugador el objetivo a buscar.
    public AudioClip AciertoSound, FalloSound;          // Clips de audio en caso de acierto o error del jugador.
    private AudioSource audioSource;                    // Para reproducir los sonidos.
    private int Puntuacion = 0;                         // Puntuación del juego.
    private int vidas = 3;                              // Vidas del juego.
    public TextMeshProUGUI puntuacionText, vidasText;   // Puntuación y vidas del juego, pero en texto.
    private bool isChecking = false;                    // Evitar clicks múltiples
    public GameObject gameOverPanel;                    // Panel que sale tras perder en el juego.
    private int objetosEnPantalla = 35;                 // Número de objetos que se generarán en pantalla.
    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        UpdatePuntuacion();
        UpdateVidas();
        GenerateCharacters(35);
    }

    // función que genera los personajes que queramos, con el número indicado por parámetro, separando 2 o 3 instancias del objeto que se está
    // buscando, añadiéndole más dificultad al juego.
    void GenerateCharacters(int total)
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Elegir personaje objetivo y su sprite
        targetCharacterId = Random.Range(0, characterSprites.Count);
        imagenDelObjetivo.sprite = characterSprites[targetCharacterId];

        // Elegimos 3 índices únicos donde irá el personaje correcto
        int numObjetivos = 3; 
        HashSet<int> objetivosIndices = new HashSet<int>();
        while (objetivosIndices.Count < numObjetivos)
        {
            objetivosIndices.Add(Random.Range(0, total));
        }

        for (int i = 0; i < total; i++)
        {
            CharacterButton newChar = Instantiate(characterPrefab, gridParent);

            int id;
            if (objetivosIndices.Contains(i))
            {
                id = targetCharacterId;
            }
            else
            {
                // Asegurarse de que NO elige el objetivo por accidente
                do
                {
                    id = Random.Range(0, characterSprites.Count);
                } while (id == targetCharacterId);
            }

            newChar.Setup(characterSprites[id], id, this);

            RectTransform charRect = newChar.GetComponent<RectTransform>();
            charRect.anchoredPosition = GetRandomPositionInside(gridParent);
        }
    }


    // Función con la que podemos obtener la posicion (x,y) donde poner un sprite dentro del panel negro.
    Vector2 GetRandomPositionInside(RectTransform parent)
    {
        float width = parent.rect.width;
        float height = parent.rect.height;

        float x = Random.Range(-width / 2f, width / 2f);
        float y = Random.Range(-height / 2f, height / 2f);

        return new Vector2(x, y);
    }

    // Función con la que podremos mostrar el mensaje de acierto o error con su color correspondiente.
    // Además, genera un nuevo nivel.
    void ShowFeedback(string message, Color color)
    {
        StopAllCoroutines(); // Por si se hace clic rápido varias veces
        StartCoroutine(ShowFeedbackCoroutine(message, color));
    }
    
    // Corrutina de la función anterior que hace dicho cometido.
    IEnumerator ShowFeedbackCoroutine(string message, Color color)
    {
        feedbackTxt.text = message;
        feedbackTxt.color = color;
        feedbackTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        feedbackTxt.gameObject.SetActive(false);

        // Liberamos el input para nuevos clicks.
        isChecking = false;

        Debug.Log("Objetos en pantalla: " + objetosEnPantalla);
        GenerateCharacters(objetosEnPantalla++);
    }

    // Función con la que podemos reproducir cualquier sonido.
    void PlaySound(AudioClip sonido)
    {
        if(sonido != null && audioSource != null)
            audioSource.PlayOneShot(sonido);
    }
    
    void UpdatePuntuacion()
    {
        puntuacionText.text = "Puntuación: " + Puntuacion;
    }

    void UpdateVidas()
    {
        vidasText.text = "Vidas: " + vidas;
    }
    
    void MostrarHasPerdido()
    {
        StopAllCoroutines();
        feedbackTxt.text = "¡Has perdido!";
        feedbackTxt.color = Color.red;
        feedbackTxt.gameObject.SetActive(true);
        isChecking = true; // Bloquear input para siempre

        gameOverPanel.SetActive(true);
    }

    private void AddPoints(int value)
    {
        GameState.Instance.Set("wanted_points", value);
    }

    // función con la que nos permite comprobar si el personaje, que el jugador ha clickeado, es el que se busca o no.
    public void CheckCharacter(int clickedId)
    {
        if (isChecking)
            return; // Ignorar clicks si ya estamos mostrando feedback

        isChecking = true; // Bloqueamos nuevos clicks

        if (clickedId == targetCharacterId)
        {
            PlaySound(AciertoSound);
            Puntuacion += 3;
            UpdatePuntuacion();
            ShowFeedback("¡Correcto!", Color.green);
        }
        else
        {
            PlaySound(FalloSound);
            Puntuacion -= 2;
            vidas -= 1;

            if (Puntuacion < 0) Puntuacion = 0;

            UpdatePuntuacion();
            UpdateVidas();

            if(vidas <= 0)
            {
                MostrarHasPerdido();
                AddPoints(Puntuacion);
                return;
            }
            else
                ShowFeedback("¡Incorrecto!", Color.red);
        }
    }

    public void ReintentarJuego()
    {
        vidas = 3;
        Puntuacion = 0;
        UpdateVidas();
        UpdatePuntuacion();
        feedbackTxt.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        GenerateCharacters(35);
    }

    public void SalirDelJuego()
    {
        SceneManager.LoadScene("GameSelector");
    }

}