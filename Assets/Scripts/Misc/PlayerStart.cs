using UnityEngine;

sealed public class PlayerStart : MonoBehaviour
{
    [SerializeField]
    private uint id = 0;

    public uint ID { get { return id; } }

    /// <summary>
    /// This method places the player at this object transform.
    /// </summary>
    /// <param name="controller"></param>
    public void Spawn(PlayerController controller)
    {
        if (controller.MovementSettings.Controller != null)
            controller.MovementSettings.Controller.enabled = false;

        controller.gameObject.transform.position = transform.position;
        controller.gameObject.transform.rotation = transform.rotation;

        if (controller.MovementSettings.Controller != null)
            controller.MovementSettings.Controller.enabled = true;
    }
}
