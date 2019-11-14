using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is an Interactable object that loads a scene
/// on interaction.
/// </summary>
sealed public class LoadSceneInteractable : Interactable
{
    /// <summary>
    /// This variable will tell on which player Start object
    /// the player should start.
    /// </summary>
    [SerializeField]
    private int toId = 0;
    /// <summary>
    /// This variable will tell which scene to be 
    /// loaded on interaction.
    /// </summary>
    [SerializeField]
    private string sceneName = "Default";
    /// <summary>
    /// The items needed to load the scene.
    /// </summary>
    [SerializeField]
    private Item[] itemsNeeded = new Item[0];

    /// <summary>
    /// This method is called When the player interacts with this object.
    /// </summary>
    /// <param name="controller">The object the player is currently controlling.</param>
    protected override void OnInteract(PlayerController controller)
    {
        // Check if the player has the items necessary to load the scene.
        bool hasItems = true;
        foreach (Item i in itemsNeeded)
        {
            hasItems &= controller.Inventory.Contains(i.name);
            if (!hasItems)
                break;
        }

        // If the player does not have the necessary items to load the scene
        // then pop a message saying the items necessary and quit this method
        if (!hasItems)
        {
            foreach (Item i in itemsNeeded)
                controller.HudSettings.PopMessage("You need a " + i.name);

            return;
        }

        // To reach this line it means the player has the necessary items
        // So Change the level in a coroutine so we can load it in async mode
        StartCoroutine(ChangeLevel());
    }

    /// <summary>
    /// Changes the level in async mode
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeLevel()
    {
        // Shows the loading screen.
        GameInstance.HUD.EnableLoadingScreen(true);

        // Fades out the master mixer.
        GameInstance.Audio.FadeOut(AudioManager.Channel.Master, 0.4f);

        // Don't continue while we're still fading the master mixer 
        while (GameInstance.Audio.IsFadingOut(AudioManager.Channel.Master))
            yield return null;

        float time = Time.time;

        // Save the game.
        GameInstance.Save();

        // Store the id where the player should be spawned
        // in the next (scene) level.
        Spawner.SpawnAtID = toId;

        // Load the scene async.
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // AsyncOperation will not progress above 0.89f. 
        // That's why We're using the value 0.8f.
        // Don't continue if we're still loading.
        while (op.progress <= 0.8f)
            yield return null;

        // Calculate the time it took to load.
        float passedTime = Time.time - time;

        // Wait for one seconds minus the time it took to load.
        // The loading screen will always be shown 
        // for at least 1 second.
        yield return new WaitForSecondsRealtime(
            Mathf.Max(1f - passedTime, 0));

        // Activate the new scene.
        op.allowSceneActivation = true;
    }
}
