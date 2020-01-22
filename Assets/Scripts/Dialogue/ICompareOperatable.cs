/// <summary>
/// Defines a compare operation with a value
/// </summary>
/// <typeparam name="S"> the value to be compared </typeparam>
public interface ICompareOperatable<S>
{
    /// <summary>
    /// Defines a bool that receives a LogicOperator and a generic value
    /// </summary>
    /// <param name="operation"> The logic operator to be compared </param>
    /// <param name="value"> The value to be compared against </param>
    /// <returns> a bool </returns>
    bool Get(LogicOperator operation, S value);
}
