using UnityEngine;

public class AdvancedNPCController : MonoBehaviour
{
    public enum NPCState
    {
        Idle,
        Chase
    }

    public NPCState currentState = NPCState.Idle;

    private NavMeshMovement movement;
    private VisionPerception perception;

    void Awake()
    {
        movement = GetComponent<NavMeshMovement>();
        perception = GetComponent<VisionPerception>();
    }

    void Update()
    {
        if (movement == null || perception == null)
            return;

        switch (currentState)
        {
            case NPCState.Idle:
                IdleState();
                break;

            case NPCState.Chase:
                ChaseState();
                break;
        }
    }

    void IdleState()
    {
        movement.Stop();

        transform.Rotate(Vector3.up, 60f * Time.deltaTime);

        if (perception.CanSeeTargetWithForward(GetPerceptionForward()))
        {
            currentState = NPCState.Chase;
        }
    }

    void ChaseState()
    {
        Vector3 targetPos = perception.GetTargetPosition();
        movement.MoveTo(targetPos);
        FaceTarget(targetPos);

        if (!perception.CanSeeTargetWhileChasing(GetPerceptionForward()))
        {
            currentState = NPCState.Idle;
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
}
