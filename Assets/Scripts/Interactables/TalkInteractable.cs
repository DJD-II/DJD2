using UnityEngine;

sealed public class TalkInteractable : Interactable
{
    [SerializeField]
    private Conversation currentConvo = null;

    public Conversation Conversation { get { return currentConvo; } }

    protected override void OnInteract(PlayerController controller)
    {
        if (currentConvo != null)
            GameInstance.HUD.EnableConversation(true, this, controller);
        else
            Debug.LogError("No CurrentConvo");
    }
}
