using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
abstract public class Character : Controller
{
    [System.Serializable] 
    abstract public class CharacterSavable : Savable
    {
        private List<ItemID> Items { get; }
        private float HpScalar { get; }
        private float HpMax { get; }
        private float X { get; }
        private float Y { get; }
        private float Z { get; }
        private float Qx { get; }
        private float Qy { get; }
        private float Qz { get; }

        public CharacterSavable(Character controller)
            : base(controller.UniqueID, SceneManager.GetActiveScene().name)
        {
            Vector3 position = controller.transform.position;
            X = position.x;
            Y = position.y;
            Z = position.z;

            Vector3 euler = controller.transform.rotation.eulerAngles;
            Qx = euler.x;
            Qy = euler.y;
            Qz = euler.z;

            HpScalar = controller.Hp.Scalar;
            HpMax    = controller.Hp.Max;

            Items = new List<ItemID>();
            foreach (Item i in controller.Inventory)
                Items.Add(new ItemID(i));
        }

        public virtual void Set(Character controller)
        {
            controller.Inventory.Clear();
            foreach (ItemID i in Items)
                controller.Inventory.Add(ItemUtility.GetItem(i.Name));

            controller.Hp = new ScaledValue(HpScalar, HpMax);

            controller.MovementSettings.Controller.enabled = false;
            controller.gameObject.transform.position = new Vector3(X, Y, Z);
            controller.gameObject.transform.rotation = Quaternion.Euler(Qx, Qy, Qz);
            controller.MovementSettings.Controller.enabled = true;
        }
    }

    public delegate void LandEventHandler(Character sender, float velocityOnY);

    public event LandEventHandler OnLanded;

    [Header("Movement & Rotation")]
    [Tooltip("The movement settings. Defines the walk, run speed etc...")]
    [SerializeField] private MovementSettings movementSettings = null;
    [Tooltip("Should the character rotate towards the movement direction.")]
    [SerializeField] private bool rotateTowardsMovement = true;
    [Tooltip("The rotation speed.")]
    [SerializeField] private float rotationSpeed = 3f;
    [Header("Items")]
    [Tooltip("The characters inventory.")]
    [SerializeField] private Inventory inventory = new Inventory();
    protected Vector3 direction = Vector3.zero;

    /// <summary>
    /// Gets or sets if the character should rotate towards movement.
    /// </summary>
    public bool RotateTowardsMovement
    {
        get => rotateTowardsMovement;

        set => rotateTowardsMovement = value;
    }
    /// <summary>
    /// Gets how much the character rotates.
    /// </summary>
    public float RotationSpeed { get => rotationSpeed; }
    public MovementSettings MovementSettings { get => movementSettings; }
    /// <summary>
    /// Get or set whether the character is running.
    /// </summary>
    public virtual bool IsRunning { get; set; } = false;
    /// <summary>
    /// Returns the player current velocity.
    /// </summary>
    public Vector3 Velocity { get => movementSettings.Controller.velocity; }
    /// <summary>
    /// Returns the players inventory.
    /// </summary>
    public Inventory Inventory { get => inventory; }
    /// <summary>
    /// Gets or sets if the player can control this object.
    /// </summary>
    public bool CanControl
    {
        get => movementSettings.CanControl;

        set => movementSettings.CanControl = value;
    }

    protected override void Awake()
    {
        movementSettings.Controller = GetComponent<CharacterController>();

        base.Awake();
    }

    protected virtual void FixedUpdate()
    {
        UpdateMovement();
    }

    /// <summary>
    /// Updates the npc gravity, and applies to it.
    /// </summary>
    private void UpdateGravity()
    {
        if (movementSettings.IsClimbing)
        {
            movementSettings.GravityForce = Vector3.zero;
            return;
        }

        if (movementSettings.Grounded)
            movementSettings.GravityForce =
                movementSettings.Mass *
                Physics.gravity *
                Time.fixedDeltaTime;
        else
            movementSettings.GravityForce +=
                2.3f *
                Physics.gravity *
                Time.fixedDeltaTime;
    }

