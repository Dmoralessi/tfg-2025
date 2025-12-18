using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WaypointPatrol : MonoBehaviour
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

    void Update()
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

        Vector3 direction = toTarget.normalized;
        controller.Move(direction * moveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
