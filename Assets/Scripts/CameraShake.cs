using UnityEngine;

sealed public class CameraShake
{
    [System.Serializable]
    public struct ElementShake
    {
        public float amplitude,
                     frequency;

        public ElementShake(float amplitude, float frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }
    }

    [SerializeField]
    private ElementShake[] rotationShake = new ElementShake[3];
    [SerializeField]
    private ElementShake[] positionShake = new ElementShake[3];
    [SerializeField]
    private ElementShake fieldOfViewShake;

    public ElementShake[] RotationShake { get { return rotationShake; } }
    public ElementShake[] PositionShake { get { return positionShake; } }
    public ElementShake FieldOfViewShake { get { return fieldOfViewShake; } set { fieldOfViewShake = value; } }
    public float Duration { get; }
    public float BlendInTime { get; }
    public float BlendOutTime { get; }

    public CameraShake()
        : this(1f, 0.1f, 0.2f)
    {
    }

    public CameraShake(float duration, float blendInTime, float blendOutTime)
    {
        Duration = duration;
        BlendInTime = blendInTime;
        BlendOutTime = blendOutTime;

        for (int i = 0; i < rotationShake.Length; i++)
            rotationShake[i] = new ElementShake(0f, 0f);
        for (int i = 0; i < positionShake.Length; i++)
            rotationShake[i] = new ElementShake(0f, 0f);

        fieldOfViewShake = new ElementShake(0f, 0f);
    }
}