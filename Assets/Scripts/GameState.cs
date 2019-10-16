using UnityEngine;

/// <summary>
/// Represents the Game State. It holds quest and event information as well
/// as the game pause state.
/// </summary>
sealed public class GameState : MonoBehaviour
{
    #region --- Events ---

    public delegate void EventHandler(GameState sender);

    public event EventHandler OnPausedChanged;

    #endregion

    #region --- Fields ---
    
    /// <summary>
    /// The quest holder system.
    /// </summary>
    [Header("Quest System")]
    [SerializeField]
    private QuestController questController = new QuestController();
    /// <summary>
    /// The event holder system.
    /// </summary>
    [Header("Events")]
    [SerializeField]
    private EventController eventController = new EventController();
    /// <summary>
    /// Pause state.
    /// </summary>
    private bool paused = false;

    #endregion

    #region --- Properties ---

    /// <summary>
    /// The Quest controller. It holds the quests.
    /// </summary>
    public QuestController QuestController { get { return questController; } }
    /// <summary>
    /// The Event Controller. Ite holds the global game events.
    /// </summary>
    public EventController EventController { get { return eventController; } }
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

    #endregion

    #region --- Methods ---

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void Start()
    {
        GameInstance.OnSave += OnSave;
    }

    private void Update()
    {
        questController.Update();
    }

    private void OnDestroy()
    {
        GameInstance.OnSave -= OnSave;
        GameInstance.OnLoad -= OnLoad;
    }

    /// <summary>
    /// This method is called when the game is about to be saved.
    /// </summary>
    /// <param name="io"></param>
    private void OnSave (PlaySaveGameObject io)
    {
        Savable a = io.objects.Find(x => x is EventController.EventSaveGameObject);
        if (a != null)
            io.objects.Remove(a);
        io.objects.Add(((ISavable)EventController).IO);

        Savable b = io.objects.Find(x => x is QuestController.QuestSavable);
        if (b != null)
            io.objects.Remove(b);
        io.objects.Add(((ISavable)QuestController).IO);
    }

    /// <summary>
    /// This method is called when the game is loading.
    /// </summary>
    /// <param name="io"></param>
    private void OnLoad(PlaySaveGameObject io)
    {
        eventController.Clear();
        Savable a = io.objects.Find(x => x is EventController.EventSaveGameObject);
        if (a != null)
            foreach (Event evnt in ((EventController.EventSaveGameObject)a).Events)
                eventController.Add(evnt);

        QuestController.Clear();
        Savable b = io.objects.Find(x => x is QuestController.QuestSavable);
        if (b != null)
            foreach (QuestController.QuestInfo info in ((QuestController.QuestSavable)b).QuestsInfo)
                questController.Add(new QuestController.QuestID(info));
    }

    #endregion
}
