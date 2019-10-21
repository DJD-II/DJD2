using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class DoorInteractable : Interactable, ISavable
{
    [System.Serializable]
    sealed public class DoorSavable : Savable
    {
        private bool locked;

        public bool Locked { get => locked; }

        public DoorSavable(DoorInteractable interactable)
            : base (interactable.GetUniqueID(), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            locked = interactable.Locked;
        }
    }

    [SerializeField]
    private Animation anim = null;

    public Animation Animation { get => anim; }
    Savable ISavable.IO { get => new DoorSavable(this); }

    protected override void Start ()
    {
        base.Start();

        OnUnlocked += (Interactable sender, PlayerController controller) =>
        {
            StartCoroutine(OpenDoor(controller));
        };
    }

    private IEnumerator OpenDoor (PlayerController controller)
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
                controller.PopMessage("Not Enough Bobby Pins");
        }
        else
            anim.Play();
    }

    protected override void OnLoad(PlaySaveGameObject io)
    {
        DoorSavable savable = io.Get(GetUniqueID(), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, GetUniqueIDPersistent()) as DoorSavable;
        if (savable != null)
        {
            Locked = savable.Locked;
            anim["ControlRoomDoor1"].normalizedTime = 1f;
            anim.Play();
        }
    }

    protected override void OnSave(PlaySaveGameObject io)
    {
        io.Feed(this, false);
    }
}
