using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastrarNumeros : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.root); // Para arrastrar por encima del resto
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        canvasGroup.blocksRaycasts = true;
    }
}
