using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

sealed public class ItemInteractable : Interactable, ISavable
{
    [System.Serializable]
    sealed public class ItemSavable : Savable
    {
        private bool exists;
        private float X { get; }
        private float Y { get; }
        private float Z { get; }
        private float Qx { get; }
        private float Qy { get; }
        private float Qz { get; }

        public ItemSavable (ItemInteractable item)
            : base (item.UniqueID, SceneManager.GetActiveScene().name)
        {
            exists = item.gameObject.activeInHierarchy;

            X = item.transform.position.x;
            Y = item.transform.position.y;
            Z = item.transform.position.z;

            Vector3 euler = item.transform.eulerAngles;
            Qx = euler.x;
            Qy = euler.y;
            Qz = euler.z;
        }

        public void Set(ItemInteractable item)
        {
            item.gameObject.SetActive(exists);

            item.transform.position = new Vector3(X, Y, Z);
            item.transform.rotation = Quaternion.Euler(Qx, Qy, Qz);
        }
    }
    
    [SerializeField] Item.Tag itemTag = Item.Tag.Bobby_Pin;

    Savable ISavable.IO { get => new ItemSavable(this); }

    protected override void OnInteract(PlayerController controller)
    {
        controller.Inventory.Add(ItemUtility.GetItem(itemTag.ToString()));
        gameObject.SetActive(false);
    }

    protected override void OnSave(SaveGame io)
    {
        base.OnSave(io);

        io.Override(this, UniqueID, PersistentAcrossLevels);
    }

    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Get(UniqueID,
                      PersistentAcrossLevels) is ItemSavable savable))
            return;

        savable.Set(this);
    }
}
