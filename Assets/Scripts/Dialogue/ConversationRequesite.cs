using UnityEngine;
using System;

[System.Serializable]
public class ConversationRequesite
{
    [System.Serializable]
    public abstract class Operation<T, S> where T : IOperatable<S> where S : IConvertible, new()
    {
        [SerializeField]
        protected S obj;
    }

    [System.Serializable]
    public abstract class SimpleOperation<T, S> : Operation<T, S> where T : IOperatable<S> where S : IConvertible, new()
    {
        [SerializeField]
        protected LogicOperator operation = LogicOperator.Equal;

        public abstract bool Calculate(T value);
    }

    [System.Serializable]
    public abstract class RefOperation<T, S> : Operation<T, S> where T : IOperatable<S> where S : IConvertible, new()
    {
        [SerializeField]
        protected T value;

        public abstract bool Calculate();
    }

    [System.Serializable]
    sealed public class ScaledValueOperation : SimpleOperation<ScaledValue, float>
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
    sealed public class QuestOperation : RefOperation<Quest, bool>
    {
        public override bool Calculate()
        {
            return ((IOperatable<bool>)value).Get(LogicOperator.Equal, obj);
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

        if (!fullfilled)
            return false;

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
