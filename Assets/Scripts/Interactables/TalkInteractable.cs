using System.Collections.Generic;
using UnityEngine;

sealed public class TalkInteractable : Interactable
{
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private new string name = "";
    [SerializeField] private List<Conversation> conversations = null;
    [SerializeField] private bool rotateTowardsPlayer = true;
    [SerializeField] private bool changeAnimator = true;
    [SerializeField] private bool rotateToInitialRotation = true;
    [SerializeField] private bool unpauseOnClose = true;
    [Header("On Interact")]
    [Tooltip("The items the player will earn if he has interacted with " + 
        "this object.")]
    [SerializeField] private List<Item> itemsToEarn = new List<Item>();
    [Tooltip("The quests the player will earn if he has interacted with " +
        "this object.")]
    [SerializeField] private List<Quest> questsToEarn = new List<Quest>();
    [Tooltip("The items the player will earn if he has interacted with " +
        "this object.")]
    [SerializeField] private List<Event> eventsToGive = new List<Event>();
    [Tooltip("The items the player will lose if he has interacted with " +
        "this object.")]
    [SerializeField] private List<Item> itemsToGive = new List<Item>();
    [Tooltip("The quests the player will complete if he has interacted with " +
        "this object.")]
    [SerializeField] private List<Quest> questsToComplete = new List<Quest>();
    private bool listening = false;

    public bool OverrideRotation { get; set; }
    public bool RotateToInitialRotation
    {
        get => rotateToInitialRotation;

        set => rotateToInitialRotation = value;
    }
    public AudioSource AudioSource { get => audioSource; }
    public string Name { get => name; }
    public bool UnpauseOnClose { get => unpauseOnClose; }
    public RuntimeAnimatorController InitController { get; private set; }
    public RuntimeAnimatorController Controller { get; private set; }
    public List<Conversation> Conversations { get => conversations; }
    public Conversation Conversation { get; private set; }
    public bool IsTalking { get; set; }
    public Quaternion InitRotation { get; set; }
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
            if (changeAnimator)
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
        if (!IsTalking && rotateToInitialRotation && !OverrideRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                 InitRotation,
                                                 Time.deltaTime * 3f);
        else if (IsTalking && rotateTowardsPlayer)
        {
            Vector3 dir = PlayerController.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                 Quaternion.LookRotation(dir),
                                                 Time.unscaledDeltaTime * 3f);
        }
    }

    protected override void OnInteract(PlayerController controller)
    {
        Conversation = null;
        List<Conversation> reversedConversation = new List<Conversation>(Conversations);
        reversedConversation.Reverse();
        foreach (Conversation c in reversedConversation)
            if (c.Fullfills(controller))
            {
                Conversation = c;
                break;
            }

        if (Conversation != null)
        {
            GameInstance.GameState.QuestController.CompleteQuests(questsToComplete);
            controller.Inventory.Add(itemsToEarn);
            controller.Inventory.Remove(itemsToGive);
            GameInstance.GameState.EventController.Add(eventsToGive);

            PlayerController = controller;
            GameInstance.GameState.QuestController.CompleteQuest("Talk To Someone!");
            GameInstance.HUD.EnableConversation(true, this, controller);
            GameInstance.HUD.OnTalkClose += OnTalkOver;

            Animator animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            if (changeAnimator)
                animator.runtimeAnimatorController = Controller;

            IsTalking = true;
            Listening = false;
        }
        else
            Debug.Log("No Conversation");
    }

    public void Talk(AudioClip clip)
    {
        if (audioSource == null)
            return;

        audioSource.clip = clip;

        if (clip != null)
            audioSource.Play();
    }

    public void StopTalk()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    private void OnTalkOver(HUD sender)
    {
        GameInstance.HUD.OnTalkClose -= OnTalkOver;

        Animator animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.Normal;
        if (changeAnimator)
            animator.runtimeAnimatorController = InitController;
    }
}