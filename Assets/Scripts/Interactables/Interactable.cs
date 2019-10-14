using UnityEngine;
abstract public class Interactable : MonoBehaviour
{
    public delegate void EventHandler(Interactable sender);

    public event EventHandler OnUnlocked;

    [SerializeField]
    private bool canInteract = true;
    [SerializeField]
    private bool locked = false;
    [SerializeField]
    private string message = "Interact";
    private UniqueID uniqueID;

    public bool Locked
    {
        get { return locked; }

        set
        {
            locked = value;

            if (!locked)
                OnUnlocked?.Invoke(this);
        }
    }
    public string Message { get { return message; } }

    protected virtual void Awake()
    {
        uniqueID = GetComponent<UniqueID>();
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

    protected bool GetUniqueIDPersistent()
    {
        if (uniqueID != null)
            return uniqueID.persistentAcrossLevels;

        return false;
    }

    protected abstract void OnInteract(PlayerController controller);
}