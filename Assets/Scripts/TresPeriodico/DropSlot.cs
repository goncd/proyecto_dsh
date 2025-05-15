using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragged = eventData.pointerDrag;

        if (dragged != null)
        {
            ArrastrarNumeros arrastrar = dragged.GetComponent<ArrastrarNumeros>();

            if (arrastrar != null)
            {
                if (transform.childCount > 0)
                {
                    Transform hijoExistente = transform.GetChild(0);
                    hijoExistente.SetParent(arrastrar.parentToReturnTo);
                }

                // Asignar el nuevo padre al slot actual
                arrastrar.parentToReturnTo = this.transform;
            }
        }
    }
}
