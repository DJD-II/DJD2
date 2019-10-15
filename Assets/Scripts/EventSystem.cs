using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Event : ushort
{
    Beginings = 0,
    End = 1000,
}

sealed public class EventController : object, ISavable
{
    [System.Serializable]
    sealed public class EventSaveGameObject : Savable
    {
        public List<Event> Events { get; }

        public EventSaveGameObject(List<Event> events)
            : base ("", "")
        {
            Events = new List<Event>(events);
        }
    }

    private List<Event> events;

    public EventController()
    {
        events = new List<Event>();
    }

    public void Add (Event evnt)
    {
        if (events.Contains(evnt))
            return;

        events.Add(evnt);
    }

    public void Clear ()
    {
        events.Clear();
    }

    public bool Contains (Event evnt)
    {
        return events.Contains(evnt);
    }

    Savable ISavable.IO { get { return new EventSaveGameObject(events); } }
}
