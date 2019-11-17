using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

sealed public class HUD : MonoBehaviour
{
    public delegate void EventHandler(HUD sender);

    public event EventHandler   OnTalkBegin,
                                OnTalkClose;

    [Header("CrossHair")]
    [SerializeField] private Text crossHair = null;
    [Header("Interaction")]
    [SerializeField] private Text interactMessage = null;
    [Header("Loot")]
    [SerializeField] private LootInventory loot = null;
    [Header("News")]
    [SerializeField] private DigitalNewsPaperController digitalNewsPaper = null;
    [Header("Lock Pick")]
    [SerializeField] private LockController lockPickController = null;
    [Header("Hack")]
    [SerializeField] private HackController hackController = null;
    [Header("Trade")]
    [SerializeField] private TradingController tradingController = null;
    [Header("Menu")]
    [SerializeField] private HUDMenuController menu = null;
    [Header("Conversation")]
    [SerializeField] private TalkUIController talkUIController = null;
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreenPanel = null;
    [Header("Past Lifes")]
    [SerializeField] private Image pastLifePanel = null;
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu = null;
    [Header("Fade")]
    [SerializeField] private GameObject fadeToWhitePanel = null;
    [SerializeField] private GameObject fadeToBlackPanel = null;
    [SerializeField] private GameObject maskPanel = null;

    public TradingController TradingController { get => tradingController; }
    public TalkUIController TalkUIController { get { return talkUIController; } }

    /// <summary>
    /// Initializes the HUD System.
    /// </summary>
    private void Start()
    {
        GameInstance.GameState.OnPausedChanged += OnPauseHasChanged;
    }

    private void OnDestroy()
    {
        GameInstance.GameState.OnPausedChanged -= OnPauseHasChanged;
    }

    /// <summary>
    /// This method is called when the game pause state
    /// changes.
    /// </summary>
    /// <param name="sender">The Game State.</param>
    private void OnPauseHasChanged(GameState sender)
    {
        EnableInteractMessage(false);
        EnableCrossHair(!sender.Paused);
    }

    /// <summary>
    /// Enables/Disables the crosshair. 
    /// </summary>
    /// <param name="enable">If the crosshair should enabled/Disabled.</param>
    public void EnableCrossHair(bool enable)
    {
        crossHair.gameObject.SetActive(enable);
    }

    /// <summary>
    /// Enables/Disables the interact message. When the player is near something
    /// that can be interacted with this message should pop up.
    /// </summary>
    /// <param name="visible">The vsibility of the message.</param>
    /// <param name="interactable">The object the player can interact with.</param>
    public void EnableInteractMessage(bool visible, Interactable interactable = null)
    {
        interactMessage.gameObject.SetActive(visible);
        interactMessage.text =
            interactable == null ? "" :
            "[E] " + interactable.Message +
            (interactable.Locked ? " [LOCKED]" : "");
    }

