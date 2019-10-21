using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controls the lockpick HUD mini-game.
/// </summary>
sealed public class LockController : MonoBehaviour
{
    #region --- Events ---

    public delegate void EventHandler(LockController sender);

    public event EventHandler OnUnlock;

    #endregion

    #region --- Fields ---

    [Header("Speed")]
    [SerializeField]
    private float pickSpeed = 60f;
    [SerializeField]
    private float pickPullback = 8f;
    [Header("Bobby Pin")]
    [SerializeField]
    private GameObject bobbyPin = null;
    [SerializeField]
    private Text bobbyPinsAmmount = null;
    [SerializeField]
    private Vector2 maxMinRotation = Vector2.zero;
    [Header("Lock")]
    [SerializeField]
    private GameObject lockPick = null;
    [Header("Buttons")]
    [SerializeField]
    private GameObject quitButton = null;
    [Header("Sound FX")]
    [SerializeField]
    private AudioSource enterSFX = null;
    [SerializeField]
    private AudioClip[] lockpickMovements = new AudioClip[0];
    [SerializeField]
    private AudioSource lockpickMovement = null;
    [SerializeField]
    private AudioSource unlockSFX = null;
    [SerializeField]
    private AudioSource bobbyPinBreak = null;
    private Animator bobbyPinAnimator = null;
    private bool picking = false;
    private bool active = false;

    #endregion

    #region --- Properties ---

    /// <summary>
    /// The angle Tolerance. The bigger the tolerance, the easier to lockpick.
    /// When the lock rotates by pressing "Space" the opening angle will be 
    /// the angle minus the Tolerance.
    /// </summary>
    private float Tolerance { get; set; }
    /// <summary>
    /// The Current Bobby pin angle (Rotation).
    /// </summary>
    private float BobbyPinAngle { get; set; } = 0;
    /// <summary>
    /// The correct angle to open.
    /// </summary>
    private float Angle { get; set; }
    /// <summary>
    /// The damage to be given to the bobby pin when lockpick fails.
    /// </summary>
    private float Damage { get; set; }
    /// <summary>
    /// The object to be lockpicked.
    /// </summary>
    public Interactable Interactable { get; set; }
    /// <summary>
    /// The player lockpicking.
    /// </summary>
    public PlayerController PlayerController { get; set; }

    #endregion

    #region --- Methods ---

    private void Start()
    {
        bobbyPinAnimator = bobbyPin.GetComponent<Animator>();

        OnUnlock += (LockController sender) =>
        {
            if (quitButton != null)
                quitButton.SetActive(false);

            unlockSFX.Play();

            active = false;

            Interactable.Unlock(PlayerController);
        };
    }

    public void Initialize()
    {
        if (quitButton != null)
            quitButton.SetActive(true);

        //Generate a random correct angle.
        Angle = Random.Range(maxMinRotation.x, maxMinRotation.y);
        Tolerance = 5f;
        Damage = 0;
        active = true;

        //Reset the lockpick and bobby pin sprite rotation.
        if (lockPick != null)
            lockPick.transform.localRotation = Quaternion.identity;

        if (bobbyPin != null)
            bobbyPin.transform.localRotation = Quaternion.identity;

        //Remove a bobby pin from the players inventory.
        PlayerController.Inventory.Remove("Bobby Pin");
        //Set the label text to players bobby pin ammount.
        bobbyPinsAmmount.text = "+" + PlayerController.Inventory.GetAmmount("Bobby Pin").ToString();
    }

    /// <summary>
    /// Plays a sound when starting lockpick.
    /// </summary>
    public void PlayEnterSound()
    {
        if (enterSFX != null)
            enterSFX.Play();
    }

