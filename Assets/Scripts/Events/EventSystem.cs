using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
sealed public class EventController : object, ISavable
{
    [System.Serializable]
    sealed public class EventsSavable : Savable
    {
        public List<Event> SavedEvents { get; }

        public EventsSavable(List<EventID> events)
            : base("", "")
        {
            SavedEvents = new List<Event>();
            foreach (EventID id in events)
                SavedEvents.Add(id.Event);
        }

        public void Set(EventController controller)
        {
            foreach (Event evnt in SavedEvents)
                controller.Add(evnt);
        }
    }

    [SerializeField] private List<EventID> events = null;

    private List<EventID> Events { get => events; }
    Savable ISavable.IO { get { return new EventsSavable(Events); } }

    public void Add(Event evnt)
    {
        if (Contains(evnt))
            return;

        Events.Add(new EventID(evnt));
    }

    public void Add(IEnumerable<Event> eventsToGive)
    {
        foreach (Event e in eventsToGive)
            Add(e);
    }

    private void Clear()
    {
        Events.Clear();
    }

    public bool Contains(Event evnt)
    {
        return Events.Find(x => x.Event.Equals(evnt)) != null;
    }

    public void Load(SaveGame io)
    {
        Clear();
        if (io.Find(x => x is EventsSavable) is EventsSavable savable)
            savable.Set(this);
    }
}