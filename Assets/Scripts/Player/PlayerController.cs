using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
sealed public class PlayerController : Character, ISavable
{
    /// <summary>
    /// This class holds the player information that is to be 
    /// saved to the disk.
    /// Later on is going to be added to a SaveGame object and 
    /// dumped to the disk.
    /// </summary>
    [System.Serializable]
    sealed public class PlayerSavable : CharacterSavable
    {
        public PlayerSavable(PlayerController controller)
            : base(controller)
        {
        }
    }

    [Header("Misc")]
    [Tooltip("Should the player be able to pause the game.")]
    [SerializeField] private bool canPause = true;
    [Header("Camera")]
    [Tooltip("The Player camera settings. Defines main camera and pivots...")]
    [SerializeField] private PlayerCameraSettings cameraSettings = null;
    [Tooltip("How much should the camera wobble when moving.")]
    [SerializeField] private CameraWobble wobble = null;
    [Header("Interactivity")]
    [Tooltip("Interactivity settings. Defines how far away from interactables " +
             "can the player reach etc...")]
    [SerializeField] private InteractController interaction = null;
    [Header("Heads Up Display")]
    [Tooltip("The Player Heads up display. Where the battery/life should be displayed.")]
    [SerializeField] private PlayerHUDSettings hudSettings = null;
    [Header("Cloud Transition")]
    [Tooltip("The cloud transition settings. This defines the transition.")]
    [SerializeField] private WormHoleController wormHoleController = null;
    [Header("Glitches")]
    [Tooltip("When the player is damaged this controller should be triggered.")]
    [SerializeField] private GlitchController glitchController = null;
    private PointDamage reducer;
    private PointHeal recharger;
    private List<Transform> ladders = new List<Transform>();

    /// <summary>
    /// Gets the player camera settings.
    /// </summary>
    public PlayerCameraSettings CameraSettings { get => cameraSettings; }
    /// <summary>
    /// Gets the player interaction controller.
    /// </summary>
    public InteractController Interaction { get => interaction; }
    /// <summary>
    /// Returns whether the player is running.
    /// </summary>
    public override bool IsRunning
    {
        get => Input.GetKey(KeyCode.LeftShift) ||
               Input.GetKey(KeyCode.RightShift);
    }
    /// <summary>
    /// This property returns the player huds.
    /// </summary>
    public PlayerHUDSettings HudSettings { get => hudSettings; }
    /// <summary>
    /// Returns a savable object to be saved to the disk.
    /// </summary>
    Savable ISavable.IO { get => new PlayerSavable(this); }

    protected override void Awake()
    {
        reducer     = new PointDamage(this, false, 1);
        recharger   = new PointHeal(this, 1f, false);

        base.Awake ();
    }

    protected override void Start()
    {
        GameInstance.HUD.EnableCrossHair(true);
        GameInstance.GameState.QuestController.Initialize(this);

        GameInstance.GameState.OnPausedChanged += OnPauseChanged;
        GameInstance.GameState.OnPausedChanged += hudSettings.OnPauseChanged;
        GameInstance.GameState.OnPausedChanged += interaction.OnPausedChanged;

        OnDeath  += OnPlayerHasDied;
        OnLanded += cameraSettings.OnLanded;

        base.Start();

        InvokeRepeating("UpdateInteraction", 0, interaction.InteractTime);
    }

    private void Update()
    {
        hudSettings.UpdateLifeHUD(this);
        wobble.UpdateWobble(this,
                            Input.GetAxis("Vertical"),
                            Input.GetAxis("Horizontal"));

        if (!MovementSettings.CanControl || UpdateMainMenuInput())
            return;

        interaction.Update(this);
        UpdateJumpInput();
        cameraSettings.Look(this);
    }

    private void OnEnable()
    {
        if (GameInstance.HUD != null)
            GameInstance.HUD.EnableCrossHair(true);
    }

