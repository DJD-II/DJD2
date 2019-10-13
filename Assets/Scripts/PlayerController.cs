using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Controller, ISavable
{
    #region --- Structs ---
    internal struct CameraShakeSettings
    {
        public Vector3      pos,
                            initialPosition;
        public Vector3      rot,
                            initialRotation;
        public float        fov,
                            initialFieldOfView;
        public Coroutine    shakeCoroutine;
    }

    #endregion

    sealed public class PlayerSavable : Savable
    {
        public float   hpScalar;
        public float     max;
        public List<ItemID> items;

        public PlayerSavable(string id, ScaledValue hp, List<Item> items)
            : base(id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            hpScalar = hp.Scalar;
            max = hp.Max;
            this.id = id;
            this.items = new List<ItemID>();
            foreach (Item i in items)
                this.items.Add(new ItemID(i));
        }
    }

    #region --- Fields ---

    [Header("Character")]
    public GameObject           graphics;
    [Header("Weight")]
    public float                mass = 12f;
    float                       yRotation = 0f;
    CharacterController         controller;
    Vector3                     gravityForce = Vector3.zero;
    private bool                grounded = false;
    [Header("Movement")]
    [SerializeField]
    private bool                canControl = true;
    [SerializeField]
    private float               speed = 4f;
    private float               currentSpeed = 0f;
    [SerializeField]
    private float               runSpeed = 8f;
    [SerializeField]
    private float               jumpSpeed = 20;
    private Vector3             force = Vector3.zero;
    private Vector3             jumpForce = Vector3.zero;
    [Header("Camera")]
    public Camera               cam;
    public Animation            fallPivot;
    public Transform            cameraPivot;
    public Vector2              minMaxY;
    public float                lookSpeed = 20f;
    [Header("Wobble")]
    public Transform            wobblePivot;
    [Range(0f, 5f)]
    [SerializeField]
    private float               walkWoobleAmount = 1.2f;
    [Range(0f, 5f)]
    [SerializeField]
    private float               runWoobleAmount = 1.2f;
    [Range(0f, 90f)]
    [SerializeField]
    private float               walkZRotation = 20f;
    [Range(0f, 90f)]
    [SerializeField]
    private float               runZRotation = 20f;
    [Range(0f, 100f)]
    [SerializeField]
    private float               walkWobbleSpeed = 20f;
    [Range(0f, 100f)]
    [SerializeField]
    private float               runWobbleSpeed = 20f;
    [HideInInspector]
    CameraShakeSettings         cameraShakeSettings;
    public static CameraShake   explosionShake;
    private float               fallTimer = 0;
    [Header("Interaction")]
    [SerializeField]
    [Range(1f, 500f)]
    private float               interactDistance = 100f;
    private Interactable        interactable;
    [SerializeField]
    private LayerMask           ignoreLayers = 0;
    [Header("Inventory")]
    [SerializeField]
    private Inventory           inventory = new Inventory();
    [Header("HUDs")]
    public GameObject           huds;
    public Image                batteryImage;
    private Damage.Point        reducer;
    private UniqueID            uniqueID;
    [SerializeField]
    private Quest               mainQuest = null;

    #endregion

    #region --- Properties ---

    Savable ISavable.IO { get { return new PlayerSavable(GetUniqueID(), Hp, Inventory.Items); } }
    public bool CanControl { get { return canControl; } set { canControl = value; } }
    public Inventory Inventory { get { return inventory; } }
    private float WobbleSpeed { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runWobbleSpeed : walkWobbleSpeed; } }
    private float ZRotation { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runZRotation : walkZRotation; } }
    private float WobleAmount { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runWoobleAmount : walkWoobleAmount; } }

    #endregion

    #region --- Unity ---

    private void Awake()
    {
        GameInstance.OnSave += OnSceneChange;

        controller = GetComponent<CharacterController>();
        reducer = new Damage.Point(this, true, 1);
    }

    private void OnDestroy()
    {
        GameInstance.OnSave -= OnSceneChange;
        GameInstance.GameState.OnPausedChanged -= OnPause;
    }

    private void OnPause(GameState sender)
    {
        canControl = !sender.Paused;
        huds.SetActive(!sender.Paused);
    }

    private void Start()
    {
        uniqueID = GetComponent<UniqueID>();

        PlayerSavable savable = GameInstance.Singleton.GetSavable(GetUniqueID(), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, GetUniqueIDPersistent()) as PlayerSavable;
        if (savable != null)
        {
            hp = new ScaledValue(savable.hpScalar, savable.max);

            foreach (ItemID i in savable.items)
                inventory.Add(ItemUtility.GetItem(i.name));
        }

        GameInstance.GameState.OnPausedChanged += OnPause;

        cameraShakeSettings = new CameraShakeSettings
        {
            initialPosition = cameraPivot.localPosition,
            initialRotation = cameraPivot.localRotation.eulerAngles,
            initialFieldOfView = cam.fieldOfView
        };

        if (explosionShake == null)
        {
            explosionShake = new CameraShake(1f, 0.1f, 0.4f);

            explosionShake.fieldOfViewShake = new CameraShake.ElementShake(0.3f, 2f);

            explosionShake.positionShake[0] = new CameraShake.ElementShake(0.1f, 2f);
            explosionShake.positionShake[1] = new CameraShake.ElementShake(0.1f, 2f);
            explosionShake.positionShake[2] = new CameraShake.ElementShake(0.1f, 2f);

            explosionShake.rotationShake[0] = new CameraShake.ElementShake(0.1f, 2f);
            explosionShake.rotationShake[1] = new CameraShake.ElementShake(0.1f, 2f);
            explosionShake.rotationShake[2] = new CameraShake.ElementShake(0.1f, 2f);
        }

        InvokeRepeating("UpdateInteraction", 0f, 0.15f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GameInstance.GameState.QuestController.Add(mainQuest);

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        UpdateWobbleAnimation(vertical, horizontal);

        if (!canControl)
            return;

        if (vertical != 0 || horizontal != 0)
            GameInstance.GameState.QuestController.CompleteQuest("Learn Quick!");

        if (!GameInstance.GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            GameInstance.HUD.EnableMenu(this);

        Interact();

        UpdateJumpInput();

        Look();
    }

    protected virtual void FixedUpdate()
    {
        UpdateMovement();
    }

    #endregion

    private void OnSceneChange()
    {
        GameInstance.Singleton.FeedSavable(this, GetUniqueIDPersistent());
    }

    #region --- HUD ---

    private void UpdateLifeHUD()
    {
        if (batteryImage == null)
            return;

        batteryImage.fillAmount = Hp.Scalar;
    }

    #endregion

    #region --- Camera Shake ---

    public void PlayCameraShake(CameraShake cameraShake, float scale)
    {
        if (cameraShakeSettings.shakeCoroutine != null)
            StopCoroutine(cameraShakeSettings.shakeCoroutine);

        cameraShakeSettings.shakeCoroutine = StartCoroutine(ShakeCamera(cameraShake, scale));
    }

    private IEnumerator ShakeCamera(CameraShake cameraShake, float scale)
    {
        float blendScale;
        float blendInTimer = cameraShake.Duration * Mathf.Min(cameraShake.BlendInTime, 1f);
        float blendOutTimer = cameraShake.Duration * Mathf.Min(cameraShake.BlendOutTime, 1f);
        float timer = 0f;

        Transform t = cameraPivot.transform;

        while (timer <= cameraShake.Duration)
        {
            float blendInScale = Mathf.Clamp01(timer / blendInTimer);
            float blendOutScale = Mathf.Clamp01((cameraShake.Duration - timer) / blendOutTimer);

            blendScale = scale * blendInScale * blendOutScale;

            float blendTime = timer / cameraShake.Duration;

            cameraShakeSettings.rot.x = (Mathf.Cos(blendTime * cameraShake.rotationShake[0].frequency * Mathf.Rad2Deg) * cameraShake.rotationShake[0].amplitude) * blendScale;
            cameraShakeSettings.rot.y = (Mathf.Cos(blendTime * cameraShake.rotationShake[1].frequency * Mathf.Rad2Deg) * cameraShake.rotationShake[1].amplitude) * blendScale;
            cameraShakeSettings.rot.z = (Mathf.Cos(blendTime * cameraShake.rotationShake[2].frequency * Mathf.Rad2Deg) * cameraShake.rotationShake[2].amplitude) * blendScale;

            t.localRotation = Quaternion.Euler(cameraShakeSettings.initialRotation.x + cameraShakeSettings.rot.x,
                                                cameraShakeSettings.initialRotation.y + cameraShakeSettings.rot.y,
                                                cameraShakeSettings.initialRotation.z + cameraShakeSettings.rot.z);

            cameraShakeSettings.pos.x = (Mathf.Cos(blendTime * cameraShake.positionShake[0].frequency * Mathf.Rad2Deg) * cameraShake.positionShake[0].amplitude) * blendScale;
            cameraShakeSettings.pos.y = (Mathf.Cos(blendTime * cameraShake.positionShake[1].frequency * Mathf.Rad2Deg) * cameraShake.positionShake[1].amplitude) * blendScale;
            cameraShakeSettings.pos.z = (Mathf.Cos(blendTime * cameraShake.positionShake[2].frequency * Mathf.Rad2Deg) * cameraShake.positionShake[2].amplitude) * blendScale;

            t.localPosition = cameraShakeSettings.initialPosition + cameraShakeSettings.pos;

            cameraShakeSettings.fov = (Mathf.Cos(blendTime * cameraShake.fieldOfViewShake.frequency * Mathf.Deg2Rad) * cameraShake.fieldOfViewShake.amplitude) * blendScale;

            cam.fieldOfView = cameraShakeSettings.initialFieldOfView + cameraShakeSettings.fov;

            timer += Time.deltaTime;
            yield return null;
        }

        t.localRotation = Quaternion.Euler(cameraShakeSettings.initialRotation);
        t.localPosition = cameraShakeSettings.initialPosition;
        cam.fieldOfView = cameraShakeSettings.initialFieldOfView;
    }

    #endregion

    #region --- Pick ---

    private void Interact ()
    {
        if (interactable == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            interactable.Interact(this);
    }

    private void UpdateInteraction()
    {
        if (GameInstance.GameState.Paused)
            return;

        GameInstance.HUD.EnableInteractMessage(false, null);
        interactable = null;

        RaycastHit[] hits = Physics.RaycastAll(new Ray(cam.transform.position, cam.transform.forward), interactDistance, ~ignoreLayers);
        List<RaycastHit> sortedHits = new List<RaycastHit>(hits);
        sortedHits.Sort((x, y) => Vector3.Distance(x.point, cam.transform.position).CompareTo(Vector3.Distance(y.point, cam.transform.position)));
        foreach (RaycastHit hit in sortedHits)
        {
            Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                GameInstance.HUD.EnableInteractMessage(true, interactable);
                this.interactable = interactable;
            }
            break;
        }
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
                if (velocityOnY <= -8)
                    fallPivot.Play();

                force = Vector3.zero;

                if (fallTimer > 0.3f && velocityOnY <= -23)
                {
                    ApplyDamage(new Damage.Point(this, true, (uint)(10 * velocityOnY * 1.2f)));
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
        {
            float multiplier = currentSpeed / runSpeed;
            multiplier *= Mathf.Clamp01(controller.velocity.magnitude);
            float time = Time.time;

            wobblePivot.transform.localPosition = Vector3.Lerp(wobblePivot.transform.localPosition, new Vector3(0f, Mathf.Sin(time * WobbleSpeed) * WobleAmount * multiplier, 0f) , Time.deltaTime * 2f);
            wobblePivot.transform.localRotation = Quaternion.Slerp(wobblePivot.transform.localRotation, Quaternion.Euler(0f, 0f, Mathf.Sin(time * WobbleSpeed / 2) * ZRotation * multiplier), Time.deltaTime * 2f);
        }
        else
        {
            wobblePivot.transform.localPosition = Vector3.Lerp(wobblePivot.transform.localPosition, Vector3.zero, Time.deltaTime * 6f);
            wobblePivot.transform.localRotation = Quaternion.Slerp(wobblePivot.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 6f);
        }
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
}