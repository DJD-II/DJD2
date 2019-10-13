using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMenuController : MonoBehaviour
{
    private PlayerController controller = null;
    [SerializeField]
    private GameObject inventoryMenu = null;
    [SerializeField]
    private Transform inventoryListContent = null;
    [SerializeField]
    private GameObject itemButton = null;
    [SerializeField]
    private Image itemIconImage = null;
    [SerializeField]
    private Text itemDescriptionLabel = null;
    [SerializeField]
    private ItemMenuController itemMenuController = null;

    private void Update()
    {
        if (gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Initialize(PlayerController controller)
    {
        this.controller = controller;
    }

    private void Close ()
    {
        inventoryMenu.SetActive(false);

        gameObject.SetActive(false);
        GameInstance.GameState.Paused = false;
    }

    private void OnItemHoverEnter(LootButton sender)
    {
        if (itemIconImage != null)
        {
            itemIconImage.gameObject.SetActive(true);
            itemIconImage.sprite = sender.Item.icon;
        }

        if (itemDescriptionLabel != null)
            itemDescriptionLabel.text = sender.Item.description;
    }

    private void OnItemHoverExit(LootButton sender)
    {
        if (itemIconImage != null)
            itemIconImage.gameObject.SetActive(false);

        if (itemDescriptionLabel == null)
            return;

        itemDescriptionLabel.text = "";
    }

    public void OnSideMenuClick(int id)
    {
        itemMenuController.gameObject.SetActive(false);
        inventoryMenu.SetActive(id == 0);

        switch(id)
        {
            case 0:
                InitializeInventoryList();
                break;
        }
    }

    private void InitializeInventoryList()
    {
        while (inventoryListContent.childCount > 0)
            DestroyImmediate(inventoryListContent.GetChild(0).gameObject);

        List<Item> items = controller.Inventory.Items.Distinct().ToList();

        foreach (Item i in items)
        {
            Debug.Log("Item " + i.name + " desc = " + i.description);
            GameObject go = Instantiate(itemButton, inventoryListContent);
            LootButton b = go.GetComponent<LootButton>();
            b.Initialize(i, controller.Inventory.Items.Where(x => x.tag.Equals(i.tag)).Count());
            b.OnHoverEnter += OnItemHoverEnter;
            b.OnHoverExit += OnItemHoverExit;
            b.OnClicked += (LootButton sender) =>
            {
                itemMenuController.Initialize(controller, sender);
                itemMenuController.gameObject.SetActive(true);
            };
        }
    }

    private void InitializeQuestList()
    {

    }
}
