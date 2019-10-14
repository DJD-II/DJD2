using System.Collections.Generic;
using UnityEngine;

sealed public class LootInteractable : Interactable, ISavable
{
    [System.Serializable]
    public class LootSavable : Savable
    {
        public bool locked;
        public List<ItemID> items;

        public LootSavable(string id, List<Item> items, bool locked)
            : base(id, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            this.id = id;
            this.items = new List<ItemID>();
            foreach (Item i in items)
                this.items.Add(new ItemID(i));
            this.locked = locked;
        }
    }

    [SerializeField]
    private Item[] items = new Item[0];
    private Inventory inventory;

    public Inventory Inventory { get { return inventory; } }

    protected override void Awake()
    {
        base.Awake();

        GameInstance.OnLoad += OnLoaded;

        inventory = new Inventory();
    }

    private void Start()
    {
        GameInstance.OnSave += OnSceneChange;
    }

    private void OnLoaded()
    {
        LootSavable savable = GameInstance.Singleton.GetSavable(GetUniqueID(), UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, GetUniqueIDPersistent()) as LootSavable;
        if (savable != null)
        {
            Locked = savable.locked;

            foreach (ItemID i in savable.items)
                inventory.Add(ItemUtility.GetItem(i.name));

            return;
        }

        foreach (Item item in items)
            inventory.Add(item);
    }

    private void OnSceneChange()
    {
        GameInstance.Singleton.FeedSavable(this, GetUniqueIDPersistent());
    }

    protected override void OnInteract(PlayerController controller)
    {
        if (Locked && controller.Inventory.Contains("Bobby Pin"))
            GameInstance.HUD.EnableLockPick(true, this, controller);
        else if (!Locked)
            GameInstance.HUD.EnableObjectInventory(this, controller);
    }

    void OnDestroy()
    {
        GameInstance.OnSave -= OnSceneChange;
    }

    Savable ISavable.IO { get { return new LootSavable(GetUniqueID(), Inventory.Items, Locked); } }
}
