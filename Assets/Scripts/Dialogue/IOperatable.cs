
public interface IOperatable<S> where S : System.IConvertible
{
    bool Get(LogicOperator operation, S value);
}
