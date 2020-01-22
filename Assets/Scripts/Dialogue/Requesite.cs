using System;
using UnityEngine;

/// <summary>
/// Responsible for checking if the requirements for conversations are fufilled
/// </summary>
[Serializable]
public class Requesite
{
    /// <summary>
    /// A class for storying a protected generic object
    /// </summary>
    /// <typeparam name="T"> The first object to be compared </typeparam>
    /// <typeparam name="S"> The second object to be compared </typeparam>
    [Serializable]
    public abstract class ComparableOperation<T, S> where T : 
        ICompareOperatable<S>
    {
        /// <summary>
        /// The generic object to be compared
        /// </summary>
        [SerializeField] protected S obj;
    }
    /// <summary>
    /// A class for storying a protected generic object
    /// </summary>
    /// <typeparam name="T"> The first object to be compared </typeparam>
    /// <typeparam name="S"> The second object to be compared </typeparam>
    [Serializable]
    public abstract class LogicOperation<T, S> where T : ILogicOperatable<S>
    {
        /// <summary>
        /// The generic object to be compared
        /// </summary>
        [SerializeField] protected S obj;
    }

    /// <summary>
    /// Stores a logic operatos and checks if that object matches the requesites
    /// </summary>
    /// <typeparam name="T"> The first object to be compared </typeparam>
    /// <typeparam name="S"> The second object to be compared </typeparam>
    [Serializable]
    public abstract class SimpleOperation<T, S> : ComparableOperation<T, S> 
        where T : ICompareOperatable<S> where S : IConvertible, new()
    {
        /// <summary>
        /// Stores the wanted logic operator
        /// </summary>
        [SerializeField]
        protected LogicOperator operation = LogicOperator.Equal;

        /// <summary>
        /// Checks if the object matches the requesites
        /// </summary>
        /// <param name="value"> The generic object to be compared </param>
        /// <returns> a bool </returns>
        public abstract bool Calculate(T value);
    }

    /// <summary>
    /// Checks if the references of the player are fufilled
    /// </summary>
    /// <typeparam name="T"> The first object to be compared </typeparam>
    /// <typeparam name="S"> The second object to be compared </typeparam>
    [Serializable]
    public abstract class RefOperation<T, S> : LogicOperation<T, S> where T :
        ILogicOperatable<S>
    {
        /// <summary>
        /// Stores a generic object
        /// </summary>
        [SerializeField] protected T value;

        /// <summary>
        /// Checks if the object matches the requesites
        /// </summary>
        /// <returns> a bool </returns>
        public abstract bool Calculate();
    }

    /// <summary>
    /// Checks if the player fufills the requesites given
    /// </summary>
    /// <typeparam name="T"> The first object to be compared </typeparam>
    /// <typeparam name="S"> The second object to be compared </typeparam>
    [Serializable]
    public abstract class ControllerOperation<T, S> : LogicOperation<T, S> where T :
        ILogicOperatable<S>
    {
        /// <summary>
        /// Checks if the object matches the requesites
        /// </summary>
        /// <param name="value"> The generic object to be compared </param>
        /// <returns> a bool </returns>
        public abstract bool Calculate(PlayerController controller);
    }

    /// <summary>
    /// Checks if the requesites aresbeing fufilled
    /// </summary>
    [Serializable]
    sealed public class ScaledValueOperation : SimpleOperation<ScaledValue, float>
    {
        /// <summary>
        /// Stores a bool if the requesit is active or not
        /// </summary>
        [SerializeField] private bool enabled = false;

        /// <summary>
        /// Checks if the object matches the requesites returning true by default
        /// </summary>
        /// <param name="value"> The generic object to be compared </param>
        /// <returns> a bool </returns>
        public override bool Calculate(ScaledValue value)
        {
            // Checks if the requesite is enabled
            if (enabled)
                // Returns a bool comparing the operation to the object
                return ((ICompareOperatable<float>)value).Get(operation, obj);
            // Returns true by default
            return true;
        }
    }

    /// <summary>
    /// Checks if a quest is fufilled
    /// </summary>
    [Serializable]
    sealed public class QuestOperation : RefOperation<Quest, bool>
    {
        /// <summary>
        /// Compares the value with the object given
        /// </summary>
        /// <returns> a bool </returns>
        public override bool Calculate()
        {
            // Returns true when the quest is fufilled
            return ((ILogicOperatable<bool>)value).Get(obj);
        }
    }

    /// <summary>
    /// Checks if the event is fufilled
    /// </summary>
    [Serializable]
    sealed public class EventOperation : RefOperation<EventID, bool>
    {
        /// <summary>
        /// Compares the value with the object given
        /// </summary>
        /// <returns> a bool </returns>
        public override bool Calculate()
        {
            // Returns true when the event is fufilled
            return ((ILogicOperatable<bool>)value).Get(obj);
        }
    }

    /// <summary>
    /// Cheks if the player has a certain item
    /// </summary>
    [Serializable]
    sealed public class InventoryOperation : ControllerOperation<Inventory, Item>
    {
        /// <summary>
        /// Stores a bool for if the player has the item or not
        /// </summary>
        [SerializeField] private bool contains = false;

        /// <summary>
        /// Compares the value with the object given
        /// </summary>
        /// <param name="controller"> The player controller </param>
        /// <returns> a bool </returns>
        public override bool Calculate(PlayerController controller)
        {
            // Creates a local bool and checks if that item exists
            bool e = ((ILogicOperatable<Item>)controller.Inventory).Get(obj);
            // Checks if the contains bool is true, if it is returns e,
            // otherwise returns the oposite of e
            return contains ? e : !e;
        }
    }

    /// <summary>
    /// Creates an array of QuestOperations
    /// </summary>
    [SerializeField] private QuestOperation[] quests = 
        new QuestOperation[0];
    /// <summary>
    /// Creates an array of events
    /// </summary>
    [SerializeField] private EventOperation[] events = 
        new EventOperation[0];
    /// <summary>
    /// Creates an array of items to be checked
    /// </summary>
    [SerializeField] private InventoryOperation[] items = 
        new InventoryOperation[0];
    /// <summary>
    /// Checks if the player is above a certain hp value
    /// </summary>
    [SerializeField] private ScaledValueOperation hp = null;
    /// <summary>
    /// Checks if the armor is above a certain value
    /// </summary>
    [SerializeField] private ScaledValueOperation armour = null;

    /// <summary>
    /// Checks if the armor and hp fufill the requesites
    /// </summary>
    /// <param name="controller"> the character controller </param>
    /// <returns> a bool if it's fufilled or not </returns>
    public bool Fullfills(PlayerController controller)
    {
        // checks if the hp and armor are above the threshold
        bool fullfilled = hp.Calculate(controller.Hp) 
            && armour.Calculate(controller.Armour);

        // If it's not fufilled returns false
        if (!fullfilled)
            return false;

        // Cheks every Quest to see if they are fufilled
        foreach (QuestOperation op in quests)
        {
            fullfilled &= op.Calculate();
            // if they are not fufilled returns false
            if (!fullfilled)
                return false;
        }

        // Checks every event to see if they are fufilled
        foreach (EventOperation evnt in events)
        {
            fullfilled &= evnt.Calculate();
            // if they are not fufilled returns false
            if (!fullfilled)
                return false;
        }

        // Checks every Item in to see if the player has it
        foreach (InventoryOperation item in items)
        {
            fullfilled &= item.Calculate(controller);
            // if they are not fufilled returns false
            if (!fullfilled)
                return false;
        }

        // Rreturns fufilled
        return fullfilled;
    }
}
