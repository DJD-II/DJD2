using UnityEngine;

sealed public class GameInstance : MonoBehaviour, ISavable
{
    [System.Serializable]
    public class GameInstanceSaveGameObject : Savable
    {
        public string SceneName { get; private set; }

        public GameInstanceSaveGameObject()
            : base ("", "")
        {
            SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
    }

    public delegate void IOEventHandler(PlaySaveGameObject io);

    public static GameInstance Singleton { get; private set; }
    public static HUD HUD { get; private set; }
    public static GameState GameState { get; private set; }
    public int ToID { get; set; }
    private bool IsSingleton { get; set; }
    Savable ISavable.IO { get { return new GameInstanceSaveGameObject(); } }
    public bool IsLoadingSavedGame { get; private set; }
    private static PlaySaveGameObject SaveGameObject { get; set; }

    public static event IOEventHandler      OnSave,
                                            OnLoad;

    private void Awake()
    {
        if (Singleton != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        IsSingleton = true;

        Singleton = this;
        HUD = GetComponentInChildren<HUD>();
        GameState = GetComponentInChildren<GameState>();

        HUD.Initialize();

        GameState.OnPausedChanged += (GameState sender) =>
        {
            if (sender.Paused)
                SetMouseCursorState(true, CursorLockMode.None);
            else
                SetMouseCursorState(false, CursorLockMode.Locked);
        };

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        SaveGameObject = IO.Load<PlaySaveGameObject>(IO.tempFilename);
        if (SaveGameObject == null)
            SaveGameObject = new PlaySaveGameObject();

        OnLoad?.Invoke(SaveGameObject);
        SaveGameObject = null;
        IsLoadingSavedGame = false;
        HUD.EnableLoadingScreen(false);
    }

    private void OnDestroy()
    {
        if (!IsSingleton)
            return;

        IO.Delete(IO.tempFilename);
    }

    public static void Save(string filename = "")
    {
        HUD.EnableLoadingScreen(true);

        SaveGameObject = IO.Load<PlaySaveGameObject>(IO.tempFilename);

        if (SaveGameObject == null)
        {
            Debug.Log("Temp Save Game does not exist! Creating one...");
            SaveGameObject = new PlaySaveGameObject();
        }

        OnSave?.Invoke(SaveGameObject);
            
        Savable s = SaveGameObject.objects.Find(x => x is GameInstanceSaveGameObject);
        if (s != null)
            SaveGameObject.objects.Remove(s);
        SaveGameObject.objects.Add(((ISavable)Singleton).IO);

        IO.Save(string.IsNullOrEmpty(filename) ? IO.tempFilename : filename, SaveGameObject);
        SaveGameObject = null;
    }

    public static void Load(string filename = "")
    {
        HUD.EnableLoadingScreen(true);

        Singleton.IsLoadingSavedGame = !string.IsNullOrEmpty(filename);

        SaveGameObject = IO.Load<PlaySaveGameObject>(string.IsNullOrEmpty(filename) ? IO.tempFilename : filename);

        if (SaveGameObject == null)
            SaveGameObject = new PlaySaveGameObject();

        if (Singleton.IsLoadingSavedGame)
            IO.Save(IO.tempFilename, SaveGameObject);

        if (SaveGameObject.objects.Find(x => x is GameInstanceSaveGameObject) is GameInstanceSaveGameObject s)
            UnityEngine.SceneManagement.SceneManager.LoadScene(s.SceneName);
    }

    public void SetMouseCursorState(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }
}
