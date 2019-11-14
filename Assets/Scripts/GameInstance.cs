using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This object represents the game instance. Only one is permited and it holds the main game information.
/// HUD, GameState and I/O are in this class.
/// </summary>
sealed public class GameInstance : MonoBehaviour, ISavable
{
    [Serializable]
    public class GameInstanceSaveGameObject : Savable
    {
        public GameInstanceSaveGameObject()
            : base("", SceneManager.GetActiveScene().name)
        {
        }
    }

    public delegate void IOEventHandler(SaveGame io);


    public static event IOEventHandler  OnSave,
                                        OnLoad;

    public static GameInstance Singleton { get; private set; }
    public static HUD HUD { get; private set; }
    public static GameState GameState { get; private set; }
    public static AudioManager Audio { get; private set; }
    Savable ISavable.IO { get { return new GameInstanceSaveGameObject(); } }
    public static bool IsLoadingSavedGame { get; private set; }
    private static SaveGame SaveGame { get; set; }

    private void Awake()
    {
        if (Singleton != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Singleton   = this;
        HUD         = GetComponentInChildren<HUD>();
        GameState   = GetComponentInChildren<GameState>();
        Audio       = GetComponentInChildren<AudioManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Singleton != this)
            return;

        IO.Delete(IO.tempFilename);
    }

    /// <summary>
    /// This method is called after a scene has been loaded.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The Load scene mode.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveGame = IO.Load<SaveGame>(IO.tempFilename);
        if (SaveGame == null)
            SaveGame = new SaveGame();

        OnLoad?.Invoke(SaveGame);
        SaveGame = null;
        IsLoadingSavedGame = false;

        Audio.FadeIn(AudioManager.Channel.Master, 0.5f);

        StartCoroutine(HideLoadingScreen());
    }

    /// <summary>
    /// This method will hide the HUD (UI) loading screen 
    /// after rendering 10 frames.
    /// </summary> 
    /// <returns></returns>
    private IEnumerator HideLoadingScreen()
    {
        for (int i = 0; i < 10; i++)
            yield return new WaitForEndOfFrame();

        HUD.EnableLoadingScreen(false);
    }

    /// <summary>
    /// This method is called when the game is about to save.
    /// </summary>
    /// <param name="filename"></param>
    public static void Save(string filename = "")
    {
        SaveGame = IO.Load<SaveGame>(IO.tempFilename);

        if (SaveGame == null)
            SaveGame = new SaveGame();

        OnSave?.Invoke(SaveGame);

        SaveGame.Override(Singleton);

        IO.Save(string.IsNullOrEmpty(filename) ? 
            IO.tempFilename : filename, SaveGame);

        SaveGame = null;
    }

    /// <summary>
    /// This method is called when the game is loading.
    /// </summary>
    /// <param name="filename"></param>
    public static void Load(string filename = "")
    {
        Singleton.StartCoroutine(LoadSavedGame(filename));
    }

    /// <summary>
    /// This method allows the scene to be loaded async
    /// and to fade out the music/ambient.
    /// </summary>
    /// <param name="filename">The filename to be loaded.</param>
    /// <returns>IEnumerator</returns>
    private static IEnumerator LoadSavedGame(string filename)
    {
        HUD.EnableLoadingScreen(true);

        Audio.FadeOut(AudioManager.Channel.Master, 0.8f);
        IsLoadingSavedGame = !string.IsNullOrEmpty(filename);

        SaveGame = IO.Load<SaveGame>(string.IsNullOrEmpty(filename) ? IO.tempFilename : filename);

        if (SaveGame == null)
            SaveGame = new SaveGame();

        if (IsLoadingSavedGame)
            IO.Save(IO.tempFilename, SaveGame);

        // Don't continue while we're fading the master mixer.
        while (Audio.IsFadingOut(AudioManager.Channel.Master))
            yield return null;

        // Find the Game instance savable object in the saved game
        // object list.
        if (SaveGame.Find(x => x is GameInstanceSaveGameObject)
            is GameInstanceSaveGameObject savedSceneName)
        {
            // Start Loading the scene in async mode.
            AsyncOperation op = SceneManager.LoadSceneAsync(savedSceneName.SceneName);
            op.allowSceneActivation = false;

            // AsyncOperation will not progress above 0.89f. That's why
            // We're using 0.8f value.
            // Don't continue if we're still loading.
            while (op.progress < 0.8f)
                yield return null;

            // Activate the scene.
            op.allowSceneActivation = true;
        }
        else
        {
            Debug.LogError("No Game Instance Saved state.");
            Application.Quit();
        }
    }
}
