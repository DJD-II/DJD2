using UnityEngine;
using UnityEngine.UI;

sealed public class ItemMenuController : MonoBehaviour
{
    private PlayerController controller = null;
    private LootButton button = null;
    [SerializeField]
    private Text itemNameLabel = null;

    public void Initialize(PlayerController controller, LootButton button)
    {
        this.controller = controller;
        this.button = button;
        itemNameLabel.text = button.Item.Name;
    }

    public void OnUseButtonClick()
    {
        button.Item.Use(controller);
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
