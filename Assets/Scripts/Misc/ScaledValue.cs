using UnityEngine;

[System.Serializable]
public struct ScaledValue : ICompareOperatable<float>
{
    private float limit;
    [SerializeField]
    private float max;

    public float Limit { get => limit; }
    public float Value { get { return Scalar * max; } }
    public float Max { get { return max; } set { max = Mathf.Max(0, value); } }
    public float Scalar { get; private set; }
    public bool IsFull { get { return Scalar >= 1; } }
    public bool IsEmpty { get { return Scalar <= 0; } }

    public ScaledValue(float scalar, float max, float limit = 1f)
    {
        Scalar = scalar;
        this.max = Mathf.Max(0, max);
        this.limit = limit;
    }

    public void SubtractLimit(float value)
    {
        limit = Mathf.Max(0, limit - Mathf.Clamp01(value));
        Scalar = Mathf.Min(Scalar, limit);
    }

    public void AddLimit(float value)
    {
        limit = Mathf.Min(Max, Mathf.Min(value, 1));
    }

    public void Subtract(float value)
    {
        Scalar = Mathf.Max((Value - value) / Max, 0);
    }

    public void Add(float value)
    {
        Scalar = Mathf.Min((Value + value) / Max, 1);
    }

    bool ICompareOperatable<float>.Get(LogicOperator operation, float value)
    {
        switch (operation)
        {
            case LogicOperator.LessThan:
                return value > Scalar;
            case LogicOperator.LessOrEqual:
                return value >= Scalar;
            case LogicOperator.Equal:
                return value == Scalar;
            case LogicOperator.GreaterOrEqual:
                return value <= Scalar;
            case LogicOperator.Greater:
                return value < Scalar;
        }

        return false;
    }
}
