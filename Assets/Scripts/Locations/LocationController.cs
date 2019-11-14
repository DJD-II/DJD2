using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed public class LocationController : ISavable
{
    public delegate void EventHandler(LocationController sener, Location newLocation);

    public event EventHandler OnDiscoveredNewLocation;

    [System.Serializable]
    sealed public class LocationsSavable : Savable
    {
        private List<Location> DiscoveredLocations { get; }

        public LocationsSavable(LocationController controller)
            : base ("", "")
        {
            DiscoveredLocations = 
                new List<Location>(controller.locations);
        }

        public void Set (LocationController controller)
        {
            controller.locations.Clear();
            controller.locations.AddRange(DiscoveredLocations.ToArray());
        }
    }

    [SerializeField] private List<Location> locations = null;

    Savable ISavable.IO { get => new LocationsSavable(this); }

    public bool Add(Location location)
    {
        if (locations.Contains(location))
            return false;

        locations.Add(location);
        OnDiscoveredNewLocation?.Invoke(this, location);

        return true;
    }

    public void Load (SaveGame io)
    {
        if (!(io.Find(x => x is LocationsSavable) is LocationsSavable savable))
            return;

        savable.Set(this);
    }
}