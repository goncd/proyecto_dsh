using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform doorTransform;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public KeyCode openKey = KeyCode.E;
    public float interactDistance = 3f;

    private Camera playerCamera;
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = doorTransform.rotation;
        openRotation = Quaternion.Euler(doorTransform.eulerAngles + new Vector3(0, openAngle, 0));

        // Busca automáticamente la cámara del jugador (usa el tag "MainCamera")
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogWarning("No se encontró ninguna cámara con el tag 'MainCamera'.");
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        if (Input.GetKeyDown(openKey) && IsLookingAtDoor())
        {
            isOpen = !isOpen;
        }

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    bool IsLookingAtDoor()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return hit.transform == doorTransform;
        }
        return false;
    }
}