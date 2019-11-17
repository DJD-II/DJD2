using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
sealed public class Item : ScriptableObject
{
    public enum Tag
    {
        Battery,
        Used_Battery,
        Oil,
        Bobby_Pin,
        Bubble_Gum,
        Scrap_Metal,
        Crow_Bar,
        Water_Bottle,
        Tin_Can,
        Marobodo_Cigarette_Pack,
        Box,
        Ramen,
        Super_Cola_Can,
        Chinese_Sorted_Food,
        Hamburger,
        Rent_Cigarette_Pack,
        Beer_Can,
        Patato_Pack,
        Food_Tray,
        Chinese_Packed_Food,
        WirSun_Cigarette_Pack,
    }

    [SerializeField] private Sprite icon = null;
    [SerializeField] private GameObject gameObject = null;
    [SerializeField] private Tag tag = Tag.Bobby_Pin;
    [SerializeField] private string description = "";
    [SerializeField] private float weight = 0f;
    [SerializeField] private float cost = 0f;

    public Tag ItemTag { get => tag; }
    public Sprite Icon { get => icon; }
    public string Name { get { return tag.ToString().Replace("_", " "); } }
    public string Description { get => description; }
    public float Weight { get => weight; }
    public float Cost { get => cost; }

    public void Use(PlayerController controller)
    {
        controller.Inventory.Remove(this);

        switch (tag)
        {
            case Tag.Battery:
                controller.ApplyHeal(new PointHeal(null, 50));
                break;

            case Tag.Used_Battery:
                controller.ApplyHeal(new PointHeal(null, 20));
                break;

            case Tag.Oil:
                break;

            case Tag.Bubble_Gum:
                break;

            case Tag.Bobby_Pin:
                if (controller.Interaction.Interactable != null &&
                    controller.Interaction.Interactable.Locked)
                {
                    GameInstance.HUD.EnableMenu(false);
                    controller.Interaction.Interact(controller);
                }
                break;
        }
    }

    public void Discard(PlayerController controller)
    {
        controller.Inventory.Remove(this);

        if (gameObject == null)
        {
            Debug.LogWarning("Item gameObject is null, Can't instantiate " +
                "The item in the world. Consider assigning, the item '" + Name + 
                "', gameObject variable a prefab.");
            return;
        }

        // TODO : This position should check if it is in the world and 
        // if is not blocked.
        Vector3 position = controller.CameraSettings.Origin +
            controller.CameraSettings.Forward * Random.Range(0.3f, 0.5f) +
            controller.transform.up * Random.Range(-0.1f, 0.1f) +
            controller.transform.right * Random.Range(-0.1f, 0.1f);

        Quaternion rotation = Quaternion.Euler(
            Random.Range(-180f, 180f), 
            Random.Range(-180f, 180f),
            Random.Range(-180f, 180f));

        GameObject go = Instantiate(gameObject, position, rotation);
        go.AddComponent<UniqueID>();
    }

    public void Check(PlayerController controller)
    {
        GameInstance.HUD.CheckItem(this, controller);
    }
}