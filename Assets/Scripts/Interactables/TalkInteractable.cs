using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkInteractable : Interactable
{
    public CONVERSATION currentConvo;

    protected override void OnInteract(PlayerController controller)
    {
        if (currentConvo != null)
            GameInstance.HUD.EnableConversation(true, this, controller);
    }
}
