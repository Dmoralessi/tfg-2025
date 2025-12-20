using UnityEngine;

public class ProximityPerception : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 4f;

    public bool IsTargetDetected()
    {
        if (target == null)
            return false;

        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= detectionRadius;
    }

    public Vector3 GetTargetPosition()
    {
        if (target == null)
            return transform.position;

        return target.position;
    }
}
