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
        movement.MoveTowards(perception.GetTargetPosition());

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
}
