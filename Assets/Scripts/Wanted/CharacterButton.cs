using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Image characterImage; // Asigna la imagen en el inspector
    public int characterId; // Un ID para saber qué personaje es

    private GameController gameController;

    public Button characterButton;

    private RectTransform rectTransform; // Necesario para el efecto pop al tocar el personaje.

    // Inicializamos el rectTransform antes de que comience el juego.
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Función que prepara el botón con su id, su sprite y cargamos el controlador del juego.
    public void Setup(Sprite sprite, int id, GameController controller)
    {
        characterImage.sprite = sprite;
        characterId = id;
        gameController = controller;

        // Agregamos el evento al botón dinámicamente
        characterButton.onClick.RemoveAllListeners();
        characterButton.onClick.AddListener(OnClick);
    }


    public void OnClick()
    {
        StartCoroutine(EfectoPopPersonaje());
        gameController.CheckCharacter(characterId);
    }

    IEnumerator EfectoPopPersonaje()
    {
        Vector3 escalaOriginal = rectTransform.localScale;

        // Agrandamos el sprite un 20 por ciento.
        rectTransform.localScale = escalaOriginal * 1.2f;

        yield return new WaitForSeconds(0.1f);

        // Devolvemos el sprite a su tamaño original, generando dicho efecto pop.
        rectTransform.localScale = escalaOriginal;
    }
}
