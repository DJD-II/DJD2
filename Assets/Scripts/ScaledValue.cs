using UnityEngine;

[System.Serializable]
public struct ScaledValue : IOperatable<float>
{
    [SerializeField]
    private float max;

    public ScaledValue(float scalar, float max)
    {
        Scalar = scalar;
        this.max = Mathf.Max(0, max);
    }

    public void Subtract(float value)
    {
        Scalar = Mathf.Max((Value - value) / Max, 0);
    }

    public void Add(float value)
    {
        Scalar = Mathf.Min((Value + value) / Max, 1);
    }

    bool IOperatable<float>.Get(LogicOperator operation, float value)
    {
        switch (operation)
        {
            case LogicOperator.LessThan:
                return Value < value;
            case LogicOperator.LessOrEqual:
                return Value <= value;
            case LogicOperator.Equal:
                return Value == value;
            case LogicOperator.GreaterOrEqual:
                return Value >= value;
            case LogicOperator.Greater:
                return Value > value;
        }

        return false;
    }

    public float Value { get { return Scalar * max; } }
    public float Max { get { return max; } set { max = Mathf.Max(0, value); } }
    public float Scalar { get; private set; }
    public bool IsFull { get { return Scalar >= 1; } }
    public bool IsEmpty { get { return Scalar <= 0; } }
}
