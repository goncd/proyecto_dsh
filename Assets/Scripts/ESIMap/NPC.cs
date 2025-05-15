using UnityEngine;

public class NPC : MonoBehaviour
{
    public KeyCode useKey = KeyCode.E;
    public float interactDistance = 3f;

    public Dialogue dialogue;

    public int dialogueIndex;

    private Camera playerCamera;

    void Start()
    {
        // Look for the player camera (which must have the "MainCamera" tag).
        playerCamera = Camera.main;

        if (playerCamera == null)
            Debug.LogWarning("No se ha encontrado ninguna cámara con el tag 'MainCamera'.");
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (Input.GetKeyDown(useKey) && !dialogue.IsBeingShown() && IsLookingAtUs())
            dialogue.TriggerDialogue(dialogueIndex);
    }

    bool IsLookingAtUs()
    {
        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return hit.transform == transform;

        return false;
    }
}