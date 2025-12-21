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

    public Vector3 GetDesiredDirection()
    {
        if (agent == null)
            return transform.forward;

        if (agent.desiredVelocity.sqrMagnitude < 0.01f)
            return transform.forward;

        return agent.desiredVelocity.normalized;
    }
}
