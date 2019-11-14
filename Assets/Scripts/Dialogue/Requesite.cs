using System;
using UnityEngine;

[System.Serializable]
public class Requesite
{
    [System.Serializable]
    public abstract class ComparableOperation<T, S> where T : ICompareOperatable<S>
    {
        [SerializeField]
        protected S obj;
    }

    [System.Serializable]
    public abstract class LogicOperation<T, S> where T : ILogicOperatable<S>
    {
        [SerializeField]
        protected S obj;
    }

    [System.Serializable]
    public abstract class SimpleOperation<T, S> : ComparableOperation<T, S> where T : ICompareOperatable<S> where S : IConvertible, new()
    {
        [SerializeField]
        protected LogicOperator operation = LogicOperator.Equal;

        public abstract bool Calculate(T value);
    }

    [System.Serializable]
    public abstract class RefOperation<T, S> : LogicOperation<T, S> where T : ILogicOperatable<S>
    {
        [SerializeField]
        protected T value;

        public abstract bool Calculate();
    }

    [System.Serializable]
    public abstract class ControllerOperation<T, S> : LogicOperation<T, S> where T : ILogicOperatable<S>
    {
        public abstract bool Calculate(PlayerController controller);
    }

    [System.Serializable]
    sealed public class ScaledValueOperation : SimpleOperation<ScaledValue, float>
    {
        [SerializeField]
        private bool enabled = false;

        public override bool Calculate(ScaledValue value)
        {
            if (enabled)
                return ((ICompareOperatable<float>)value).Get(operation, obj);

            return true;
        }
    }

    [System.Serializable]
    sealed public class QuestOperation : RefOperation<Quest, bool>
    {
        public override bool Calculate()
        {
            return ((ILogicOperatable<bool>)value).Get(obj);
        }
    }

    [System.Serializable]
    sealed public class EventOperation : RefOperation<EventID, bool>
    {
        public override bool Calculate()
        {
            return ((ILogicOperatable<bool>)value).Get(obj);
        }
    }

    [System.Serializable]
    sealed public class InventoryOperation : ControllerOperation<Inventory, Item>
    {
        [SerializeField] private bool contains = false;

        public override bool Calculate(PlayerController controller)
        {
            bool e = ((ILogicOperatable<Item>)controller.Inventory).Get(obj);
            return contains ? e : !e;
        }
    }

    [SerializeField] private QuestOperation[] quests = 
        new QuestOperation[0];
    [SerializeField] private EventOperation[] events = 
        new EventOperation[0];
    [SerializeField] private InventoryOperation[] items = 
        new InventoryOperation[0];
    [SerializeField] private ScaledValueOperation hp = null;
    [SerializeField] private ScaledValueOperation armour = null;

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

        foreach (EventOperation evnt in events)
        {
            fullfilled &= evnt.Calculate();
            if (!fullfilled)
                return false;
        }

        foreach (InventoryOperation item in items)
        {
            fullfilled &= item.Calculate(controller);
            if (!fullfilled)
                return false;
        }

        return fullfilled;
    }
}
