using UnityEngine;

[System.Serializable]
sealed public class PlayerCameraSettings
{
    [SerializeField] private Camera cam = null;
    [SerializeField] private Animation fallAnimation = null;
    [SerializeField] private Vector2 minMaxY = Vector2.zero;
    [SerializeField] private float lookSpeed = 20f;

    public Vector3 Origin { get => cam.transform.position; }
    public Vector3 Forward { get => cam.transform.forward; }
    public Vector3 ForwardRotation { get =>
            Quaternion.LookRotation(cam.transform.forward)
            * Vector3.forward; }

    /// <summary>
    /// This method checks for user input. It checks the mouse movement
    /// So that it can turn the player and the camera.
    /// </summary>
    public void Look(PlayerController controller)
    {
        if (controller.Hp.IsEmpty)
            return;

        Vector3 rotation = controller.transform.eulerAngles;
        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        controller.transform.eulerAngles = rotation;

        controller.MovementSettings.YRotation = Mathf.Clamp(
                                controller.MovementSettings.YRotation
                                + Input.GetAxis("Mouse Y") *
                                lookSpeed,
                                minMaxY.x,
                                minMaxY.y);

        cam.transform.localEulerAngles =
            new Vector3(-controller.MovementSettings.YRotation,
                        cam.transform.localEulerAngles.y,
                        0);
    }

    public void OnLanded(Character sender, float velocityOnY)
    {
        if (velocityOnY < -2f)
            fallAnimation.Play();
    }
}
