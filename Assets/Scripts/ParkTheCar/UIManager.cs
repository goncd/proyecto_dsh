using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] LinesDrawer linesDrawer;

    [Space]
    [SerializeField] private CanvasGroup availableLineCanvasGroup;
    [SerializeField] private GameObject availableLineHolder;
    [SerializeField] private Image availableLineFill;
    private bool isAvailableLineUIActive = false;

    [Space]
    [SerializeField] Image fadePanel;
    [SerializeField] float fadeDuration;

    private Route activeRoute;

    private void Start()
    {
        fadePanel.DOFade(0f, fadeDuration).From(1f);

        availableLineCanvasGroup.alpha = 0f;
        //availableLineCanvasGroup.DOFade(1f, 0.3f);

        linesDrawer.OnBeginDraw += OnBeginDrawHandler;
        linesDrawer.OnDraw += OnDrawHandler;
        linesDrawer.OnEndDraw += OnEndDrawHandler;
    }

    private void OnBeginDrawHandler(Route route)
    {
        activeRoute = route;

        availableLineFill.color = Color.white;
        availableLineFill.fillAmount = 1f;
        availableLineCanvasGroup.DOFade(1f, 0.3f);
        isAvailableLineUIActive = true;
    }

    private void OnDrawHandler()
    {
        if(isAvailableLineUIActive)
        {
            float maxLineLength = activeRoute.maxLineLength;
            float lineLength = activeRoute.line.lenght;

            availableLineFill.fillAmount = 1 - (lineLength / maxLineLength);
        }
    }

    private void OnEndDrawHandler()
    {
        if(isAvailableLineUIActive)
        {
            isAvailableLineUIActive = false;
            activeRoute = null;
            availableLineCanvasGroup.DOFade(0f, 0.3f);
        }
    }
}
