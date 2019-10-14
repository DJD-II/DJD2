using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public delegate void LoadEventHandler();

    public static GameInstance Singleton { get; private set; }
    public static HUD HUD { get; private set; }
    public static GameState GameState { get; private set; }
    public int ToID { get; set; }
    private bool IsSingleton { get; set; }

    PlaySaveGameObject saveGameObject = new PlaySaveGameObject();

    public static event LoadEventHandler OnSave,
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

        OnSave += () =>
        {
            saveGameObject = IO.Load<PlaySaveGameObject>("Temp");
            if (saveGameObject == null)
                saveGameObject = new PlaySaveGameObject();
        };

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        saveGameObject = IO.Load<PlaySaveGameObject>("Temp");
        if (saveGameObject == null)
            saveGameObject = new PlaySaveGameObject();

        Debug.Log("Loading... " + saveGameObject.objects.Count);
        OnLoad?.Invoke();
        saveGameObject = null;
    }

    private void OnDestroy()
    {
        if (!IsSingleton)
            return;

        IO.Delete("Temp");
    }

    public static void Save()
    {
        OnSave?.Invoke();
        IO.Save("Temp", Singleton.saveGameObject);
        Singleton.saveGameObject = null;
    }

    public void SetMouseCursorState(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    public void FeedSavable(ISavable savable, bool persistent)
    {
        Savable io = savable.IO;

        Savable io2;

        if (persistent)
            io2 = saveGameObject.objects.Find(x => io.id.Equals(x.id));
        else
            io2 = saveGameObject.objects.Find(x => io.id.Equals(x.id) && io.sceneName.Equals(x.sceneName));

        if (io2 != null)
            saveGameObject.objects.Remove(io2);

        saveGameObject.objects.Add(io);
    }

    public Savable GetSavable(string id, string sceneName, bool persistent)
    {
        if (persistent)
            return saveGameObject.objects.Find(i => id == i.id);

        return saveGameObject.objects.Find(i => id == i.id && i.sceneName == sceneName);
    }
}
