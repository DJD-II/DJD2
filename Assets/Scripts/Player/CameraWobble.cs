using UnityEngine;

[System.Serializable]
public class CameraWobble
{
    [SerializeField] private Transform pivot = null;
    [Range(0f, 5f)]
    [SerializeField] private float walkWoobleAmount = 0;
    [Range(0f, 5f)]
    [SerializeField] private float runWoobleAmount = 0;
    [Range(0f, 90f)]
    [SerializeField] private float walkZRotation = 0;
    [Range(0f, 90f)]
    [SerializeField] private float runZRotation = 0;
    [Range(0f, 100f)]
    [SerializeField] private float walkWobbleSpeed = 0;
    [Range(0f, 100f)]
    [SerializeField] private float runWobbleSpeed = 0;

    private void UpdateMove(PlayerController controller)
    {
        float multiplier = controller.MovementSettings.CurrentSpeed / controller.MovementSettings.RunSpeed;
        multiplier *= Mathf.Clamp01(controller.MovementSettings.Controller.velocity.magnitude);
        float time = Time.time;

        float currentWobbleSpeed = (controller.IsRunning && controller.MovementSettings.CanRun) ?
                                    runWobbleSpeed : walkWobbleSpeed;
        float currentZRotation = (controller.IsRunning && controller.MovementSettings.CanRun) ?
                                    runZRotation : walkZRotation;
        float currentWobleAmount = (controller.IsRunning && controller.MovementSettings.CanRun) ?
                                    runWoobleAmount : walkWoobleAmount;

        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition,
                                                     new Vector3(0f,
                                                                 Mathf.Sin(time * currentWobbleSpeed) *
                                                                 currentWobleAmount * multiplier,
                                                                 0f),
                                                     Time.deltaTime * 2f);
        pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation,
                                                         Quaternion.Euler(0f,
                                                                          0f,
                                                                          Mathf.Sin(time * currentWobbleSpeed / 2) *
                                                                          currentZRotation * multiplier),
                                                         Time.deltaTime * 2f);
    }

    private void UpdateStopped()
    {
        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition,
                                                     Vector3.zero,
                                                     Time.deltaTime * 6f);
        pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation,
                                                         Quaternion.Euler(0f, 0f, 0f),
                                                         Time.deltaTime * 6f);
    }

    public void UpdateWobble(
        PlayerController controller,
        float vertical, 
        float horizontal)
    {
        if (controller.MovementSettings.Grounded &&
            (vertical != 0 || horizontal != 0) &&
            controller.MovementSettings.CanControl)
            UpdateMove(controller);
        else
            UpdateStopped();
    }
}