    /// <summary>
    /// Enables/Disables the object inventory list. As the player interacts
    /// with lootable objects this HUD shows what items are in the object.
    /// It allows the player to exchange items.
    /// </summary>
    /// <param name="interactable">The lootable object.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableObjectInventory(LootInteractable interactable,
                                      PlayerController controller)
    {
        GameInstance.GameState.Paused = true;

        loot.Interactable = interactable;
        loot.PlayerInventory = controller.Inventory;
        loot.gameObject.SetActive(true);
        loot.Initialize();
    }

    /// <summary>
    /// Enables/Disables the newspaper HUD.
    /// As the player interacts with tablets this HUD should pop up.
    /// </summary>
    /// <param name="enable">The visibility if the HUD.</param>
    /// <param name="news">The current news.</param>
    public void EnableDigitalNewsPaper(bool enable, News[] news = null)
    {
        digitalNewsPaper.gameObject.SetActive(enable);

        GameInstance.GameState.Paused = enable;

        if (enable)
            digitalNewsPaper.Initialize(news);
    }

    /// <summary>
    /// Enables/Disables Hacking HUD. When trying to hack computers,
    /// This HUD should pop up.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    /// <param name="interactable">The computer to be hacked.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableHacking(bool enable,
                               HackInteractable interactable = null,
                               PlayerController controller = null)
    {
        GameInstance.GameState.Paused = enable;
        hackController.gameObject.SetActive(enable);

        if (enable)
            hackController.Initialize(interactable, controller);
    }

    /// <summary>
    /// Enables/Disables lock pick HUD. When the player tries
    /// to unlock locked objects, this HUD should pop up.
    /// It enables the player to try an lock pick the locked object.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    /// <param name="interactable">The object to be lock picked.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableLockPick(bool enable,
                               Interactable interactable = null,
                               PlayerController controller = null)
    {
        lockPickController.gameObject.SetActive(enable);

        if (enable)
        {
            if (interactable == null || controller == null)
                return;

            GameInstance.GameState.Paused = true;

            lockPickController.Interactable = interactable;
            lockPickController.PlayerController = controller;
            lockPickController.Initialize();
            lockPickController.PlayEnterSound();
        }
    }

    /// <summary>
    /// Enables/Disables conversation HUD. When the player starts a 
    /// conversation with NPCs this HUD should pop up.
    /// It enables the player to actively talk with the NPC.
    /// </summary>
    /// <param name="enable">The visibility of HUD.</param>
    /// <param name="interactable">The object to talk to.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableConversation(bool enable,
                                   TalkInteractable interactable = null,
                                   PlayerController controller = null)
    {
        bool opened = talkUIController.gameObject.activeInHierarchy;

        talkUIController.gameObject.SetActive(enable);

        if (!enable)
        {
            if (opened)
                OnTalkClose?.Invoke(this);

            return;
        }

        OnTalkBegin?.Invoke(this);

        GameInstance.GameState.Paused = true;

        talkUIController.Interactable = interactable;
        talkUIController.PlayerController = controller;
        talkUIController.Initialize();
    }

    /// <summary>
    /// Enables/Disables the trading HUD. When the player is actively
    /// trading with an NPC this HUD should pop up.
    /// It enables the player to exchange any items with the trader.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    /// <param name="interactable">The object to talk to.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableTrading(bool enable,
                               TalkInteractable interactable = null,
                               PlayerController controller = null)
    {
        if (enable)
            tradingController.Initialize(interactable, controller);

        tradingController.gameObject.SetActive(enable);
    }

    /// <summary>
    /// Enables/Disables the main menu. By pressing a key possibly escape
    /// this HUD should pop up. It enables the player to check for its inventory
    /// as well as objectives and load save the game.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    /// <param name="controller">The player controller.</param>
    public void EnableMenu(bool enable, PlayerController controller = null)
    {
        if (enable)
        {
            GameInstance.GameState.Paused = true;

            menu.Initialize(controller);
            menu.gameObject.SetActive(true);
        }
        else
        {
            GameInstance.GameState.Paused = false;
            menu.Close();
        }
    }

    public void EnableMainMenu(bool enable)
    {
        mainMenu.SetActive(enable);
    }

    /// <summary>
    /// Enables/Disables the loading screen. When a scene is about to load
    /// this HUD should pop up. It tells the player that the game is curretly 
    /// loading a new scene.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    public void EnableLoadingScreen(bool enable)
    {
        loadingScreenPanel.SetActive(enable);
    }

    /// <summary>
    /// Enables/Disables the screen mask. This is only used when transitioning 
    /// to the cloud. It masks the screen through a circular mask.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    public void MaskScreen(bool enable)
    {
        maskPanel.SetActive(enable);

        if (!enable)
            return;

        maskPanel.GetComponent<Animation>().Play();
    }

    /// <summary>
    /// Fades the screen from transparent to white.
    /// </summary>
    /// <param name="multiplier">The speed to fade.</param>
    /// <returns></returns>
    public IEnumerator FadeToWhite(float multiplier = 1f)
    {
        multiplier = Mathf.Max(multiplier, 0.001f);
        Animation anim = fadeToWhitePanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha Reversed");
        anim["Alpha Reversed"].normalizedSpeed = anim["Alpha Reversed"].normalizedSpeed * multiplier;
        anim.Play();

        yield return new WaitForAnimationEnd(anim);
    }

    /// <summary>
    /// Fades the screen from white to transparent.
    /// </summary>
    /// <param name="multiplier">The speed to fade to.</param>
    /// <returns></returns>
    public IEnumerator FadeFromWhite(float multiplier = 1f)
    {
        multiplier = Mathf.Max(multiplier, 0.001f);
        Animation anim = fadeToWhitePanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha");
        anim["Alpha"].normalizedSpeed = anim["Alpha"].normalizedSpeed * multiplier;
        anim.Play();

        yield return new WaitForAnimationEnd(anim);
    }

    /// <summary>
    /// Fades the screen from transparent to black.
    /// </summary>
    /// <param name="multiplier">The speed to fade to.</param>
    /// <returns></returns>
    public IEnumerator FadeToBlack(float multiplier = 1f)
    {
        multiplier = Mathf.Max(multiplier, 0.001f);
        Animation anim = fadeToBlackPanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha Reversed");
        anim["Alpha Reversed"].normalizedSpeed = anim["Alpha Reversed"].normalizedSpeed * multiplier;

        anim.Play();

        yield return new WaitForAnimationEnd(anim);
    }

    /// <summary>
    /// Fades the screen from black to transparent.
    /// </summary>
    /// <param name="multiplier">The speed to fade to.</param>
    /// <returns></returns>
    public IEnumerator FadeFromBlack(float multiplier = 1f)
    {
        multiplier = Mathf.Max(multiplier, 0.001f);
        Animation anim = fadeToBlackPanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha");
        anim["Alpha"].normalizedSpeed = anim["Alpha"].normalizedSpeed * multiplier;
        anim.Play();

        yield return new WaitForAnimationEnd(anim);
    }

    /// <summary>
    /// Enables/Disables the past lifes HUD.
    /// This screen shows the player choices. This screen should be
    /// poped up in the cloud scene.
    /// </summary>
    /// <param name="enable">The visibility of the HUD.</param>
    public void EnablePastLife(bool enable)
    {
        pastLifePanel.gameObject.SetActive(enable);
    }

    public void CheckItem(Item item, PlayerController controller = null)
    {
        if (item == null)
            return;

    }
}