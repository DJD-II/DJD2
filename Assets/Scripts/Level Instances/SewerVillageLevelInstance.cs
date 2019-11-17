public class SewerVillageLevelInstance : LevelInstance
{
    protected override void Start()
    {
        base.Start();

        GameInstance.GameState.LocationController.Add(Location.Neverland);
    }
}
