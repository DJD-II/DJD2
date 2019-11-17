using UnityEngine;

[RequireComponent(typeof(NPC))]
[DisallowMultipleComponent]
abstract public class AIController : MonoBehaviour
{
    protected NPC Controller { get; private set; }

    protected virtual void Awake()
    {
        Controller = GetComponent<NPC>();
    }
}