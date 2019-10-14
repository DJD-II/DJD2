using UnityEngine;
using UnityEngine.UI;

public class ItemMenuController : MonoBehaviour
{
    private PlayerController controller;
    private LootButton button;
    [SerializeField]
    private Text itemNameLabel;

    public void Initialize(PlayerController controller, LootButton button)
    {
        this.controller = controller;
        this.button = button;
        itemNameLabel.text = button.Item.Name;
    }

    public void OnUseButtonClick()
    {
        button.Item.Use(controller);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
