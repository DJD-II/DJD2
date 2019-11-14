using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

sealed public class HUDMenuController : MonoBehaviour
{
    private enum SaveGameOperation : byte
    {
        Delete = 0,
        Load,
        Override,
    }

    private PlayerController controller = null;
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryMenu = null;
    [SerializeField] private Transform inventoryListContent = null;
    [SerializeField] private GameObject itemButton = null;
    [SerializeField] private Image itemIconImage = null;
    [SerializeField] private Text itemDescriptionLabel = null;
    [SerializeField] private ItemMenuController itemMenuController = null;
    [SerializeField] private Scrollbar verticalItemListScrollBar = null;

    [Header("Objectives")]
    [SerializeField] private Text objectiveDescriptionLabel = null;
    [SerializeField] private GameObject objectivesPanel = null;
    [SerializeField] private Transform objectivesListContent = null;
    [SerializeField] private GameObject objectiveButton = null;

    [Header("Save Menu")]
    [SerializeField] private GameObject savedGamesPanel = null;
    [SerializeField] private GameObject savedGameButton = null;
    [SerializeField] private Transform savedGamesContent = null;
    [SerializeField] private GameObject messagePanel = null;
    [SerializeField] private Text messagePanelTitle = null;
    private SaveGameOperation saveOperation = SaveGameOperation.Delete;
    private SaveGameButton currentSaveButton = null;
    [SerializeField] private InputField saveGameNameField = null;

    private void Awake()
    {
        itemMenuController.OnDiscard += OnItemDiscarded;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Initialize(PlayerController controller)
    {
        this.controller = controller;
    }

    private void OnItemDiscarded(ItemMenuController sender)
    {
        float value = verticalItemListScrollBar.value;
        InitializeInventoryList();
        verticalItemListScrollBar.value = value;
    }

    public void Close()
    {
        inventoryMenu.SetActive(false);
        objectivesPanel.SetActive(false);
        savedGamesPanel.SetActive(false);
        objectiveDescriptionLabel.text = "";
        messagePanel.SetActive(false);

        gameObject.SetActive(false);
        GameInstance.GameState.Paused = false;
    }

    private void OnItemHoverEnter(LootButton sender)
    {
        if (itemIconImage != null)
        {
            itemIconImage.gameObject.SetActive(true);
            itemIconImage.sprite = sender.Item.Icon;
        }

        if (itemDescriptionLabel != null)
            itemDescriptionLabel.text = sender.Item.Description;
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
        objectivesPanel.SetActive(id == 1);
        savedGamesPanel.SetActive(id == 3);

        switch (id)
        {
            case 0:
                InitializeInventoryList();
                break;
            case 1:
                InitializeQuestList();
                break;
            case 3:
                InitializeSavedGames();
                break;
        }
    }

    private void InitializeInventoryList()
    {
        while (inventoryListContent.childCount > 0)
            DestroyImmediate(inventoryListContent.GetChild(0).gameObject);

        List<Item> items = controller.Inventory.Distinct().ToList();
        items.Sort((x, y) => x.Name.CompareTo(y.Name));

        foreach (Item i in items)
        {
            GameObject go = Instantiate(itemButton, inventoryListContent);
            LootButton b = go.GetComponent<LootButton>();
            b.Initialize(i, 
                controller.Inventory.Where(x => x.ItemTag.Equals(i.ItemTag)).Count());
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
        while (objectivesListContent.childCount > 0)
            DestroyImmediate(objectivesListContent.GetChild(0).gameObject);

        foreach (QuestID i in GameInstance.GameState.QuestController.Quests)
        {
            GameObject go = Instantiate(objectiveButton, objectivesListContent);
            ObjectiveButton b = go.GetComponent<ObjectiveButton>();
            b.Initialize(i);
            b.OnHoverEnter += OnObjectiveHoverEnter;
            b.OnHoverExit += OnObjectiveHoverExit;
        }
    }

    private void OnObjectiveHoverEnter(ObjectiveButton sender)
    {
        objectiveDescriptionLabel.text = sender.QuestID.quest.description;
    }

    private void OnObjectiveHoverExit(ObjectiveButton sender)
    {
        objectiveDescriptionLabel.text = "";
    }

    private void InitializeSavedGames()
    {
        while (savedGamesContent.childCount > 0)
            DestroyImmediate(savedGamesContent.GetChild(0).gameObject);

        System.IO.FileInfo[] infos = IO.GetFilenames();

        foreach (System.IO.FileInfo info in infos)
        {
            GameObject go = Instantiate(savedGameButton, savedGamesContent);
            SaveGameButton button = go.GetComponent<SaveGameButton>();
            button.Initialize(info);
            button.OnOverride += OnOverrideSavedGame;
            button.OnDelete += OnSaveGameDelete;
            button.OnLoad += OnLoadSavedGame;
        }
    }

    public void OnSaveGame()
    {
        if (string.IsNullOrEmpty(saveGameNameField.text))
            return;

        GameInstance.Save(saveGameNameField.text + ".sv");
        saveGameNameField.text = "";
        InitializeSavedGames();
    }

    private void OnOverrideSavedGame(SaveGameButton sender)
    {
        messagePanelTitle.text = "Are you sure you want to override?";
        messagePanel.SetActive(true);
        saveOperation = SaveGameOperation.Override;
        currentSaveButton = sender;
    }

    private void OnSaveGameDelete(SaveGameButton sender)
    {
        messagePanelTitle.text = "Are you sure you want to delete?";
        messagePanel.SetActive(true);
        saveOperation = SaveGameOperation.Delete;
        currentSaveButton = sender;
    }

    private void OnLoadSavedGame(SaveGameButton sender)
    {
        messagePanelTitle.text = "Are you sure you want to load? Any unsaved progress will be lost!";
        messagePanel.SetActive(true);
        saveOperation = SaveGameOperation.Load;
        currentSaveButton = sender;
    }

    public void OnMessageButtonAccept()
    {
        string name = currentSaveButton.FileInfo.Name;

        switch (saveOperation)
        {
            case SaveGameOperation.Delete:
                IO.Delete(name);
                InitializeSavedGames();
                break;

            case SaveGameOperation.Load:
                Close();
                GameInstance.HUD.EnableMainMenu(false);
                GameInstance.Load(name);
                return;

            case SaveGameOperation.Override:
                GameInstance.Save(name);
                InitializeSavedGames();
                break;
        }

        messagePanel.SetActive(false);
    }

    public void OnMessagePanelDeclineButton()
    {
        messagePanel.SetActive(false);
    }
}