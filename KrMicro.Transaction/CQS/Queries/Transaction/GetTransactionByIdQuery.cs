using KrMicro.Core.CQS.Query.Abstraction;

namespace KrMicro.Transaction.CQS.Queries.Transaction;

public record GetTransactionByIdQueryRequest;

public class GetTransactionByIdQueryResult : GetByIdQueryResult<Models.Transaction>
{
    public GetTransactionByIdQueryResult(Models.Transaction? data, bool isSuccess = true) : base(data, isSuccess)
    {
    }
}