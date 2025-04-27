using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        UpdatePuntuacion();
        UpdateVidas();
        GenerateCharacters(35);
    }

    // función que genera los personajes que queramos, con el número indicado por parámetro.
    void GenerateCharacters(int total)
    {
        //Debug.Log("Generando " + total + " personajes");

        // Limpiar personajes anteriores
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Elegir personaje objetivo y ponemos la imagen del mismo.
        targetCharacterId = Random.Range(0, characterSprites.Count);
        imagenDelObjetivo.sprite = characterSprites[targetCharacterId];
        int targetIndex = Random.Range(0, total);

        for (int i = 0; i < total; i++)
        {
            CharacterButton newChar = Instantiate(characterPrefab, gridParent);

            int id = (i == targetIndex) ? targetCharacterId : Random.Range(0, characterSprites.Count);
            newChar.Setup(characterSprites[id], id, this);

            // Posición aleatoria dentro del panel
            RectTransform charRect = newChar.GetComponent<RectTransform>();
            charRect.anchoredPosition = GetRandomPositionInside(gridParent);

            //Debug.Log("Personaje creado con sprite ID: " + id);
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

        GenerateCharacters(35);
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
                MostrarHasPerdido();
            else
                ShowFeedback("¡Incorrecto!", Color.red);
        }
    }
}
