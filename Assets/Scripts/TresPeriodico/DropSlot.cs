using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            ArrastrarNumeros d = eventData.pointerDrag.GetComponent<ArrastrarNumeros>();
            if (d != null)
            {
                d.parentToReturnTo = this.transform;
            }
        }
    }
}
