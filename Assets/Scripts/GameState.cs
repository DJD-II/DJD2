using UnityEngine;

/// <summary>
/// Represents the Game State. It holds quest and event information as well
/// as the game pause state.
/// </summary>
sealed public class GameState : MonoBehaviour
{
    public delegate void EventHandler(GameState sender);

    public event EventHandler OnPausedChanged;

    /// <summary>
    /// The quest holder system.
    /// </summary>
    [Header("Quest System")]
    [SerializeField] private QuestController questController = null;
    /// <summary>
    /// The event holder system.
    /// </summary>
    [Header("Event System")]
    [SerializeField] private EventController eventController = null;
    [Header("Locations")]
    [SerializeField] private LocationController locationController = null;
    /// <summary>
    /// Pause state.
    /// </summary>
    private bool paused = false;

    /// <summary>
    /// The Quest controller. It holds the quests.
    /// </summary>
    public QuestController QuestController { get => questController; }
    /// <summary>
    /// The Event Controller. It holds the global game events.
    /// </summary>
    public EventController EventController { get => eventController; }
    /// <summary>
    /// The Location controller. It Hold places (Locations)
    /// that have been discovered.
    /// </summary>
    public LocationController LocationController { get => locationController; }
    /// <summary>
    /// The game pause state. Use this to pause and unpause the game.
    /// </summary>
    public bool Paused
    {
        get { return paused; }

        set
        {
            paused = value;

            OnPausedChanged(this);

            if (paused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
    }

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void Start()
    {
        GameInstance.OnSave += OnSave;
        OnPausedChanged += OnGameHasPaused;
    }

    private void Update()
    {
        questController.Update();
    }

    private void OnDestroy()
    {
        GameInstance.OnSave -= OnSave;
        GameInstance.OnLoad -= OnLoad;

        OnPausedChanged -= OnGameHasPaused;
    }

    /// <summary>
    /// This method is called when the game pause state has changed.
    /// </summary>
    /// <param name="sender">The Game State.</param>
    private void OnGameHasPaused(GameState sender)
    {
        if (sender.Paused)
            SetMouseCursorState(true, CursorLockMode.None);
        else
            SetMouseCursorState(false, CursorLockMode.Locked);
    }

    /// <summary>
    /// Sets the game cursor state.
    /// its visibility and lock mode.
    /// </summary>
    /// <param name="visible">Whether the mouse cursor is visible.</param>
    /// <param name="lockMode">Whether the cursor is locked within the screen.</param>
    public void SetMouseCursorState(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible      = visible;
        Cursor.lockState    = lockMode;
    }

    /// <summary>
    /// This method is called when the game is about to be saved.
    /// </summary>
    /// <param name="io"></param>
    private void OnSave(SaveGame io)
    {
        io.Override(eventController);
        io.Override(questController);
        io.Override(locationController);
    }

    /// <summary>
    /// This method is called when the game is loading.
    /// </summary>
    /// <param name="io"></param>
    private void OnLoad(SaveGame io)
    {
        eventController.Load(io);
        questController.Load(io);
        locationController.Load(io);
    }
}
