using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WaypointMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float reachDistance = 0.2f;

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

        MoveInDirection(toTarget);
    }

    public bool IsNearWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return true;

        Vector3 toTarget = waypoints[currentIndex].position - transform.position;
        toTarget.y = 0f;

        return toTarget.magnitude < reachDistance;
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
}
