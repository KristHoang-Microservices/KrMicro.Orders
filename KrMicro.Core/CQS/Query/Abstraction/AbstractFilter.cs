using System.Linq.Expressions;
using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Core.CQS.Query.Abstraction;

public abstract class AbstractFilter<TData> : IQueryFilter<TData>
{
    private List<Expression<Func<TData, bool>>> _expressions = new();

    public bool Validate(TData data)
    {
        _expressions = AddRequestToExpression(data);
        foreach (var exp in _expressions)
            if (!exp.Compile().Invoke(data))
                return false;
        return true;
    }

    protected abstract List<Expression<Func<TData, bool>>> AddRequestToExpression(TData data);
}