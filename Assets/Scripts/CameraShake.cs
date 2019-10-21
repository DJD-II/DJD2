using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Camera Shake", menuName = "Camera Shake")]
public class CameraShake : ScriptableObject
{
    #region --- classes ---

    private class Settings
    {
        private Vector3 position = Vector3.zero;
        private Vector3 initialPosition = Vector3.zero;
        private Vector3 rotation = Vector3.zero;
        private Vector3 initialRotation = Vector3.zero;

        public float FOV { get; set; }
        public float InitialFOV { get; }
        public Coroutine Coroutine { get; set; }
        public Transform Pivot { get; }
        public MonoBehaviour Controller { get; }
        public Camera Camera { get; }

        public Settings(MonoBehaviour controller, Camera camera, Transform pivot, float fov)
        {
            Camera = camera;
            Controller = controller;
            Pivot = pivot;
            initialPosition = this.position = pivot.localPosition;
            initialRotation = this.rotation = pivot.localRotation.eulerAngles;
            InitialFOV = FOV = fov;
        }

        public void Reset ()
        {
            if (Coroutine != null)
                Controller.StopCoroutine(Coroutine);

            Pivot.localPosition = initialPosition;
            Pivot.localRotation = Quaternion.Euler(initialRotation);
            Camera.fieldOfView = InitialFOV;
        }

        public void Apply(CameraShake shake, float blendTime, float blendScale)
        {
            rotation.x = (Mathf.Cos(blendTime * shake.rotationX.frequency * Mathf.Rad2Deg) * shake.rotationX.amplitude) * blendScale;
            rotation.y = (Mathf.Cos(blendTime * shake.rotationY.frequency * Mathf.Rad2Deg) * shake.rotationY.amplitude) * blendScale;
            rotation.z = (Mathf.Cos(blendTime * shake.rotationZ.frequency * Mathf.Rad2Deg) * shake.rotationZ.amplitude) * blendScale;

            Pivot.localRotation = Quaternion.Euler( initialRotation.x + rotation.x,
                                                    initialRotation.y + rotation.y,
                                                    initialRotation.z + rotation.z);

            position.x = (Mathf.Cos(blendTime * shake.positionX.frequency * Mathf.Rad2Deg) * shake.positionX.amplitude) * blendScale;
            position.y = (Mathf.Cos(blendTime * shake.positionY.frequency * Mathf.Rad2Deg) * shake.positionY.amplitude) * blendScale;
            position.z = (Mathf.Cos(blendTime * shake.positionZ.frequency * Mathf.Rad2Deg) * shake.positionZ.amplitude) * blendScale;

            Pivot.localPosition = initialPosition + position;

            FOV = (Mathf.Cos(blendTime * shake.fov.frequency * Mathf.Deg2Rad) * shake.fov.amplitude) * blendScale;
            Camera.fieldOfView = InitialFOV + FOV;
        }
    }

    [System.Serializable]
    public class Element
    {
        public float amplitude = 1f,
                     frequency = 0.2f;
    }

    #endregion

    #region --- Fields ---

    [Header("Position")]
    [SerializeField]
    private Element positionX = null;
    [SerializeField]
    private Element positionY = null;
    [SerializeField]
    private Element positionZ = null;
    [Header("Rotation")]
    [SerializeField]
    private Element rotationX = null;
    [SerializeField]
    private Element rotationY = null;
    [SerializeField]
    private Element rotationZ = null;
    [Header("Field Of View")]
    [SerializeField]
    private Element fov = null;
    [SerializeField]
    public float duration = 1f;
    [SerializeField]
    public float blendInTime = 0.1f;
    [SerializeField]
    public float blendOutTime = 0.1f;
    private Settings settings = null;

    #endregion

    #region --- Methods ---

    public void Play (MonoBehaviour controller, Camera camera, Transform pivot, float scale)
    {
        if (settings != null)
            settings.Reset();

        settings = new Settings(controller, camera, pivot, camera.fieldOfView);
        settings.Coroutine = settings.Controller.StartCoroutine(ShakeCamera(camera, scale));
    }

    private IEnumerator ShakeCamera(Camera camera, float scale)
    {
        float blendScale;
        float blendInTimer = duration * Mathf.Min(blendInTime, 1f);
        float blendOutTimer = duration * Mathf.Min(blendOutTime, 1f);
        float timer = 0f;

        while (timer <= duration)
        {
            float blendInScale = Mathf.Clamp01(timer / blendInTimer);
            float blendOutScale = Mathf.Clamp01((duration - timer) / blendOutTimer);

            blendScale = scale * blendInScale * blendOutScale;

            float blendTime = timer / duration;

            settings.Apply(this, blendTime, blendScale);

            timer += Time.deltaTime;
            yield return null;
        }

        settings.Reset();
        settings = null;
    }

    #endregion
}