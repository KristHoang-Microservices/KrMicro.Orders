using System.Linq.Expressions;
using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Orders.CQS.Queries.Transaction;

public class TransactionQueryFilter : AbstractFilter<Models.Transaction>
{
    private readonly GetAllTransactionQueryRequest _request;

    public TransactionQueryFilter(GetAllTransactionQueryRequest request)
    {
        _request = request;
    }

    protected override List<Expression<Func<Models.Transaction, bool>>> AddRequestToExpression(Models.Transaction data)
    {
        var exp = new List<Expression<Func<Models.Transaction, bool>>>
        {
            x => !_request.TransactionStatus.HasValue || data.TransactionStatus == _request.TransactionStatus,
            x => !_request.Status.HasValue || data.Status == _request.Status,
            x => !_request.FromDate.HasValue || data.CreatedAt!.Value.CompareTo(_request.FromDate.Value) >= 0,
            x => !_request.ToDate.HasValue || data.CreatedAt!.Value.CompareTo(_request.ToDate.Value) <= 0
        };
        return exp;
    }
}