    private void OnDisable()
    {
        if (GameInstance.HUD != null)
            GameInstance.HUD.EnableCrossHair(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Ladder"))
            ladders.Add(other.transform);

        MovementSettings.IsClimbing = ladders.Count > 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Ladder"))
            ladders.Remove(other.transform);

        MovementSettings.IsClimbing = ladders.Count > 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GameInstance.GameState.OnPausedChanged -= OnPauseChanged;
        GameInstance.GameState.OnPausedChanged -= hudSettings.OnPauseChanged;
        GameInstance.GameState.OnPausedChanged -= interaction.OnPausedChanged;

        OnDeath  -= OnPlayerHasDied;
        OnLanded -= cameraSettings.OnLanded;

        GameInstance.GameState.QuestController.Initialize(null);
    }

    /// <summary>
    /// This method is called when damage is applied to this object.
    /// </summary>
    /// <param name="damage">The type of damage to be applied.</param>
    /// <returns>If the damage was applied.</returns>
    protected override bool OnApplyDamage(Damage damage)
    {
        bool appliedDamage = base.OnApplyDamage(damage);

        if (appliedDamage && damage.Instigator != this)
            glitchController.Impulse(1f, 0.1f);

        return appliedDamage;
    }

    /// <summary>
    /// This method is called when the player has died.
    /// In other words, when the players battery has
    /// reached its end.
    /// </summary>
    /// <param name="sender">The Actor that has died.</param>
    private void OnPlayerHasDied(Actor sender)
    {
        MovementSettings.CanControl = false;
        StartCoroutine(wormHoleController.SwitchToCloudScene(this));
    }

    /// <summary>
    /// This method is called when the game pause state has changed.
    /// </summary>
    /// <param name="sender">The Game state.</param>
    private void OnPauseChanged(GameState sender)
    {
        MovementSettings.CanControl = !sender.Paused;
    }

    /// <summary>
    /// This method is called when a saved game is loading.
    /// </summary>
    /// <param name="io">The object that holds all saved objects.</param>
    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Get(UniqueID,
                     PersistentAcrossLevels) is PlayerSavable savable))
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
    /// This method checks for the player input so that it 
    /// can show the main menu.
    /// </summary>
    /// <returns>Returns true if the player has pressed the key to open the menu.</returns>
    private bool UpdateMainMenuInput()
    {
        if (!GameInstance.GameState.Paused &&
            Input.GetKeyDown(KeyCode.Escape) &&
            canPause)
        {
            GameInstance.HUD.EnableMainMenu(true, this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// This method is called through Unitys InvokeRepeating Method.
    /// So that we can save CPU we don't call this method every frame.
    /// Instead we only call this method in timed intervals.
    /// This method is responsable for calling the InteractController
    /// That allows the player to interact with objects in the world.
    /// </summary>
    private void UpdateInteraction()
    {
        interaction.UpdateInteraction(MovementSettings.CanControl);
    }

    /// <summary>
    /// This method checks for user input. If the player has pressed the 
    /// jump key it applies the jump speed to the current movement vector.
    /// </summary>
    private void UpdateJumpInput()
    {
        if (!MovementSettings.Grounded)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !Hp.IsEmpty)
            MovementSettings.JumpForce = 
                new Vector3(0f, 
                            MovementSettings.JumpSpeed, 
                            0f);
    }

    /// <summary>
    /// As the player runs this method should be responsable for applying
    /// damage. And also to replenish it when stopped or walking.
    /// </summary>
    /// <param name="dir">The current Players direction.</param>
    private void UpdateBattery()
    {
        if (IsRunning && (MovementSettings.Grounded ||
                          MovementSettings.IsClimbing))
        {
            reducer.BaseDamage = 0.1f *
                Velocity.magnitude / MovementSettings.RunSpeed *
                3.5f * Time.fixedDeltaTime;
            ApplyDamage(reducer);
        }
        else
        {
            recharger.BaseHeal = 0.1f * Time.fixedDeltaTime;
            ApplyHeal(recharger);
        }
    }

    /// <summary>
    /// This method checks for user input and applies movement accordingly.
    /// </summary>
    protected override void UpdateMovement()
    {
        direction = transform.forward * Input.GetAxis("Vertical") +
                    transform.right * Input.GetAxis("Horizontal");

        UpdateClimbMovement();
        Move();
        UpdateBattery();
    }

    /// <summary>
    /// This method checks for user input and applies "vertical" movement accordingly.
    /// </summary>
    private void UpdateClimbMovement()
    {
        // If we're not climbing don't continue.
        if (!MovementSettings.IsClimbing)
            return;

        // Add to direction the camera forward rotation
        // Making it go up.
        direction.y += cameraSettings.ForwardRotation.y *
                       Input.GetAxis("Vertical") *
                       MovementSettings.ClimbSpeed;

        // Take out the forward component if we're not grounded.
        // This makes the player move only up and down if he's not
        // grounded. Which makes sense if he's in the middle of the ladder
        // We just want up and down movement. But if he's grounded
        // then let the player walk away from the stairs 
        // (Vertical/Forward movement).
        if (!MovementSettings.Grounded && ladders.Count > 0)
        { 
            direction -= Vector3.Project(direction, transform.forward);
            // If the stairs are inclined then add a bit of forward
            // movement according to the stairs rotation.
            direction += Vector3.Project(
                ladders[ladders.Count - 1].forward,
                transform.forward) * 
                Vector3.Dot(direction, 
                            ladders[ladders.Count - 1].forward);
        }
    }

    /// <summary>
    /// This method (for now) is only called by Unitys animation event system 
    /// So that it can add a quest.
    /// </summary>
    /// <param name="quest">The quest to be added.</param>
    public void GiveQuest(Quest quest)
    {
        GameInstance.GameState.QuestController.Add(quest);
    }

    /// <summary>
    /// This method (for now) is only called by Unitys animation event system 
    /// So that it can add a location.
    /// </summary>
    /// <param name="quest">The quest to be added.</param>
    public void AddLocation(Location location)
    {
        GameInstance.GameState.LocationController.Add(location);
    }
}