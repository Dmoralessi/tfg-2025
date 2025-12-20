using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 position)
    {
        if (!agent.pathPending)
        {
            agent.SetDestination(position);
        }
    }

    public bool HasReachedDestination(float threshold = 0.3f)
    {
        if (!agent.pathPending)
        {
            return agent.remainingDistance <= threshold;
        }

        return false;
    }

    public void Stop()
    {
        agent.ResetPath();
    }
}
