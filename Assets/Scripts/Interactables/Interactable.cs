using UnityEngine;
abstract public class Interactable : MonoBehaviour
{
    public delegate void EventHandler(Interactable sender, PlayerController controller);

    public event EventHandler OnUnlocked;

    [SerializeField]
    private bool canInteract = true;
    [SerializeField]
    private bool locked = false;
    [SerializeField]
    private string message = "Interact";
    private UniqueID uniqueID;

    public bool CanInteract { get { return canInteract; } protected set { canInteract = value; } }
    public bool Locked { get { return locked; }  protected set { locked = value; } }
    public string Message { get { return message; } }

    protected virtual void Awake()
    {
        uniqueID = GetComponent<UniqueID>();
        GameInstance.OnLoad += OnLoad;
    }

    protected virtual void Start()
    {
        GameInstance.OnSave += OnSave;
    }

    public void Interact(PlayerController controller)
    {
        if (!canInteract)
            return;

        OnInteract(controller);
    }

    protected string GetUniqueID()
    {
        if (uniqueID != null)
            return uniqueID.uniqueId;

        return "";
    }

    protected virtual void OnSave (PlaySaveGameObject io)
    {

    }

    protected virtual void OnLoad(PlaySaveGameObject io)
    {

    }

    protected bool GetUniqueIDPersistent()
    {
        if (uniqueID != null)
            return uniqueID.persistentAcrossLevels;

        return false;
    }

    public void Unlock (PlayerController controller)
    {
        if (!locked)
            return;

        locked = false;

        OnUnlocked?.Invoke(this, controller);
    }

    protected virtual void OnDestroy()
    {
        GameInstance.OnSave -= OnSave;
        GameInstance.OnLoad -= OnLoad;
    }

    protected abstract void OnInteract(PlayerController controller);
}