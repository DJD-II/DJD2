using UnityEngine;

[System.Serializable]
sealed public class MovementSettings
{
    [SerializeField]
    private float mass = 12f;
    [SerializeField]
    private bool applyGravity = true;
    [SerializeField]
    private bool canControl = true;
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private bool canRun = false;
    [SerializeField]
    private float runSpeed = 8f;
    [SerializeField]
    private float climbSpeed = 1.8f;
    private bool isClimbing = false;
    [SerializeField]
    private float jumpSpeed = 20;
    private Vector3 force = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;
    private Vector3 gravityForce = Vector3.zero;

    public bool IsClimbing { get => isClimbing; set => isClimbing = value; }
    public float ClimbSpeed { get => climbSpeed; set => climbSpeed = value; }
    public bool ApplyGravity { get => applyGravity; set => applyGravity = value; }
    public float FallTimer { get; set; }
    public float Mass { get => mass; }
    public float YRotation { get; set; } = 0;
    public CharacterController Controller { get; set; }
    public bool Grounded { get; set; }
    public bool CanControl { get => canControl; set => canControl = value; }
    public float Speed { get => speed; }
    public float CurrentSpeed { get; set; }
    public bool CanRun { get => canRun; set => canRun = value; }
    public float RunSpeed { get => runSpeed; }
    public float JumpSpeed { get => jumpSpeed; }
    public Vector3 Force { get => force; set => force = value; }
    public Vector3 JumpForce { get => jumpForce; set => jumpForce = value; }
    public Vector3 GravityForce { get => gravityForce; set => gravityForce = value; }
}
