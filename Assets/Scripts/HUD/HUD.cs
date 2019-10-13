using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("CrossHair")]
    public Text crossHair;
    [Header ("Interaction")]
    public TextMeshProUGUI interactMessage;
    [Header("Loot")]
    public LootInventory loot;
    [Header("News")]
    public GameObject digitalNewsPaper;
    [Header("Lock Pick")]
    public LockController lockPickController;
    [Header("Conversaton")]


    [Header("Menu")]
    [SerializeField]
    private HUDMenuController menu;
=======
    public TalkUIController talkUIController;
>>>>>>> origin/Freeze

    public void Initialize()
    {
        GameInstance.GameState.OnPausedChanged += (GameState sender) =>
        {
            if (interactMessage == null)
                return;

            EnableInteractMessage(false, null);
            crossHair.gameObject.SetActive(!sender.Paused);
        };
    }

    public void EnableInteractMessage (bool visible, Interactable interactable)
    {
        if (interactMessage == null)
            return;

        interactMessage.gameObject.SetActive(visible);
        interactMessage.text = interactable == null ? "" : "[E] " + interactable.Message + (interactable.Locked ? " [LOCKED]" : "");
    }

    public void EnableObjectInventory (LootInteractable interactable, PlayerController controller)
    {
        if (loot == null)
            return;

        GameInstance.GameState.Paused = true;

        loot.Interactable = interactable;
        loot.PlayerInventory = controller.Inventory;
        loot.gameObject.SetActive(true);

        loot.Initialize();
    }

    public void EnableDigitalNewsPaper(bool enable)
    {
        if (digitalNewsPaper == null)
            return;

        GameInstance.GameState.Paused = enable;

        digitalNewsPaper.gameObject.SetActive(enable);
    }

    public void EnableLockPick (bool enable, Interactable interactable, PlayerController controller)
    {
        if (lockPickController == null)
            return;
        
        GameInstance.GameState.Paused = true;

        lockPickController.Interactable = interactable;
        lockPickController.PlayerController = controller;
        lockPickController.Initialize();
        lockPickController.gameObject.SetActive(enable);
        lockPickController.PlayEnterSound();
    }

    public void EnableConversation(bool enable, TalkInteractable interactable, PlayerController controller)
    {
        if (talkUIController == null)
            return;

        GameInstance.GameState.Paused = true;

        talkUIController.Interactable = interactable;
        talkUIController.PlayerController = controller;
        talkUIController.Initialize();
        talkUIController.gameObject.SetActive(enable);
    }

    public void EnableMenu (PlayerController controller)
    {
        GameInstance.GameState.Paused = true;

        menu.Initialize(controller);
        menu.gameObject.SetActive(true);
    }
}
