using UnityEngine;

/// <summary>
/// This object represents the game instance. Only one is permited and holds the main game information.
/// HUD, GameState and Save/Load is in this class.
/// </summary>
sealed public class GameInstance : MonoBehaviour, ISavable
{
    #region --- Classes ---

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

    #endregion

    #region --- Events ---

    public delegate void IOEventHandler(PlaySaveGameObject io);


    public static event IOEventHandler  OnSave,
                                        OnLoad;

    #endregion

    #region --- Properties ---

    public static GameInstance Singleton { get; private set; }
    public static HUD HUD { get; private set; }
    public static GameState GameState { get; private set; }
    public int ToID { get; set; }
    private bool IsSingleton { get; set; }
    Savable ISavable.IO { get { return new GameInstanceSaveGameObject(); } }
    public bool IsLoadingSavedGame { get; private set; }
    private static PlaySaveGameObject SaveGameObject { get; set; }

    #endregion

    #region --- Methods ---

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

    /// <summary>
    /// This method is called when the game is about to save.
    /// </summary>
    /// <param name="filename"></param>
    public static void Save(string filename = "")
    {
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

    /// <summary>
    /// This method is called when the game is loading.
    /// </summary>
    /// <param name="filename"></param>
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

    /// <summary>
    /// Sets the game cursor state.
    /// its visibility and lock mode.
    /// </summary>
    /// <param name="visible"></param>
    /// <param name="lockMode"></param>
    public void SetMouseCursorState(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    #endregion
}
