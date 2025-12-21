using UnityEngine;

public class BasicNPCController : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Chase,
        ReturnToPatrol
    }

    public NPCState currentState = NPCState.Patrol;

    private WaypointMovement movement;
    private ProximityPerception perception;

    void Awake()
    {
        movement = GetComponent<WaypointMovement>();
        perception = GetComponent<ProximityPerception>();
    }

    void Update()
    {
        if (movement == null || perception == null)
            return;
        
        switch (currentState)
        {
            case NPCState.Patrol:
                PatrolState();
                break;
            
            case NPCState.Chase:
                ChaseState();
                break;
            
            case NPCState.ReturnToPatrol:
                ReturnState();
                break;
        }
    }

    void PatrolState()
    {
        movement.MoveAlongWaypoints();

        if (IsPlayerInRange())
        {
            currentState = NPCState.Chase;
        }
    }

    void ChaseState()
    {
        Vector3 targetPos = perception.GetTargetPosition();
        movement.MoveTowards(targetPos);
        FaceTarget(targetPos);

        if(!IsPlayerInRange())
        {
            currentState = NPCState.ReturnToPatrol;
        }
    }

    void ReturnState()
    {
        movement.MoveAlongWaypoints();

        if(movement.IsNearWaypoint())
        {
            currentState = NPCState.Patrol;
        }
    }

    bool IsPlayerInRange()
    {
        return perception != null && perception.IsTargetDetected();
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
