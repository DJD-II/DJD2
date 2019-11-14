using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
sealed public class NPC : Character, ISavable
{
    [System.Serializable]
    sealed public class NPCSavable : CharacterSavable
    {
        public NPCSavable(NPC controller)
            : base(controller)
        {
        }
    }

    private Animator animator = null;

    public TalkInteractable Interactable { get; private set; }
    Savable ISavable.IO { get => new NPCSavable(this); }

    protected override void Awake()
    {
        Interactable    = GetComponent<TalkInteractable>();
        animator        = GetComponent<Animator>();

        base.Awake();
    }

    protected override void FixedUpdate()
    {
        if (Interactable != null)
            Interactable.OverrideRotation = direction.sqrMagnitude != 0;

        if (animator != null)
            animator.SetFloat("Speed",
                Mathf.Clamp01(direction.magnitude * MovementSettings.CurrentSpeed /
                Mathf.Max(MovementSettings.RunSpeed - 0.5f, 0.0001f)));

        base.FixedUpdate();
    }

    /// <summary>
    /// This method is called when a saved game is loading.
    /// </summary>
    /// <param name="io">The object that holds all saved objects.</param>
    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Get(UniqueID,
                     PersistentAcrossLevels) is NPCSavable savable))
            return;

        savable.Set(this);
    }

    /// <summary>
    /// This method is called when the game is about to be saved.
    /// </summary>
    /// <param name="io">The object that holds all saved objects.</param>
    protected override void OnSave(SaveGame io)
    {
        base.OnSave(io);

        io.Override(this, UniqueID, PersistentAcrossLevels);
    }

    /// <summary>
    /// This method updates the NPC move direction.
    /// This direction must be updated on each FixedUpdate.
    /// </summary>
    /// <param name="motion">The normalized move direction.</param>
    public void SetMoveDirection(ref Vector3 motion)
    {
        direction.x = motion.x;
        direction.y = motion.y;
        direction.z = motion.z;
    }
}