
public interface ICompareOperatable<S>
{
    bool Get(LogicOperator operation, S value);
}

public interface ILogicOperatable<S>
{
    bool Get(S value);
}
