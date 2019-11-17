using UnityEngine;

sealed public class PatrolPoint : MonoBehaviour
{
    public enum ReachedAction : byte
    {
        Continue = 0,
        Stop = 1,
    }

    [SerializeField] private bool rotateTowardsForward = true;
    [SerializeField] private ReachedAction action = ReachedAction.Continue;
    [SerializeField] private Vector2 stopTime = new Vector2(1f, 5f);

    public bool RotateTowardsForward { get => rotateTowardsForward; }
    public ReachedAction Action { get => action; }
    public float StopTime { get => Random.Range(stopTime.x, stopTime.y); }
}
