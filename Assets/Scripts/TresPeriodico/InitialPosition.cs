using UnityEngine;

public class InitialPosition : MonoBehaviour
{
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector3 originalPosition;

    void Awake()
    {
        // Guardamos la posición y el padre original
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
    }

    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
    }
}