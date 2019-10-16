using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Camera Shake", menuName = "Camera Shake")]
public class CameraShake : ScriptableObject
{
    #region --- classes ---

    private class CameraShakeSettings
    {
        private Vector3 position = Vector3.zero;
        private Vector3 initialPosition = Vector3.zero;
        private Vector3 rotation = Vector3.zero;
        private Vector3 initialRotation = Vector3.zero;

        public ref Vector3 Position { get { return ref position; } }
        public ref Vector3 InitialPosition { get { return ref initialPosition; } }
        public ref Vector3 Rotation { get { return ref rotation; } }
        public ref Vector3 InitialRotation { get { return ref initialRotation; } }
        public float FOV { get; set; }
        public float InitialFOV { get; }
        public Coroutine Coroutine { get; set; }
        public Transform Pivot { get; }

        public CameraShakeSettings (Transform pivot, Vector3 position, Quaternion rotation, float fov)
        {
            Pivot = pivot;
            initialPosition = this.position = position;
            initialRotation = this.rotation = rotation.eulerAngles;
            InitialFOV = FOV = fov;
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
    private CameraShakeSettings settings = null;
    private MonoBehaviour controller = null;
    private Camera camera = null;

    #endregion

    #region --- Methods ---

    public void Play (MonoBehaviour controller, Camera camera, Transform pivot, float scale)
    {
        if (settings != null)
        {
            if (settings.Coroutine != null)
                this.controller.StopCoroutine(settings.Coroutine);

            settings.Pivot.localPosition = settings.InitialPosition;
            settings.Pivot.localRotation = Quaternion.Euler(settings.InitialRotation);
            this.camera.fieldOfView = settings.InitialFOV;

            settings = null;
        }

        this.controller = controller;
        this.camera = camera;
        settings = new CameraShakeSettings(pivot, pivot.localPosition, pivot.localRotation, camera.fieldOfView);
        settings.Coroutine = this.controller.StartCoroutine(ShakeCamera(camera, scale));
    }

    private IEnumerator ShakeCamera(Camera camera, float scale)
    {
        float blendScale;
        float blendInTimer = duration * Mathf.Min(blendInTime, 1f);
        float blendOutTimer = duration * Mathf.Min(blendOutTime, 1f);
        float timer = 0f;

        Transform t = settings.Pivot;

        while (timer <= duration)
        {
            float blendInScale = Mathf.Clamp01(timer / blendInTimer);
            float blendOutScale = Mathf.Clamp01((duration - timer) / blendOutTimer);

            blendScale = scale * blendInScale * blendOutScale;

            float blendTime = timer / duration;

            settings.Rotation.x = (Mathf.Cos(blendTime * rotationX.frequency * Mathf.Rad2Deg) * rotationX.amplitude) * blendScale;
            settings.Rotation.y = (Mathf.Cos(blendTime * rotationY.frequency * Mathf.Rad2Deg) * rotationY.amplitude) * blendScale;
            settings.Rotation.z = (Mathf.Cos(blendTime * rotationZ.frequency * Mathf.Rad2Deg) * rotationZ.amplitude) * blendScale;

            t.localRotation = Quaternion.Euler(settings.InitialRotation.x + settings.Rotation.x,
                                                settings.InitialRotation.y + settings.Rotation.y,
                                                settings.InitialRotation.z + settings.Rotation.z);

            settings.Position.x = (Mathf.Cos(blendTime * positionX.frequency * Mathf.Rad2Deg) * positionX.amplitude) * blendScale;
            settings.Position.y = (Mathf.Cos(blendTime * positionY.frequency * Mathf.Rad2Deg) * positionY.amplitude) * blendScale;
            settings.Position.z = (Mathf.Cos(blendTime * positionZ.frequency * Mathf.Rad2Deg) * positionZ.amplitude) * blendScale;

            t.localPosition = settings.InitialPosition + settings.Position;

            settings.FOV = (Mathf.Cos(blendTime * fov.frequency * Mathf.Deg2Rad) * fov.amplitude) * blendScale;

            camera.fieldOfView = settings.InitialFOV + settings.FOV;

            timer += Time.deltaTime;
            yield return null;
        }

        t.localRotation = Quaternion.Euler(settings.InitialRotation);
        t.localPosition = settings.InitialPosition;
        camera.fieldOfView = settings.InitialFOV;

        settings = null;
    }

    #endregion
}