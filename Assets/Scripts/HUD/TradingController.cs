using UnityEngine;

public class TradingController : MonoBehaviour
{
    public delegate void EventHandler(TradingController sender);

    public event EventHandler OnClose;

    public PlayerController PlayerController { get; private set; }
    public TalkInteractable Interactable { get; private set; }

    public void Initialize(TalkInteractable interactable, PlayerController playerController)
    {
        Interactable = interactable;
        PlayerController = playerController;
    }

    public void Close()
    {
        GameInstance.HUD.EnableTrading(false);
        OnClose?.Invoke(this);
    }
}
