using UnityEngine;
using UnityEngine.SceneManagement;

public class HackInteractable : Interactable, ISavable
{
    [System.Serializable]
    public class HackSavable : Savable
    {
        private bool Locked { get; }

        public HackSavable(HackInteractable interactable)
            : base(interactable.UniqueID, SceneManager.GetActiveScene().name)
        {
            Locked = interactable.Locked;
        }

        public void Set(HackInteractable interactable)
        {
            interactable.Locked = Locked;
        }
    }

    [Header("Hacking")]
    [SerializeField] private Hack hack = null;

    public Hack Hack { get => hack; }
    Savable ISavable.IO { get { return new HackSavable(this); } }

    protected override void OnInteract(PlayerController controller)
    {
        GameInstance.HUD.EnableHacking(true, this, controller);
    }

    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Get(UniqueID,
                     PersistentAcrossLevels) is HackSavable savable))
            return;

        savable.Set(this);
    }

    protected override void OnSave(SaveGame io)
    {
        base.OnSave(io);

        io.Override(this, UniqueID, false);
    }
}
