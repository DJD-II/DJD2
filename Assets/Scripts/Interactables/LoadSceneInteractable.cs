using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

sealed public class LoadSceneInteractable : Interactable
{
    [SerializeField]
    private int toId = 0;
    [SerializeField]
    private string sceneName = "Default";
    [SerializeField]
    private List<Item> itemsNeeded = new List<Item>();

    protected override void OnInteract(PlayerController controller)
    {
        bool hasItems = true;
        foreach(Item i in itemsNeeded)
        {
            hasItems &= controller.Inventory.Contains(i.name);
            if (!hasItems)
                break;
        }

        if (!hasItems)
        {
            foreach (Item i in itemsNeeded)
                controller.PopMessage("You need a " + i.name);

            return;
        }

        StartCoroutine(ChangeLevel());
    }

    private IEnumerator ChangeLevel ()
    {
        GameInstance.HUD.EnableLoadingScreen(true);

        GameInstance.Singleton.FadeOutMasterMixer(0.8f);

        while (GameInstance.Singleton.FadingOutMasterMixer)
            yield return null;

        float time = Time.time;

        GameInstance.Save();
        GameInstance.Singleton.ToID = toId;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress <= 0.8f)
            yield return null;

        float passedTime = Time.time - time;

        yield return new WaitForSecondsRealtime(Mathf.Max(1f - passedTime, 0));

        op.allowSceneActivation = true;

        GameInstance.Singleton.FadeInMasterMixer(1f);
    }
}
