using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

sealed public class DoorInteractable : Interactable, ISavable
{
    [System.Serializable]
    sealed public class DoorSavable : Savable
    {
        public bool Locked { get; }

        public DoorSavable(DoorInteractable interactable)
            : base(interactable.UniqueID, SceneManager.GetActiveScene().name)
        {
            Locked = interactable.Locked;
        }

        public void Set(DoorInteractable interactable)
        {
            interactable.Locked = Locked;

            if (Locked)
                return;

            interactable.Animation["ControlRoomDoor1"].normalizedTime = 1f;
            interactable.Animation.Play();
        }
    }

    [SerializeField]
    private Animation anim = null;

    public Animation Animation { get => anim; }
    Savable ISavable.IO { get => new DoorSavable(this); }

    protected override void Start()
    {
        base.Start();

        OnUnlocked += OnHasBeenUnlocked;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        OnUnlocked -= OnHasBeenUnlocked;
    }

    private void OnHasBeenUnlocked(Interactable sender, PlayerController controller)
    {
        StartCoroutine(OpenDoor(controller));
    }

    private IEnumerator OpenDoor(PlayerController controller)
    {
        yield return new WaitForSecondsRealtime(2f);

        GameInstance.GameState.Paused = false;

        GameInstance.HUD.EnableLockPick(false);
        anim.Play();
        CanInteract = false;
    }

    protected override void OnInteract(PlayerController controller)
    {
        if (Locked)
        {
            if (controller.Inventory.Contains("Bobby Pin"))
                GameInstance.HUD.EnableLockPick(true, this, controller);
            else
                controller.HudSettings.PopMessage("Not Enough Bobby Pins");
        }
        else
            anim.Play();
    }

    protected override void OnLoad(SaveGame io)
    {
        if (!(io.Get(UniqueID,
                    PersistentAcrossLevels) is DoorSavable savable))
            return;

        savable.Set(this);
    }

    protected override void OnSave(SaveGame io)
    {
        io.Override(this, UniqueID, false);
    }
}
