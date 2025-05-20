using UnityEngine;
using UnityEngine.AI;

public class ChickenWalk : MonoBehaviour
{
    public Vector3 areaCenter;
    public Vector3 areaSize;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private float timer;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetNewDestination();
    }

    void Update()
    {
        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                SetNewDestination();
                timer = 0;
            }
        }
    }

    void SetNewDestination()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector3 randomPoint = areaCenter + new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                0,
                Random.Range(-areaSize.z / 2f, areaSize.z / 2f)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}