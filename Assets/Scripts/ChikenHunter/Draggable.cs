using UnityEngine;
using UnityEngine.AI;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private float distanceToCamera;
    private Spawner spawner;
    private bool hasBeenDelivered = false;

    void Start()
    {
        spawner = FindFirstObjectByType<Spawner>();
    }

    void OnMouseDown()
    {
        if (!hasBeenDelivered)
        {
            distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
            isDragging = true;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (!hasBeenDelivered && spawner.IsInsideDeliveryZone(transform.position))
        {
            hasBeenDelivered = true;

            var walk = GetComponent<ChikenWalk>();
            if (walk != null)
            {
                walk.areaCenter = spawner.deliveryZoneCenter;
                walk.areaSize = spawner.deliveryZoneSize;

                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.ResetPath();
                    agent.isStopped = false;
                }

                walk.enabled = true;
            }

            spawner.AddChickenDelivered();
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 newPosition = hit.point;
                newPosition.y = Terrain.activeTerrain.SampleHeight(newPosition);
                transform.position = newPosition;
            }
        }
    }
}