    /// <summary>
    /// Closes the lockpick HUD.
    /// </summary>
    public void Close()
    {
        active = false;

        GameInstance.GameState.Paused = false;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Closes the lockpick HUD with a delay.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator Close(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        Close();
    }

    /// <summary>
    /// Clamps a given angle between 0 and 360.
    /// </summary>
    /// <param name="currentValue"></param>
    /// <returns></returns>
    private float ClampAngle(float currentValue)
    {
        float angle = currentValue - 180;

        while (angle < 0)
            angle += 360;

        angle = Mathf.Repeat(angle, 360);
        return Mathf.Clamp(angle - 180, maxMinRotation.x, maxMinRotation.y) + 360;
    }

    /// <summary>
    /// Updates the bobby pin rotation and plays lockpick sounds. 
    /// </summary>
    private void UpdateBobbyPin()
    {
        if (picking)
            return;

        Vector3 lookAt = Input.mousePosition;
        float lastBobbyPinAngle = BobbyPinAngle;
        BobbyPinAngle = ClampAngle(Mathf.Atan2(lookAt.y - bobbyPin.transform.position.y, lookAt.x - bobbyPin.transform.position.x) * Mathf.Rad2Deg - 90f);
        bobbyPin.transform.localRotation = Quaternion.Euler(0f, 0f, BobbyPinAngle);

        float d = Random.Range(0, 100);
        if (d > 30 && !lockpickMovement.isPlaying && Mathf.Abs(BobbyPinAngle - lastBobbyPinAngle) > 2f)
        {
            int randomClip = Random.Range(0, lockpickMovements.Length);
            lockpickMovement.clip = lockpickMovements[randomClip];
            lockpickMovement.Play();
        }
    }

    /// <summary>
    /// Updates the lock and gives damage to bobby pin if lockpick fails.
    /// </summary>
    private void UpdateLock()
    {
        float angle;

        //reset the lock's angle to 0 if we're not lockpicking.
        if (!picking)
        {
            angle = Mathf.MoveTowardsAngle(lockPick.transform.localEulerAngles.z, 0, Time.unscaledDeltaTime * pickSpeed);
            lockPick.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            return;
        }

        //Calculate the distance between the angle and the correct angle.
        float distance = Mathf.Abs(Angle - (BobbyPinAngle - 360));
        float maxDistance = Mathf.Max(Mathf.Abs(100 + Angle), Mathf.Abs(100 - Angle));
        //Calculate the influence (The ammount wich the lock will rotate).
        float influence = Mathf.Abs( 90f * (1 - distance / maxDistance));
        angle = Mathf.MoveTowardsAngle(lockPick.transform.localEulerAngles.z, influence, Time.unscaledDeltaTime * pickSpeed);
        lockPick.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        //Check if lock is suitable to be uncloked. otherwise if the lock reaches
        //the influence angle minus 2 degrees, damage the bobby pin and rotate it 
        //towards 0 angle.
        if (lockPick.transform.localEulerAngles.z > 90 - Tolerance)
            OnUnlock?.Invoke(this);
        else if (Mathf.Abs(angle - influence) < 2f)
        {
            lockPick.transform.localRotation = Quaternion.Euler(0f, 0f, angle - pickPullback);
            Damage += 0.2f;
        }

        //If the current Damage is bigger than 3, break the bobby pin.
        if (Damage > 3)
        {
            //Remove a bobby pin from the players inventory.
            int ammount = PlayerController.Inventory.GetAmmount("Bobby Pin");
            PlayerController.Inventory.Remove("Bobby Pin");
            //Update Bobby pin label to the new bobby pin ammount.
            bobbyPinsAmmount.text = "+" + Mathf.Max(ammount - 1, 0).ToString();

            //Deactive breifely the HUD
            active = false;
            //Play bobby pin breaking sound.
            bobbyPinBreak.Play();

            //if the current bobby pin ammount is 0 then close the lockpick HUD
            if (ammount == 0)
            {
                StartCoroutine(Close(3f));
                return;
            }

            //Deactivate the Bobby pin sprite.
            bobbyPin.SetActive(false);
            //Reset the Bobby Pin
            StartCoroutine(Reset());
        }
    }

    /// <summary>
    /// Resets the Bobby pin whitin a second.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(1f);

        bobbyPin.SetActive(true);
        active = true;
        Damage = 0;
    }

    private void Update()
    {
        //Update the bobby pin break animation.
        bobbyPinAnimator.SetFloat("Damage", Damage);

        //If the HUD is deactivated, then go no further.
        if (!active)
            return;

        //If the user presses the "Escape" Key then quit lockpicking.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        //Picking is true if the user is pressing the "Space" key.
        picking = Input.GetKey(KeyCode.Space);

        //Updates the Bobby Pin.
        UpdateBobbyPin();

        //Updates the lock.
        UpdateLock();
    }

    #endregion
}
