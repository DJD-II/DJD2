using UnityEngine;

sealed public class PatrolArea : MonoBehaviour
{
    public PatrolPoint[] PatrolPoints { get; private set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform t in transform)
            Gizmos.DrawLine(transform.position, t.position);
    }

    private void Awake()
    {
        PatrolPoints = GetComponentsInChildren<PatrolPoint>();
    }
}
