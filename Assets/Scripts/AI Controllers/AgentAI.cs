using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
abstract public class AgentAI : AIController
{
    public delegate void AIEventHandler(AgentAI sender);

    public event AIEventHandler OnReachedDestination;

    [Header("Distances")]
    [SerializeField] private float navMeshSampleDistance = 0.3f;
    [SerializeField] private float stopDistance = 0.3f;

    private NavMeshAgent Agent    { get; set; }
    public Vector3 Destination      { get => Agent.destination; }
    protected bool ReachedDestination { get =>
            (Destination - Controller.transform.position).sqrMagnitude
            < Mathf.Pow(stopDistance, 2); }

    protected override void Awake()
    {
        base.Awake();

        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = 0f;
        Agent.angularSpeed = 0f;
        Agent.acceleration = 0f;
        Agent.stoppingDistance = stopDistance;
        Agent.autoBraking = false;

        Controller.MovementSettings.CanControl = false;
    }

    protected virtual void FixedUpdate()
    {
        if (!Controller.MovementSettings.CanControl)
            return;

        Vector3 motion = (Agent.steeringTarget -
            Controller.transform.position).normalized;
        motion.y = 0;
        Controller.SetMoveDirection(ref motion);
    }

    protected virtual void Update()
    {
        if (Controller.MovementSettings.CanControl &&
            ReachedDestination)
        {
            Controller.MovementSettings.CanControl = false;
            OnReachedDestination?.Invoke(this);
        }
    }

    protected virtual void SetDestination(Vector3 position)
    {
        NavMesh.SamplePosition(
            position,
            out NavMeshHit hit,
            navMeshSampleDistance,
            NavMesh.AllAreas);

        Agent.SetDestination(hit.position);

        Controller.MovementSettings.CanControl = true;
    }
}
