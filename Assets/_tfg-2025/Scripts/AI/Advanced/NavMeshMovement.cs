using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    public Transform[] patrolPoints;
    int currentPatrolIndex = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public bool HasReachedDestination()
    {
        if (agent.pathPending)
            return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.05f;
    }

    public void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (!agent.hasPath)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            return;
        }

        if (HasReachedDestination())
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public void ResumePatrol()
    {
        agent.isStopped = false;
        agent.ResetPath();
        //agent.SetDestination(firstDestination);
    }

    public Vector3 GetDesiredDirection()
    {
        if (agent.desiredVelocity.sqrMagnitude < 0.01f)
            return transform.forward;

        return agent.desiredVelocity.normalized;
    }

    public float GetStoppingDistance()
    {
        return agent.stoppingDistance;
    }

    public void SetStoppingDistance(float distance)
    {
        agent.stoppingDistance = distance;
    }
}
