/// <summary>
/// Defines a logic compare operation
/// </summary>
/// <typeparam name="S"></typeparam>
public interface ILogicOperatable<S>
{
    /// <summary>
    /// Returns the value of an object
    /// </summary>
    /// <param name="value"> the object in question </param>
    /// <returns> a bool </returns>
    bool Get(S value);
}

