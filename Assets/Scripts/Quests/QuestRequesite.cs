using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestRequesite : object
{
    #region --- Fields ---

    [SerializeField]
    private List<Item> itemsNeeded = null;
    [SerializeField]
    private List<Event> eventsNeeded = null;
    [SerializeField]
    private bool needsMovement = false;
    [SerializeField]
    private bool needsLooking = false;
    [SerializeField]
    private bool custom = false;
    [SerializeField]
    private List<Quest> questsToGive = null;

    #endregion

    #region --- Properties ---

    public List<Quest> QuestsToGive { get { return questsToGive; } }

    #endregion

    public bool IsValid (PlayerController controller)
    {
        bool valid = true;

        valid = custom;

        if (needsMovement)
            valid = controller.Velocity.sqrMagnitude > 0;

        if (needsLooking)
            valid = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;

        if (!valid)
            return false;

        foreach (Item item in itemsNeeded)
        {
            valid &= controller.Inventory.Contains(item.name);
            if (!valid)
                return false;
        }

        foreach (Event evnt in eventsNeeded)
        {
            valid &= GameInstance.GameState.EventController.Contains(evnt);
            if (!valid)
                return false;
        }

        return valid;
    }
}
