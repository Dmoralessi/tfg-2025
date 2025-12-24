using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WaypointMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float reachDistance = 0.25f;
    public float stopDistance = 1.5f;

    int frozenIndex = -1;

    private int currentIndex = 0;
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void MoveAlongWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentIndex];

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < reachDistance)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            return;
        }

        MoveInDirection(toTarget);
    }

    public void MoveTowards(Vector3 worldPosition)
    {
        Vector3 toTarget = worldPosition - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= stopDistance)
            return;

        MoveInDirection(toTarget);
    }

    public void MoveInDirection(Vector3 direction)
    {
        Vector3 dir = direction.normalized;

        controller.Move(dir * moveSpeed * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void FreezeCurrentWaypoint()
    {
        frozenIndex = currentIndex;
    }

    public void MoveTowardsFrozenWaypoint()
    {
        if (frozenIndex < 0 || waypoints == null || waypoints.Length == 0)
            return;

        Vector3 toTarget = waypoints[frozenIndex].position - transform.position;
        toTarget.y = 0f;

        MoveInDirection(toTarget);
    }

    public float GetDistanceToFrozenWaypoint()
    {
        if (frozenIndex < 0)
            return 0f;

        Vector3 a = transform.position;
        Vector3 b = waypoints[frozenIndex].position;

        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    public void TeleportToFrozenWaypoint()
    {
        if (frozenIndex < 0)
            return;

        controller.enabled = false;

        transform.position = waypoints[frozenIndex].position;

        controller.enabled = true;

        currentIndex = frozenIndex;
        frozenIndex = -1;
    }

    public Vector3 GetCurrentNavigationTarget()
    {
        if (waypoints == null || waypoints.Length == 0)
            return transform.position;

        return waypoints[currentIndex].position;
    }
}
