using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerController : Controller, ISavable
{
    [System.Serializable]
    sealed public class PlayerSavable : Savable
    {
        public float hpScalar;
        public float max;
        public List<ItemID> items;
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float Qx { get; private set; }
        public float Qy { get; private set; }
        public float Qz { get; private set; }

        public PlayerSavable(string id, ScaledValue hp, List<Item> items, Transform transform)
            : base(id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            X = transform.position.x;
            Y = transform.position.y;
            Z = transform.position.z;
            Vector3 euler = transform.rotation.eulerAngles;
            Qx = euler.x;
            Qy = euler.y;
            Qz = euler.z;
            hpScalar = hp.Scalar;
            max = hp.Max;
            this.id = id;
            this.items = new List<ItemID>();
            foreach (Item i in items)
                this.items.Add(new ItemID(i));
        }
    }

    #region --- Fields ---

    /*[Header("Character")]
    [SerializeField]
    private GameObject graphics = null;*/
    [SerializeField]
    private bool canPause = true;
    [Header("Weight")]
    [SerializeField]
    private float mass = 12f;
    float yRotation = 0f;
    CharacterController controller;
    Vector3 gravityForce = Vector3.zero;
    private bool grounded = false;
    [Header("Movement")]
    [SerializeField]
    private bool canControl = true;
    [SerializeField]
    private float speed = 4f;
    private float currentSpeed = 0f;
    [SerializeField]
    private float runSpeed = 8f;
    [SerializeField]
    private float jumpSpeed = 20;
    private Vector3 force = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;
    [Header("Camera")]
    [SerializeField]
    private Camera cam = null;
    [SerializeField]
    private Animation fallPivot = null;
    [SerializeField]
    private Vector2 minMaxY = Vector2.zero;
    [SerializeField]
    private float lookSpeed = 20f;
    [Header("Wobble")]
    [SerializeField]
    private CameraWobble wobble = null;
    private float fallTimer = 0;
    [Header("Interaction")]
    [SerializeField]
    private InteractController interaction = null;
    [Header("Inventory")]
    [SerializeField]
    private Inventory inventory = new Inventory();
    [Header("HUDs")]
    [SerializeField]
    private bool hudsEnabled = true;
    [SerializeField]
    private GameObject huds = null;
    [SerializeField]
    private Image batteryImage = null;
    private PointDamage reducer;
    private UniqueID uniqueID;
    [SerializeField]
    private Transform messageContents = null;
    [SerializeField]
    private GameObject messagePrefab = null;
    public CameraShake explosion = null;
    [Header("Cloud Transition")]
    [SerializeField]
    private WormHoleController wormHoleController = null;

    #endregion

    #region --- Properties ---

    Savable ISavable.IO { get { return new PlayerSavable(GetUniqueID(), Hp, Inventory.Items, transform); } }
    public bool CanControl { get { return canControl; } set { canControl = value; } }
    public Inventory Inventory { get { return inventory; } }
    public Vector3 Velocity { get { return controller.velocity; } }
    public bool HudsEnabled
    {
        get { return hudsEnabled; }

        set
        {
            hudsEnabled = value;
            huds.SetActive(!GameInstance.GameState.Paused && value);
        }
    }

    #endregion

    #region --- Unity ---

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        uniqueID = GetComponent<UniqueID>();
        reducer = new PointDamage(this, true, 1);

        GameInstance.OnLoad += OnLoad;
    }

    private void OnDestroy()
    {
        GameInstance.OnSave -= OnSave;
        GameInstance.OnLoad -= OnLoad;
        GameInstance.GameState.OnPausedChanged -= OnPause;

        GameInstance.GameState.QuestController.Initialize(null);
    }

    private void OnPause(GameState sender)
    {
        canControl = !sender.Paused;
        huds.SetActive(!sender.Paused && hudsEnabled);
    }

    private void Start()
    {
        GameInstance.HUD.EnableCorssHair(true);

        GameInstance.GameState.QuestController.Initialize(this);

        GameInstance.OnSave += OnSave;
        GameInstance.GameState.OnPausedChanged += OnPause;

        InvokeRepeating("UpdateInteraction", 0, interaction.InteractTime);
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        UpdateWobbleAnimation(vertical, horizontal);

        if (!canControl)
            return;

        if (!GameInstance.GameState.Paused && Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            GameInstance.HUD.EnableMenu(true, this);
            return;
        }

        interaction.Update(this);

        UpdateJumpInput();

        Look();

        if (hp.IsEmpty)
        {
            canControl = false;
            StartCoroutine (SwitchToCloudScene());
        }
    }

    protected virtual void FixedUpdate()
    {
        UpdateMovement();
    }

    #endregion

    #region --- To Cloud Scene ---

    private IEnumerator SwitchToCloudScene()
    {
        GameInstance.HUD.EnableCorssHair(false);

        ApplyHeal(new PointHeal(this, 100));
        GameInstance.Singleton.FadeOutMasterMixer(0.2f);

        HudsEnabled = false;

        wormHoleController.ShuttDown.clip = wormHoleController.ShuttDown.GetClip("Shut Down");

        wormHoleController.ShuttDown.Play();
        while (wormHoleController.ShuttDown.isPlaying)
            yield return null;

        wormHoleController.ShuttDown.clip = wormHoleController.ShuttDown.GetClip("Turn On");

        wormHoleController.WormHoleEnterSFX.Play();

        yield return new WaitForSecondsRealtime(1f);

        wormHoleController.WormHoleTunnel.SetActive(true);
        wormHoleController.TunnelCamera.SetActive(true);

        GameInstance.HUD.MaskScreen(true);

        wormHoleController.WomHoleShake.Play(this, wormHoleController.TunnelCamera.GetComponent<Camera>(), wormHoleController.TunnelCamera.transform, 1f);

        StartCoroutine(GameInstance.HUD.FadeFromWhite(4f));
        wormHoleController.ShuttDown.Play();

        GameInstance.Save();

        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation sceneLoadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("ICloud");
        sceneLoadOp.allowSceneActivation = false;

        float passedTime = 0;

        while (sceneLoadOp.progress <= 0.8f)
        {
            passedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(6f - passedTime, 0f));

        StartCoroutine(ChangeTunnelVolume());

        wormHoleController.WormHoleExitSFX.Play();

        yield return GameInstance.HUD.FadeToWhite(2.2f);

        while (wormHoleController.WormHoleExitSFX.isPlaying)
            yield return null;

        sceneLoadOp.allowSceneActivation = true;
        GameInstance.HUD.MaskScreen(false);
        GameInstance.Singleton.FadeInMasterMixer(2f);
    }

    private IEnumerator ChangeTunnelVolume()
    {
        while (wormHoleController.TunnelSFX.volume > 0)
        {
            wormHoleController.TunnelSFX.volume = Mathf.Lerp(wormHoleController.TunnelSFX.volume, 0, Time.deltaTime * 2f);
            yield return null;
        }
    }

    #endregion

    #region --- IO ---

    private void OnLoad(PlaySaveGameObject io)
    {
        PlayerSavable savable = io.Get(GetUniqueID(), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, GetUniqueIDPersistent()) as PlayerSavable;
        if (savable != null)
        {
            hp = new ScaledValue(savable.hpScalar, savable.max);

            foreach (ItemID i in savable.items)
                inventory.Add(ItemUtility.GetItem(i.name));

            controller.enabled = false;
            transform.position = new Vector3(savable.X, savable.Y, savable.Z);
            transform.rotation = Quaternion.Euler(savable.Qx, savable.Qy, savable.Qz);
            controller.enabled = true;
        }
    }

    private void OnSave(PlaySaveGameObject io)
    {
        io.Feed(this, GetUniqueIDPersistent());
    }

    #endregion

    #region --- HUD ---

    private void UpdateLifeHUD()
    {
        if (batteryImage == null)
            return;

        batteryImage.fillAmount = Hp.Scalar;
    }

    #endregion

    #region --- Interaction ---

    private void UpdateInteraction ()
    {
        interaction.UpdateInteraction();
    }

    #endregion

    #region --- Movement ---

    public virtual void Rotate(Quaternion rotation)
    {
        yRotation = 0;
        transform.rotation = rotation;
    }

    #region --- Forces ---

    public virtual void ApplyJumpForce(Vector3 from, float force)
    {
        this.force = force * (transform.position - from).normalized;
        this.force.y *= 1.4f;
    }

    public virtual void ApplyForce(Vector3 direction, float force)
    {
        this.force = direction * force;
    }

    #endregion

    #region --- Walk ---

    private void UpdateGravity()
    {
        if (grounded)
        {
            fallTimer = 0f;
            gravityForce += mass * Physics.gravity * Time.fixedDeltaTime;
        }
        else
        {
            if (controller.velocity.y < 0)
                fallTimer += Time.deltaTime;
            else
                fallTimer = 0f;

            gravityForce += 2.3f * Physics.gravity * Time.fixedDeltaTime;
        }
    }

    private void UpdateJumpInput()
    {
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !Hp.IsEmpty)
                jumpForce.y = jumpSpeed;
        }
    }

    private void ResetForces()
    {
        fallTimer = 0f;
        jumpForce = Vector3.zero;
        force = Vector3.zero;
        gravityForce = Vector3.zero;
    }

    private void UpdateFall(bool tempGrounded, float velocityOnY)
    {
        if (grounded)
        {
            jumpForce = Vector3.zero;

            if (!tempGrounded)
            {
                if (velocityOnY <= -2)
                    fallPivot.Play();

                force = Vector3.zero;

                if (fallTimer > 0.3f && velocityOnY <= -23)
                {
                    ApplyDamage(new PointDamage(this, true, (uint)(10 * velocityOnY * 1.2f)));
                }
            }
        }
    }

    private void UpdateForceDrag()
    {
        jumpForce = Vector3.Lerp(jumpForce, Vector3.zero, Time.fixedDeltaTime);
        force = Vector3.Lerp(force, Vector3.zero, grounded ? Time.fixedDeltaTime * 7.5f : Time.fixedDeltaTime * 1.2f);
    }

    private void UpdateGroundedFlags()
    {
        grounded = (controller.collisionFlags & CollisionFlags.CollidedBelow) == CollisionFlags.CollidedBelow;
    }

    private void UpdateWobbleAnimation(float vertical, float horizontal)
    {
        if (grounded && (vertical != 0 || horizontal != 0))
            wobble.UpdateMove(controller.velocity.magnitude, currentSpeed, runSpeed);
        else
            wobble.UpdateStopped();
    }

    private void UpdateMovement()
    {
        float forward = Input.GetAxis("Vertical"),
                sides = Input.GetAxis("Horizontal");

        Vector3 dir = transform.forward * forward;
        dir += transform.right * sides;

        if (grounded)
            gravityForce = Vector3.zero;

        UpdateGravity();

        bool tempGrounded = grounded;
        float velocityOnY = controller.velocity.y;

        float velocity = 0;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runSpeed : speed, ref velocity, 0.1f);
        controller.Move(((Hp.IsEmpty || !canControl ? Vector3.zero : dir) * currentSpeed + force + gravityForce + jumpForce) * Time.fixedDeltaTime);

        if (currentSpeed * dir.magnitude > 0)
        {
            reducer.BaseDamage = 0.1f * currentSpeed / runSpeed * 2.5f * Time.fixedDeltaTime;
            ApplyDamage(reducer);
        }

        UpdateLifeHUD();

        UpdateGroundedFlags();

        UpdateFall(tempGrounded, velocityOnY);

        UpdateForceDrag();
    }

    #endregion

    #endregion

    #region --- Look ---

    private void Look()
    {
        if (Hp.IsEmpty)
            return;

        Vector3 rotation = transform.eulerAngles;
        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        transform.eulerAngles = rotation;

        yRotation = Mathf.Clamp(yRotation + Input.GetAxis("Mouse Y") * lookSpeed, minMaxY.x, minMaxY.y);
        cam.transform.localEulerAngles = new Vector3(-yRotation, cam.transform.localEulerAngles.y, 0);
    }

    #endregion

    #region --- Misc ---

    public void GiveQuest (Quest quest)
    {
        GameInstance.GameState.QuestController.Add(quest);
    }

    public void PopMessage(string message)
    {
        GameObject go = Instantiate(messagePrefab, messageContents);
        HUDMessageController messageController = go.GetComponent<HUDMessageController>();
        messageController.Initialize(message);
    }

    protected string GetUniqueID()
    {
        if (uniqueID != null)
            return uniqueID.uniqueId;

        return "";
    }

    protected bool GetUniqueIDPersistent()
    {
        if (uniqueID != null)
            return uniqueID.persistentAcrossLevels;

        return false;
    }

    #endregion
}