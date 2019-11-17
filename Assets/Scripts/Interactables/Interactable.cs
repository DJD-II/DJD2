using UnityEngine;

[DisallowMultipleComponent]
abstract public class Interactable : MonoBehaviour
{
    public delegate void EventHandler(Interactable sender, PlayerController controller);

    public event EventHandler OnUnlocked;

    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool locked = false;
    [SerializeField] private string message = "Interact";
    private UniqueID uniqueID;

    public bool CanInteract { get { return canInteract; } protected set { canInteract = value; } }
    public bool Locked { get { return locked; } protected set { locked = value; } }
    public string Message { get { return message; } }
    public string UniqueID
    {
        get
        {
            if (uniqueID == null)
                return "";

            return uniqueID.Id;
        }
    }
    public bool PersistentAcrossLevels
    {
        get
        {
            if (uniqueID == null)
                return false;

            return uniqueID.PersistentAcrossLevels;
        }
    }

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

    protected abstract void OnInteract(PlayerController controller);

    protected virtual void OnSave(SaveGame io)
    {

    }

    protected virtual void OnLoad(SaveGame io)
    {

    }

    public void Unlock(PlayerController controller)
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
}