using UnityEngine;

[RequireComponent(typeof(PlayerController))]
sealed public class Spawner : MonoBehaviour
{
    private bool ignore = false;

    /// <summary>
    /// This property stores the id in which the playerController will spawn at 
    /// the start of the (scene) level. In each level there 
    /// are PlayerStart objects that mark the spawn points.
    /// When this script starts it will try to find (in the LevelInstance) 
    /// the Spawn point (PlayerStart) with this id and 
    /// Spawn the player in that (PlayerStart) Spawn point.
    /// </summary>
    public static int SpawnAtID { get; set; }

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void Start()
    {
        if (ignore)
            return;

        PlayerStart[] playerStartPositions = LevelInstance.Singleton.StartPositions;
        PlayerStart startPosition = null;

        foreach (PlayerStart startPos in playerStartPositions)
            if (startPos.ID == SpawnAtID)
            {
                startPosition = startPos;
                break;
            }

        if (startPosition == null)
        {
            Debug.LogWarning("No Player Start Position for " + gameObject.name);
            return;
        }

        startPosition.Spawn(GetComponent<PlayerController>());
    }

    private void OnDestroy()
    {
        GameInstance.OnLoad -= OnLoad;
    }

    private void OnLoad(SaveGame io)
    {
        ignore = GameInstance.IsLoadingSavedGame;
    }
}
