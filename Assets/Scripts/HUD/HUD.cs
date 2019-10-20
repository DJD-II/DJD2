using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

sealed public class HUD : MonoBehaviour
{
    [Header("CrossHair")]
    [SerializeField]
    private Text crossHair = null;
    [Header("Interaction")]
    [SerializeField]
    private TextMeshProUGUI interactMessage = null;
    [Header("Loot")]
    [SerializeField]
    private LootInventory loot = null;
    [Header("News")]
    [SerializeField]
    private GameObject digitalNewsPaper = null;
    [Header("Lock Pick")]
    [SerializeField]
    private LockController lockPickController = null;
    [Header("Menu")]
    [SerializeField]
    private HUDMenuController menu = null;
    [Header("Conversation")]
    [SerializeField]
    private TalkUIController talkUIController = null;
    [Header("Loading Screen")]
    [SerializeField]
    private GameObject loadingScreenPanel = null;
    [Header("Past Lifes")]
    [SerializeField]
    private Image pastLifePanel = null;
    [Header("Fade")]
    [SerializeField]
    private GameObject fadeToWhitePanel = null;
    [SerializeField]
    private GameObject fadeToBlackPanel = null;
    [SerializeField]
    private GameObject maskPanel = null;

    public TalkUIController TalkUIController { get { return talkUIController; } }

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

    public void EnableInteractMessage(bool visible, Interactable interactable)
    {
        if (interactMessage == null)
            return;

        interactMessage.gameObject.SetActive(visible);
        interactMessage.text = interactable == null ? "" : "[E] " + interactable.Message + (interactable.Locked ? " [LOCKED]" : "");
    }

    public void EnableObjectInventory(LootInteractable interactable, PlayerController controller)
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

    public void EnableLockPick(bool enable, Interactable interactable, PlayerController controller)
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
        talkUIController.gameObject.SetActive(true);
        talkUIController.Initialize();
    }

    public void EnableMenu(bool enable, PlayerController controller)
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
            menu.gameObject.SetActive(false);
        }
    }

    public void EnableLoadingScreen(bool enable)
    {
        loadingScreenPanel.SetActive(enable);
    }

    public void MaskScreen (bool enable)
    {
        maskPanel.SetActive(enable);
        if (!enable)
            return;

        maskPanel.GetComponent<Animation>().Play();
    }

    public IEnumerator FadeToWhite (float multiplier = 1f)
    {
        Animation anim = fadeToWhitePanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha Reversed");
        anim["Alpha Reversed"].normalizedSpeed = anim["Alpha Reversed"].normalizedSpeed * multiplier;
        anim.Play();

        yield return WaitWhileAnimation(anim);
    }

    public IEnumerator FadeFromWhite (float multiplier = 1f)
    {
        Animation anim = fadeToWhitePanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha");
        anim.Play();

        yield return WaitWhileAnimation(anim);
    }

    public IEnumerator FadeToBlack(float multiplier = 1f)
    {
        Animation anim = fadeToBlackPanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha Reversed");
        anim["Alpha Reversed"].normalizedSpeed = anim["Alpha Reversed"].normalizedSpeed * multiplier;

        anim.Play();

        yield return WaitWhileAnimation(anim);
    }

    public IEnumerator FadeFromBlack(float multiplier = 1f)
    {
        Animation anim = fadeToBlackPanel.GetComponent<Animation>();
        anim.clip = anim.GetClip("Alpha");
        anim["Alpha"].normalizedSpeed = anim["Alpha"].normalizedSpeed * multiplier;
        anim.Play();

        yield return WaitWhileAnimation(anim);
    }

    private IEnumerator WaitWhileAnimation (Animation anim)
    {
        while (anim.isPlaying)
            yield return null;
    }

    public void EnablePastLife(bool enable)
    {
        pastLifePanel.gameObject.SetActive(enable);
    }
}
