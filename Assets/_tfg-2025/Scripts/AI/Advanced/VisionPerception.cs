using UnityEngine;

public class VisionPerception : MonoBehaviour
{
    public Transform target;
    public float viewDistance = 4f;
    public float viewAngle = 90f;
    public float chaseViewAngleMultiplier = 1.5f;
    public LayerMask obstructionMask;

    public bool CanSeeTargetWithForward(Vector3 forward)
    {
        return CanSeeTargetInternal(forward, 1f);
    }

    public bool CanSeeTargetWhileChasing(Vector3 forward)
    {
        return CanSeeTargetInternal(forward, chaseViewAngleMultiplier);
    }

    bool CanSeeTargetInternal(Vector3 forward, float angleMultiplier)
    {
        if (target == null)
            return false;

        Vector3 origin = GetCenter(transform);
        Vector3 targetPoint = GetCenter(target);

        Vector3 toTarget = targetPoint - origin;
        float distance = toTarget.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(forward, toTarget);
        if (angle > viewAngle * angleMultiplier * 0.5f)
            return false;

        if (Physics.Raycast(origin, toTarget.normalized, distance, obstructionMask))
            return false;

        return true;
    }

    Vector3 GetCenter(Transform t)
    {
        Collider col = t.GetComponent<Collider>();
        if (col != null)
            return col.bounds.center;

        return t.position;
    }

    public Vector3 GetTargetPosition()
    {
        if (target == null)
            return transform.position;

        return target.position;
    }
}
