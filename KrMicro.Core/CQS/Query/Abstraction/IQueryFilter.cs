namespace KrMicro.Core.CQS.Query.Abstraction;

public interface IQueryFilter<in T>
{
    bool Validate(T data);
}