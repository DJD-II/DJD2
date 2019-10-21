using System.Collections.Generic;
using UnityEngine;
using System;


sealed public class TalkInteractable : Interactable
{
    [SerializeField]
    private List<Conversation> conversations = null;
    [SerializeField]
    private bool rotateTowardsPlayer = true;
    [SerializeField]
    private bool changeAnimator = true;
    [SerializeField]
    private bool rotateToInitialRotation = true;
    [SerializeField]
    private bool unpauseOnClose = true;
    private bool listening = false;

    public bool UnpauseOnClose { get => unpauseOnClose; }
    public RuntimeAnimatorController InitController { get; private set; }
    public RuntimeAnimatorController Controller { get; private set; }
    public List<Conversation> Conversations { get => conversations; }
    public Conversation Conversation { get; private set; }
    public bool IsTalking { get; set; }
    private Quaternion InitRotation { get; set; }
    private PlayerController PlayerController { get; set; }
    public bool Listening
    {
        get
        {
            return listening;
        }

        set
        {
            listening = value;
            GetComponent<Animator>().SetBool("Talking", !value);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitRotation = transform.rotation;

        Controller = Resources.Load<RuntimeAnimatorController>("Animations/Talk");
        InitController = GetComponent<Animator>().runtimeAnimatorController;
    }

    private void Update()
    {
        if (!IsTalking && rotateToInitialRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, InitRotation, Time.unscaledDeltaTime * 3f);
        else if (IsTalking && rotateTowardsPlayer)
        {
            Vector3 dir = PlayerController.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.unscaledDeltaTime * 3f);
        }
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
            PlayerController = controller;
            GameInstance.GameState.QuestController.CompleteQuest("Talk To Someone!");
            GameInstance.HUD.EnableConversation(true, this, controller);
            GameInstance.HUD.OnTalkClose += OnTalkOver;

            Animator animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.runtimeAnimatorController = Controller;

            IsTalking = true;
        }
        else
            Debug.Log("No Conversation");
    }

    private void OnTalkOver(HUD sender)
    {
        GameInstance.HUD.OnTalkClose -= OnTalkOver;

        Animator animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.runtimeAnimatorController = InitController;
    }
}

