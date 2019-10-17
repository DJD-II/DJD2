using System.Collections.Generic;
using UnityEngine;
using System;


sealed public class TalkInteractable : Interactable
{
    [SerializeField]
    private List<Conversation> conversations = null;

    public RuntimeAnimatorController InitController { get; private set; }
    public RuntimeAnimatorController Controller { get; private set; }
    public List<Conversation> Conversations { get => conversations; }
    public Conversation Conversation { get; private set; }
    public bool IsTalking { get; set; }
    private Quaternion InitRotation { get; set; }

    protected override void Awake()
    {
        base.Awake();
        InitRotation = transform.rotation;
        Controller = Resources.Load<RuntimeAnimatorController>("Animations/Talk");
        InitController = GetComponent<Animator>().runtimeAnimatorController;
    }

    private void Update()
    {
        if (!IsTalking)
            transform.rotation = Quaternion.Lerp(transform.rotation, InitRotation, Time.unscaledDeltaTime * 3f);
    }

    protected override void OnInteract(PlayerController controller)
    {
        Conversation = null;
        List<Conversation> reversedConversation = new List<Conversation>(Conversations);
        reversedConversation.Reverse();
        foreach (Conversation c in reversedConversation)
            if (c.Requesites.Fullfills(controller))
            {
                Conversation = c;
                break;
            }

        if (Conversation != null)
        {
            GameInstance.HUD.EnableConversation(true, this, controller);
            IsTalking = true;
        }
        else
            Debug.LogError("No Conversation");
    }
}