    /// <summary>
    /// This method updates the npc fall.
    /// It is responsable for applying damage if the fall is to big.
    /// </summary>
    /// <param name="tempGrounded">If the npc was grounded.</param>
    /// <param name="velocityOnY">The current Y (vertical) npc velocity.</param>
    private void UpdateFall(ref bool tempGrounded, ref float velocityOnY)
    {
        if (!movementSettings.Grounded)
        {
            if (movementSettings.Controller.velocity.y < 0)
                movementSettings.FallTimer += Time.deltaTime;
            else
                movementSettings.FallTimer = 0f;

            return;
        }

        movementSettings.FallTimer = 0f;

        movementSettings.JumpForce = Vector3.zero;

        if (tempGrounded)
            return;

        movementSettings.Force = Vector3.zero;

        OnLanded?.Invoke(this, velocityOnY);

        if (movementSettings.FallTimer > 0.3f && velocityOnY <= -23)
            ApplyDamage(new PointDamage(this,
                                        true,
                                        (10 * Mathf.Abs(velocityOnY) * 1.2f)));
    }

    /// <summary>
    /// This method checks if the player is grounded.
    /// It's more precise than CharacterController.isGrounded.
    /// </summary>
    private void UpdateGroundedFlags()
    {
        movementSettings.Grounded = (movementSettings.Controller.collisionFlags &
                                  CollisionFlags.CollidedBelow) ==
                                  CollisionFlags.CollidedBelow;
    }

    /// <summary>
    /// Updates drag to forces.
    /// </summary>
    private void UpdateForceDrag()
    {
        movementSettings.JumpForce = Vector3.Lerp(movementSettings.JumpForce,
                                                  Vector3.zero,
                                                  Time.fixedDeltaTime);

        movementSettings.Force = Vector3.Lerp(movementSettings.Force,
                                              Vector3.zero,
                                              movementSettings.Grounded ?
                                              Time.fixedDeltaTime * 7.5f :
                                              Time.fixedDeltaTime * 1.2f);
    }

    /// <summary>
    /// Updates the NPC movement.
    /// </summary>
    protected virtual void UpdateMovement()
    {
        Move();

        if (rotateTowardsMovement && direction.sqrMagnitude > 0)
            RotateTowardsDirection(ref direction);

        direction.x = direction.y = direction.z = 0;
    }

    /// <summary>
    /// Moves this character. 
    /// </summary>
    protected void Move()
    {
        UpdateGravity();

        bool tempGrounded = MovementSettings.Grounded;
        float velocityOnY = Velocity.y;

        float velocity = 0;
        MovementSettings.CurrentSpeed = Mathf.SmoothDamp(
            MovementSettings.CurrentSpeed,
            (IsRunning && MovementSettings.CanRun) ? 
            MovementSettings.RunSpeed : MovementSettings.Speed,
            ref velocity, 0.1f);

        MovementSettings.Controller.Move(
            ((Hp.IsEmpty || !MovementSettings.CanControl ? Vector3.zero : direction) *
            MovementSettings.CurrentSpeed + MovementSettings.Force +
            MovementSettings.GravityForce + MovementSettings.JumpForce) *
            Time.fixedDeltaTime);

        UpdateGroundedFlags();
        UpdateFall(ref tempGrounded, ref velocityOnY);
        UpdateForceDrag();
    }

    /// <summary>
    /// Rotates this objects transform to the given direction.
    /// </summary>
    /// <param name="direction">The normalized direction to rotate.</param>
    public void RotateTowardsDirection(ref Vector3 direction)
    {
        Vector3 rot = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        rot.z = rot.x = 0;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(rot),
            Time.fixedDeltaTime * rotationSpeed);
    }
}