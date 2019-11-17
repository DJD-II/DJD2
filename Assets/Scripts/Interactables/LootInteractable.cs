using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

sealed public class LootInteractable : Interactable, ISavable
{
    [System.Serializable]
    public class LootSavable : Savable
    {
        private bool Locked { get; }
        private List<ItemID> Items { get; }

        public LootSavable(LootInteractable interactable)
            : base(interactable.UniqueID, SceneManager.GetActiveScene().name)
        {
            Items = new List<ItemID>();
            foreach (Item i in interactable.Inventory)
                Items.Add(new ItemID(i));

            Locked = interactable.Locked;
        }

        public void Set(LootInteractable interactable)
        {
            interactable.Locked = Locked;

            foreach (ItemID i in Items)
                interactable.Inventory.Add(ItemUtility.GetItem(i.Name));
        }
    }

    [SerializeField]
    private Item[] items = new Item[0];

    public Inventory Inventory { get; } = new Inventory();
    Savable ISavable.IO { get { return new LootSavable(this); } }

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
        StartCoroutine(SwitchToLootInventory(controller));
    }

    private IEnumerator SwitchToLootInventory(PlayerController controller)
    {
        yield return new WaitForSecondsRealtime(2f);

        GameInstance.HUD.EnableLockPick(false);
        GameInstance.HUD.EnableObjectInventory(this, controller);
    }

    protected override void OnLoad(SaveGame io)
    {
        if (!(io.Get(UniqueID,
                     PersistentAcrossLevels) is LootSavable savable))
        {
            Inventory.Clear();
            foreach (Item item in items)
                Inventory.Add(item);

            return;
        }

        savable.Set(this);
    }

    protected override void OnSave(SaveGame io)
    {
        io.Override(this, UniqueID, PersistentAcrossLevels);
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
            GameInstance.HUD.EnableObjectInventory(this, controller);
    }
}