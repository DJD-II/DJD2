using System;
using UnityEngine;

[Serializable]
sealed public class EventID : ILogicOperatable<bool>
{
    [SerializeField] private Event evnt;

    public Event Event { get { return evnt; } }

    public EventID(Event evnt)
    {
        this.evnt = evnt;
    }

    bool ILogicOperatable<bool>.Get(bool value)
    {
        bool e = GameInstance.GameState.EventController.Contains(evnt);
        return value ? e : !e;
    }
}
