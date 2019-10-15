using UnityEngine;

sealed public class GameState : MonoBehaviour
{
    public delegate void EventHandler(GameState sender);

    public event EventHandler OnPausedChanged;

    [Header("Quest System")]
    [SerializeField]
    private QuestController questController = new QuestController();
    [Header("Events")]
    [SerializeField]
    private EventController eventController = new EventController();

    private bool paused = false;

    public QuestController QuestController { get { return questController; } }
    public EventController EventController { get { return eventController; } }

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void Start()
    {
        GameInstance.OnSave += OnSave;
    }

    private void OnDestroy()
    {
        GameInstance.OnSave -= OnSave;
        GameInstance.OnLoad -= OnLoad;
    }

    private void OnSave ()
    {
        Savable a = GameInstance.SaveGameObject.objects.Find(x => x is EventController.EventSaveGameObject);
        if (a != null)
            GameInstance.SaveGameObject.objects.Remove(a);
        GameInstance.SaveGameObject.objects.Add(((ISavable)EventController).IO);

        Savable b = GameInstance.SaveGameObject.objects.Find(x => x is QuestController.QuestSavable);
        if (b != null)
            GameInstance.SaveGameObject.objects.Remove(b);
        GameInstance.SaveGameObject.objects.Add(((ISavable)QuestController).IO);
    }

    private void OnLoad()
    {
        eventController.Clear();
        Savable a = GameInstance.SaveGameObject.objects.Find(x => x is EventController.EventSaveGameObject);
        if (a != null)
            foreach (Event evnt in ((EventController.EventSaveGameObject)a).Events)
                eventController.Add(evnt);

        QuestController.Clear();
        Savable b = GameInstance.SaveGameObject.objects.Find(x => x is QuestController.QuestSavable);
        if (b != null)
            foreach (QuestController.QuestInfo info in ((QuestController.QuestSavable)b).QuestsInfo)
                questController.Add(new QuestController.QuestID(info));
    }

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
}
