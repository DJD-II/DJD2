using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraWobble 
{
    #region --- Fields ---

    [SerializeField]
    private Transform pivot = null;
    [Range(0f, 5f)]
    [SerializeField]
    private float walkWoobleAmount = 0;
    [Range(0f, 5f)]
    [SerializeField]
    private float runWoobleAmount = 0;
    [Range(0f, 90f)]
    [SerializeField]
    private float walkZRotation = 0;
    [Range(0f, 90f)]
    [SerializeField]
    private float runZRotation = 0;
    [Range(0f, 100f)]
    [SerializeField]
    private float walkWobbleSpeed = 0;
    [Range(0f, 100f)]
    [SerializeField]
    private float runWobbleSpeed = 0;

    #endregion

    #region --- Properties ---

    public float WobbleSpeed { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runWobbleSpeed : walkWobbleSpeed; } }
    public float ZRotation { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runZRotation : walkZRotation; } }
    public float WobleAmount { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runWoobleAmount : walkWoobleAmount; } }

    #endregion

    #region --- Methods ---

    public void UpdateMove (float normalizedVelocity, float currentSpeed, float runSpeed)
    {
        float multiplier = currentSpeed / runSpeed;
        multiplier *= Mathf.Clamp01(normalizedVelocity);
        float time = Time.time;

        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, new Vector3(0f, Mathf.Sin(time * WobbleSpeed) * WobleAmount * multiplier, 0f), Time.deltaTime * 2f);
        pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(0f, 0f, Mathf.Sin(time * WobbleSpeed / 2) * ZRotation * multiplier), Time.deltaTime * 2f);
    }

    public void UpdateStopped()
    {
        pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, Vector3.zero, Time.deltaTime * 6f);
        pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 6f);
    }

    #endregion
}