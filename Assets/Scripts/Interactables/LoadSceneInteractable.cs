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

        GameInstance.HUD.EnableLoadingScreen(true);
        GameInstance.Save();
        GameInstance.Singleton.ToID = toId;
        SceneManager.LoadScene(sceneName);
    }
}
