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

    float stuckTimer = 0f;
    float maxStuckTime = 3f;
    float lastDistanceToWaypoint;
    Vector3 lastPosition;

    SimpleProfiler profiler = new SimpleProfiler();
    public float AvgDecisionMs => profiler.averageMs;

    private WaypointMovement movement;
    private ProximityPerception perception;

    void Awake()
    {
        movement = GetComponent<WaypointMovement>();
        perception = GetComponent<ProximityPerception>();
    }

    void Update()
    {
        profiler.Begin();

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

        profiler.End();
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
        
        if (!IsPlayerInRange())
        {
            stuckTimer = 0f;

            movement.FreezeCurrentWaypoint();
            lastDistanceToWaypoint = movement.GetDistanceToFrozenWaypoint();
            lastPosition = transform.position;

            currentState = NPCState.ReturnToPatrol;
        }
    }

    void ReturnState()
    {
        movement.MoveTowardsFrozenWaypoint();

        float currentDistance = movement.GetDistanceToFrozenWaypoint();

        if (currentDistance <= movement.reachDistance)
        {
            stuckTimer = 0f;
            currentState = NPCState.Patrol;
            return;
        }

        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        bool noProgress = currentDistance >= lastDistanceToWaypoint - 0.02f;
        bool barelyMoving = movedDistance < 0.01f;

        if (noProgress || barelyMoving)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
            lastDistanceToWaypoint = currentDistance;
            lastPosition = transform.position;
        }

        if (stuckTimer >= maxStuckTime)
        {
            movement.TeleportToFrozenWaypoint();
            stuckTimer = 0f;
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
