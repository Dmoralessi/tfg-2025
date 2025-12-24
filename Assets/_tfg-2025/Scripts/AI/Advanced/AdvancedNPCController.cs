using UnityEngine;

public class AdvancedNPCController : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Chase,
        Investigate
    }

    public NPCState currentState = NPCState.Patrol;

    Vector3 lastSeenPosition;
    float investigateTimer = 0f;
    int investigatePhase = 0;
    float investigatePhaseDuration = 0.8f;
    float reacquireGraceTime = 0.5f;
    float lastReacquireTime = -Mathf.Infinity;

    SimpleProfiler profiler = new SimpleProfiler();
    public float AvgDecisionMs => profiler.averageMs;

    private NavMeshMovement movement;
    private VisionPerception perception;

    void Awake()
    {
        movement = GetComponent<NavMeshMovement>();
        perception = GetComponent<VisionPerception>();
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

            case NPCState.Investigate:
                InvestigateState();
                break;
        }

        profiler.End();
    }

    void PatrolState()
    {
        movement.SetStoppingDistance(0.05f);
        movement.Patrol();

        if (perception.CanSeeTargetWithForward(GetPerceptionForward()))
        {
            movement.Stop();
            currentState = NPCState.Chase;
        }
    }

    void ChaseState()
    {
        movement.SetStoppingDistance(1.5f);
        Vector3 targetPos = perception.GetTargetPosition();
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > movement.GetStoppingDistance())
            movement.MoveTo(targetPos);
        else
            movement.Stop();

        FaceTarget(targetPos);
        lastSeenPosition = targetPos;

        bool recentlyReacquired = Time.time - lastReacquireTime < reacquireGraceTime;
        bool facingTarget = IsFacingTarget(targetPos);

        if (!perception.CanSeeTargetWhileChasing(GetPerceptionForward())
                && !recentlyReacquired
                && facingTarget)
        {
            investigateTimer = 0f;
            investigatePhase = 0;
            currentState = NPCState.Investigate;
        }
    }

    void InvestigateState()
    {
        if (investigatePhase == 0)
        {
            movement.MoveTo(lastSeenPosition);
            FaceTarget(lastSeenPosition);

            if (perception.CanSeeTargetWithForward(GetPerceptionForward()))
            {
                lastReacquireTime = Time.time;
                currentState = NPCState.Chase;
                return;
            }

            if (movement.HasReachedDestination())
            {
                movement.Stop();
                investigatePhase = 1;
                investigateTimer = 0f;
            }
            return;
        }

        investigateTimer += Time.deltaTime;

        if (investigatePhase == 1)
        {
            RotateInPlace(-1f);
        }
        else if (investigatePhase == 2)
        {
            RotateInPlace(1f);
        }
        else if (investigatePhase == 3)
        {
            RotateInPlace(1f);
        }

        if (investigateTimer >= investigatePhaseDuration)
        {
            investigateTimer = 0f;
            investigatePhase++;
        }

        if (perception.CanSeeTargetWithForward(GetPerceptionForward()))
        {
            lastReacquireTime = Time.time;
            currentState = NPCState.Chase;
            return;
        }

        if (investigatePhase > 3)
        {
            movement.Stop();
            movement.ResumePatrol();
            currentState = NPCState.Patrol;
        }
    }

    Vector3 GetPerceptionForward()
    {
        if (movement == null)
            return transform.forward;

        Vector3 desired = movement.GetDesiredDirection();
        if (desired != Vector3.zero)
        {
            return desired;
        }
        else
        {
            return transform.forward;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            360f * Time.deltaTime
        );
    }

    void RotateInPlace(float direction)
    {
        transform.Rotate(Vector3.up, direction * 120f * Time.deltaTime);
    }

    bool IsFacingTarget(Vector3 targetPos, float threshold = 25f)
    {
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.001f)
            return true;

        return Vector3.Angle(transform.forward, toTarget) <= threshold;
    }
}
