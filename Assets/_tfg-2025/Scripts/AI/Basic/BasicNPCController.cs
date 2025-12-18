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

    public Transform player;
    public float detectionRadius = 4f;
    
    private WaypointMovement movement;

    void Awake()
    {
        movement = GetComponent<WaypointMovement>();
    }

    void Update()
    {
        if (movement == null || player == null)
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
        movement.MoveTowards(player.position);

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
        float distance = Vector3.Distance(transform.position, player.position);
        return distance < detectionRadius;
    }
}
