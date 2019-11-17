using UnityEngine;

/// <summary>
/// Level Instances are used to connect scripts to the 
/// current level state. Because there are Scene 
/// events unique to that particular scene,
/// they will contain scene event-driven code.
/// Represents the scene itself. Any scene event-driven code
/// will be in these instances, that can be easily reached
/// by a "singleton".
/// </summary>
abstract public class LevelInstance : MonoBehaviour
{
    [SerializeField]
    [Tooltip("These will be the possible player spawn positions.")]
    private PlayerStart[] startPositions = new PlayerStart[0];

    /// <summary>
    /// This is not a typical Singleton. As It will change
    /// over scene loads. Each level should have its own
    /// Level Instance to represent that scene state.
    /// This property makes easier to reach the level instance
    /// without searching for it in the hierarchy list
    /// which can be an expensive operation.
    /// </summary>
    public static LevelInstance Singleton { get; private set; }
    /// <summary>
    /// The Start Positions at which the player can spawn.
    /// </summary>
    public PlayerStart[] StartPositions { get { return startPositions; } }

    protected virtual void Awake()
    {
        if (Singleton != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Singleton = this;

        GameInstance.OnLoad += OnLoad;
    }

    protected virtual void Start()
    {
        GameInstance.OnSave += OnSave;
    }

    protected virtual void OnDestroy()
    {
        Singleton = null;

        GameInstance.OnLoad -= OnLoad;
        GameInstance.OnSave -= OnSave;
    }

    /// <summary>
    /// This method is called when the game is loading.
    /// </summary>
    /// <param name="io">The save game object to load from.</param>
    protected virtual void OnLoad(SaveGame io)
    {

    }

    /// <summary>
    /// This method is called when the game is about to save.
    /// </summary>
    /// <param name="io">The save game object to be saved.</param>
    protected virtual void OnSave(SaveGame io)
    {

    }
}