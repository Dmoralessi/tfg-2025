using UnityEngine;

public class AdvancedNPCTest : MonoBehaviour
{
    public Transform target;
    private NavMeshMovement movement;

    void Awake()
    {
        movement = GetComponent<NavMeshMovement>();
    }

    void Update()
    {
        if (target != null)
        {
            movement.MoveTo(target.position);
        }
    }
}
