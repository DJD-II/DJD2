using UnityEngine;

public class TalkInteractable : Interactable
{
    public Conversation currentConvo;

    protected override void OnInteract(PlayerController controller)
    {
        if (currentConvo != null)
            GameInstance.HUD.EnableConversation(true, this, controller);
        else
            Debug.LogError("No CurrentConvo");
    }
}
