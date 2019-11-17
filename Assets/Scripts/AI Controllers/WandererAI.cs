using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
sealed public class WandererAI : AgentAI
{
    public delegate void EventHandler(WandererAI sender);

    public event EventHandler OnWaypointChanged;

    public enum LoopMode : byte
    {
        Single = 0,
        Loop = 1,
        PingPong = 2,
        Random = 3,
    }

    [Header("Area")]
    [Tooltip("The Area where the wanderer should move around.")]
    [SerializeField] private PatrolArea patrolArea = null;
    [Header("Wander Mode")]
    [SerializeField] private bool startAtRandomWaypoint = true;
    [SerializeField] private LoopMode playMode = LoopMode.PingPong;
    private bool odd = false;
    private int waypointIndex = 0;
    private Coroutine waiting = null;

    public PatrolPoint Waypoint { get => patrolArea.PatrolPoints[waypointIndex]; }

    protected override void Awake()
    {
        base.Awake();

        OnReachedDestination += OnDestinationReached;

        if (patrolArea == null)
            return;

        OnWaypointChanged += OnWaypointHasChanged;
    }

    private void Start()
    {
        if (startAtRandomWaypoint)
            waypointIndex = Random.Range(0, patrolArea.PatrolPoints.Length);

        OnWaypointChanged?.Invoke(this);
    }

    protected override void Update()
    {
        if (patrolArea == null)
            return;

        if (waiting == null)
            base.Update();
    }

    private void OnDestroy()
    {
        OnWaypointChanged -= OnWaypointHasChanged;
    }

    private void OnDestinationReached(AgentAI sender)
    {
        switch (Waypoint.Action)
        {
            case PatrolPoint.ReachedAction.Stop:
                waiting = StartCoroutine(
                          WaitForNextMove(Waypoint.StopTime));
                break;

            case PatrolPoint.ReachedAction.Continue:
                GetNextWaypoint();
                break;
        }
    }

    public void GetNextWaypoint()
    {
        int auxIndex = waypointIndex;

        switch (playMode)
        {
            case LoopMode.PingPong:
                waypointIndex = odd ? 
                    waypointIndex - 1 : waypointIndex + 1;

                if (waypointIndex <= 0 ||
                    waypointIndex >= patrolArea.PatrolPoints.Length - 1)
                    odd = !odd;
                break;

            case LoopMode.Loop:
                waypointIndex += 1;
                if (waypointIndex > patrolArea.PatrolPoints.Length)
                    waypointIndex = 0;
                break;

            case LoopMode.Single:
                waypointIndex += 1;
                break;

            case LoopMode.Random:
                waypointIndex = Random.Range(
                    0,
                    patrolArea.PatrolPoints.Length - 1);
                break;
        }

        waypointIndex = Mathf.Clamp(waypointIndex,
            0,
            patrolArea.PatrolPoints.Length - 1);

        if (waypointIndex != auxIndex)
            OnWaypointChanged?.Invoke(this);
    }

    private void OnWaypointHasChanged(WandererAI sender)
    {
        SetDestination(Waypoint.transform.position);
    }

    private IEnumerator WaitForNextMove(float time)
    {
        bool rotateToInitialRotation = false,
             talking = false;

        if (Controller.Interactable != null)
        {
            rotateToInitialRotation = Controller.Interactable.RotateToInitialRotation;
            Controller.Interactable.RotateToInitialRotation = false;
        }

        Vector3 moveDirection = Waypoint.transform.forward.normalized;
        moveDirection.y = 0; 

        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;

            if (Controller.Interactable != null)
                talking = Controller.Interactable.IsTalking;

            if (Waypoint.RotateTowardsForward && !talking)
                Controller.RotateTowardsDirection(ref moveDirection);

            yield return null;
        }

        if (Controller.Interactable != null)
            Controller.Interactable.RotateToInitialRotation = rotateToInitialRotation;

        GetNextWaypoint();

        waiting = null;
    }
}