using System.Collections.Generic;
using UnityEngine;
using System;

public interface IOperatable<S> where S : IConvertible
{
    bool Get(LogicOperator operation, S value);
}

[System.Serializable]
public class ConversationRequesite
{
    [System.Serializable]
    public abstract class Operation<T, S> where T : IOperatable<S> where S : IConvertible, new()
    {
        [SerializeField]
        protected LogicOperator operation = LogicOperator.Equal;
        [SerializeField]
        protected S obj;

        public abstract bool Calculate(T value);
    }

    [System.Serializable]
    public class ScaledValueOperation : Operation<ScaledValue, float>
    {
        [SerializeField]
        private bool enabled = false;

        public override bool Calculate(ScaledValue value)
        {
            if (enabled)
                return ((IOperatable<float>)value).Get(operation, obj);

            return true;
        } 
    }

    [System.Serializable]
    public class QuestOperation : Operation<Quest, bool>
    {
        [SerializeField]
        private Quest value = null;

        public override bool Calculate(Quest value = null)
        {
            return ((IOperatable<bool>)this.value).Get(operation, obj); 
        }
    }

    [SerializeField]
    private QuestOperation[] quests = new QuestOperation[0];
    [SerializeField]
    private Event[] events = new Event[0];
    [SerializeField]
    private ScaledValueOperation hp = null;
    [SerializeField]
    private ScaledValueOperation armour = null; 

    public bool Fullfills(PlayerController controller)
    {
        bool fullfilled = hp.Calculate(controller.Hp) && armour.Calculate(controller.Armour);

        foreach (QuestOperation op in quests)
        {
            fullfilled &= op.Calculate();
            if (!fullfilled)
                return false;
        }

        foreach (Event evnt in events)
        {
            fullfilled &= GameInstance.GameState.EventController.Contains(evnt);
            if (!fullfilled)
                return false;
        }

        return fullfilled;
    }
}

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

public enum LogicOperator : byte
{
    LessThan        = 0,
    LessOrEqual     = 1,
    Equal           = 2,
    GreaterOrEqual  = 3,
    Greater         = 4,
}
